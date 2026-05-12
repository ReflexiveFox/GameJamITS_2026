using UnityEngine;

namespace GameJam
{
    public abstract class SelectableEntity : MonoBehaviour, ISelectable
    {
        [SerializeField] private bool isSelected;
        [SerializeField] private Outline outline;

        public bool IsSelected => isSelected;

        private void Awake()
        {
            if (outline == null)
            {
                Debug.LogError($"Outline component is not assigned into inspector!", this);
            }
        }

        public virtual void Select()
        {
            isSelected = true;
            outline.enabled = true;
        }

        public virtual void Deselect()
        {
            isSelected = false;
            outline.enabled = false;
        }
    }
}
