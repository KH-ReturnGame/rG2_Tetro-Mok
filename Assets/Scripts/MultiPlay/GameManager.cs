using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using MultiPlay.State;
using TMPro;
using UnityEngine;

namespace MultiPlay
{
    public class GameManager : MonoBehaviour
    {
        [Header("Stone Prefabs")] [SerializeField]
        public GameObject blackStone;

        public GameObject whiteStone;
        public GameObject blackStoneNew;
        public GameObject whiteStoneNew;
        public GameObject blackStoneError;
        public GameObject whiteStoneError;
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
        public float holdThreshold = 0.7f;
        public float interval = 0.05f;

        private IState _currentState;

        private bool _isPaused, _isGameEnded;

        public int[,] GameBoard;
        public int BlackScore { get; private set; }

        public int WhiteScore { get; private set; }
        
        public enum MoveDirection
        {
            Up,
            Down,
            Left,
            Right,
        }

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

                if (Input.GetKeyDown(KeyCode.Escape) && !_isGameEnded) PauseResume();
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
                _currentState.HandleInput(moveDirection);
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

        public void ChangeState(IState newState)
        {
            _currentState.OnExit();
            _currentState = newState;
            _currentState.OnEnter();
        }

        // 플레이를 중지하고 점수 산정 진행
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
        /// <param name="stoneType">돌 종류 [흑/백]</param>
        public void PutStones((int, int)[] stonesToPut, int stoneType)
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
                else if (stoneType == 1)
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

            SoundManager.PlaySound(""); // TODO: Put Stone Sound
            CheckConnections(stonesToPut, stoneType);
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

        /// <summary>
        /// 현재 조각(pieceMatrix)이 보드(GameBoard)에 더 이상 놓을 수 없는지를 판정하고,
        /// 놓을 수 없으면 게임 종료 상태로 전환합니다.
        /// </summary>
        /// <param name="pieceMatrix">
        /// 19×19 크기로, 조각 모양에 해당하는 칸만 1(또는 2)이며 나머지는 0인 배열</param>
        public void CheckEndGame(int[,] pieceMatrix)
        {
            // 1) 조각의 최소 바운딩 박스(crop) 추출
            int minX = 19, maxX = -1, minY = 19, maxY = -1;
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 19; j++)
                    if (pieceMatrix[i, j] != 0)
                    {
                        minX = Mathf.Min(minX, i);
                        maxX = Mathf.Max(maxX, i);
                        minY = Mathf.Min(minY, j);
                        maxY = Mathf.Max(maxY, j);
                    }
            // 조각이 아예 없으면(비정상) 바로 종료
            if (maxX < minX || maxY < minY)
            {
                ChangeState(new EndGameState(this, GameBoard));
                return;
            }
            int width  = maxX - minX + 1;
            int height = maxY - minY + 1;

            // 2) 바운딩 박스로 조각만 추출
            int[,] shape = new int[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    shape[x, y] = pieceMatrix[minX + x, minY + y];

            // 3) 4회전 + 모든 보드 위치에 대해 충돌 검사
            for (int rot = 0; rot < 4; rot++)
            {
                // 회전된 shape와 크기 얻기
                var (rotShape, rotW, rotH) = RotateShape90(shape, width, height, rot);

                // 보드 위에 놓을 수 있는 좌표 한계
                int maxOffsetX = 19 - rotW;
                int maxOffsetY = 19 - rotH;

                for (int offY = 0; offY <= maxOffsetY; offY++)
                {
                    for (int offX = 0; offX <= maxOffsetX; offX++)
                    {
                        bool canPlace = true;
                        // 각 셀이 비어 있는지 확인
                        for (int x = 0; x < rotW && canPlace; x++)
                            for (int y = 0; y < rotH; y++)
                                if (rotShape[x, y] != 0 && GameBoard[x + offX, y + offY] != 0)
                                {
                                    canPlace = false;
                                    break;
                                }

                        if (canPlace)
                            return;  // 한 번이라도 놓을 수 있으면 게임 종료 아님
                    }
                }
            }

            // 4) 한번도 놓을 수 없으면 게임 종료
            ChangeState(new EndGameState(this, GameBoard));
        }

        /// <summary>
        /// 주어진 shape를 90° 회전(rotTimes 회)한 새 배열과 그 크기를 반환합니다.
        /// </summary>
        private (int[,], int, int) RotateShape90(int[,] shape, int w, int h, int rotTimes)
        {
            int[,] cur = shape;
            int curW = w, curH = h;

            for (int r = 0; r < rotTimes; r++)
            {
                // 다음 회전 상태 생성
                int[,] next = new int[curH, curW];
                for (int x = 0; x < curW; x++)
                    for (int y = 0; y < curH; y++)
                        next[y, curW - 1 - x] = cur[x, y];

                cur = next;
                int tmp = curW; 
                curW = curH; 
                curH = tmp;
            }

            return (cur, curW, curH);
        }
    }
}