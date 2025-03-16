public static class GlobalGameData
{
    public static int[,] MainBoard { get; set; } = new int[19, 19];

    public enum GameState
    {
        GameStart, BlackTurn, WhiteTurn, GameEnd
    }

    public static GameState CurrentState = GameState.GameStart;
}