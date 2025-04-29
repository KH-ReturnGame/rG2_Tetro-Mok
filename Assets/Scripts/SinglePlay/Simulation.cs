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
                    stones = GetStones(i, j, 2);
                    if (IsExists(stones[1].Item1, stones[1].Item2) && IsExists(stones[2].Item1, stones[2].Item2))
                        moves.Add((i, j, 3));
                    stones = GetStones(i, j, 2);
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

        public void MakeMove((int i, int j, int r) move)
        {
            var random = new Random();
            foreach ((int y, int x) pos in GetStones(move.i, move.j, move.r)) _board[pos.y, pos.x] = _currentPlayer;

            _stoneType = random.Next(1, 4);
            _depth++;
            _currentPlayer = 3 - _currentPlayer; // 1 <-> 2 스위치
        }


        // TODO: 이거 하면 MCTS는 끝. 이후 Value Function 설계해야.
        // MakeMove 이전에 동작함. 유의하세요
        public double CalculateScore(int i, int j, int r)
        {
            var score = 0.0;

            var connectionScore = 0.0;
            foreach (var (y, x) in GetStones(i, j, r))
            {
                // 가로 방향 연결 확인
                var horizontalCount = 1;
                var horizontalSkips = 0;
                for (var k = 1; k < 10; k++)
                {
                    if (x + k > 18) break;

                    if (_board[y, x + k] == _currentPlayer)
                    {
                        horizontalCount++;
                    }
                    else if (_board[y, x + k] == 3)
                    {
                        horizontalCount++;
                        connectionScore += 3;
                    }
                    else if (_board[y, x + k] == 0 && horizontalSkips < 3)
                    {
                        horizontalSkips++;
                    }
                    else
                    {
                        break;
                    }
                }

                for (var k = 1; k < 10; k++)
                {
                    if (x - k < 0) break;
                    if (_board[y, x - k] == _currentPlayer)
                    {
                        horizontalCount++;
                    }
                    else if (_board[y, x - k] == 3)
                    {
                        horizontalCount++;
                        connectionScore += 3;
                    }
                    else if (_board[y, x - k] == 0 && horizontalSkips < 3)
                    {
                        horizontalSkips++;
                    }
                    else
                    {
                        break;
                    }
                }

                connectionScore += Math.Min(horizontalCount, 10);

                // 세로 방향 연결 확인
                var verticalCount = 1;
                var verticalSkips = 0;
                for (var k = 1; k < 10; k++)
                {
                    if (y + k > 18) break;
                    if (_board[y + k, x] == _currentPlayer)
                    {
                        verticalCount++;
                    }
                    else if (_board[y + k, x] == 3)
                    {
                        verticalCount++;
                        connectionScore += 3;
                    }
                    else if (_board[y + k, x] == 0 && verticalSkips < 3)
                    {
                        verticalSkips++;
                    }
                    else
                    {
                        break;
                    }
                }

                for (var k = 1; k < 10; k++)
                {
                    if (y - k < 0) break;
                    if (_board[y - k, x] == _currentPlayer)
                    {
                        verticalCount++;
                    }
                    else if (_board[y - k, x] == 3)
                    {
                        verticalCount++;
                        connectionScore += 3;
                    }
                    else if (_board[y - k, x] == 0 && verticalSkips < 3)
                    {
                        verticalSkips++;
                    }
                    else
                    {
                        break;
                    }
                }

                connectionScore += Math.Min(verticalCount, 10);

                // 대각선 방향 (/) 연결 확인
                var diagonal1Count = 1;
                var diagonal1Skips = 0;
                for (var k = 1; k < 10; k++)
                {
                    if (y - k < 0 || x + k > 18) break;
                    if (_board[y - k, x + k] == _currentPlayer)
                    {
                        diagonal1Count++;
                    }
                    else if (_board[y - k, x + k] == 3)
                    {
                        diagonal1Count++;
                        connectionScore += 3;
                    }
                    else if (_board[y - k, x + k] == 0 && diagonal1Skips < 3)
                    {
                        diagonal1Skips++;
                    }
                    else
                    {
                        break;
                    }
                }

                for (var k = 1; k < 10; k++)
                {
                    if (y + k > 18 || x - k < 0) break;
                    if (_board[y + k, x - k] == _currentPlayer)
                    {
                        diagonal1Count++;
                    }
                    else if (_board[y + k, x - k] == 3)
                    {
                        diagonal1Count++;
                        connectionScore += 3;
                    }
                    else if (_board[y + k, x - k] == 0 && diagonal1Skips < 3)
                    {
                        diagonal1Skips++;
                    }
                    else
                    {
                        break;
                    }
                }

                connectionScore += Math.Min(diagonal1Count, 10);

                // 대각선 방향 (\) 연결 확인
                var diagonal2Count = 1;
                var diagonal2Skips = 0;
                for (var k = 1; k < 10; k++)
                {
                    if (y + k > 18 || x + k > 18) break;
                    if (_board[y + k, x + k] == _currentPlayer)
                    {
                        diagonal2Count++;
                    }
                    else if (_board[y + k, x + k] == 3)
                    {
                        diagonal2Count++;
                        connectionScore += 3;
                    }
                    else if (_board[y + k, x + k] == 0 && diagonal2Skips < 3)
                    {
                        diagonal2Skips++;
                    }
                    else
                    {
                        break;
                    }
                }

                for (var k = 1; k < 10; k++)
                {
                    if (y - k < 0 || x - k < 0) break;
                    if (_board[y - k, x - k] == _currentPlayer)
                    {
                        diagonal2Count++;
                    }
                    else if (_board[y - k, x - k] == 3)
                    {
                        diagonal2Count++;
                        connectionScore += 3;
                    }
                    else if (_board[y - k, x - k] == 0 && diagonal2Skips < 3)
                    {
                        diagonal2Skips++;
                    }
                    else
                    {
                        break;
                    }
                }

                connectionScore += Math.Min(diagonal2Count, 10);
            }

            // 여기에 방해 판정
            var otherPlayer = 3 - _currentPlayer;
            var interferenceScore = 0;

            // 가로 방향 방해 확인
            for (var m = 0; m < 19; m++)
            for (var n = 0; n < 19; n++)
                if (_board[m, n] == otherPlayer)
                {
                    var count = 1;
                    var skips = 0;
                    for (var k = 1; k < 10; k++)
                        if (n + k < 19 && _board[m, n + k] == otherPlayer) count++;
                        else if (n + k < 19 && _board[m, n + k] == 0 && skips < 3)
                            skips++;
                        else break;

                    if (count >= 3 && IsInterfering(m, n, count + skips, 0, 1, i, j, r))
                        interferenceScore += Math.Min(count, 8);
                }

            // 세로 방향 방해 확인
            for (var m = 0; m < 19; m++)
            for (var n = 0; n < 19; n++)
                if (_board[m, n] == otherPlayer)
                {
                    var count = 1;
                    var skips = 0;
                    for (var k = 1; k < 10; k++)
                        if (m + k < 19 && _board[m + k, n] == otherPlayer) count++;
                        else if (m + k < 19 && _board[m + k, n] == 0 && skips < 3)
                            skips++;
                        else break;

                    if (count >= 3 && IsInterfering(m, n, count + skips, 1, 0, i, j, r))
                        interferenceScore += Math.Min(count, 8);
                }

            // 대각선 방향 (/) 방해 확인
            for (var m = 0; m < 19; m++)
            for (var n = 0; n < 19; n++)
                if (_board[m, n] == otherPlayer)
                {
                    var count = 1;
                    var skips = 0;
                    for (var k = 1; k < 10; k++)
                        if (m - k >= 0 && n + k < 19 && _board[m - k, n + k] == otherPlayer) count++;
                        else if (m - k >= 0 && n + k < 19 && _board[m - k, n + k] == 0 && skips < 3)
                            skips++;
                        else break;

                    if (count >= 3 && IsInterfering(m, n, count + skips, -1, 1, i, j, r))
                        interferenceScore += Math.Min(count, 8);
                }

            // 대각선 방향 (\) 방해 확인
            for (var m = 0; m < 19; m++)
            for (var n = 0; n < 19; n++)
                if (_board[m, n] == otherPlayer)
                {
                    var count = 1;
                    var skips = 0;
                    for (var k = 1; k < 10; k++)
                        if (m + k < 19 && n + k < 19 && _board[m + k, n + k] == otherPlayer) count++;
                        else if (m + k < 19 && n + k < 19 && _board[m + k, n + k] == 0 && skips < 3)
                            skips++;
                        else break;

                    if (count >= 3 && IsInterfering(m, n, count + skips, 1, 1, i, j, r))
                        interferenceScore += Math.Min(count, 8);
                }

            score += connectionScore;
            score += interferenceScore;
            score /= 20;

            return score * (_currentPlayer * 2 - 3);
        }

        private bool IsInterfering(int startY, int startX, int count, int dy, int dx, int i, int j, int r)
        {
            var checkX = startX - 2 * dx;
            var checkY = startY - 2 * dy;
            var stones = GetStones(i, j, r);

            for (var m = -2; m <= count + 1; m++)
            {
                if ((checkY == stones[0].Item1 && checkX == stones[0].Item2) ||
                    (checkY == stones[1].Item1 && checkX == stones[1].Item2) ||
                    (checkY == stones[2].Item1 && checkX == stones[2].Item2)) return true;
                checkY += dy;
                checkX += dx;
            }

            return false;
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

        // 게임 종료 여부 (승리 또는 무승부)
        public bool IsTerminal(int maxDepth)
        {
            return _depth >= maxDepth || GetMoves().Count == 0;
        }
    }

    // MCTS 노드 클래스
    public class Node
    {
        public Node((int, int, int)? move, Node? parent, TriminoMok state)
        {
            Move = move;
            Parent = parent;
            State = state;
            UntriedMoves = state.GetMoves();
        }

        public (int, int, int)? Move { get; }
        public Node? Parent { get; }
        public TriminoMok State { get; }
        public List<Node> Children { get; } = new();
        public List<(int, int, int)> UntriedMoves { get; private set; }
        public int Visits { get; private set; }
        public double Score { get; private set; }

        // UCB1 점수 계산
        private double Ucb1(int parentVisits)
        {
            if (Visits == 0) return double.MaxValue;
            return Score / Visits + 1.4 * Math.Sqrt(Math.Log(parentVisits) / Visits);
        }

        // 가장 유망한 자식 선택
        public Node SelectChild()
        {
            return Children.OrderByDescending(c => c.Ucb1(Visits)).First();
        }

        // 자식 노드 추가
        public Node AddChild((int, int, int) move, TriminoMok state)
        {
            var child = new Node(move, this, state);
            Children.Add(child);
            return child;
        }

        // 통계 업데이트
        public void Update(double score)
        {
            Visits++;
            Score += score;
        }

        // UntriedMoves 업데이트
        public void UpdateUntriedMoves()
        {
            UntriedMoves = State.GetMoves();
        }
    }

    // MCTS 클래스
    public class Mcts
    {
        private readonly Random _random = new();

        public (int, int, int) Run(TriminoMok rootState, int iterations = 50)
        {
            var root = new Node(null, null, new TriminoMok(rootState));

            for (var i = 0; i < iterations; i++)
            {
                var node = root;
                var state = new TriminoMok(root.State);

                // 1. 선택
                while (node.UntriedMoves.Count == 0 && node.Children.Count > 0)
                {
                    node = node.SelectChild();
                    state.MakeMove(node.Move!.Value);
                    node.UpdateUntriedMoves();
                }

                // 2. 확장
                if (node.UntriedMoves.Count > 0)
                {
                    var move = node.UntriedMoves[_random.Next(node.UntriedMoves.Count)];
                    state.MakeMove(move);
                    node.UpdateUntriedMoves();
                    node = node.AddChild(move, new TriminoMok(state));
                }

                // 3. 시뮬레이션
                var score = 0.0;

                while (!state.IsTerminal(10))
                {
                    var moves = state.GetMoves();
                    if (moves.Count == 0) break;
                    var selectedMove = moves[_random.Next(moves.Count)];
                    score += state.CalculateScore(selectedMove.Item1, selectedMove.Item2, selectedMove.Item3);
                    state.MakeMove(selectedMove);
                    node.UpdateUntriedMoves();
                }

                // 4. 역전파
                while (node != null)
                {
                    node.Update(score);
                    node = node.Parent;
                }
            }

            // 가장 높은 점수를 가진 자식 노드의 수 반환
            return root.Children.OrderByDescending(c => c.Score / c.Visits).First().Move!.Value;
        }
    }
}