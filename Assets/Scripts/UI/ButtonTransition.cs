using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameJam
{
    /// <summary>
    /// Handles simple hover and press transitions for menu buttons.
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    public class ButtonTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Scale")]
        [SerializeField] private float normalScale = 1f;
        [SerializeField] private float selectedScale = 1.08f;
        [SerializeField] private float pressedScale = 0.95f;
        [SerializeField] private float scaleSpeed = 12f;

        private Vector3 targetScale;

        private void Awake()
        {
            targetScale = Vector3.one * normalScale;
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Select();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Deselect();
        }

        public void OnSelect(BaseEventData eventData)
        {
            Select();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Deselect();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            targetScale = Vector3.one * pressedScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            targetScale = Vector3.one * selectedScale;
        }

        private void Select()
        {
            targetScale = Vector3.one * selectedScale;
        }

        private void Deselect()
        {
            targetScale = Vector3.one * normalScale;
        }
    }
}