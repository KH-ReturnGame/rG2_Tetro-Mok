using UnityEngine;

namespace Global
{
    public class CameraResolution : MonoBehaviour
    {
        private void Awake()
        {
            var mainCamera = GetComponent<Camera>();
            var rect = mainCamera.rect;
            var scaleHeight = (float)Screen.width / Screen.height / ((float)16 / 9);
            var scaleWidth = 1f / scaleHeight;

            if (scaleHeight < 1)
            {
                rect.height = scaleHeight;
                rect.y = (1f - scaleHeight) / 2f;
            }
            else
            {
                rect.width = scaleWidth;
                rect.x = (1f - scaleWidth) / 2f;
            }

            mainCamera.rect = rect;
        }
    }
}