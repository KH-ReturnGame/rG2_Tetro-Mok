using System;
using System.Collections.Generic;
using System.Linq;

namespace SinglePlay
{
    public class TriminoMok
    {
        private readonly int[,] _board;
        private List<(int, int)> _availableSpaces;
        private int _currentPlayer;
        private int _depth;
        private int _stoneType;

        public TriminoMok(int[,] board, int type)
        {
            _depth = 0;
            _currentPlayer = 2;
            _stoneType = type;
            _board = board;

            _availableSpaces = new List<(int, int)>();
        }

        public TriminoMok(TriminoMok otherBoard)
        {
            _board = (int[,])otherBoard._board.Clone();
            _currentPlayer = otherBoard._currentPlayer;
            _stoneType = otherBoard._stoneType;
            _depth = otherBoard._depth;

            _availableSpaces = new List<(int, int)>();
        }

        /// <summary>
        ///     게입 보드에서 유효한 공간을 반환함
        /// </summary>
        /// <returns>List(int y, int x)</returns>
        private List<(int, int)> GetAvailableSpaces()
        {
            var availableSpaces = new List<(int, int)>();

            // 모서리 구하기
            var corners = new List<(int, int)>();

            for (var i = 0; i < 19; i++)
            for (var j = 1; j < 19; j++)
                if (_board[i, j - 1] == 0 && _board[i, j] != 0)
                    corners.Add((i, j - 1));

            for (var i = 0; i < 19; i++)
            for (var j = 19 - 2; j >= 0; j--)
                if (_board[i, j + 1] == 0 && _board[i, j] != 0)
                    corners.Add((i, j + 1));

            for (var i = 1; i < 19; i++)
            for (var j = 0; j < 19; j++)
                if (_board[i - 1, j] == 0 && _board[i, j] != 0)
                    corners.Add((i - 1, j));

            for (var i = 19 - 2; i >= 0; i--)
            for (var j = 0; j < 19; j++)
                if (_board[i + 1, j] == 0 && _board[i, j] != 0)
                    corners.Add((i + 1, j));

            for (var i = 1; i < 19; i++)
            for (var j = 1; j < 19; j++)
                if (_board[i - 1, j - 1] == 0 && _board[i, j] != 0)
                    corners.Add((i - 1, j - 1));

            for (var i = 19 - 2; i >= 0; i--)
            for (var j = 19 - 2; j >= 0; j--)
                if (_board[i + 1, j + 1] == 0 && _board[i, j] != 0)
                    corners.Add((i + 1, j + 1));

            for (var i = 1; i < 19; i++)
            for (var j = 19 - 2; j >= 0; j--)
                if (_board[i - 1, j + 1] == 0 && _board[i, j] != 0)
                    corners.Add((i - 1, j + 1));

            for (var i = 19 - 2; i >= 0; i--)
            for (var j = 1; j < 19; j++)
                if (_board[i + 1, j - 1] == 0 && _board[i, j] != 0)
                    corners.Add((i + 1, j - 1));

            // 공간 더하기
            foreach (var (i, j) in corners)
                for (var m = -2; m < 3; m++)
                for (var n = -2; n < 3; n++)
                {
                    if (i + m < 0 || i + m > 18 || j + n < 0 || j + n > 18) continue;
                    if (_board[i + m, j + n] != 0) continue;
                    availableSpaces.Add((i + m, j + n));
                }

            return availableSpaces;
        }

        /// <summary>
        ///     블럭을 배치할 수 있는 위치를 반환한다
        /// </summary>
        /// <returns>List of ( y, x, rotation(1~4) )</returns>
        public List<(int, int, int)> GetMoves()
        {
            _availableSpaces = GetAvailableSpaces();

            // (y, x, rotation)
            var moves = new List<(int, int, int)>();

            if (_stoneType == 2)
                foreach (var (i, j) in _availableSpaces)
                {
                    var stones = GetStones(i, j, 1);
                    if (IsExists(stones[1].Item1, stones[1].Item2) && IsExists(stones[2].Item1, stones[2].Item2))
                        moves.Add((i, j, 1));
                    stones = GetStones(i, j, 2);
                    if (IsExists(stones[1].Item1, stones[1].Item2) && IsExists(stones[2].Item1, stones[2].Item2))
                        moves.Add((i, j, 2));
                    stones = GetStones(i, j, 3);
                    if (IsExists(stones[1].Item1, stones[1].Item2) && IsExists(stones[2].Item1, stones[2].Item2))
                        moves.Add((i, j, 3));
                    stones = GetStones(i, j, 4);
                    if (IsExists(stones[1].Item1, stones[1].Item2) && IsExists(stones[2].Item1, stones[2].Item2))
                        moves.Add((i, j, 4));
                }
            else
                foreach (var (i, j) in _availableSpaces)
                {
                    var stones = GetStones(i, j, 1);
                    if (IsExists(stones[1].Item1, stones[1].Item2) && IsExists(stones[2].Item1, stones[2].Item2))
                        moves.Add((i, j, 1));
                    stones = GetStones(i, j, 2);
                    if (IsExists(stones[1].Item1, stones[1].Item2) && IsExists(stones[2].Item1, stones[2].Item2))
                        moves.Add((i, j, 2));
                }

            return moves;
        }

