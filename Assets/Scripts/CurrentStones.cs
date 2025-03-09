using UnityEngine;
using static GlobalGameData;

public class CurrentStones : MonoBehaviour
{
    [SerializeField] private GameObject blackStone;
    [SerializeField] private GameObject whiteStone;
    public static int[,] GameBoard;
    private static bool _isInTurn;
    private static GameState _currentState;

    private enum MoveDirection { Up, Down, Left, Right, Turn }

    void Start()
    {
        _isInTurn = false;
        InitBoard();
    }

    void Update()
    {
        if (!_isInTurn) return;
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                MoveStones(MoveDirection.Up);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                MoveStones(MoveDirection.Down);
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveStones(MoveDirection.Left);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveStones(MoveDirection.Right);
            else if (Input.GetKeyDown(KeyCode.R))
                MoveStones(MoveDirection.Turn);
            else if (Input.GetKeyDown(KeyCode.Exclaim))
                EndTurn();
        }
    }

    static void InitBoard()
    {
        GameBoard = new int[19, 19];
    }

    public static void StartTurn(GameState state)
    {
        InitBoard();
        _isInTurn = true;
        _currentState = state;
    }

    void MoveStones(MoveDirection direction)
    {
        // 현재 형태 지정
        int minX = 19, maxX = -1, minY = 19, maxY = -1;
        for (int i = 0; i < 19; i++)
        {
            for (int j = 0; j < 19; j++)
            {
                if (GameBoard[i, j] == 1)
                {
                    if (i < minX) minX = i;
                    if (i > maxX) maxX = i;
                    if (j < minY) minY = j;
                    if (j > maxY) maxY = j;
                }
            }
        }

        int height = maxY - minY + 1;
        int width = maxX - minX + 1;
        int[,] shape = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                shape[i, j] = GameBoard[minX + i, minY + j];
            }
        }

        // 이동 방향에 따라 조정
        switch (direction)
        {
            case MoveDirection.Up:
                break;
            case MoveDirection.Turn:
                // 4. 시계 방향 90도 회전
                int[,] rotatedShape = new int[height, width];
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        rotatedShape[j, width - 1 - i] = shape[i, j];
                    }
                }

                // 5. 원래 배열에서 도형 지우기
                GameBoard = new int[19, 19];

                // 6. 회전된 도형을 원래 위치에 맞춰 배치
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (minX + i < 19 && minY + j < 19) // 경계 체크
                        {
                            GameBoard[minX + i, minY + j] = rotatedShape[i, j];
                        }
                    }
                }
                break;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (_currentState == GameState.BlackTurn)
        {
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 19; j++)
                if (GameBoard[i, j] == 1)
                {
                    GameObject stone = (_currentState == GameState.BlackTurn) ?
                        Instantiate(blackStone, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity) :
                        Instantiate(whiteStone, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity);
                    stone.transform.SetParent(transform);
                }
        }
    }

    void EndTurn()
    {
        _isInTurn = false;
        GameManager.EndTurn = true;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
