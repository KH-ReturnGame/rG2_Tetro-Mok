using Global;
using UnityEngine;

namespace SinglePlay.State
{
    public class GameStartState : IState
    {
        private readonly GameManager _manager;

        public GameStartState(GameManager manager)
        {
            _manager = manager;
        }

        public void OnEnter()
        {
            Debug.Log("Entered Game Start State");
            SoundManager.PlayBgm();
            _manager.pauseScreen.SetActive(false);
            _manager.gameEndScreen.SetActive(false);
            _manager.GameBoard = new int[19, 19];
            _manager.ChangeState(new BlackState(_manager));
        }

        public void OnExit()
        {
            Debug.Log("Exited Game Start State");
        }

        public void Update()
        {
        }

        public void HandleInput(string input)
        {
            if (input.Equals("start")) _manager.ChangeState(new BlackState(_manager));
        }
    }
}