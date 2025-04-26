using UnityEngine;

namespace Global
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private GameObject gameModePanel, optionPanel, otherButtons, optionButton, returnButton;

        private void Start()
        {
            SoundManager.StopAllSounds();
            gameModePanel.SetActive(false);
            optionPanel.SetActive(false);
            otherButtons.SetActive(true);
            optionButton.SetActive(true);
            returnButton.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ExitGame();
        }

        public void ReturnToMain()
        {
            gameModePanel.SetActive(false);
            optionPanel.SetActive(false);
            otherButtons.SetActive(true);
            optionButton.SetActive(true);
            returnButton.SetActive(false);
            SoundManager.PlaySound("UI");
        }

        public void GameMode()
        {
            gameModePanel.SetActive(true);
            otherButtons.SetActive(false);
            optionButton.SetActive(false);
            returnButton.SetActive(true);
            SoundManager.PlaySound("UI");
        }

        public void Option()
        {
            optionPanel.SetActive(true);
            optionButton.SetActive(false);
            otherButtons.SetActive(false);
            optionButton.SetActive(false);
            returnButton.SetActive(true);
            SoundManager.PlaySound("UI");
        }

        public void ExitGame()
        {
            SoundManager.PlaySound("UI");
            Application.Quit();
        }
    }
}