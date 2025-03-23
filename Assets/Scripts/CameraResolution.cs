using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private void Awake()
    {
        var camera = GetComponent<Camera>();
        var rect = camera.rect;
        var scaleheight = (float)Screen.width / Screen.height / ((float)16 / 9);
        var scalewidth = 1f / scaleheight;

        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }

        camera.rect = rect;
    }
}