using UnityEngine;
using UnityEngine.SceneManagement;

namespace Global
{
    public class SceneHandler : MonoBehaviour
    {
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            SoundManager.PlaySound("UI");
        }
    }
}