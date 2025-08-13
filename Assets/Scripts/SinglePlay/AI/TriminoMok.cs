using System;
using System.Collections.Generic;

namespace SinglePlay.AI
{
    public class TriminoMok
    {
        private readonly int[,] _board;
        private readonly List<List<(int, int)>> _prevActions;
        private List<(int, int)> _availableSpaces;
        private int _blackScore;
        private int _currentPlayer;
        private int _depth;
        private int _stoneType;
        private int _whiteScore;

        public TriminoMok(int[,] board, int type, List<List<(int, int)>> prevActions, int depth = 0,
            int currentPlayer = 2)
        {
            _currentPlayer = currentPlayer;
            _board = board;
            _stoneType = type;
            _depth = depth;
            _availableSpaces = GetAvailableSpaces();
            _prevActions = prevActions;
            _blackScore = 0;
            _whiteScore = 0;
        }

        public TriminoMok(TriminoMok other)
        {
            _currentPlayer = other._currentPlayer;
            _board = (int[,])other._board.Clone();
            _stoneType = other._stoneType;
            _depth = other._depth;
            _availableSpaces = GetAvailableSpaces();
            _prevActions = other._prevActions;
            _blackScore = other._blackScore;
            _whiteScore = other._whiteScore;
        }

        public bool IsTerminal(int maxDepth)
        {
            return _depth >= maxDepth || GetMoves().Count == 0;
        }

        public List<(int, int, int)> GetMoves()
        {
            var moves = new List<(int, int, int)>();
            if (_stoneType == 2)
                foreach (var (i, j) in _availableSpaces)
                    for (var r = 0; r < 4; r++)
                    {
                        var stones = GetStones(i, j, r);
                        if (IsExists(stones[1]) && IsExists(stones[2])) moves.Add((i, j, r));
                    }
            else
                foreach (var (i, j) in _availableSpaces)
                    for (var r = 0; r < 2; r++)
                    {
                        var stones = GetStones(i, j, r);
                        if (IsExists(stones[1]) && IsExists(stones[2])) moves.Add((i, j, r));
                    }

            return moves;
        }

        public void MakeMove((int, int, int) move, bool addDepth = true)
        {
            var (y, x, r) = move;
            var currentStones = GetStones(y, x, r);

            if (_prevActions.Count == 3) _prevActions.RemoveAt(0);

            var currentAction = new List<(int, int)>();
            foreach (var stone in currentStones)
                currentAction.Add(stone);

            _prevActions.Add(currentAction);

            var rand = new Random();
            foreach (var (i, j) in currentStones)
                if (rand.Next(1, 16) == 1)
                    _board[i, j] = 3;
                else
                    _board[i, j] = _currentPlayer;

            _blackScore += GetClearLine(1);
            _whiteScore += GetClearLine(2);

            ChangePlayer(addDepth);
            _availableSpaces = GetAvailableSpaces();
        }

        public float IsWin()
        {
            var scores = new int[2]; // Player 1 and 2 scores

            // Horizontal
            for (var y = 18; y >= 0; y--)
            {
                var checkingStoneType = 0;
                var connectionLength = 0;
                var bonusNumber = 0;
                for (var x = 0; x < 19; x++)
                {
                    if (_board[x, y] == 0)
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                        continue;
                    }

                    if (checkingStoneType == 0)
                    {
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    else
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }
                }

                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
            }

            // Vertical
            for (var x = 0; x < 19; x++)
            {
                var checkingStoneType = 0;
                var connectionLength = 0;
                var bonusNumber = 0;
                for (var y = 18; y >= 0; y--)
                {
                    if (_board[x, y] == 0)
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                        continue;
                    }

                    if (checkingStoneType == 0)
                    {
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    else
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }
                }

                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
            }

