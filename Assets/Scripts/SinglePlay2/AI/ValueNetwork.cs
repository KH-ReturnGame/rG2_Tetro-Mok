using Unity.Sentis;
using UnityEngine;

namespace SinglePlay2.AI
{
    public class ValueNetwork : MonoBehaviour
    {
        [SerializeField] private ModelAsset modelAsset;
        private TensorShape _inputShape;
        private ModelAsset _staticModelAsset;
        private Worker _worker;
        public Model RuntimeModel { get; private set; }

        private void Start()
        {
            _inputShape = new TensorShape(1, 13, 19, 19);
            _staticModelAsset = modelAsset;
            RuntimeModel = ModelLoader.Load(_staticModelAsset);
            _worker = new Worker(RuntimeModel, BackendType.CPU);
        }

        public float Forward(float[] data)
        {
            var inputTensor = new Tensor<float>(_inputShape, data);
            _worker.Schedule(inputTensor);
            var outputTensor = _worker.PeekOutput() as Tensor<float>;
            if (outputTensor != null)
            {
                var result = outputTensor.DownloadToArray()[0];

                return result; // 1
            }

            return 0; // error
        }
    }
}