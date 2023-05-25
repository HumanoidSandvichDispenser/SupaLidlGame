namespace SupaLidlGame.State.Character
{
    public abstract partial class NPCState : CharacterState
    {
        protected Characters.NPC _npc => Character as Characters.NPC;

        public override CharacterState Process(double delta)
        {
            _npc.ThinkProcess(delta);
            return base.Process(delta);
        }
    }
}
