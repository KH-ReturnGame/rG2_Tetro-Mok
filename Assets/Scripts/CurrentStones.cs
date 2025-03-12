using UnityEngine;
using static GlobalGameData;

public class CurrentStones : MonoBehaviour
{
    [SerializeField] private GameObject blackStone;
    [SerializeField] private GameObject whiteStone;
    private static int[,] _gameBoard;
    public static int[,] GetCurrentStones { get { return _gameBoard; } }
    private static bool _isInTurn;
    private static GameState _currentState;

    private enum MoveDirection { Up, Down, Left, Right, Turn }

    void Start()
    {
        _isInTurn = false;
        _currentState = GameState.GameStart;
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
            else if (Input.GetKeyDown(KeyCode.Exclaim))
                EndTurn();
        }
    }

    static void InitBoard(Transform parentTransform, GameObject blackStone, GameObject whiteStone)
    {
        _gameBoard = CreateStones(GlobalGameData.NextBlock);
        for (int i = 0; i < 19; i++)
            for (int j = 0; j < 19; j++)
                if (_gameBoard[i, j] == 1)
                {
                    GameObject stone = (_currentState == GameState.BlackTurn) ?
                        Instantiate(blackStone, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity) :
                        Instantiate(whiteStone, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity);
                    stone.transform.SetParent(parentTransform);
                }
    }

    public static void StartTurn(GameState state, Transform parentTransform, GameObject blackStone, GameObject whiteStone)
    {
        Debug.Log("Started Turn; Current State: " + state);
        _isInTurn = true;
        _currentState = state;
        InitBoard(parentTransform, blackStone, whiteStone);
    }


    /// <summary>
    /// 화면에 처음 생성할 돌의 배열을 생성한다.
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

    void MoveStones(MoveDirection direction)
    {
        Debug.Log("Move Stones; Direction: " + direction);
        // 현재 형태 지정
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

        // 이동 방향에 따라 조정
        switch (direction)
        {
            case MoveDirection.Up:
                if (minY > 0)
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            _gameBoard[minX + i, minY + j + 1] = shape[i, j];
                        }
                    }
                }
                break;
            case MoveDirection.Down:
                if (maxY < 18)
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            _gameBoard[minX + i, minY + j - 1] = shape[i, j];
                        }
                    }
                }
                break;
            case MoveDirection.Left:
                if (minX > 0)
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            _gameBoard[minX + i - 1, minY + j] = shape[i, j];
                        }
                    }
                }
                break;
            case MoveDirection.Right:
                if (maxX < 18)
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            _gameBoard[minX + i + 1, minY + j] = shape[i, j];
                        }
                    }
                }
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
                _gameBoard = new int[19, 19];

                // 6. 회전된 도형을 원래 위치에 맞춰 배치
                int offsetX = 0, offsetY = 0;

                if (minX + height > 18)
                {
                    offsetX = 18 - (minX + height);
                } else if (maxX - height < 0)
                {
                    offsetX = height - maxX;
                }

                if (minY + width > 18)
                {
                    offsetY = 18 - (minY + width);
                } else if (maxY - width < 0)
                {
                    offsetY = width - maxY;
                }

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (minX + i < 19 && minY + j < 19) // 경계 체크
                        {
                            _gameBoard[minX + i + offsetX, minY + j + offsetY] = rotatedShape[i, j];
                        }
                    }
                }
                break;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 19; i++)
            for (int j = 0; j < 19; j++)
            if (_gameBoard[i, j] == 1)
            {
                GameObject stone = (_currentState == GameState.BlackTurn) ?
                    Instantiate(blackStone, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity) :
                    Instantiate(whiteStone, new Vector3((i-9)*0.5f, (j-9)*0.5f, 0), Quaternion.identity);
                stone.transform.SetParent(transform);
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
