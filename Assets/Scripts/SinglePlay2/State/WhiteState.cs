using SinglePlay2.AI;
using UnityEngine;

namespace SinglePlay2.State
{
    public class WhiteState : IState
    {
        private readonly GameManager _manager;
        private bool _isPaused;
        private int _stoneType;
        private (int, int)[] _targetStones;

        public WhiteState(GameManager manager)
        {
            _manager = manager;
        }

        public void OnEnter()
        {
            Debug.Log("Entered White State");

            _stoneType = Random.Range(1, 4);

            _targetStones = GetTargetStones();
            if (!_isPaused) _manager.ChangeState(new BlackState(_manager));
        }

        public void OnExit()
        {
            if (_targetStones != null) _manager.PutStones(_targetStones, 2);
            Debug.Log("Exited White State");
        }

        public void Update()
        {
        }

        public void HandleInput(string input)
        {
            if (input == "escape")
            {
                Debug.Log("Escape Pressed");
                if (_isPaused)
                {
                    Debug.Log("Resumed White State");
                }
                else
                {
                    Debug.Log("Paused White State");
                    _isPaused = true;
                    if (_targetStones != null) _manager.ChangeState(new BlackState(_manager));
                }
            }
        }

        private (int, int)[] GetTargetStones()
        {
            var mcts = new Mcts(_manager.policy, _manager.value);
            var game = new TriminoMok(_manager.GameBoard, _stoneType, _manager.PrevActions, _manager.currentTurns - 1);

            var move = mcts.Run(game, 50);
            return TriminoMok.GetStones(move.Item1, move.Item2, move.Item3, _stoneType);
        }
    }
}