        private bool IsExists(int i, int j)
        {
            return _availableSpaces.Exists(tuple => tuple.Item1 == i && tuple.Item2 == j);
        }

        public void MakeMove((int i, int j, int r) move, bool addDepth = true)
        {
            var random = new Random();
            foreach ((int y, int x) pos in GetStones(move.i, move.j, move.r)) _board[pos.y, pos.x] = _currentPlayer;

            _stoneType = random.Next(1, 4);
            if (addDepth) _depth++;
            _currentPlayer = 3 - _currentPlayer; // 1 <-> 2 스위치
        }

        // public double CalculateScore(int i, int j, int r)
        // {
        //     var score = 0.0;
        //
        //     // 연결 점수 산정
        //
        //     var connectionScore = 0.0;
        //
        //     foreach (var (y, x) in GetStones(i, j, r))
        //     {
        //         var horizontalCount = 1;
        //         var horizontalSkips = 0;
        //         for (var k = 1; k < 10; k++)
        //         {
        //             if (x + k > 18) break;
        //
        //             if (_board[y, x + k] == _currentPlayer)
        //             {
        //                 horizontalCount++;
        //             }
        //             else if (_board[y, x + k] == 3)
        //             {
        //                 horizontalCount++;
        //                 connectionScore += 3;
        //             }
        //             else if (_board[y, x + k] == 0 && horizontalSkips < 3)
        //             {
        //                 horizontalSkips++;
        //             }
        //             else
        //             {
        //                 break;
        //             }
        //         }
        //
        //         for (var k = 1; k < 10; k++)
        //         {
        //             if (x - k < 0) break;
        //             if (_board[y, x - k] == _currentPlayer)
        //             {
        //                 horizontalCount++;
        //             }
        //             else if (_board[y, x - k] == 3)
        //             {
        //                 horizontalCount++;
        //                 connectionScore += 3;
        //             }
        //             else if (_board[y, x - k] == 0 && horizontalSkips < 3)
        //             {
        //                 horizontalSkips++;
        //             }
        //             else
        //             {
        //                 break;
        //             }
        //         }
        //
        //         connectionScore += Math.Min(horizontalCount, 10);
        //
        //         var verticalCount = 1;
        //         var verticalSkips = 0;
        //         for (var k = 1; k < 10; k++)
        //         {
        //             if (y + k > 18) break;
        //             if (_board[y + k, x] == _currentPlayer)
        //             {
        //                 verticalCount++;
        //             }
        //             else if (_board[y + k, x] == 3)
        //             {
        //                 verticalCount++;
        //                 connectionScore += 3;
        //             }
        //             else if (_board[y + k, x] == 0 && verticalSkips < 3)
        //             {
        //                 verticalSkips++;
        //             }
        //             else
        //             {
        //                 break;
        //             }
        //         }
        //
        //         for (var k = 1; k < 10; k++)
        //         {
        //             if (y - k < 0) break;
        //             if (_board[y - k, x] == _currentPlayer)
        //             {
        //                 verticalCount++;
        //             }
        //             else if (_board[y - k, x] == 3)
        //             {
        //                 verticalCount++;
        //                 connectionScore += 3;
        //             }
        //             else if (_board[y - k, x] == 0 && verticalSkips < 3)
        //             {
        //                 verticalSkips++;
        //             }
        //             else
        //             {
        //                 break;
        //             }
        //         }
        //
        //         connectionScore += Math.Min(verticalCount, 10);
        //
        //         var diagonal1Count = 1;
        //         var diagonal1Skips = 0;
        //         for (var k = 1; k < 10; k++)
        //         {
        //             if (y - k < 0 || x + k > 18) break;
        //             if (_board[y - k, x + k] == _currentPlayer)
        //             {
        //                 diagonal1Count++;
        //             }
        //             else if (_board[y - k, x + k] == 3)
        //             {
        //                 diagonal1Count++;
        //                 connectionScore += 3;
        //             }
        //             else if (_board[y - k, x + k] == 0 && diagonal1Skips < 3)
        //             {
        //                 diagonal1Skips++;
        //             }
        //             else
        //             {
        //                 break;
        //             }
        //         }
        //
        //         for (var k = 1; k < 10; k++)
        //         {
        //             if (y + k > 18 || x - k < 0) break;
        //             if (_board[y + k, x - k] == _currentPlayer)
        //             {
        //                 diagonal1Count++;
        //             }
        //             else if (_board[y + k, x - k] == 3)
        //             {
        //                 diagonal1Count++;
        //                 connectionScore += 3;
        //             }
        //             else if (_board[y + k, x - k] == 0 && diagonal1Skips < 3)
        //             {
        //                 diagonal1Skips++;
        //             }
        //             else
        //             {
        //                 break;
        //             }
        //         }
        //
        //         connectionScore += Math.Min(diagonal1Count, 10);
        //
        //         var diagonal2Count = 1;
        //         var diagonal2Skips = 0;
        //         for (var k = 1; k < 10; k++)
        //         {
        //             if (y + k > 18 || x + k > 18) break;
        //             if (_board[y + k, x + k] == _currentPlayer)
        //             {
        //                 diagonal2Count++;
        //             }
        //             else if (_board[y + k, x + k] == 3)
        //             {
        //                 diagonal2Count++;
        //                 connectionScore += 3;
        //             }
        //             else if (_board[y + k, x + k] == 0 && diagonal2Skips < 3)
        //             {
        //                 diagonal2Skips++;
        //             }
        //             else
        //             {
        //                 break;
        //             }
        //         }
        //
        //         for (var k = 1; k < 10; k++)
        //         {
        //             if (y - k < 0 || x - k < 0) break;
        //             if (_board[y - k, x - k] == _currentPlayer)
        //             {
        //                 diagonal2Count++;
        //             }
        //             else if (_board[y - k, x - k] == 3)
        //             {
        //                 diagonal2Count++;
        //                 connectionScore += 3;
        //             }
        //             else if (_board[y - k, x - k] == 0 && diagonal2Skips < 3)
        //             {
        //                 diagonal2Skips++;
        //             }
        //             else
        //             {
        //                 break;
        //             }
        //         }
        //
        //         connectionScore += Math.Min(diagonal2Count, 10);
        //     }
        //
        //     // 방해 점수 산정
        //
        //     var otherPlayer = 3 - _currentPlayer;
        //     var interferenceScore = 0;
        //
        //     for (var m = 0; m < 19; m++)
        //     for (var n = 0; n < 19; n++)
        //         if (_board[m, n] == otherPlayer)
        //         {
        //             var count = 1;
        //             var skips = 0;
        //             for (var k = 1; k < 10; k++)
        //                 if (n + k < 19 && _board[m, n + k] == otherPlayer) count++;
        //                 else if (n + k < 19 && _board[m, n + k] == 0 && skips < 3)
        //                     skips++;
        //                 else break;
        //
        //             if (count >= 3 && IsInterfering(m, n, count + skips, 0, 1, i, j, r))
        //                 interferenceScore += Math.Min(count, 8);
        //         }
        //
        //     for (var m = 0; m < 19; m++)
        //     for (var n = 0; n < 19; n++)
        //         if (_board[m, n] == otherPlayer)
        //         {
        //             var count = 1;
        //             var skips = 0;
        //             for (var k = 1; k < 10; k++)
        //                 if (m + k < 19 && _board[m + k, n] == otherPlayer) count++;
        //                 else if (m + k < 19 && _board[m + k, n] == 0 && skips < 3)
        //                     skips++;
        //                 else break;
        //
        //             if (count >= 3 && IsInterfering(m, n, count + skips, 1, 0, i, j, r))
        //                 interferenceScore += Math.Min(count, 8);
        //         }
        //
        //     for (var m = 0; m < 19; m++)
        //     for (var n = 0; n < 19; n++)
        //         if (_board[m, n] == otherPlayer)
        //         {
        //             var count = 1;
        //             var skips = 0;
        //             for (var k = 1; k < 10; k++)
        //                 if (m - k >= 0 && n + k < 19 && _board[m - k, n + k] == otherPlayer) count++;
        //                 else if (m - k >= 0 && n + k < 19 && _board[m - k, n + k] == 0 && skips < 3)
        //                     skips++;
        //                 else break;
        //
        //             if (count >= 3 && IsInterfering(m, n, count + skips, -1, 1, i, j, r))
        //                 interferenceScore += Math.Min(count, 8);
        //         }
        //
        //     for (var m = 0; m < 19; m++)
        //     for (var n = 0; n < 19; n++)
        //         if (_board[m, n] == otherPlayer)
        //         {
        //             var count = 1;
        //             var skips = 0;
        //             for (var k = 1; k < 10; k++)
        //                 if (m + k < 19 && n + k < 19 && _board[m + k, n + k] == otherPlayer) count++;
        //                 else if (m + k < 19 && n + k < 19 && _board[m + k, n + k] == 0 && skips < 3)
        //                     skips++;
        //                 else break;
        //
        //             if (count >= 3 && IsInterfering(m, n, count + skips, 1, 1, i, j, r))
        //                 interferenceScore += Math.Min(count, 8);
        //         }
        //
        //     score += connectionScore;
        //     //score += interferenceScore * 0.5;
        //     score /= 20;
        //     //score = 1 / (1 + Math.Exp(-score));
        //
        //     return score * (_currentPlayer * 2 - 3);
        // }
        //
        // private bool IsInterfering(int startY, int startX, int count, int dy, int dx, int i, int j, int r)
        // {
        //     var checkX = startX - 2 * dx;
        //     var checkY = startY - 2 * dy;
        //     var stones = GetStones(i, j, r);
        //
        //     for (var m = -2; m <= count + 1; m++)
        //     {
        //         if ((checkY == stones[0].Item1 && checkX == stones[0].Item2) ||
        //             (checkY == stones[1].Item1 && checkX == stones[1].Item2) ||
        //             (checkY == stones[2].Item1 && checkX == stones[2].Item2)) return true;
        //         checkY += dy;
        //         checkX += dx;
        //     }
        //
        //     return false;
        // }
        public float IsWin()
        {
            // 플레이어 별 점수를 저장하는 배열 (인덱스 0: 첫 번째 플레이어, 인덱스 1: 두 번째 플레이어)
            var scores = new int[2];

            // 가로 연결 탐색 (왼쪽에서 오른쪽으로)
            for (var y = 18; y >= 0; y--)
            {
                int checkingStoneType = 0, connectionLength = 0, bonusNumber = 0;
                for (var x = 0; x < 19; x++)
                {
                    // 빈 칸일 경우
                    if (_board[x, y] == 0)
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 상태 초기화
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                        continue;
                    }

                    // 첫 돌이 놓여있을 때
                    if (checkingStoneType == 0)
                    {
                        // 특수 돌(3)이 아니면 현재 돌 타입으로 설정
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    // 같은 색 돌이 놓여있을 때
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    // 특수 돌(3)이 놓여있을 때 - 연결을 유지하면서 보너스 점수 추가
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    // 다른 색 돌이 놓여있을 때
                    else
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 새로운 돌 타입으로 변경
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }
                }

                // 행의 끝에 도달했을 때 마지막 연결 확인
                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 5 + bonusNumber * 3;
            }

