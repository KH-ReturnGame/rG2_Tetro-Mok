using Unity.Sentis;
using UnityEngine;

namespace SinglePlay.AI
{
    public class PolicyNetwork : MonoBehaviour
    {
        [SerializeField] private ModelAsset modelAsset;
        private TensorShape _inputShape;
        private Model _runtimeModel;
        public Model RuntimeModel => _runtimeModel;
        private ModelAsset _staticModelAsset;
        private Worker _worker;

        private void Start()
        {
            _inputShape = new TensorShape(1, 14, 19, 19);
            _staticModelAsset = modelAsset;
            _runtimeModel = ModelLoader.Load(_staticModelAsset);
            _worker = new Worker(_runtimeModel, BackendType.CPU);
        }

        public float[] Forward(float[] data)
        {
            var inputTensor = new Tensor<float>(_inputShape, data);
            _worker.Schedule(inputTensor);
            var outputTensor = _worker.PeekOutput() as Tensor<float>;
            var result = outputTensor.DownloadToArray();

            return result; // 19 * 19 * 4
        }
    }
}