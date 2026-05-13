using TMPro;
using UnityEngine;

namespace GameJam
{
    public class UI_Manager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UI_Panel tutorialPanel;
        [SerializeField] private UI_Panel gamePanel;
        [SerializeField] private UI_Panel pausePanel;
        [SerializeField] private UI_Panel gameOverPanel;

        [SerializeField] private TextMeshProUGUI resultText;

        private void Awake()
        {
            GameManager.OnGameStarted += HandleGameStarted;
            GameManager.OnGameOver += HandleGameOver;
            GameManager.OnGamePaused += HandlePauseMenu;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStarted -= HandleGameStarted;
            GameManager.OnGameOver -= HandleGameOver;
            GameManager.OnGamePaused -= HandlePauseMenu;
        }

        private void HandleGameStarted()
        {
            gamePanel.Show();
            tutorialPanel.Hide();
            pausePanel.Hide();
            gameOverPanel.Hide();
        }

        private void HandlePauseMenu(bool isPaused)
        {
            if (isPaused)
            {
                gamePanel.Hide();
                pausePanel.Show();
            }
            else
            {
                pausePanel.Hide();
                gamePanel.Show();
            }
        }

        private void HandleGameOver(int savedLives)
        {
            gamePanel.Hide();
            gameOverPanel.Show();
            resultText.text = $"You saved {savedLives} lives!";
        }
    }
}