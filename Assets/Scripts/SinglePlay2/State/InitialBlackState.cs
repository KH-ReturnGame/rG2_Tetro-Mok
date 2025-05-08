using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SinglePlay2.State
{
    public class InitialBlackState : IState
    {
        private readonly GameManager _manager;
        bool _canLocate =true;
        private int[,] _currentStones;
        private MoveDirection _direction;
        private GameObject _parent, _blackStoneNew, _blackStoneError;
        private int move_count = 0;

        public InitialBlackState(GameManager manager)
        {
            _manager = manager;
        }

        private (int, int)[] TargetStones
        {
            get
            {
                var stones = new (int, int)[2];
                var count = 0;

                for (var i = 0; i < 19; i++)
                for (var j = 0; j < 19; j++)
                    if (_currentStones[i, j] == 1)
                        stones[count++] = (i, j);

                return stones;
            }
        }

        public void OnEnter()
        {
            Debug.Log("Entered Initial Black State");
            _blackStoneNew = _manager.blackStoneNew;
            _blackStoneError = _manager.blackStoneError;
            _parent = _manager.currentStones;
            _direction = MoveDirection.Idle;
            InitStones();
            
            if (_manager.BlackAI)
            {
                _manager.Black_Agent.status = AgentStatus.ReadyToChoose;
            }
            if (_manager.count >= _manager.max_count_)
            {
                _manager.ChangeState(new EndGameState(_manager,_manager.GameBoard));
                //if(_manager.max_count <= 50) _manager.max_count += 0.5;
                return;
            }
        }

        public void OnExit()
        {
            foreach (Transform stone in _parent.transform) _manager.DestroyObject(stone.gameObject);
            if (_canLocate) _manager.PutStones(TargetStones, 1);
            Debug.Log("Exited Initial Black State");
            _manager.count++;
            move_count = 0;
        }

        public void Update()
        {
            if (_direction != MoveDirection.Idle)
            {
                Debug.Log("Move Stones; Direction: " + _direction);
                MoveStones();
                _direction = MoveDirection.Idle;
            }
        }

        public void HandleInput(string input)
        {
            switch (input)
            {
                case "up":
                    _direction = MoveDirection.Up;
                    _manager.Black_Agent.status = AgentStatus.Working;
                    _manager.Black_Agent.AddReward(-0.1f*move_count);
                    move_count++;
                    break;
                case "down":
                    _direction = MoveDirection.Down;
                    _manager.Black_Agent.status = AgentStatus.Working;
                    _manager.Black_Agent.AddReward(-0.1f*move_count);
                    move_count++;
                    break;
                case "left":
                    _direction = MoveDirection.Left;
                    _manager.Black_Agent.status = AgentStatus.Working;
                    _manager.Black_Agent.AddReward(-0.1f*move_count);
                    move_count++;
                    break;
                case "right":
                    _direction = MoveDirection.Right;
                    _manager.Black_Agent.status = AgentStatus.Working;
                    _manager.Black_Agent.AddReward(-0.1f*move_count);
                    move_count++;
                    break;
                case "rotate":
                    _direction = MoveDirection.Rotate;
                    _manager.Black_Agent.status = AgentStatus.Working;
                    _manager.Black_Agent.AddReward(-0.1f*move_count);
                    move_count++;
                    break;
                case "ok":
                    if (!_canLocate)
                    {
                        _manager.Black_Agent.AddReward(-5f);
                        _manager.Black_Agent.status = AgentStatus.ReadyToChoose;
                        Debug.Log("Can't put stone on others");
                        break;
                    }

                    _manager.Black_Agent.status = AgentStatus.Working;
                    _manager.Black_Agent.AddReward(4f);
                    // 다음 차례
                    _manager.ChangeState(new WhiteState(_manager));
                    break;
                case "escape": break;
            }
        }

        /// <summary>
        ///     생성할 현재 턴의 돌의 배열을 초기화한다.
        /// </summary>
        /// <list type="number">
        ///     <item>
        ///         <description>ㅡ 트리미노</description>
        ///     </item>
        ///     <item>
        ///         <description>ㄴ 트리미노</description>
        ///     </item>
        ///     <item>
        ///         <description>/ 트리미노</description>
        ///     </item>
        /// </list>
        private void InitStones()
        {
            var type = Random.Range(1, 3);
            Debug.Log("Creating Stones; Stone Type:" + type);

            _currentStones = new int[19, 19];
            switch (type)
            {
                case 1:
                    _currentStones[8, 9] = 1;
                    _currentStones[9, 9] = 1;
                    break;
                case 2:
                    _currentStones[8, 9] = 1;
                    _currentStones[9, 8] = 1;
                    break;
            }
            
            _manager.CheckEndGame(_currentStones);
            if (_manager.BlackAI)
            {
                _manager.Black_Agent.status = AgentStatus.ReadyToChoose;
            }
            RenderStones();
        }

        /// <summary>
        ///     이동 방향에 맞춰 배열 변경 및 렌더링
        /// </summary>
        private void MoveStones()
        {
            // 현재 도형의 형태 지정
            int minX = 19, maxX = -1, minY = 19, maxY = -1;
            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
                if (_currentStones[i, j] == 1)
                {
                    if (i < minX) minX = i;
                    if (i > maxX) maxX = i;
                    if (j < minY) minY = j;
                    if (j > maxY) maxY = j;
                }

            var height = maxY - minY + 1;
            var width = maxX - minX + 1;
            var shape = new int[width, height];

            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                shape[i, j] = _currentStones[minX + i, minY + j];

            // 보드 초기화
            _currentStones = new int[19, 19];

            // 회전하는 경우
            if (_direction == MoveDirection.Rotate)
            {
                // 시계 방향 90도 회전
                var rotatedShape = new int[height, width];
                for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    rotatedShape[j, width - i - 1] = shape[i, j];

                // 회전된 도형을 원래 위치에 맞춰 배치
                int offsetX = 0, offsetY = 0;

                if (minX + height - 1 > 18) offsetX = 18 - (minX + height - 1);
                if (minY + width - 1 > 18) offsetY = 18 - (minY + width - 1);

                for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                    _currentStones[minX + i + offsetX, minY + j + offsetY] = rotatedShape[i, j];
            }
            // 상하좌우로 이동하는 경우
            else
            {
                int moveX = 0, moveY = 0;
                if (_direction == MoveDirection.Up && maxY < 18) moveY = 1;
                else if (_direction == MoveDirection.Down && minY > 0) moveY = -1;
                else if (_direction == MoveDirection.Left && minX > 0) moveX = -1;
                else if (_direction == MoveDirection.Right && maxX < 18) moveX = 1;

                for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    _currentStones[minX + i + moveX, minY + j + moveY] = shape[i, j];
            }

            RenderStones();
        }


        private void RenderStones()
        {
            // 기존 돌 삭제
            foreach (Transform stone in _parent.transform) _manager.DestroyObject(stone.gameObject);

            foreach (var (i, j) in TargetStones)
                if (_currentStones[i, j] == 1)
                {
                    _canLocate = true; // 초기화
                    GameObject stone;

                    // 기존에 놓인 돌이 없을 때
                    if (_manager.GameBoard[i, j] == 0)
                    {
                        stone = _manager.InstantiateObject(_blackStoneNew,
                            new Vector3((i - 9) * 0.5f+_manager.center.position.x, (j - 9) * 0.5f+_manager.center.position.y, 0), Quaternion.identity);
                        //_manager.Black_Agent.AddReward(1f);
                    }
                    // 뭔가 놓여있을 때
                    else
                    {
                        stone = _manager.InstantiateObject(_blackStoneError,
                            new Vector3((i - 9) * 0.5f+_manager.center.position.x, (j - 9) * 0.5f+_manager.center.position.y, 0), Quaternion.identity);
                        _manager.Black_Agent.AddReward(-1f);
                        _canLocate = false;
                    }

                    stone.transform.SetParent(_parent.transform);
                }
            if (_manager.BlackAI)
            {
                _manager.Black_Agent.status = AgentStatus.ReadyToChoose;
            }
        }
    }
}