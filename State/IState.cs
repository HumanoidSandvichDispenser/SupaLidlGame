namespace SupaLidlGame.State
{
    public interface IState<T> where T : IState<T>
    {
        /// <summary>
        /// Called when this state is entered
        /// </summary>
        /// <remarks>
        /// This returns a <c>IState</c> in case a state is being
        /// transitioned to but wants to transition to another state. For
        /// example, an attack state can return to an idle state, but that idle
        /// state can override it to the move state immediately when necessary.
        /// </remarks>
        public IState<T> Enter(IState<T> previousState) => null;

        /// <summary>
        /// Called when the <c>Character</c> exits this <c>CharacterState</c>.
        /// </summary>
        public void Exit(IState<T> nextState)
        {

        }

        public IState<T> Process(double delta) => null;
    }
}
