using Unity.Sentis;
using UnityEngine;

namespace SinglePlay
{
    public class Cnn : MonoBehaviour
    {
        public static Cnn instance;
        private static TensorShape _inputShape, _outputShape;
        private static Model _runtimeModel;
        private static Worker _worker;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            _inputShape = new TensorShape(1, 3, 19, 19);
            _outputShape = new TensorShape(19, 19, 4);
            _runtimeModel = ModelLoader.Load(Application.streamingAssetsPath + "/model_output.sentis");
            _worker = new Worker(_runtimeModel, BackendType.CPU); // WebGL에서 느릴 수도 있다고 함. 어쩌겠어 근데..
        }

        public static (int, int)[] Forward(int[,] board, int currentPlayer, int stoneType)
        {
            var data = new float[3 * 19 * 19];

            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
            {
                if (board[i, j] == 1)
                {
                    board[i, j] = 3 - currentPlayer;
                    data[(2 - currentPlayer) * 19 * 19 + i * 19 + j] = 1;
                }
                else if (board[i, j] == 2)
                {
                    board[i, j] = currentPlayer;
                    data[(currentPlayer - 1) * 19 * 19 + i * 19 + j] = 1;
                }
                else if (board[i, j] == 3)
                {
                    data[i * 19 + j] = 1;
                    data[19 * 19 + i * 19 + j] = 1;
                }

                data[2 * 19 * 19 + i * 19 + j] = stoneType;
            }


            var inputTensor = new Tensor<float>(_inputShape, data);
            _worker.Schedule(inputTensor);
            var outputTensor = _worker.PeekOutput() as Tensor<float>;
            var cpuTensor = outputTensor.ReadbackAndClone();
            cpuTensor.Reshape(_outputShape);

            var tempGame = new TriminoMok(board, stoneType);
            var availableMoves = tempGame.GetMoves();

            // (value, i, j, r)
            var bestMove = (0f, 0, 0, 0);
            for (var i = 0; i < 19; i++)
            for (var j = 0; j < 19; j++)
            {
                if (cpuTensor[i, j, 0] > bestMove.Item1 && availableMoves.Contains((i, j, 1)))
                    bestMove = (cpuTensor[i, j, 0], i, j, 1);
                if (cpuTensor[i, j, 1] > bestMove.Item1 && availableMoves.Contains((i, j, 2)))
                    bestMove = (cpuTensor[i, j, 1], i, j, 2);
                if (cpuTensor[i, j, 2] > bestMove.Item1 && availableMoves.Contains((i, j, 3)))
                    bestMove = (cpuTensor[i, j, 2], i, j, 3);
                if (cpuTensor[i, j, 3] > bestMove.Item1 && availableMoves.Contains((i, j, 4)))
                    bestMove = (cpuTensor[i, j, 3], i, j, 4);
            }

            return tempGame.GetStones(bestMove.Item2, bestMove.Item3, bestMove.Item4);
        }
    }
}