public static class GlobalGameData
{
    public enum GameState
    {
        GameStart,
        BlackTurn,
        WhiteTurn,
        Pause,
        CheckScores,
        GameEnd
    }

    public static GameState CurrentState = GameState.GameStart;

    public static int[,] MainBoard { get; set; } = new int[19, 19];
    public static int BlackScore { get; set; }

    public static int WhiteScore { get; set; }
}