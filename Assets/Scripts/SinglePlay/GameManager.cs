using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using SinglePlay.State;
using TMPro;
using UnityEngine;

namespace SinglePlay
{
    public class GameManager : MonoBehaviour
    {
        [Header("Stone Prefabs")] [SerializeField]
        public GameObject blackStone;

        public GameObject whiteStone;
        public GameObject blackStoneNew;
        public GameObject blackStoneError;
        public GameObject bonusStone;

        [Space(10)] [Header("Sprites")] [SerializeField]
        public Sprite blackNormal;

        public Sprite whiteNormal;
        public Sprite blackChecking;
        public Sprite whiteChecking;
        public Sprite bonusNormal;
        public Sprite bonusChecking;

        [Space(10)] [Header("UI Objects")] [SerializeField]
        public GameObject pauseScreen;

        public GameObject gameEndScreen;
        public GameObject currentStones;
        public GameObject prevStones;

        [Space(10)] [Header("TMPs")] [SerializeField]
        public TextMeshProUGUI blackScoreText;

        public TextMeshProUGUI whiteScoreText;
        public TextMeshProUGUI resultText;

        [Space(10)] [Header("Game Settings")] [SerializeField]
        private float holdThreshold;

        [SerializeField] private float interval;
        [SerializeField] private int maxTurns;
        public float waitingTime;

        private IState _currentState;
        private int _currentTurns;

        private bool _isPaused, _isGameEnded;

        public int[,] GameBoard;
        public int BlackScore { get; private set; }

        public int WhiteScore { get; private set; }

        private void Start()
        {
            _currentState = new GameStartState(this);
            _currentState.OnEnter();
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                if (!_isPaused)
                {
                    if (Input.GetKeyDown(KeyCode.Return)) _currentState.HandleInput("ok");
                    if (Input.GetKeyDown(KeyCode.R)) _currentState.HandleInput("rotate");
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                        StartCoroutine(CheckHoldTime(KeyCode.UpArrow, "up"));

                    if (Input.GetKeyDown(KeyCode.DownArrow))
                        StartCoroutine(CheckHoldTime(KeyCode.DownArrow, "down"));

                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                        StartCoroutine(CheckHoldTime(KeyCode.LeftArrow, "left"));

                    if (Input.GetKeyDown(KeyCode.RightArrow))
                        StartCoroutine(CheckHoldTime(KeyCode.RightArrow, "right"));
                    if (Input.GetKeyDown(KeyCode.W))
                        StartCoroutine(CheckHoldTime(KeyCode.W, "w"));

                    if (Input.GetKeyDown(KeyCode.A))
                        StartCoroutine(CheckHoldTime(KeyCode.A, "a"));

                    if (Input.GetKeyDown(KeyCode.S))
                        StartCoroutine(CheckHoldTime(KeyCode.S, "s"));

                    if (Input.GetKeyDown(KeyCode.D))
                        StartCoroutine(CheckHoldTime(KeyCode.D, "d"));
                }

                if (Input.GetKeyDown(KeyCode.Escape) && !_isGameEnded)
                {
                    _currentState.HandleInput("escape");
                    PauseResume();
                }
            }


            _currentState.Update();
        }


        private IEnumerator CheckHoldTime(KeyCode key, string moveDirection)
        {
            _currentState.HandleInput(moveDirection);
            var holdTime = 0f;

            while (holdTime < holdThreshold && Input.GetKey(key))
            {
                holdTime += Time.deltaTime;
                yield return null;
            }

            if (!Input.GetKey(key)) yield break;

            while (Input.GetKey(key))
            {
                SendResumeSignal();
                yield return new WaitForSeconds(interval);
            }
        }

        public void PauseResume()
        {
            _isPaused = !_isPaused;
            pauseScreen.SetActive(_isPaused);
            SoundManager.PauseResumeBgm();
            SoundManager.PlaySound("UI");
        }

        public void UpdateScores(int black, int white)
        {
            BlackScore += black;
            WhiteScore += white;
            blackScoreText.text = "흑: " + BlackScore;
            whiteScoreText.text = "백: " + WhiteScore;
        }

        /// <summary>
        ///     전체 착수 횟수를 업데이트
        /// </summary>
        /// <returns>
        ///     게임 종료 여부를 반환
        ///     <list>
        ///         <item>false: 계속</item><item>true: 게임 종료</item>
        ///     </list>
        /// </returns>
        private bool IncreaseTurns()
        {
            if (++_currentTurns > maxTurns) return true;
            return false;
        }

