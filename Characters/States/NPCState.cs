namespace SupaLidlGame.Characters.State
{
    public partial class NPCState : CharacterState
    {
        protected NPC _npc => Character as NPC;

        public override CharacterState Process(double delta)
        {
            _npc.ThinkProcess(delta);
            return base.Process(delta);
        }
    }
}
