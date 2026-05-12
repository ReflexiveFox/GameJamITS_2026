using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask selectionLayerMask;

        private ISelectable currentSelection;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            TrySelectObject();
        }

        private void TrySelectObject()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectionLayerMask))
            {
                ClearSelection();
                return;
            }

            if (!hit.collider.TryGetComponent(out ISelectable selectable))
            {
                ClearSelection();
                return;
            }

            if (currentSelection == selectable)
            {
                return;
            }

            ClearSelection();

            currentSelection = selectable;
            currentSelection.Select();
        }

        private void ClearSelection()
        {
            if (currentSelection == null)
            {
                return;
            }

            currentSelection.Deselect();
            currentSelection = null;
        }
    }
}