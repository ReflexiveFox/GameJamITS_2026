using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(Outline))]
    public abstract class SelectableEntity : MonoBehaviour, ISelectable
    {
        [SerializeField] private bool isSelected;
        private Outline outline;

        public bool IsSelected => isSelected;

        private void Awake()
        {
            outline = GetComponent<Outline>();
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
