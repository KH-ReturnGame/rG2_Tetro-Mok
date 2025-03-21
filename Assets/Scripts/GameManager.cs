using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GlobalGameData;

public class GameManager : MonoBehaviour
{
    private static bool _isInTurn;
    public static bool EndTurn;

    private void Start()
    {
        _isInTurn = false;
        EndTurn = false;
        Debug.Log("Game Initialized");
        CurrentState = GameState.BlackTurn;
    }

    private void Update()
    {
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


        switch (CurrentState)
        {
            case GameState.BlackTurn or GameState.WhiteTurn:
                CurrentStones.StartTurn();
                _isInTurn = true;
                break;
            case GameState.GameEnd:
                _isInTurn = true; // 이후에 점수 판정 등 행함
                break;
        }
    }

    /// <summary>
    ///     이번 턴에 놓아진 돌에서부터 가로, 세로, 대각선으로 10개 이상 연결된 돌이 있는지 판정한다.
    /// </summary>
    /// <param name="getCurrentStones"></param>
    private void CheckConnections((int, int)[] getCurrentStones)
    {
        // TODO: 점수 처리(당장), 줄 클리어 이펙트(나중에)

        var gameBoard = MainBoard;
        var stone = CurrentState == GameState.BlackTurn ? 1 : 2;

        var stonesToRemove = new List<(int, int)>();

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
                if (gameBoard[--currentX, j] == stone)
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
                if (gameBoard[++currentX, j] == stone)
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
                if (gameBoard[i, --currentY] == stone)
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
                if (gameBoard[i, ++currentY] == stone)
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
                if (gameBoard[--currentX, --currentY] == stone)
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
                if (gameBoard[++currentX, ++currentY] == stone)
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
                if (gameBoard[--currentX, ++currentY] == stone)
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
                if (gameBoard[++currentX, --currentY] == stone)
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
                stonesToRemove = stonesToRemove.Union(checkingStones).ToList();
            }
        }

        foreach (var (i, j) in stonesToRemove)
        {
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