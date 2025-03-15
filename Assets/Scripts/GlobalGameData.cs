using System;
using System.Collections.Generic;

public static class GlobalGameData
{
    public static int[,] MainBoard { get; set; } = new int[19, 19];

    public enum GameState
    {
        GameStart, BlackTurn, WhiteTurn, GameEnd
    }

    public static GameState CurrentState = GameState.GameStart;

    private static readonly Queue<int> BlockQueue = new Queue<int>(7);

    public static int NextBlock
    {
        get
        {
            // Queue에 블럭이 존재할 때 다음 블럭 가져오기
            if (BlockQueue.Count != 0) return BlockQueue.Dequeue();

            // Queue를 초기화한 후 다음 블럭 가져오기
            Random random = new Random();
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7 };

            while (numbers.Count > 0)
            {
                int index = random.Next(numbers.Count);
                BlockQueue.Enqueue(numbers[index]);
                numbers.RemoveAt(index);
            }
            return BlockQueue.Dequeue();
        }
    }
}