            // 세로 연결 탐색 (아래에서 위로)
            for (var x = 0; x < 19; x++)
            {
                int checkingStoneType = 0, connectionLength = 0, bonusNumber = 0;
                for (var y = 18; y >= 0; y--)
                {
                    // 빈 칸일 경우
                    if (_board[x, y] == 0)
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 상태 초기화
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                        continue;
                    }

                    // 첫 돌이 놓여있을 때
                    if (checkingStoneType == 0)
                    {
                        // 특수 돌(3)이 아니면 현재 돌 타입으로 설정
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    // 같은 색 돌이 놓여있을 때
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    // 특수 돌(3)이 놓여있을 때 - 연결을 유지하면서 보너스 점수 추가
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    // 다른 색 돌이 놓여있을때
                    else
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 새로운 돌 타입으로 변경
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }
                }

                // 열의 끝에 도달했을 때 마지막 연결 확인
                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 5 + bonusNumber * 3;
            }

            // 왼쪽 위에서 오른쪽 아래 대각선 탐색 (↘)
            // 첫 번째 반복: 맨 위 행에서 시작하는 대각선
            for (var startX = 0; startX < 19; startX++)
            {
                int checkingStoneType = 0, connectionLength = 0, bonusNumber = 0;
                for (int x = startX, y = 0; x < 19 && y < 19; x++, y++)
                {
                    // 빈 칸일 경우
                    if (_board[x, y] == 0)
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 상태 초기화
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                        continue;
                    }

                    // 첫 돌이 놓여있을 때
                    if (checkingStoneType == 0)
                    {
                        // 특수 돌(3)이 아니면 현재 돌 타입으로 설정
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    // 같은 색 돌이 놓여있을 때
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    // 특수 돌(3)이 놓여있을 때 - 연결을 유지하면서 보너스 점수 추가
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    // 다른 색 돌이 놓여있을 때
                    else
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 새로운 돌 타입으로 변경
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }
                }

