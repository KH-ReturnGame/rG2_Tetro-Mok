using UnityEngine;

namespace Global
{
    public class TitleManager : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ExitGame();
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}