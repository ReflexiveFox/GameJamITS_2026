using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UI_Panel : MonoBehaviour
    {
        [SerializeField] private bool startVisible = true;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            if (startVisible)
                Show();
            else
                Hide();
        }

        public void Show()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        public void Hide()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}