using UnityEngine;

namespace GameJam
{
    public abstract class SelectableEntity : MonoBehaviour, ISelectable
    {
        [SerializeField] private bool isSelected;

        public bool IsSelected => isSelected;

        public virtual void Select()
        {
            isSelected = true;
        }

        public virtual void Deselect()
        {
            isSelected = false;
        }
    }
}
