using UnityEngine;

namespace State
{
    public class MainMenuState : IState
    {
        private readonly GameManager _manager;

        public MainMenuState(GameManager manager)
        {
            _manager = manager;
        }

        public void OnEnter()
        {
            _manager.LoadScene("TitleScene");
            Debug.Log("Entered Main Menu State");
        }

        public void OnExit()
        {
            Debug.Log("Exited Main Menu State");
        }

        public void Update()
        {
        }

        public void HandleInput(string input)
        {
            if (input == "start") _manager.ChangeState(new GameStartState(_manager));
        }
    }
}