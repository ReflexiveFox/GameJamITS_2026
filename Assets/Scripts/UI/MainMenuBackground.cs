using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameJam
{
    /// <summary>
    /// Continuously transitions between a list of background sprites.
    /// Uses two layered UI Images to create a smooth crossfade effect.
    /// </summary>
    public class MainMenuBackground : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image currentImage;
        [SerializeField] private Image nextImage;

        [Header("Sprites")]
        [SerializeField] private Sprite[] backgroundSprites;

        [Header("Transition Settings")]
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private float stayDuration = 3f;

        private int currentSpriteIndex;

        private void Awake()
        {
            if (backgroundSprites.Length == 0)
            {
                enabled = false;
                return;
            }

            currentImage.sprite = backgroundSprites[0];

            SetAlpha(currentImage, 1f);
            SetAlpha(nextImage, 0f);
        }

        private void Start()
        {
            StartCoroutine(BackgroundLoopCoroutine());
        }

        private IEnumerator BackgroundLoopCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(stayDuration);

                int nextIndex = currentSpriteIndex + 1;

                if (nextIndex >= backgroundSprites.Length)
                {
                    nextIndex = 0;
                }

                yield return StartCoroutine(TransitionCoroutine(backgroundSprites[nextIndex]));

                currentSpriteIndex = nextIndex;
            }
        }

        private IEnumerator TransitionCoroutine(Sprite newSprite)
        {
            nextImage.sprite = newSprite;

            float timer = 0f;

            while (timer < transitionDuration)
            {
                timer += Time.deltaTime;

                float t = timer / transitionDuration;

                SetAlpha(currentImage, 1f - t);
                SetAlpha(nextImage, t);

                yield return null;
            }

            SetAlpha(currentImage, 0f);
            SetAlpha(nextImage, 1f);

            (nextImage, currentImage) = (currentImage, nextImage);
            SetAlpha(nextImage, 0f);
        }

        private void SetAlpha(Image image, float alpha)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}