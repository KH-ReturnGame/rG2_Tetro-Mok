namespace State
{
    public class EndGameState : IState
    {
        private GameManager _manager;

        public EndGameState(GameManager manager)
        {
            _manager = manager;
        }


        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void Update()
        {
        }

        public void HandleInput(string input)
        {
        }
    }
}