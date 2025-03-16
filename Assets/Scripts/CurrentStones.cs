using UnityEngine;
using static GlobalGameData;

public class CurrentStones : MonoBehaviour
{
    [SerializeField] private GameObject privateBlackStone;
    [SerializeField] private GameObject privateWhiteStone;
    [SerializeField] private GameObject privateBlackStoneError;
    [SerializeField] private GameObject privateWhiteStoneError;
    private static GameObject _blackStone;
    private static GameObject _whiteStone;
    private static GameObject _blackStoneError;
    private static GameObject _whiteStoneError;

    private static int[,] _gameBoard;
    public static int[,] GetCurrentStones => _gameBoard;
    private static bool _isInTurn, _canLocate;

    private enum MoveDirection { Up, Down, Left, Right, Turn }

    void Start()
    {
        _blackStone = privateBlackStone;
        _whiteStone = privateWhiteStone;
        _blackStoneError = privateBlackStoneError;
        _whiteStoneError = privateWhiteStoneError;
        _isInTurn = false;
        _canLocate = true;
        _gameBoard = new int[19, 19];
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
            else if (Input.GetKeyDown(KeyCode.Return) && _canLocate)
                EndTurn();
        }
    }

    public static void StartTurn()
    {
        Debug.Log("Started Turn; Current State: " + CurrentState);
        _isInTurn = true;
        InitBoard();
    }

    static void InitBoard()
    {
        _gameBoard = CreateStones(GlobalGameData.NextBlock);

        RenderStones();
    }

    /// <summary>
    /// 생성할 현재 턴의 돌의 배열을 초기화한다.
    /// </summary>
    /// <param name="type">1~7의 테트로미노 모양</param>
    /// <list type="number">
    ///     <item>
    ///       <description>I-Tetromino</description>
    ///     </item>
    ///     <item>
    ///       <description>O-Tetromino</description>
    ///     </item>
    ///     <item>
    ///       <description>T-Tetromino</description>
    ///     </item>
    ///     <item>
    ///       <description>S-Tetromino</description>
    ///     </item>
    ///     <item>
    ///       <description>Z-Tetromino</description>
    ///     </item>
    ///     <item>
    ///       <description>J-Tetromino</description>
    ///     </item>
    ///     <item>
    ///       <description>L-Tetromino</description>
    ///     </item>
    ///   </list>
    /// <returns>[19x19] 정수 배열</returns>
    static int[,] CreateStones(int type)
    {
        Debug.Log("Creating Stones; Stone Type:" + type);
        int[,] initialStones = new int[19, 19];
        switch (type)
        {
            // 보드의 중심: [9,9]
            case 1:
                initialStones[7, 9] = 1;
                initialStones[8, 9] = 1;
                initialStones[9, 9] = 1;
                initialStones[10, 9] = 1;
                break;
            case 2:
                initialStones[9, 9] = 1;
                initialStones[8, 9] = 1;
                initialStones[9, 8] = 1;
                initialStones[8, 8] = 1;
                break;
            case 3:
                initialStones[9, 9] = 1;
                initialStones[8, 8] = 1;
                initialStones[9, 8] = 1;
                initialStones[10, 8] = 1;
                break;
            case 4:
                initialStones[9, 8] = 1;
                initialStones[10, 8] = 1;
                initialStones[8, 9] = 1;
                initialStones[9, 9] = 1;
                break;
            case 5:
                initialStones[8, 8] = 1;
                initialStones[9, 8] = 1;
                initialStones[9, 9] = 1;
                initialStones[10, 9] = 1;
                break;
            case 6:
                initialStones[8, 9] = 1;
                initialStones[9, 9] = 1;
                initialStones[10, 9] = 1;
                initialStones[8, 8] = 1;
                break;
            case 7:
                initialStones[8, 9] = 1;
                initialStones[9, 9] = 1;
                initialStones[10, 9] = 1;
                initialStones[10, 8] = 1;
                break;
        }
        return initialStones;
    }

    /// <summary>
    /// 이동 방향에 맞춰 배열 변경 및 렌더링
    /// </summary>
    /// <param name="direction">MoveDirection</param>
    void MoveStones(MoveDirection direction)
    {
        Debug.Log("Move Stones; Direction: " + direction);

        // 현재 도형의 형태 지정
        int minX = 19, maxX = -1, minY = 19, maxY = -1;
        for (int i = 0; i < 19; i++)
        {
            for (int j = 0; j < 19; j++)
            {
                if (_gameBoard[i, j] == 1)
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
                shape[i, j] = _gameBoard[minX + i, minY + j];
            }
        }

        // 보드 초기화
        _gameBoard = new int[19, 19];

        // 회전하는 경우
        if (direction == MoveDirection.Turn)
        {
            // 시계 방향 90도 회전
            int[,] rotatedShape = new int[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    rotatedShape[j, width - i - 1] = shape[i, j];
                }
            }

            // 회전된 도형을 원래 위치에 맞춰 배치
            int offsetX = 0, offsetY = 0;

            if (minX + height - 1 > 18)
            {
                offsetX = 18 - (minX + height - 1);
            }
            if (minY + width - 1 > 18)
            {
                offsetY = 18 - (minY + width -1);
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    _gameBoard[minX + i + offsetX, minY + j + offsetY] = rotatedShape[i, j];
                }
            }
        }
        // 상하좌우로 이동하는 경우
        else
        {
            int moveX = 0, moveY = 0;
            if (direction == MoveDirection.Up && maxY < 18) moveY = 1;
            else if (direction == MoveDirection.Down && minY > 0) moveY = -1;
            else if (direction == MoveDirection.Left && minX > 0) moveX = -1;
            else if (direction == MoveDirection.Right && maxX < 18) moveX = 1;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    _gameBoard[minX + i + moveX, minY + j + moveY] = shape[i, j];
                }
            }
        }

        RenderStones();
    }

    static void RenderStones()
    {
        Transform parent = GameObject.Find("CurrentStones").transform;
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        _canLocate = true;
        for (int i = 0; i < 19; i++)
        for (int j = 0; j < 19; j++)
            if (_gameBoard[i, j] == 1 && MainBoard[i,j] == 0)
            {
                GameObject stone = (CurrentState == GameState.BlackTurn) ?
                    Instantiate(_blackStone, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity) :
                    Instantiate(_whiteStone, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity);
                stone.transform.SetParent(parent);
            } else if (_gameBoard[i, j] == 1 && (MainBoard[i, j] == 1 || MainBoard[i, j] == 2))
            {
                GameObject stone = (CurrentState == GameState.BlackTurn) ?
                    Instantiate(_blackStoneError, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity) :
                    Instantiate(_whiteStoneError, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity);
                stone.transform.SetParent(parent);

                _canLocate = false;
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

        Debug.Log("End Turn;");
    }
}
