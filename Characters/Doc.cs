using Godot;
using SupaLidlGame.Extensions;
using SupaLidlGame.State.Character;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.State.Thinker;

namespace SupaLidlGame.Characters;

public partial class Doc : Boss
{
    public AnimationPlayer TelegraphAnimation { get; set; }

    public AnimationPlayer MiscAnimation { get; set; }

    [Export]
    public Items.Weapons.Sword Lance { get; set; }

    [Export]
    public override bool IsActive
    {
        get => base.IsActive;
        set
        {
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
                Inventory.SelectedItem = Lance;
            }
        };


        // when we are hurt, start the boss fight
        Hurt += (Events.HealthChangedArgs args) =>
        {
            if (!IsActive)
            {
                IsActive = true;
                Inventory.SelectedItem = Lance;
                //DialogueManager.ShowExampleDialogueBalloon();
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

    protected override float ReceiveDamage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackDir = default)
    {
        if (IsActive)
        {
            return base.ReceiveDamage(
                damage, inflictor, knockback, knockbackDir);
        }

        return 1;
    }

    /*
    public override void OnReceivedDamage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackDir = default)
    {
        GetNode<GpuParticles2D>("Effects/HurtParticles")
            .SetDirection(knockbackDir);

        base.OnReceivedDamage(damage, inflictor, knockback, knockbackDir);
    }
    */

    protected override void Think()
    {
        if (BossStateMachine.CurrentState is State.NPC.Doc.DocLanceState)
        {
            if (ThinkerStateMachine.CurrentState is not DashDefensive)
            {
                ThinkerStateMachine.ChangeState<DashDefensive>(out var _);
            }
        }
        else
        {
            base.Think();
        }
    }
}
