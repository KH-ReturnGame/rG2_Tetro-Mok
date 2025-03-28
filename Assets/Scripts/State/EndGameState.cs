using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class EndGameState : IState
    {
        private readonly int[,] _gameBoard;
        private readonly GameManager _manager;

        public EndGameState(GameManager manager, int[,] gameBoard)
        {
            _manager = manager;
            _gameBoard = gameBoard;
        }

        public void OnEnter()
        {
            Debug.Log("End Game State Entered");
            var stonesToAnimate = new List<List<Vector2Int>>();

            // TODO: 대각선 및 보너스 돌로 시작할 때 처리 필요
            // TODO: 점수 계산 필요

            // 가로 연결 탐색
            for (var y = 18; y >= 0; y--)
            {
                int checkingStoneType = 0, connectionLength = 0;
                for (var x = 0; x < 19; x++)
                {
                    if (_gameBoard[x, y] == 0)
                    {
                        if (connectionLength >= 5)
                        {
                            var currentConnections = new List<Vector2Int>();
                            for (var i = connectionLength; i > 0; i--)
                                currentConnections.Add(new Vector2Int(x - i, y));
                            stonesToAnimate.Add(currentConnections);
                        }

                        checkingStoneType = 0;
                        connectionLength = 0;
                        continue;
                    }

                    // 첫 돌이 놓여있을 때
                    if (checkingStoneType == 0)
                    {
                        checkingStoneType = _gameBoard[x, y];
                        connectionLength++;
                    }
                    // 같은 색 돌이 놓여있을 때
                    else if (_gameBoard[x, y] == checkingStoneType || _gameBoard[x, y] == 3)
                    {
                        connectionLength++;
                    }
                    // 다른 색 돌이 놓여있을때
                    else
                    {
                        if (connectionLength >= 5)
                        {
                            var currentConnections = new List<Vector2Int>();
                            for (var i = connectionLength; i > 0; i--)
                                currentConnections.Add(new Vector2Int(x - i, y));
                            stonesToAnimate.Add(currentConnections);
                        }

                        checkingStoneType = _gameBoard[x, y];
                        connectionLength = 1;
                    }
                }
            }

            // 세로 연결 탐색
            for (var x = 0; x < 19; x++)
            {
                int checkingStoneType = 0, connectionLength = 0;
                for (var y = 18; y >= 0; y--)
                {
                    if (_gameBoard[x, y] == 0)
                    {
                        if (connectionLength >= 5)
                        {
                            var currentConnections = new List<Vector2Int>();
                            for (var i = connectionLength; i > 0; i--)
                                currentConnections.Add(new Vector2Int(x, y + i));
                            stonesToAnimate.Add(currentConnections);
                        }

                        checkingStoneType = 0;
                        connectionLength = 0;
                        continue;
                    }

                    // 첫 돌이 놓여있을 때
                    if (checkingStoneType == 0)
                    {
                        checkingStoneType = _gameBoard[x, y];
                        connectionLength++;
                    }
                    // 같은 색 돌이 놓여있을 때
                    else if (_gameBoard[x, y] == checkingStoneType || _gameBoard[x, y] == 3)
                    {
                        connectionLength++;
                    }
                    // 다른 색 돌이 놓여있을때
                    else
                    {
                        if (connectionLength >= 5)
                        {
                            var currentConnections = new List<Vector2Int>();
                            for (var i = connectionLength; i > 0; i--)
                                currentConnections.Add(new Vector2Int(x, y + i));
                            stonesToAnimate.Add(currentConnections);
                        }

                        checkingStoneType = _gameBoard[x, y];
                        connectionLength = 1;
                    }
                }
            }

            // x - y = constant 대각선 탐색
            for (var startX = 0; startX < 19; startX++)
            {
                int checkingStoneType = 0, connectionLength = 0;
                for (int x = startX, y = 0; x < 19 && y < 19; x++, y++)
                {
                    if (_gameBoard[x, y] == 0)
                    {
                        if (connectionLength >= 5)
                        {
                            var currentConnections = new List<Vector2Int>();
                            for (var i = connectionLength; i > 0; i--)
                                currentConnections.Add(new Vector2Int(x - i, y - i));
                            stonesToAnimate.Add(currentConnections);
                        }

                        checkingStoneType = 0;
                        connectionLength = 0;
                        continue;
                    }

                    if (checkingStoneType == 0)
                    {
                        checkingStoneType = _gameBoard[x, y];
                        connectionLength++;
                    }
                    else if (_gameBoard[x, y] == checkingStoneType || _gameBoard[x, y] == 3)
                    {
                        connectionLength++;
                    }
                    else
                    {
                        if (connectionLength >= 5)
                        {
                            var currentConnections = new List<Vector2Int>();
                            for (var i = connectionLength; i > 0; i--)
                                currentConnections.Add(new Vector2Int(x - i, y - i));
                            stonesToAnimate.Add(currentConnections);
                        }

                        checkingStoneType = _gameBoard[x, y];
                        connectionLength = 1;
                    }
                }
            }

            // x + y = constant 대각선 탐색
            for (var startX = 18; startX >= 0; startX--)
            {
                int checkingStoneType = 0, connectionLength = 0;
                for (int x = startX, y = 0; x >= 0 && y < 19; x--, y++)
                {
                    if (_gameBoard[x, y] == 0)
                    {
                        if (connectionLength >= 5)
                        {
                            var currentConnections = new List<Vector2Int>();
                            for (var i = 0; i < connectionLength; i++)
                                currentConnections.Add(new Vector2Int(x + i - 1, y - i + 1));
                            stonesToAnimate.Add(currentConnections);
                        }

                        checkingStoneType = 0;
                        connectionLength = 0;
                        continue;
                    }

                    if (checkingStoneType == 0)
                    {
                        checkingStoneType = _gameBoard[x, y];
                        connectionLength++;
                    }
                    else if (_gameBoard[x, y] == checkingStoneType || _gameBoard[x, y] == 3)
                    {
                        connectionLength++;
                    }
                    else
                    {
                        if (connectionLength >= 5)
                        {
                            var currentConnections = new List<Vector2Int>();
                            for (var i = connectionLength; i > 0; i--)
                                currentConnections.Add(new Vector2Int(x + i, y - i));
                            stonesToAnimate.Add(currentConnections);
                        }

                        checkingStoneType = _gameBoard[x, y];
                        connectionLength = 1;
                    }
                }
            }

            _manager.StartEffectCoroutine(ApplyEffectsToGroups(stonesToAnimate));
        }

        public void OnExit()
        {
            Debug.Log("Exited End Game State");
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                // 게임 종료
                _manager.QuitGame();
        }

        public void HandleInput(string input)
        {
        }

        private IEnumerator ApplyEffectsToGroups(List<List<Vector2Int>> groups)
        {
            foreach (var group in groups)
            {
                int stoneType = 0, bonusNumber = 0;

                foreach (var pos in group)
                {
                    var stone = _manager.GetStoneByPos(pos.x, pos.y);
                    if (stone != null)
                    {
                        if (stone.GetComponent<StoneType>().stoneType != 3)
                            stoneType = stone.GetComponent<StoneType>().stoneType;
                        else bonusNumber++;

                        // 효과 적용 (예: 색상 변경)
                        stone.GetComponent<SpriteRenderer>().color = Color.red; // TODO: 효과 변경
                        yield return new WaitForSeconds(0.05f);
                    }
                }

                if (stoneType == 1) _manager.UpdateScores(5 + bonusNumber * 3, 0);
                else _manager.UpdateScores(0, 5 + bonusNumber * 3);

                yield return new WaitForSeconds(0.5f);
            }

            if (_manager.blackScore > _manager.whiteScore)
            {
                Debug.Log("Black Wins!");
                _manager.SetGameEndScreen(1);
            }
            else if (_manager.blackScore < _manager.whiteScore)
            {
                Debug.Log("White Wins!");
                _manager.SetGameEndScreen(2);
            }
            else
            {
                Debug.Log("Draw!");
                _manager.SetGameEndScreen(0);
            }
        }
    }
}