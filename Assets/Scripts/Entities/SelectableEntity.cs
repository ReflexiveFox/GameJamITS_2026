using System;
using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(Collider))]
    public abstract class SelectableEntity : MonoBehaviour, ISelectable
    {
        public static event Action OnAnyEntityDestroyed = delegate{};

        [Header("SELECTABLE ENTITY SETTINGS")]
        [Header("References")]
        [SerializeField] private Outline outline;

        [Header("Selection State")]
        [SerializeField] private bool isSelected;

        public bool IsSelected => isSelected;

        protected virtual void Awake()
        {
            if (outline == null)
            {
                Debug.LogError($"Outline component is not assigned into inspector!", this);
            }
        }

        protected virtual void OnDestroy()
        {
            OnAnyEntityDestroyed?.Invoke();
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
