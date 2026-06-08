using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameJam
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMPLinkOpener : MonoBehaviour, IPointerClickHandler
    {
        private TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked on text at position: " + eventData.position);
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(
                text,
                eventData.position,
                eventData.pressEventCamera);

            if (linkIndex < 0)
            {
                return;
            }

            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}