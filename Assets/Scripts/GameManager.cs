using UnityEngine;
using static GlobalGameData;

public class GameManager : MonoBehaviour
{
    private static bool _isInTurn;
    public static bool EndTurn;

    void Start()
    {
        _isInTurn = false;
        EndTurn = false;
        Debug.Log("Game Initialized");
        CurrentState = GameState.BlackTurn;
    }

    void Update()
    {
        if (_isInTurn)
        {
            if (EndTurn)
            {
                PrevStones.LocateStones(CurrentStones.GetCurrentStones);
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

    private void ChangeTurn()
    {
        _isInTurn = false;
        CurrentState = (CurrentState == GameState.WhiteTurn) ? GameState.BlackTurn : GameState.WhiteTurn;
    }
}
