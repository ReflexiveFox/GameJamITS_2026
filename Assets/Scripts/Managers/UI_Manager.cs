using TMPro;
using UnityEngine;

namespace GameJam
{
    public class UI_Manager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UI_Panel gamePanel;
        [SerializeField] private UI_Panel gameOverPanel;

        [SerializeField] private TextMeshProUGUI resultText;

        private void Awake()
        {
            GameManager.OnGameOver += HandleGameOver;
        }

        private void OnDestroy()
        {
            GameManager.OnGameOver -= HandleGameOver;
        }

        private void HandleGameOver(int savedLives)
        {
            gamePanel.Hide();
            gameOverPanel.Show();
            resultText.text = $"You saved {savedLives} lives!";
        }
    }
}