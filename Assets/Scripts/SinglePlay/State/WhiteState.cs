using UnityEngine;

namespace SinglePlay.State
{
    public class WhiteState : IState
    {
        private readonly GameManager _manager;

        private GameObject _parent, _whiteStoneNew, _whiteStoneError;
        private (int, int)[] _targetStones;

        public WhiteState(GameManager manager)
        {
            _manager = manager;
        }

        public void OnEnter()
        {
            Debug.Log("Entered White State");

            var simulation = new Simulation(_manager.GameBoard, Random.Range(1, 4));
            _targetStones = simulation.GetResult();
        }

        public void OnExit()
        {
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