using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.Debug;

public sealed partial class DebugConsole : Control
{
    private Node _context;
    public Node Context
    {
        get => _context;
        private set
        {
            if (value is not null)
            {
                _context = value;
                if (_entry is not null)
                {
                    _entry.PlaceholderText = "Enter Godot expression from " +
                        _context.GetPath();
                    _entry.SetContext(_context);
                    GetParent<Window>().Title = "Supa Developer Console: " +
                        _context.GetPath();
                }
            }
        }
    }

    private Entry _entry;

    public Entry Entry => _entry;

    private RichTextLabel _output;

    private Node ctx => Context;

    public const double DEBUG_REFRESH_INTERVAL = 0.25;

    public override void _Ready()
    {
        _entry = GetNode<Entry>("%Entry");
        _output = GetNode<RichTextLabel>("%Output");
        Context = Utils.World.Instance;

        _entry.ConsoleInput += (string input) =>
        {
            try
            {
                Execute(input);
            }
            catch (InterpreterException ex)
            {
                _output.Text += ex.Message;
                if (!ex.Message.StartsWith("Error occurred while"))
                {
                    _output.Text += $" @{ex.Line}:{ex.Column}" + '\n';
                    _output.Text += input + '\n';
                    _output.Text += new string(' ', ex.Column - 1) + "^\n";
                }
                else
                {
                    _output.Text += '\n';
                }
            }
        };

        GD.Print("DebugConsole init");
        // TODO: put this in a separate class
        // watch godot.log
        bool isFileLoggingEnabled = ProjectSettings
            .GetSetting("debug/file_logging/enable_file_logging.pc")
            .AsBool();

        if (isFileLoggingEnabled)
        {
            GD.Print("File logging is enabled.");
            string logPath = ProjectSettings
                .GetSetting("debug/file_logging/log_path")
                .AsString();
            var fs = FileAccess.Open(logPath, FileAccess.ModeFlags.Read);

            var timer = new Timer();
            AddChild(timer);
            timer.Timeout += () =>
            {
                // push 
                while (fs.GetPosition() < fs.GetLength())
                {
                    string line = fs.GetLine();
                    _output.Text += line;
                }
            };
            timer.Start(DEBUG_REFRESH_INTERVAL);
        }
        else
        {
            GD.PushWarning("File logging is not enabled.");
        }
    }

    public IEnumerable<NodePathToken> SplitPath(NodePath path)
    {
        CharIterator iterator = new(path);
        return NodePathParser.ParseNodePath(iterator);
    }

    public NodePath ToNodePath(string path)
    {
        return Variant.From(path).AsNodePath();
    }

    public object GetNodeOrProperty(NodePath path)
    {
        var tokens = new List<NodePathToken>(SplitPath(path));
        Node variant = Context ?? this;
        for (int i = 0; i < tokens.Count; i++)
        {
            var subpath = tokens[i];
            if (i == tokens.Count - 1)
            {
                if (subpath.Type == NodePathTokenType.Property)
                {
                    return new PropertyPointer(variant, subpath.Path);
                }
            }
            else
            {
                if (subpath.Type == NodePathTokenType.Node)
                {
                    if (subpath.Path != "")
                    {
                        variant = variant.GetNode(subpath.Path);
                    }
                }
                else
                {
                    variant = variant.GetIndexed(subpath.Path)
                        .AsGodotObject() as Node;
                }
            }
        }

        return variant;
    }

    public Variant From(NodePath path)
    {
        Variant variant = Context ?? this;
        foreach (var subpath in SplitPath(path))
        {
            if (variant.VariantType == Variant.Type.Object)
            {
                if (variant.AsGodotObject() is Node n)
                {
                    if (subpath.Type == NodePathTokenType.Node)
                    {
                        if (subpath.Path != "")
                        {
                            variant = n.GetNode(subpath.Path);
                        }
                    }
                    else
                    {
                        variant = n.GetIndexed(subpath.Path);
                    }
                }
            }
        }

        return variant;
    }

    public void SetProp(Variant prop, Variant value)
    {
        if (prop.VariantType == Variant.Type.NodePath)
        {
            var tokens = new List<NodePathToken>(SplitPath(prop.AsNodePath()));
            Node variant = Context ?? this;
            for (int i = 0; i < tokens.Count; i++)
            {
                var subpath = tokens[i];
                if (i == tokens.Count - 1)
                {
                    if (subpath.Type == NodePathTokenType.Property)
                    {
                        variant.SetIndexed(":" + subpath.Path, value);
                    }
                }
                else
                {
                    if (subpath.Type == NodePathTokenType.Node)
                    {
                        if (subpath.Path != "")
                        {
                            variant = variant.GetNode(subpath.Path);
                        }
                    }
                    else
                    {
                        variant = variant.GetIndexed(subpath.Path)
                            .AsGodotObject() as Node;
                    }
                }
            }
        }
        else
        {

        }
    }

    public string CallMethod(
        Utils.World world,
        string entityName,
        string method,
        string[] args)
    {
        var ent = world.CurrentMap.Entities.GetNodeOrNull(entityName);
        if (ent is not null)
        {
            var returnValue = ent.Call(method, args);
            return returnValue.ToString();
        }
        return "";
    }

    public void Print(string text)
    {
        GD.Print(text);
    }

    public void Inspect(NodePath node)
    {

    }

    public void Execute(string str)
    {
        //str = Sanitizer.Sanitize(str);
        str = Transpiler.Transpiler.Transpile(str);
        string inputMirror = $"[b]{Context.GetPath()}:[/b] {str}";
        _output.Text += inputMirror + "\n";
        var context = Context;

        Godot.Expression exp = new();

        string[] reserved = {
            "from",
            "set_context",
            "context",
            "set_prop",
            "to_node_path",
            "load",
        };
        Godot.Collections.Array reservedMap = new();
        reservedMap.Add(new Callable(this, MethodName.From));
        reservedMap.Add(new Callable(this, MethodName.SetContext));
        reservedMap.Add(Context);
        reservedMap.Add(new Callable(this, MethodName.SetProp));
        reservedMap.Add(new Callable(this, MethodName.ToNodePath));
        reservedMap.Add(new Callable(this, MethodName.Load));

        var err = exp.Parse(str, reserved);
        if (err != Error.Ok)
        {
            throw new InterpreterException(
                "Error occurred while parsing Godot.Expression: \n" +
                exp.GetErrorText(),
                0, 0);
        }

        Variant result = exp.Execute(reservedMap, context);
        if (exp.HasExecuteFailed())
        {
            throw new InterpreterException(
                "Error occurred while evaluating Godot.Expression: \n" +
                exp.GetErrorText(),
                0, 0);
        }

        // send result to output
        if (result.VariantType != Variant.Type.Nil)
        {
            _output.Text += result + "\n";
        }
    }

    public void SetContext(Node node)
    {
        Context = node;
    }

    private Resource Load(string path)
    {
        return ResourceLoader.Load(path);
    }
}
