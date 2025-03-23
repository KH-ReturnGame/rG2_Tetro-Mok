using UnityEngine;
using static GlobalGameData;

public class CurrentStones : MonoBehaviour
{
    private static GameObject _blackStone;
    private static GameObject _whiteStone;
    private static GameObject _blackStoneError;
    private static GameObject _whiteStoneError;

    private static int[,] _gameBoard;

    private static bool _isInTurn, _canLocate;
    [SerializeField] private GameObject privateBlackStone;
    [SerializeField] private GameObject privateWhiteStone;
    [SerializeField] private GameObject privateBlackStoneError;
    [SerializeField] private GameObject privateWhiteStoneError;

    public static (int, int)[] GetCurrentStones
    {
        get
        {
            var stones = new (int, int)[3];
            var count = 0;

            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
                if (_gameBoard[i, j] == 1)
                    stones[count++] = (i, j);

            return stones;
        }
    }

    private void Start()
    {
        _blackStone = privateBlackStone;
        _whiteStone = privateWhiteStone;
        _blackStoneError = privateBlackStoneError;
        _whiteStoneError = privateWhiteStoneError;
        _canLocate = true;
        _gameBoard = new int[19, 19];
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            // 플레이 중임을 체크
            if (_isInTurn)
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

            // 게임 종료 선언
            if (Input.GetKeyDown(KeyCode.Escape))
                Pause();
        }
    }

    public void Pause()
    {
        _isInTurn = !_isInTurn;
    }

    public static void StartTurn()
    {
        Debug.Log("Started Turn; Current State: " + CurrentState);
        _isInTurn = true;
        InitBoard();
    }

    private static void InitBoard()
    {
        _gameBoard = CreateStones(Random.Range(1, 4));
        RenderStones();
    }

    /// <summary>
    ///     생성할 현재 턴의 돌의 배열을 초기화한다.
    /// </summary>
    /// <param name="type">1~3의 트리미노 모양</param>
    /// <list type="number">
    ///     <item>
    ///         <description>ㅡ 트리미노</description>
    ///     </item>
    ///     <item>
    ///         <description>ㄴ 트리미노</description>
    ///     </item>
    ///     <item>
    ///         <description>/ 트리미노</description>
    ///     </item>
    /// </list>
    /// <returns>[19x19] 정수 배열</returns>
    private static int[,] CreateStones(int type)
    {
        Debug.Log("Creating Stones; Stone Type:" + type);
        var initialStones = new int[19, 19];
        switch (type)
        {
            // 보드의 중심: [9,9]
            case 1:
                initialStones[8, 9] = 1;
                initialStones[9, 9] = 1;
                initialStones[10, 9] = 1;
                break;
            case 2:
                initialStones[9, 9] = 1;
                initialStones[8, 9] = 1;
                initialStones[9, 8] = 1;
                break;
            case 3:
                initialStones[8, 8] = 1;
                initialStones[9, 9] = 1;
                initialStones[10, 10] = 1;
                break;
        }

        return initialStones;
    }

    /// <summary>
    ///     이동 방향에 맞춰 배열 변경 및 렌더링
    /// </summary>
    /// <param name="direction">MoveDirection</param>
    private void MoveStones(MoveDirection direction)
    {
        Debug.Log("Move Stones; Direction: " + direction);

        // 현재 도형의 형태 지정
        int minX = 19, maxX = -1, minY = 19, maxY = -1;
        for (var i = 0; i < 19; i++)
        for (var j = 0; j < 19; j++)
            if (_gameBoard[i, j] == 1)
            {
                if (i < minX) minX = i;
                if (i > maxX) maxX = i;
                if (j < minY) minY = j;
                if (j > maxY) maxY = j;
            }

        var height = maxY - minY + 1;
        var width = maxX - minX + 1;
        var shape = new int[width, height];

        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            shape[i, j] = _gameBoard[minX + i, minY + j];

        // 보드 초기화
        _gameBoard = new int[19, 19];

        // 회전하는 경우
        if (direction == MoveDirection.Turn)
        {
            // 시계 방향 90도 회전
            var rotatedShape = new int[height, width];
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                rotatedShape[j, width - i - 1] = shape[i, j];

            // 회전된 도형을 원래 위치에 맞춰 배치
            int offsetX = 0, offsetY = 0;

            if (minX + height - 1 > 18) offsetX = 18 - (minX + height - 1);
            if (minY + width - 1 > 18) offsetY = 18 - (minY + width - 1);

            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
                _gameBoard[minX + i + offsetX, minY + j + offsetY] = rotatedShape[i, j];
        }
        // 상하좌우로 이동하는 경우
        else
        {
            int moveX = 0, moveY = 0;
            if (direction == MoveDirection.Up && maxY < 18) moveY = 1;
            else if (direction == MoveDirection.Down && minY > 0) moveY = -1;
            else if (direction == MoveDirection.Left && minX > 0) moveX = -1;
            else if (direction == MoveDirection.Right && maxX < 18) moveX = 1;

            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                _gameBoard[minX + i + moveX, minY + j + moveY] = shape[i, j];
        }

        RenderStones();
    }

    private static void RenderStones()
    {
        var parent = GameObject.Find("CurrentStones").transform;
        foreach (Transform child in parent) Destroy(child.gameObject);

        _canLocate = true;
        foreach (var (i, j) in GetCurrentStones)
            if (MainBoard[i, j] == 0)
            {
                var stone = CurrentState == GameState.BlackTurn
                    ? Instantiate(_blackStone, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0), Quaternion.identity)
                    : Instantiate(_whiteStone, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0), Quaternion.identity);
                stone.transform.SetParent(parent);
            }
            else
            {
                var stone = CurrentState == GameState.BlackTurn
                    ? Instantiate(_blackStoneError, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0), Quaternion.identity)
                    : Instantiate(_whiteStoneError, new Vector3((i - 9) * 0.5f, (j - 9) * 0.5f, 0),
                        Quaternion.identity);
                stone.transform.SetParent(parent);

                _canLocate = false;
            }
    }

    private void EndTurn()
    {
        _isInTurn = false;
        GameManager.EndTurn = true;

        foreach (Transform child in transform) Destroy(child.gameObject);

        Debug.Log("End Turn;");
    }

    private enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right,
        Turn
    }
}