using UnityEngine;

namespace Global
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private GameObject modeButtons;
        [SerializeField] private GameObject otherButtons;

        private void Start()
        {
            SoundManager.StopAllSounds();
            modeButtons.SetActive(false);
            otherButtons.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ExitGame();
        }

        public void GameMode()
        {
            modeButtons.SetActive(true);
            otherButtons.SetActive(false);
            SoundManager.PlaySound("UI");
        }

        public void ExitGame()
        {
            SoundManager.PlaySound("UI");
            Application.Quit();
        }
    }
}