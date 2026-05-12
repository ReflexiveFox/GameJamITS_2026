using UnityEngine;
using TMPro;

namespace GameJam
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ScoreText : MonoBehaviour
    {
        private TextMeshProUGUI scoreText;

        private void Awake()
        {
            scoreText = GetComponent<TextMeshProUGUI>();
            GameManager.OnSavedLivesUpdated += HandleSavedLivesUpdate;
        }
        private void OnDestroy()
        {
            GameManager.OnSavedLivesUpdated -= HandleSavedLivesUpdate;
        }
        private void HandleSavedLivesUpdate(int currentLostLives)
        {
            UpdateScoreText(currentLostLives);
        }
        public void UpdateScoreText(int savedLives)
        {
            scoreText.text = $"Saved Lives: {savedLives}";
        }
    }
}