                // 대각선의 끝에 도달했을 때 마지막 연결 확인
                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 5 + bonusNumber * 3;
            }

            // 두 번째 반복: 맨 왼쪽 열에서 시작하는 대각선 (첫 번째 행은 이미 처리되었으므로 제외)
            for (var startY = 1; startY < 19; startY++)
            {
                int checkingStoneType = 0, connectionLength = 0, bonusNumber = 0;
                for (int x = 0, y = startY; x < 19 && y < 19; x++, y++)
                {
                    // 빈 칸일 경우
                    if (_board[x, y] == 0)
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 상태 초기화
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                        continue;
                    }

                    // 첫 돌이 놓여있을 때
                    if (checkingStoneType == 0)
                    {
                        // 특수 돌(3)이 아니면 현재 돌 타입으로 설정
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    // 같은 색 돌이 놓여있을 때
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    // 특수 돌(3)이 놓여있을 때 - 연결을 유지하면서 보너스 점수 추가
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    // 다른 색 돌이 놓여있을 때
                    else
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 새로운 돌 타입으로 변경
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }
                }

                // 대각선의 끝에 도달했을 때 마지막 연결 확인
                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 5 + bonusNumber * 3;
            }

            // 오른쪽 위에서 왼쪽 아래 대각선 탐색 (↙)
            // 첫 번째 반복: 맨 오른쪽 열에서 시작하는 대각선
            for (var startX = 18; startX >= 0; startX--)
            {
                int checkingStoneType = 0, connectionLength = 0, bonusNumber = 0;
                for (int x = startX, y = 0; x >= 0 && y < 19; x--, y++)
                {
                    // 빈 칸일 경우
                    if (_board[x, y] == 0)
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5)
                            scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 상태 초기화
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                        continue;
                    }

                    // 첫 돌이 놓여있을 때
                    if (checkingStoneType == 0)
                    {
                        // 특수 돌(3)이 아니면 현재 돌 타입으로 설정
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    // 같은 색 돌이 놓여있을 때
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    // 특수 돌(3)이 놓여있을 때 - 연결을 유지하면서 보너스 점수 추가
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    // 다른 색 돌이 놓여있을 때
                    else
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5)
                            scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 새로운 돌 타입으로 변경
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }
                }

                // 대각선의 끝에 도달했을 때 마지막 연결 확인
                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 5 + bonusNumber * 3;
            }

            // 두 번째 반복: 맨 위 행에서 시작하는 대각선 (맨 오른쪽 열은 이미 처리되었으므로 제외)
            for (var startY = 1; startY < 19; startY++)
            {
                int checkingStoneType = 0, connectionLength = 0, bonusNumber = 0;
                for (int x = 18, y = startY; x >= 0 && y < 19; x--, y++)
                {
                    // 빈 칸일 경우
                    if (_board[x, y] == 0)
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5)
                            scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 상태 초기화
                        checkingStoneType = 0;
                        connectionLength = 0;
                        bonusNumber = 0;
                        continue;
                    }

                    // 첫 돌이 놓여있을 때
                    if (checkingStoneType == 0)
                    {
                        // 특수 돌(3)이 아니면 현재 돌 타입으로 설정
                        if (_board[x, y] != 3) checkingStoneType = _board[x, y];
                        connectionLength++;
                    }
                    // 같은 색 돌이 놓여있을 때
                    else if (_board[x, y] == checkingStoneType)
                    {
                        connectionLength++;
                    }
                    // 특수 돌(3)이 놓여있을 때 - 연결을 유지하면서 보너스 점수 추가
                    else if (_board[x, y] == 3)
                    {
                        connectionLength++;
                        bonusNumber++;
                    }
                    // 다른 색 돌이 놓여있을 때
                    else
                    {
                        // 연결된 돌이 5개 이상이면 점수 추가
                        if (connectionLength >= 5) scores[checkingStoneType - 1] += 5 + bonusNumber * 3;

                        // 새로운 돌 타입으로 변경
                        checkingStoneType = _board[x, y];
                        connectionLength = 1;
                        bonusNumber = 0;
                    }
                }

                // 대각선의 끝에 도달했을 때 마지막 연결 확인
                if (connectionLength >= 5 && checkingStoneType > 0)
                    scores[checkingStoneType - 1] += 5 + bonusNumber * 3;
            }

            // 승자 결정
            // 두 번째 플레이어의 점수가 더 높으면 1.0 반환
            if (scores[1] > scores[0]) return 1.0f;
            // 첫 번째 플레이어의 점수가 더 높으면 0.0 반환
            if (scores[1] < scores[0]) return 0.0f;
            // 동점일 경우 0.5 반환
            return 0.5f;
        }

        public (int, int)[] GetStones(int i, int j, int r)
        {
            var stones = new (int, int)[3];

            if (_stoneType == 1)
            {
                if (r == 1)
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
            else if (_stoneType == 2)
            {
                if (r == 1)
                {
                    stones[0] = (i, j);
                    stones[1] = (i, j + 1);
                    stones[2] = (i + 1, j + 1);
                }
                else if (r == 2)
                {
                    stones[0] = (i, j);
                    stones[1] = (i, j + 1);
                    stones[2] = (i + 1, j);
                }
                else if (r == 3)
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
                if (r == 1)
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

        public bool IsTerminal(int maxDepth)
        {
            return _depth >= maxDepth || GetMoves().Count == 0;
        }
    }

    public class Node
    {
        public Node((int, int, int)? move, Node parent, TriminoMok state)
        {
            Move = move;
            Parent = parent;
            State = state;
            UntriedMoves = state.GetMoves();
        }

        public (int, int, int)? Move { get; }
        public Node Parent { get; }
        public TriminoMok State { get; }
        public List<Node> Children { get; } = new();
        public List<(int, int, int)> UntriedMoves { get; }
        public int Visits { get; private set; }
        public double Wins { get; private set; }

        private double Ucb1(int parentVisits)
        {
            if (Visits == 0) return double.MaxValue;
            return Wins / Visits + 1.4 * Math.Sqrt(Math.Log(parentVisits) / Visits);
        }

        public Node SelectChild()
        {
            return Children.OrderByDescending(c => c.Ucb1(Visits)).First();
        }

        public Node AddChild((int, int, int) move, TriminoMok state)
        {
            var child = new Node(move, this, state);
            UntriedMoves.Remove(move);
            Children.Add(child);
            return child;
        }

        public void Update(double result)
        {
            Visits++;
            Wins += result;
        }
    }

    public class Mcts
    {
        private readonly Random _random = new();

        public (int, int, int) Run(TriminoMok rootState, int iterations /*, WhiteState.SharedVars sharedVars*/)
        {
            var root = new Node(null, null, new TriminoMok(rootState));

            for (var i = 0; i < iterations; i++)
            {
                // lock (sharedVars)
                // {
                //     sharedVars.Progress = (float)(i + 1) / iterations;
                //     sharedVars.IsChanged = true;
                // }

                var node = TreePolicy(root);
                var wins = DefaultPolicy(node.State);
                BackPropagate(node, wins);
            }

            return root.Children.OrderByDescending(c => c.Wins / c.Visits).First().Move!.Value;
        }


        private Node TreePolicy(Node node)
        {
            while (!node.State.IsTerminal(10))
            {
                if (node.UntriedMoves.Count != 0) return Expand(node);

                node = node.SelectChild();
            }

            return node;
        }

        private Node Expand(Node node)
        {
            var move = Choice(node.UntriedMoves);
            node.State.MakeMove(move);
            return node.AddChild(move, node.State);
        }

        private float DefaultPolicy(TriminoMok state)
        {
            var currentState = state;
            while (!currentState.IsTerminal(10))
            {
                var moves = currentState.GetMoves();
                if (moves.Count == 0) break;
                state.MakeMove(Choice(moves));
            }

            return state.IsWin();
        }

        private void BackPropagate(Node node, float wins)
        {
            while (node != null)
            {
                node.Update(wins);
                node = node.Parent;
            }
        }

        private (int, int, int) Choice(List<(int, int, int)> moves)
        {
            return moves[_random.Next(moves.Count)];
        }
    }
}