using System.Collections.Generic;
using System.Linq;
using State;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public GameObject blackStone,
        whiteStone,
        blackStoneNew,
        whiteStoneNew,
        blackStoneError,
        whiteStoneError,
        bonusStone;

    [SerializeField] public GameObject pauseScreen, currentStones, prevStones;
    [SerializeField] public TextMeshProUGUI blackScoreText, whiteScoreText;
    public int blackScore, whiteScore;
    private IState _currentState;
    private bool _isPaused;

    public int[,] GameBoard;
    public static GameManager Instance { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        _currentState = new GameStartState(this);
        _currentState.OnEnter();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!_isPaused)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                    _currentState.HandleInput("ok");
                else if (Input.GetKeyDown(KeyCode.R))
                    _currentState.HandleInput("rotate");
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                    _currentState.HandleInput("up");
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                    _currentState.HandleInput("down");
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    _currentState.HandleInput("left");
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    _currentState.HandleInput("right");
            }

            if (Input.GetKeyDown(KeyCode.Escape)) PauseResume();
        }

        _currentState.Update();
    }

    public void PauseResume()
    {
        _isPaused = !_isPaused;
        pauseScreen.SetActive(_isPaused);
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
        _currentState.OnExit();
        _currentState = new EndGameState(this);
        _currentState.OnEnter();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public GameObject InstantiateObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Instantiate(prefab, position, rotation);
    }

    public void DestroyObject(GameObject target)
    {
        Destroy(target);
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
                stone = Instantiate(bonusStone, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0), Quaternion.identity);
            }
            else if (stoneType == 1)
            {
                GameBoard[i, j] = 1;
                stone = Instantiate(blackStone, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0), Quaternion.identity);
            }
            else
            {
                GameBoard[i, j] = 2;
                stone = Instantiate(whiteStone, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0), Quaternion.identity);
            }

            stone.name = i + "_" + j;
            stone.transform.SetParent(prevStones.transform);
        }

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
            blackScore += deletedLines * 10;
        else
            whiteScore += deletedLines * 10;

        foreach (var (i, j) in stonesToRemove)
        {
            if (GameBoard[i, j] == 3)
            {
                if (stoneType == 1)
                    blackScore += 3;
                else
                    whiteScore += 3;
            }

            GameBoard[i, j] = 0;
            Destroy(GameObject.Find(i + "_" + j));
        }

        blackScoreText.text = "흑: " + blackScore;
        whiteScoreText.text = "백: " + whiteScore;
    }
}