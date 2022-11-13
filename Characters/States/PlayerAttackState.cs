using Godot;
using Godot.Collections;
using Godot.NativeInterop;

namespace SupaLidlGame.Characters.State
{
    public partial class PlayerAttackState : PlayerState
    {
        [Export]
        public CharacterState IdleState { get; set; }

        private float _attackTime = 0;

        public override CharacterState Enter(CharacterState previousState)
        {
            _attackTime = 0;
            return base.Enter(previousState);
        }

        public override void Exit(CharacterState nextState)
        {
            _attackTime = 0;
            base.Exit(nextState);
        }

        public override CharacterState Input(InputEvent @event)
        {
            return base.Input(@event);
        }

        public override CharacterState PhysicsProcess(double delta)
        {
            return base.PhysicsProcess(delta);
        }

        public override CharacterState Process(double delta)
        {
            return base.Process(delta);
        }
    }
}
