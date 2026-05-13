using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    [RequireComponent(typeof(Image))]
    public abstract class Chibi : MonoBehaviour
    {
        [SerializeField] private float happyDuration = 2f;
        [SerializeField] private Sprite normalChibi;
        [SerializeField] private Sprite happyChibi;
        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
            HandleEventRegistrations(true);
        }

        private void OnDestroy()
        {
            HandleEventRegistrations(false);
        }

        protected void SetHappyForDuration()
        {
            SetHappyChibi();
            Invoke(nameof(SetNormalChibi), happyDuration);
        }

        private void SetNormalChibi()
        {
            image.sprite = normalChibi;
        }
        private void SetHappyChibi()
        {
            image.sprite = happyChibi;
        }

        protected abstract void HandleEventRegistrations(bool isRegistering);
    }
}