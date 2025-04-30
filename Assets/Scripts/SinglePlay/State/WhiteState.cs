using System.Threading;
using UnityEngine;

namespace SinglePlay.State
{
    public class WhiteState : IState
    {
        private readonly GameManager _manager;
        private bool _endProgress;
        private GameObject _parent, _whiteStoneNew;
        private SharedVars _progressVars;
        private (int, int)[] _targetStones;

        private Thread _workerThread;

        public WhiteState(GameManager manager)
        {
            _manager = manager;
        }

        public void OnEnter()
        {
            Debug.Log("Entered White State");
            var game = new TriminoMok(_manager.GameBoard, Random.Range(1, 4));
            var mcts = new Mcts();

            SliderController.SetScrollbarVisible(true);

            _progressVars = new SharedVars();
            _workerThread = new Thread(() => WorkerThread(mcts, game));
            _workerThread.Start();
        }

        public void OnExit()
        {
            SliderController.SetScrollbarVisible(false);
            if (_workerThread != null) _workerThread.Join();
            _manager.PutStones(_targetStones, 2);
            Debug.Log("Exited White State");
        }

        public void Update()
        {
            lock (_progressVars)
            {
                if (_progressVars.IsChanged)
                {
                    SliderController.SetScrollbarValue(_progressVars.Progress);
                    _progressVars.IsChanged = false;
                }
            }

            if (_endProgress) _manager.ChangeState(new BlackState(_manager));
        }

        public void HandleInput(string input)
        {
        }

        private void WorkerThread(Mcts mcts, TriminoMok game)
        {
            var move = mcts.Run(game, 30, _progressVars);

            _targetStones = game.GetStones(move.Item1, move.Item2, move.Item3);
            _endProgress = true;
        }

        public class SharedVars
        {
            public bool IsChanged;
            public float Progress = 0f;
        }
    }
}