namespace SupaLidlGame.Characters.State
{
    public partial class PlayerState : CharacterState
    {
        //public PlayerMachine PlayerMachine => Machine as PlayerMachine;
        protected Player _player => Character as Player;

        public override CharacterState Process(double delta)
        {
            Character.Direction = Godot.Input.GetVector("ui_left", "ui_right",
                                                        "ui_up", "ui_down");
            return base.Process(delta);
        }
    }
}
