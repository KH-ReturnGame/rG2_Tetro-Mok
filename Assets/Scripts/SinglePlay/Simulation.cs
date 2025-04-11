using System.Collections.Generic;

namespace SinglePlay
{
    public class Simulation
    {
        private readonly int[,] board;
        private int _minX = 18, _minY = 18, _maxX, _maxY;

        public Simulation(int[,] initialBoard, int type)
        {
            board = initialBoard;
            SetLimits();
        }

        public Simulation(Simulation other, int type)
        {
            board = (int[,])other.board.Clone();
            SetLimits();
        }

        private void SetLimits()
        {
            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
                if (board[i, j] != 0)
                {
                    if (i > _maxX) _maxX = i;
                    if (j > _maxY) _maxY = j;
                    if (i < _minX) _minX = i;
                    if (j < _minY) _minY = j;
                }

            if (_minX > 2) _minX -= 3;
            else _minX = 0;
            if (_minY > 2) _minY -= 3;
            else _minY = 0;
            if (_maxX < 15) _maxX += 3;
            else _maxX = 18;
            if (_maxY < 15) _maxY += 3;
            else _maxY = 18;
        }

        public (int, int)[] GetResult()
        {
            return new (int, int)[3];
        }

        private int[,] InitStones(int type)
        {
            var firstStones = new int[3, 3];
            switch (type)
            {
                case 1:
                    firstStones[0, 1] = 2;
                    firstStones[1, 1] = 2;
                    firstStones[2, 1] = 2;
                    break;
                case 2:
                    firstStones[2, 2] = 2;
                    firstStones[1, 2] = 2;
                    firstStones[2, 1] = 2;
                    break;
                case 3:
                    firstStones[0, 0] = 2;
                    firstStones[1, 1] = 2;
                    firstStones[2, 2] = 2;
                    break;
            }

            return firstStones;
        }

        private List<(int, int)> GetAvailablePos()
        {
            var moves = new List<(int, int)>();
            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
                if (board[i, j] == 0)
                    moves.Add((i, j));
            return moves;
        }
    }
}