            // Diagonal (top-left to bottom-right)
            for (var startX = 0; startX < 19; startX++)
            {
                var checkingStoneType = 0;
                var connectionLength = 0;
                var bonusNumber = 0;
                int x = startX, y = 0;
                while (x < 19 && y < 19)
                {
                    if (_board[x, y] == 0)
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                    }
                    else if (checkingStoneType == 0)
                    {
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    else
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }

                    x++;
                    y++;
                }

                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
            }

            for (var startY = 1; startY < 19; startY++)
            {
                var checkingStoneType = 0;
                var connectionLength = 0;
                var bonusNumber = 0;
                int x = 0, y = startY;
                while (x < 19 && y < 19)
                {
                    if (_board[x, y] == 0)
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                    }
                    else if (checkingStoneType == 0)
                    {
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    else
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }

                    x++;
                    y++;
                }

                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
            }

            // Diagonal (top-right to bottom-left)
            for (var startX = 18; startX >= 0; startX--)
            {
                var checkingStoneType = 0;
                var connectionLength = 0;
                var bonusNumber = 0;
                int x = startX, y = 0;
                while (x >= 0 && y < 19)
                {
                    if (_board[x, y] == 0)
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                    }
                    else if (checkingStoneType == 0)
                    {
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    else
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }

                    x--;
                    y++;
                }

                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
            }

            for (var startY = 1; startY < 19; startY++)
            {
                var checkingStoneType = 0;
                var connectionLength = 0;
                var bonusNumber = 0;
                int x = 18, y = startY;
                while (x >= 0 && y < 19)
                {
                    if (_board[x, y] == 0)
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                    }
                    else if (checkingStoneType == 0)
                    {
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    else
                    {
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }

                    x--;
                    y++;
                }

                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 3 + bonusNumber * 1;
            }

            _blackScore += scores[0];
            _whiteScore += scores[1];

            if (_whiteScore > _blackScore) return 1.0f;
            if (_whiteScore < _blackScore) return -1.0f;
            return 0.0f;
        }

        private void ChangePlayer(bool addDepth)
        {
            _stoneType = new Random().Next(1, 4);
            if (addDepth) _depth++;
            _currentPlayer = 3 - _currentPlayer;
        }

        private List<(int, int)> GetAvailableSpaces()
        {
            var spaces = new List<(int, int)>();
            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
                if (_board[i, j] == 0)
                    spaces.Add((i, j));

            return spaces;
        }

        private (int, int)[] GetStones(int i, int j, int r)
        {
            return GetStones(i, j, r, _stoneType);
        }

        private bool IsExists((int, int) pos)
        {
            return _availableSpaces.Contains(pos);
        }

        private int GetClearLine(int player)
        {
            var clearLinesCount = 0;
            var bonusCount = 0;
            var directions = new[] { (0, 1), (1, 0), (1, 1), (1, -1) };

            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
            {
                if (_board[i, j] != player && _board[i, j] != 3) continue;

                foreach (var (di, dj) in directions)
                {
                    var checkingStones = new List<(int, int)> { (i, j) };
                    var count = 1;
                    var localBonusCount = 0;

                    for (var step = 1; step < 19; step++)
                    {
                        int ni = i + di * step, nj = j + dj * step;
                        if (ni is >= 0 and < 19 && nj is >= 0 and < 19)
                        {
                            if (_board[ni, nj] == player)
                            {
                                count++;
                            }
                            else if (_board[ni, nj] == 3)
                            {
                                count++;
                                localBonusCount++;
                            }
                            else
                            {
                                break;
                            }

                            checkingStones.Add((ni, nj));
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (var step = 1; step < 19; step++)
                    {
                        int ni = i - di * step, nj = j - dj * step;
                        if (ni is >= 0 and < 19 && nj is >= 0 and < 19)
                        {
                            if (_board[ni, nj] == player)
                            {
                                count++;
                            }
                            else if (_board[ni, nj] == 3)
                            {
                                count++;
                                localBonusCount++;
                            }
                            else
                            {
                                break;
                            }

                            checkingStones.Add((ni, nj));
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (count >= 10)
                    {
                        clearLinesCount++;
                        bonusCount += localBonusCount;
                        foreach (var (y, x) in checkingStones) _board[y, x] = 0;
                    }
                }
            }

            return clearLinesCount * 20 + bonusCount * 3;
        }

        private (float, float, float) CheckConnections(int i, int j, int player)
        {
            float connectedLines = 0;
            float bonusCount = 0;
            float clearedLines = 0;

            // Horizontal
            var checkingStones = new List<(int, int)>();
            var xLength = 1;
            checkingStones.Add((i, j));
            var currentX = i;
            while (currentX > 0)
            {
                currentX--;
                if (_board[currentX, j] == player)
                {
                    xLength++;
                    checkingStones.Add((currentX, j));
                }
                else if (_board[currentX, j] == 3)
                {
                    xLength++;
                    checkingStones.Add((currentX, j));
                    bonusCount++;
                }
                else
                {
                    break;
                }
            }

            currentX = i;
            while (currentX < 18)
            {
                currentX++;
                if (_board[currentX, j] == player)
                {
                    xLength++;
                    checkingStones.Add((currentX, j));
                }
                else if (_board[currentX, j] == 3)
                {
                    xLength++;
                    checkingStones.Add((currentX, j));
                    bonusCount++;
                }
                else
                {
                    break;
                }
            }

            if (xLength >= 5)
            {
                if (xLength >= 10) clearedLines++;
                else connectedLines++;
            }

            // Vertical
            checkingStones.Clear();
            var yLength = 1;
            checkingStones.Add((i, j));
            var currentY = j;
            while (currentY > 0)
            {
                currentY--;
                if (_board[i, currentY] == player)
                {
                    yLength++;
                    checkingStones.Add((i, currentY));
                }
                else if (_board[i, currentY] == 3)
                {
                    yLength++;
                    checkingStones.Add((i, currentY));
                    bonusCount++;
                }
                else
                {
                    break;
                }
            }

            currentY = j;
            while (currentY < 18)
            {
                currentY++;
                if (_board[i, currentY] == player)
                {
                    yLength++;
                    checkingStones.Add((i, currentY));
                }
                else if (_board[i, currentY] == 3)
                {
                    yLength++;
                    checkingStones.Add((i, currentY));
                    bonusCount++;
                }
                else
                {
                    break;
                }
            }

            if (yLength >= 5)
            {
                if (yLength >= 10) clearedLines++;
                else connectedLines++;
            }

            // Diagonal (top-left to bottom-right)
            checkingStones.Clear();
            var diagLength = 1;
            checkingStones.Add((i, j));
            int curX = i, curY = j;
            while (curX > 0 && curY > 0)
            {
                curX--;
                curY--;
                if (_board[curX, curY] == player)
                {
                    diagLength++;
                    checkingStones.Add((curX, curY));
                }
                else if (_board[curX, curY] == 3)
                {
                    diagLength++;
                    checkingStones.Add((curX, curY));
                    bonusCount++;
                }
                else
                {
                    break;
                }
            }

            curX = i;
            curY = j;
            while (curX < 18 && curY < 18)
            {
                curX++;
                curY++;
                if (_board[curX, curY] == player)
                {
                    diagLength++;
                    checkingStones.Add((curX, curY));
                }
                else if (_board[curX, curY] == 3)
                {
                    diagLength++;
                    checkingStones.Add((curX, curY));
                    bonusCount++;
                }
                else
                {
                    break;
                }
            }

            if (diagLength >= 5)
            {
                if (diagLength >= 10) clearedLines++;
                else connectedLines++;
            }

            // Diagonal (top-right to bottom-left)
            checkingStones.Clear();
            diagLength = 1;
            checkingStones.Add((i, j));
            curX = i;
            curY = j;
            while (curX > 0 && curY < 18)
            {
                curX--;
                curY++;
                if (_board[curX, curY] == player)
                {
                    diagLength++;
                    checkingStones.Add((curX, curY));
                }
                else if (_board[curX, curY] == 3)
                {
                    diagLength++;
                    checkingStones.Add((curX, curY));
                    bonusCount++;
                }
                else
                {
                    break;
                }
            }

            curX = i;
            curY = j;
            while (curX < 18 && curY > 0)
            {
                curX++;
                curY--;
                if (_board[curX, curY] == player)
                {
                    diagLength++;
                    checkingStones.Add((curX, curY));
                }
                else if (_board[curX, curY] == 3)
                {
                    diagLength++;
                    checkingStones.Add((curX, curY));
                    bonusCount++;
                }
                else
                {
                    break;
                }
            }

            if (diagLength >= 5)
            {
                if (diagLength >= 10) clearedLines++;
                else connectedLines++;
            }

            return (connectedLines, bonusCount, clearedLines);
        }

        public float[] GetBoardTensor(string networkType)
        {
            var boardTensor = networkType == "policy" ? new float[14, 19, 19] : new float[13, 19, 19];

            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
            {
                if (_board[i, j] == 1) boardTensor[0, i, j] = 1.0f;
                else if (_board[i, j] == 2) boardTensor[1, i, j] = 1.0f;
                else if (_board[i, j] == 3) boardTensor[2, i, j] = 1.0f;

                if (_board[i, j] == 0)
                {
                    var (conn, bonus, clear) = CheckConnections(i, j, _currentPlayer);
                    boardTensor[7, i, j] = conn;
                    boardTensor[8, i, j] = bonus;
                    boardTensor[9, i, j] = clear;
                }
            }

            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
                boardTensor[3, i, j] = 1.0f;

            if (_prevActions != null)
            {
                var player = 3 - _currentPlayer;
                for (var i = 0; i < _prevActions.Count; i++)
                {
                    foreach (var (y, x) in _prevActions[i]) boardTensor[3 + i, y, x] = player;
                    player = 3 - player;
                }
            }

            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
                boardTensor[11, i, j] = 0.0f;
            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
                boardTensor[12, i, j] = _currentPlayer;

            float[] boardTensor1D;

            if (networkType == "policy")
            {
                for (var i = 0; i < 19; i++)
                for (var j = 0; j < 19; j++)
                    boardTensor[13, i, j] = _stoneType;

                boardTensor1D = new float[14 * 19 * 19];
            }
            else
            {
                boardTensor1D = new float[13 * 19 * 19];
            }

            return boardTensor1D;
        }

        public static (int, int)[] GetStones(int i, int j, int r, int stoneType)
        {
            var stones = new (int, int)[3];
            if (stoneType == 1)
            {
                if (r == 0)
                {
                    stones[0] = (i, j);
                    stones[1] = (i, j + 1);
                    stones[2] = (i, j + 2);
                }
                else
                {
                    stones[0] = (i, j);
                    stones[1] = (i + 1, j);
                    stones[2] = (i + 2, j);
                }
            }
            else if (stoneType == 2)
            {
                if (r == 0)
                {
                    stones[0] = (i, j);
                    stones[1] = (i, j + 1);
                    stones[2] = (i + 1, j + 1);
                }
                else if (r == 1)
                {
                    stones[0] = (i, j);
                    stones[1] = (i, j + 1);
                    stones[2] = (i + 1, j);
                }
                else if (r == 2)
                {
                    stones[0] = (i, j);
                    stones[1] = (i, j + 1);
                    stones[2] = (i - 1, j + 1);
                }
                else
                {
                    stones[0] = (i, j);
                    stones[1] = (i + 1, j);
                    stones[2] = (i + 1, j + 1);
                }
            }
            else
            {
                if (r == 0)
                {
                    stones[0] = (i, j);
                    stones[1] = (i - 1, j + 1);
                    stones[2] = (i - 2, j + 2);
                }
                else
                {
                    stones[0] = (i, j);
                    stones[1] = (i + 1, j + 1);
                    stones[2] = (i + 2, j + 2);
                }
            }

            return stones;
        }
    }
}