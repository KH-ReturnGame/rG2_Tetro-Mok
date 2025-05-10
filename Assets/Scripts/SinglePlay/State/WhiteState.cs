using UnityEngine;

namespace SinglePlay.State
{
    public class WhiteState : IState
    {
        private readonly GameManager _manager;
        private readonly float _waitingTime;

        // private Thread _workerThread;
        // private Thread _cnnThread;
        // private bool _endProgress;

        private GameObject _parent, _whiteStoneNew;

        // private SharedVars _progressVars;
        private (int, int)[] _targetStones;
        private float _timer;

        public WhiteState(GameManager manager)
        {
            _manager = manager;
            _waitingTime = _manager.waitingTime;
        }

        public void OnEnter()
        {
            Debug.Log("Entered White State");
            // var game = new TriminoMok(_manager.GameBoard, Random.Range(1, 4));
            // var mcts = new Mcts();
            //
            // SliderController.SetScrollbarVisible(true);
            //
            // _progressVars = new SharedVars();
            //
            // _workerThread = new Thread(() => WorkerThread(mcts, game));
            // _workerThread.Start();

            _targetStones = Cnn.Forward(_manager.GameBoard, 2, Random.Range(1, 4));
        }

        public void OnExit()
        {
            // SliderController.SetScrollbarValue(0f);
            // SliderController.SetScrollbarVisible(false);
            // if (_workerThread != null) _workerThread.Join();

            _manager.PutStones(_targetStones, 2);
            Debug.Log("Exited White State");
        }

        public void Update()
        {
            //     lock (_progressVars)
            //     {
            //         if (_progressVars.IsChanged)
            //         {
            //             SliderController.SetScrollbarValue(_progressVars.Progress);
            //             _progressVars.IsChanged = false;
            //         }
            //     }
            //
            //     if (_endProgress) _manager.ChangeState(new BlackState(_manager));
            _timer += Time.deltaTime;

            if (_targetStones.Length == 3 && _timer >= _waitingTime) _manager.ChangeState(new BlackState(_manager));
        }

        public void HandleInput(string input)
        {
            if (input == "escape") Debug.Log("Escape Pressed");
            // if (_workerThread.IsAlive)
            // {
            //     Debug.Log("Stopping worker thread");
            //
            //     SliderController.SetScrollbarValue(0f);
            //     SliderController.SetScrollbarVisible(false);
            //
            //     _workerThread.Abort();
            // }
            // else
            // {
            //     var game = new TriminoMok(_manager.GameBoard, Random.Range(1, 4));
            //     var mcts = new Mcts();
            //
            //     SliderController.SetScrollbarVisible(true);
            //
            //     _workerThread = new Thread(() => WorkerThread(mcts, game));
            //     _workerThread.Start();
            // }
        }

        // private void WorkerThread(Mcts mcts, TriminoMok game)
        // {
        //     var move = mcts.Run(game, 1000, _progressVars);
        //
        //     _targetStones = game.GetStones(move.Item1, move.Item2, move.Item3);
        //     _endProgress = true;
        // }
        //
        // public class SharedVars
        // {
        //     public bool IsChanged;
        //     public float Progress;
        // }
    }
}