        public void ChangeState(IState newState)
        {
            if (IncreaseTurns())
            {
                PauseResume();
                EndGame();
            }
            else
            {
                _currentState.OnExit();
                _currentState = newState;
                _currentState.OnEnter();
            }
        }

        public void SendResumeSignal()
        {
            _currentState.HandleInput("escape");
        }

        public void EndGame()
        {
            PauseResume();
            _isGameEnded = true;
            _currentState.OnExit();
            _currentState = new EndGameState(this, GameBoard);
            _currentState.OnEnter();
        }

        public void QuitGame()
        {
            SceneHandler.LoadScene("TitleScene");
        }

        public GameObject InstantiateObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Instantiate(prefab, position, rotation);
        }

        public void DestroyObject(GameObject target)
        {
            Destroy(target);
        }

        public void SetGameEndScreen(int whoWins)
        {
            switch (whoWins)
            {
                case 0:
                    resultText.text = "비김!";
                    break;
                case 1:
                    resultText.text = "바둑이 승리!";
                    break;
                case 2:
                    resultText.text = "흰둥이 승리!";
                    break;
            }

            SoundManager.PlaySound("GameWin");
            gameEndScreen.SetActive(true);
        }

        /// <summary>
        ///     돌을 배치 및 렌더링하고, 연결 상태를 확인한다.
        /// </summary>
        /// <param name="stonesToPut">배치할 돌의 좌표</param>
        /// <param name="player">돌 종류 [흑/백]</param>
        public void PutStones((int, int)[] stonesToPut, int player)
        {
            // 돌 배치 및 렌더링
            foreach (var (i, j) in stonesToPut)
            {
                GameObject stone;
                if (Random.Range(0, 15) == 0)
                {
                    GameBoard[i, j] = 3;
                    stone = Instantiate(bonusStone, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0),
                        Quaternion.identity);
                }
                else if (player == 1)
                {
                    GameBoard[i, j] = 1;
                    stone = Instantiate(blackStone, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0),
                        Quaternion.identity);
                }
                else
                {
                    GameBoard[i, j] = 2;
                    stone = Instantiate(whiteStone, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0),
                        Quaternion.identity);
                }

                stone.name = i + "_" + j;
                stone.transform.SetParent(prevStones.transform);
            }

            SoundManager.PlaySound("Put");
            CheckConnections(stonesToPut, player);
        }

        /// <summary>
        ///     이번 턴에 놓아진 돌에서부터 가로, 세로, 대각선으로 10개 이상 연결된 돌이 있는지 판정한다.
        /// </summary>
        /// <param name="getCurrentStones"></param>
        /// <param name="stoneType"></param>
        private void CheckConnections((int, int)[] getCurrentStones, int stoneType)
        {
            var stonesToRemove = new List<(int, int)>();
            var deletedLines = 0;

            // 가로 탐색
            var checkedY = new List<int>();
            foreach (var (i, j) in getCurrentStones)
            {
                if (checkedY.Contains(j)) continue;
                checkedY.Add(j);

                var checkingStones = new List<(int, int)>();

                int xLength = 1, currentX = i;
                checkingStones.Add((i, j));

                while (true)
                {
                    if (currentX == 0) break;
                    if (GameBoard[--currentX, j] == stoneType || GameBoard[currentX, j] == 3)
                    {
                        xLength++;
                        checkingStones.Add((currentX, j));
                    }
                    else
                    {
                        break;
                    }
                }

                currentX = i;
                while (true)
                {
                    if (currentX == 18) break;
                    if (GameBoard[++currentX, j] == stoneType || GameBoard[currentX, j] == 3)
                    {
                        xLength++;
                        checkingStones.Add((currentX, j));
                    }
                    else
                    {
                        break;
                    }
                }

                if (xLength >= 10)
                {
                    Debug.Log("Complete Line!");
                    deletedLines++;
                    stonesToRemove = stonesToRemove.Union(checkingStones).ToList();
                }
            }

            // 세로 탐색
            var checkedX = new List<int>();
            foreach (var (i, j) in getCurrentStones)
            {
                if (checkedX.Contains(i)) continue;
                checkedX.Add(i);

                var checkingStones = new List<(int, int)>();

                int yLength = 1, currentY = j;
                checkingStones.Add((i, j));

                while (true)
                {
                    if (currentY == 0) break;
                    if (GameBoard[i, --currentY] == stoneType || GameBoard[i, currentY] == 3)
                    {
                        yLength++;
                        checkingStones.Add((i, currentY));
                    }
                    else
                    {
                        break;
                    }
                }

                currentY = j;
                while (true)
                {
                    if (currentY == 18) break;
                    if (GameBoard[i, ++currentY] == stoneType || GameBoard[i, currentY] == 3)
                    {
                        yLength++;
                        checkingStones.Add((i, currentY));
                    }
                    else
                    {
                        break;
                    }
                }

                if (yLength >= 10)
                {
                    Debug.Log("Complete Line!");
                    deletedLines++;
                    stonesToRemove = stonesToRemove.Union(checkingStones).ToList();
                }
            }

            // x - y = constant 대각선 탐색
            var checkedXmY = new List<int>();
            foreach (var (i, j) in getCurrentStones)
            {
                if (checkedXmY.Contains(i - j)) continue;
                checkedXmY.Add(i - j);
                var checkingStones = new List<(int, int)>();

                int diagLength = 1, currentX = i, currentY = j;
                checkingStones.Add((i, j));

                while (true)
                {
                    if (currentX == 0 || currentY == 0) break;
                    if (GameBoard[--currentX, --currentY] == stoneType ||
                        GameBoard[currentX, currentY] == 3)
                    {
                        diagLength++;
                        checkingStones.Add((currentX, currentY));
                    }
                    else
                    {
                        break;
                    }
                }

                currentX = i;
                currentY = j;
                while (true)
                {
                    if (currentX == 18 || currentY == 18) break;
                    if (GameBoard[++currentX, ++currentY] == stoneType ||
                        GameBoard[currentX, currentY] == 3)
                    {
                        diagLength++;
                        checkingStones.Add((currentX, currentY));
                    }
                    else
                    {
                        break;
                    }
                }

                if (diagLength >= 10)
                {
                    Debug.Log("Complete Line!");
                    deletedLines++;
                    stonesToRemove = stonesToRemove.Union(checkingStones).ToList();
                }
            }

            // x + y = constant 대각선 탐색
            var checkedXpY = new List<int>();
            foreach (var (i, j) in getCurrentStones)
            {
                if (checkedXpY.Contains(i + j)) continue;
                checkedXpY.Add(i + j);
                var checkingStones = new List<(int, int)>();

                int diagLength = 1, currentX = i, currentY = j;
                checkingStones.Add((i, j));

                while (true)
                {
                    if (currentX == 0 || currentY == 18) break;
                    if (GameBoard[--currentX, ++currentY] == stoneType ||
                        GameBoard[currentX, currentY] == 3)
                    {
                        diagLength++;
                        checkingStones.Add((currentX, currentY));
                    }
                    else
                    {
                        break;
                    }
                }

                currentX = i;
                currentY = j;
                while (true)
                {
                    if (currentX == 18 || currentY == 0) break;
                    if (GameBoard[++currentX, --currentY] == stoneType ||
                        GameBoard[currentX, currentY] == 3)
                    {
                        diagLength++;
                        checkingStones.Add((currentX, currentY));
                    }
                    else
                    {
                        break;
                    }
                }

                if (diagLength >= 10)
                {
                    Debug.Log("Complete Line!");
                    deletedLines++;
                    stonesToRemove = stonesToRemove.Union(checkingStones).ToList();
                }
            }

            if (stoneType == 1)
                UpdateScores(deletedLines * 10, 0);
            else
                UpdateScores(0, deletedLines * 10);

            if (deletedLines > 0) SoundManager.PlaySound("Remove");

            foreach (var (i, j) in stonesToRemove)
            {
                if (GameBoard[i, j] == 3)
                {
                    if (stoneType == 1)
                        UpdateScores(3, 0);
                    else
                        UpdateScores(0, 3);
                }

                GameBoard[i, j] = 0;
                Destroy(GameObject.Find(i + "_" + j));
            }
        }

        public GameObject GetStoneByPos(int x, int y)
        {
            return GameObject.Find(x + "_" + y);
        }

        public void StartEffectCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }
    }
}