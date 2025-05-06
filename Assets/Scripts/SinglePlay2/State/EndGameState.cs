using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

namespace SinglePlay2.State
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
            SoundManager.PauseResumeBgm();
            var stonesToAnimate = new List<List<Vector2Int>>();

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
                        if (_gameBoard[x, y] != 3) checkingStoneType = _gameBoard[x, y];
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
                        if (_gameBoard[x, y] != 3) checkingStoneType = _gameBoard[x, y];
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

            // 왼쪽 위에서 오른쪽 아래 대각선 탐색
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
                        if (_gameBoard[x, y] != 3) checkingStoneType = _gameBoard[x, y];
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

            for (var startY = 1; startY < 19; startY++)
            {
                int checkingStoneType = 0, connectionLength = 0;
                for (int x = 0, y = startY; x < 19 && y < 19; x++, y++)
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
                        if (_gameBoard[x, y] != 3) checkingStoneType = _gameBoard[x, y];
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

            // 오른쪽 위에서 왼쪽 아래 대각선 탐색
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
                            for (var i = connectionLength; i > 0; i--)
                                currentConnections.Add(new Vector2Int(x + i, y - i));
                            stonesToAnimate.Add(currentConnections);
                        }

                        checkingStoneType = 0;
                        connectionLength = 0;
                        continue;
                    }

                    if (checkingStoneType == 0)
                    {
                        if (_gameBoard[x, y] != 3) checkingStoneType = _gameBoard[x, y];
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

            for (var startY = 1; startY < 19; startY++)
            {
                int checkingStoneType = 0, connectionLength = 0;
                for (int x = 18, y = startY; x >= 0 && y < 19; x--, y++)
                {
                    if (_gameBoard[x, y] == 0)
                    {
                        if (connectionLength >= 5)
                        {
                            var currentConnections = new List<Vector2Int>();
                            for (var i = connectionLength; i > 0; i--)
                                currentConnections.Add(new Vector2Int(x + i, y - i));
                            stonesToAnimate.Add(currentConnections);
                        }

                        checkingStoneType = 0;
                        connectionLength = 0;
                        continue;
                    }

                    if (checkingStoneType == 0)
                    {
                        if (_gameBoard[x, y] != 3) checkingStoneType = _gameBoard[x, y];
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
            if (_manager.BlackAI && _manager.WhiteAI)
            {
                foreach (var group in groups)
                {
                    int stoneType = 0, bonusNumber = 0;

                    foreach (var pos in group)
                    {
                        //SoundManager.PlaySound("Point");
                        var stone = _manager.GetStoneByPos(pos.x, pos.y);
                        if (stone != null)
                        {
                            if (stone.GetComponent<StoneType>().stoneType == 1)
                            {
                                stoneType = 1;
                                //stone.GetComponent<SpriteRenderer>().sprite = _manager.blackChecking;
                            }
                            else if (stone.GetComponent<StoneType>().stoneType == 2)
                            {
                                stoneType = 2;
                                //stone.GetComponent<SpriteRenderer>().sprite = _manager.whiteChecking;
                            }
                            else
                            {
                                bonusNumber++;
                                //stone.GetComponent<SpriteRenderer>().sprite = _manager.bonusChecking;
                            }

                            //yield return new WaitForSeconds(0.01f);
                        }
                    }

                    if (stoneType == 1) _manager.UpdateScores(5 + bonusNumber * 3, 0);
                    else _manager.UpdateScores(0, 5 + bonusNumber * 3);

                    //yield return new WaitForSeconds(0.5f);

                    foreach (var pos in group)
                    {
                        var stone = _manager.GetStoneByPos(pos.x, pos.y);
                        if (stone != null)
                        {
                            if (stone.GetComponent<StoneType>().stoneType == 1)
                                stone.GetComponent<SpriteRenderer>().sprite = _manager.blackNormal;
                            else if (stone.GetComponent<StoneType>().stoneType == 2)
                                stone.GetComponent<SpriteRenderer>().sprite = _manager.whiteNormal;
                            else stone.GetComponent<SpriteRenderer>().sprite = _manager.bonusNormal;
                        }
                    }
                }

                //yield return new WaitForSeconds(2f);

                if (_manager.BlackScore > _manager.WhiteScore)
                {
                    Debug.Log("바둑이 승리!");
                    _manager.Black_Agent.AddReward(100f);
                }

                else if (_manager.BlackScore < _manager.WhiteScore)
                {
                    Debug.Log("흰둥이 승리!");
                    _manager.White_Agent.AddReward(100f);
                }
                else
                {
                    Debug.Log("비김!");
                    _manager.Black_Agent.AddReward(-5f);
                    _manager.White_Agent.AddReward(-5f);
                }
                _manager.Black_Agent.EndEpisode();
                _manager.White_Agent.EndEpisode();
            }
            else
            {
                foreach (var group in groups)
                {
                    int stoneType = 0, bonusNumber = 0;

                    foreach (var pos in group)
                    {
                        SoundManager.PlaySound("Point");
                        var stone = _manager.GetStoneByPos(pos.x, pos.y);
                        if (stone != null)
                        {
                            if (stone.GetComponent<StoneType>().stoneType == 1)
                            {
                                stoneType = 1;
                                stone.GetComponent<SpriteRenderer>().sprite = _manager.blackChecking;
                            }
                            else if (stone.GetComponent<StoneType>().stoneType == 2)
                            {
                                stoneType = 2;
                                stone.GetComponent<SpriteRenderer>().sprite = _manager.whiteChecking;
                            }
                            else
                            {
                                bonusNumber++;
                                stone.GetComponent<SpriteRenderer>().sprite = _manager.bonusChecking;
                            }

                            yield return new WaitForSeconds(0.01f);
                        }
                    }

                    if (stoneType == 1) _manager.UpdateScores(5 + bonusNumber * 3, 0);
                    else _manager.UpdateScores(0, 5 + bonusNumber * 3);

                    yield return new WaitForSeconds(0.5f);

                    foreach (var pos in group)
                    {
                        var stone = _manager.GetStoneByPos(pos.x, pos.y);
                        if (stone != null)
                        {
                            if (stone.GetComponent<StoneType>().stoneType == 1)
                                stone.GetComponent<SpriteRenderer>().sprite = _manager.blackNormal;
                            else if (stone.GetComponent<StoneType>().stoneType == 2)
                                stone.GetComponent<SpriteRenderer>().sprite = _manager.whiteNormal;
                            else stone.GetComponent<SpriteRenderer>().sprite = _manager.bonusNormal;
                        }
                    }
                }

                yield return new WaitForSeconds(2f);

                if (_manager.BlackScore > _manager.WhiteScore)
                {
                    Debug.Log("바둑이 승리!");
                    _manager.SetGameEndScreen(1);
                }

                else if (_manager.BlackScore < _manager.WhiteScore)
                {
                    Debug.Log("흰둥이 승리!");
                    _manager.SetGameEndScreen(2);
                }
                else
                {
                    Debug.Log("비김!");
                    _manager.SetGameEndScreen(0);
                }
            }
            
            
        }
    }
}