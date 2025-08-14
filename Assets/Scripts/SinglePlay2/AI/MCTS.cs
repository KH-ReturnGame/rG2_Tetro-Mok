using System;
using System.Collections.Generic;
using System.Linq;

namespace SinglePlay2.AI
{
    public class Node
    {
        public Node((int, int, int) move, Node parent, TriminoMok state)
        {
            Move = move;
            Parent = parent;
            State = state;
            Children = new List<Node>();
            UntriedMoves = state.GetMoves();
            Visits = 0;
            Value = 0.0;
        }

        public (int, int, int) Move { get; }
        public Node Parent { get; }
        public TriminoMok State { get; }
        public List<Node> Children { get; }
        public List<(int, int, int)> UntriedMoves { get; }
        public int Visits { get; private set; }
        public double Value { get; private set; }

        public Node SelectChild(float[] policyValues)
        {
            var children = Children
                .OrderByDescending(c => policyValues[19 * 19 * c.Move.Item1 + 19 * c.Move.Item2 + c.Move.Item3])
                .ToList();

            while (children.Count > 0)
            {
                if (UntriedMoves.Contains(children[0].Move)) return children[0];

                children.RemoveAt(0);
            }

            return Children[0]; // Fallback
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
            Value += result;
        }
    }

    public class Mcts
    {
        private readonly PolicyNetwork _policyNetwork;
        private readonly ValueNetwork _valueNetwork;
        private int _maxDepth;

        public Mcts(PolicyNetwork policy, ValueNetwork value)
        {
            _policyNetwork = policy;
            _valueNetwork = value;
        }

        public (int, int, int) Run(TriminoMok rootState, int iterations, int maxDepth = 40)
        {
            _maxDepth = maxDepth;
            var root = new Node((0, 0, 0), null, new TriminoMok(rootState));

            for (var i = 0; i < iterations; i++)
            {
                var node = TreePolicy(root);
                BackPropagate(node);
            }

            if (root.Children.Count == 0) return (0, 0, 0); // Error detection
            return root.Children.OrderByDescending(c => c.Value / c.Visits).First().Move;
        }

        private Node TreePolicy(Node node)
        {
            while (!node.State.IsTerminal(_maxDepth))
            {
                if (node.UntriedMoves.Count != 0) return Expand(node);

                var boardTensor = node.State.GetBoardTensor("policy");
                var policyValues = _policyNetwork.Forward(boardTensor);
                node = node.SelectChild(policyValues);
            }

            return node;
        }

        private Node Expand(Node node)
        {
            var move = node.UntriedMoves[new Random().Next(node.UntriedMoves.Count)];
            var nextState = new TriminoMok(node.State);
            nextState.MakeMove(move);
            return node.AddChild(move, nextState);
        }

        private void BackPropagate(Node node)
        {
            var boardTensor = node.State.GetBoardTensor("value");
            var winProb = _valueNetwork.Forward(boardTensor);

            while (node != null)
            {
                node.Update(winProb);
                winProb *= 0.05f; // Discount factor
                node = node.Parent;
            }
        }
    }
}