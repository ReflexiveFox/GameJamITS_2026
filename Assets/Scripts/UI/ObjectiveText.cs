using UnityEngine;
using TMPro;

namespace GameJam
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ObjectiveText : MonoBehaviour
    {
        private TextMeshProUGUI objectiveText;

        private void Awake()
        {
            objectiveText = GetComponent<TextMeshProUGUI>();
            GameManager.OnTargetLostLivesUpdated += HandleLostLivesUpdate;
        }

        private void OnDestroy()
        {
            GameManager.OnTargetLostLivesUpdated -= HandleLostLivesUpdate;
        }

        private void HandleLostLivesUpdate(int currentLostLives)
        {
            UpdateObjectiveText(currentLostLives);
        }

        public void UpdateObjectiveText(int newValue)
        {
            objectiveText.text = $"Lives lost: {newValue} / {GameManager.Instance.TargetLostLives}";
        }
    }
}
