using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static GlobalGameData;

public class GameManager : MonoBehaviour
{
    private static bool _isInTurn;
    public static bool EndTurn;

    [SerializeField] private TextMeshProUGUI blackScoreText;
    [SerializeField] private TextMeshProUGUI whiteScoreText;
    [SerializeField] private GameObject pauseScreen;
    private GameState _prevState;

    private void Start()
    {
        _isInTurn = false;
        EndTurn = false;
        Debug.Log("Game Initialized");
        CurrentState = GameState.BlackTurn;
        pauseScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Pause();

        // 플레이 중일 때
        if (_isInTurn)
        {
            if (EndTurn)
            {
                PrevStones.LocateStones(CurrentStones.GetCurrentStones);
                CheckConnections(CurrentStones.GetCurrentStones);
                ChangeTurn();
            }

            return;
        }

        // 상태가 변경될 때
        switch (CurrentState)
        {
            case GameState.BlackTurn or GameState.WhiteTurn:
                CurrentStones.StartTurn();
                _isInTurn = true;
                break;
            case GameState.Pause:
                break;
            case GameState.CheckScores:
                break;
            case GameState.GameEnd:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // TODO: 메인 신으로 돌아가기
                }

                break;
        }
    }

    public void Pause()
    {
        _isInTurn = !_isInTurn;

        if (_isInTurn)
        {
            CurrentState = _prevState;
            pauseScreen.SetActive(false);
        }
        else
        {
            _prevState = CurrentState;
            CurrentState = GameState.Pause;
            pauseScreen.SetActive(true);
        }
    }

    public void EndGame()
    {
        _isInTurn = false;
        CurrentState = GameState.CheckScores;
        pauseScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    ///     이번 턴에 놓아진 돌에서부터 가로, 세로, 대각선으로 10개 이상 연결된 돌이 있는지 판정한다.
    /// </summary>
    /// <param name="getCurrentStones"></param>
    private void CheckConnections((int, int)[] getCurrentStones)
    {
        var gameBoard = MainBoard;
        var stone = CurrentState == GameState.BlackTurn ? 1 : 2;

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
                if (gameBoard[--currentX, j] == stone || gameBoard[currentX, j] == 3)
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
                if (gameBoard[++currentX, j] == stone || gameBoard[currentX, j] == 3)
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
                if (gameBoard[i, --currentY] == stone || gameBoard[i, currentY] == 3)
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
                if (gameBoard[i, ++currentY] == stone || gameBoard[i, currentY] == 3)
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
                if (gameBoard[--currentX, --currentY] == stone || gameBoard[currentX, currentY] == 3)
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
                if (gameBoard[++currentX, ++currentY] == stone || gameBoard[currentX, currentY] == 3)
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
                if (gameBoard[--currentX, ++currentY] == stone || gameBoard[currentX, currentY] == 3)
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
                if (gameBoard[++currentX, --currentY] == stone || gameBoard[currentX, currentY] == 3)
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

        if (CurrentState == GameState.BlackTurn)
        {
            BlackScore += deletedLines * 10;
            blackScoreText.text = "흑: " + BlackScore;
        }
        else
        {
            WhiteScore += deletedLines * 10;
            whiteScoreText.text = "백: " + WhiteScore;
        }

        foreach (var (i, j) in stonesToRemove)
        {
            if (gameBoard[i, j] == 3)
            {
                if (CurrentState == GameState.BlackTurn)
                {
                    BlackScore += 3;
                    blackScoreText.text = "흑: " + BlackScore;
                }
                else
                {
                    WhiteScore += 3;
                    whiteScoreText.text = "백: " + WhiteScore;
                }
            }

            MainBoard[i, j] = 0;
            Destroy(GameObject.Find(i + "_" + j));
        }
    }

    private void ChangeTurn()
    {
        _isInTurn = false;
        EndTurn = false;
        CurrentState = CurrentState == GameState.WhiteTurn ? GameState.BlackTurn : GameState.WhiteTurn;
    }
}