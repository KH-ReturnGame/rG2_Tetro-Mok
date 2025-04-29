using UnityEngine;

namespace SinglePlay.State
{
    public class WhiteState : IState
    {
        private readonly GameManager _manager;

        private GameObject _parent, _whiteStoneNew;
        private (int, int)[] _targetStones;

        public WhiteState(GameManager manager)
        {
            _manager = manager;
        }

        public void OnEnter()
        {
            Debug.Log("Entered White State");

            var game = new TriminoMok(_manager.GameBoard, Random.Range(1, 4));
            var mcts = new Mcts();

            var move = mcts.Run(game);
            _targetStones = game.GetStones(move.Item1, move.Item2, move.Item3);

            _manager.ChangeState(new BlackState(_manager));
        }

        public void OnExit()
        {
            _manager.PutStones(_targetStones, 2);
            Debug.Log("Exited White State");
        }

        public void Update()
        {
        }

        public void HandleInput(string input)
        {
        }
    }
}