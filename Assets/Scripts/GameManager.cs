using UnityEngine;
using static GlobalGameData;

public class GameManager : MonoBehaviour
{
    private GameState _currentState;
    private static bool _isInTurn;
    public static bool EndTurn;

    void Start()
    {
        _currentState = GameState.GameStart;
        _isInTurn = false;
        EndTurn = false;
    }

    void Update()
    {
        if (_isInTurn)
        {
            if (EndTurn)
                ChangeTurn();
            return;
        }


        switch (_currentState)
        {
            case GameState.BlackTurn or GameState.WhiteTurn:
                CurrentStones.StartTurn(_currentState);
                _isInTurn = true;
                break;
            case GameState.GameEnd:
                _isInTurn = true; // 이후에 점수 판정 등 행함
                break;
        }
    }

    private void ChangeTurn()
    {
        _isInTurn = false;
        _currentState = (_currentState == GameState.WhiteTurn) ? GameState.BlackTurn : GameState.WhiteTurn;
    }
}
