using Godot;
using SupaLidlGame.Extensions;
using SupaLidlGame.BoundingBoxes;

namespace SupaLidlGame.Characters;

public partial class Doc : Boss
{
    public AnimationPlayer TelegraphAnimation { get; set; }

    public AnimationPlayer MiscAnimation { get; set; }

    [Export]
    public override bool IsActive
    {
        get => base.IsActive;
        set
        {
            // if player not alive or doesn't exist then don't activate
            if (!this.GetWorld()?.CurrentPlayer?.IsAlive ?? true)
            {
                if (value)
                {
                    return;
                }
            }

            base.IsActive = value;
            var introState = BossStateMachine
                .GetNode<State.NPC.Doc.DocIntroState>("Intro");
            BossStateMachine.ChangeState(introState);

            if (IsActive)
            {
                var trig = GetNode<InteractionTrigger>("InteractionTrigger");
                var coll = trig.GetNode<CollisionShape2D>("CollisionShape2D");
                coll.Disabled = true;
            }
            else
            {
                Reset();
            }
        }
    }

    public override float Health
    {
        get => base.Health;
        set
        {
            if (IsActive)
            {
                base.Health = value;
            }
            else
            {
                // play opening animation
                // then become active when it finishes
                base.Health = value;
            }
        }
    }

    public override int Intensity
    {
        get
        {
            switch (Health)
            {
                case < 300:
                    return 3;
                case < 600:
                    return 2;
                default:
                    return 1;
            }
        }
    }

    public Doc()
    {
        ShouldMove = false;
    }

    public override void _Ready()
    {
        TelegraphAnimation = GetNode<AnimationPlayer>("Animations/Telegraph");
        MiscAnimation = GetNode<AnimationPlayer>("Animations/Misc");

        base._Ready();

        var dialog = GD.Load<Resource>("res://Assets/Dialogue/doc.dialogue");

        GetNode<BoundingBoxes.InteractionTrigger>("InteractionTrigger")
            .Interaction += () =>
        {
            //DialogueManager.ShowExampleDialogueBalloon(dialog, "duel");
            this.GetWorld().DialogueBalloon
                .Start(dialog, "duel");
                //.Call("start", dialog, "duel");
        };

        GetNode<State.Global.GlobalState>("/root/GlobalState")
            .SummonBoss += (string name) =>
        {
            if (name == "Doc")
            {
                IsActive = true;
                Inventory.SelectedItem = GetNode<Items.Item>("%DocLance");
            }
        };


        CanAttack = false;

        // when we are hurt, start the boss fight
        Hurt += (Events.HurtArgs args) =>
        {
            if (this.GetWorld().CurrentPlayer.IsAlive && !IsActive)
            {
                IsActive = true;
                Inventory.SelectedItem = GetNode<Items.Item>("%DocLance");
            }
        };
    }

    public override void _Process(double delta)
    {
        if (IsActive)
        {
            BossStateMachine.Process(delta);
        }
        base._Process(delta);
    }

    protected override void Reset()
    {
        base.Reset();
        GlobalPosition = Vector2.Zero;
        var trig = GetNode<InteractionTrigger>("InteractionTrigger");
        var coll = trig.GetNode<CollisionShape2D>("CollisionShape2D");
        coll.Disabled = false;
    }
}
