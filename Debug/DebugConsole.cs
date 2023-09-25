using Godot;

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
                    GetParent<Window>().Title = "Supa Developer Console: " +
                        _context.GetPath();
                }
            }
        }
    }

    private Entry _entry;

    private RichTextLabel _output;

    private Node ctx => Context;

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
                _output.Text += ex.Message + '\n';
            }
        };
    }

    enum PathType
    {
        Node,
        Property
    };

    public Variant From(NodePath path)
    {
        CharIterator iterator = new(path);
        Variant variant = Context ?? this;
        foreach (var subpath in NodePathParser.ParseNodePath(iterator))
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

    public void SetProp(NodePath path, Variant value)
    {
        var node = GetNode(path.GetAsPropertyPath());
        //var ent = CurrentMap.Entities.GetNodeOrNull(entityName);
        //if (ent is not null)
        //{
        //    ent.Set(property, value);
        //}
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

    public void Execute(string str)
    {
        str = Sanitizer.Sanitize(str);
        string inputMirror = $"[b]{Context.GetPath()}:[/b] {str}";
        _output.Text += inputMirror + "\n";
        var context = Context;

        Godot.Expression exp = new();

        string[] reserved = { "from", "set_context", "context" };
        Godot.Collections.Array reservedMap = new();
        reservedMap.Add(new Callable(this, MethodName.From));
        reservedMap.Add(new Callable(this, MethodName.SetContext));
        reservedMap.Add(Context);

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
}
