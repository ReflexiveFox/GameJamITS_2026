using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam
{ 
    public class SelectionManager : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference selectAction;
        [SerializeField] private InputActionReference deselectAction;

        [Header("Raycast")]
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private LayerMask selectionLayerMask;
        [SerializeField] private float raycastDistance = 1000f;
        [SerializeField] private int maxSelectionAmount = 3;

        private List<ISelectable> currentSelectionList;

        private void Awake()
        {
            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }

            SelectableEntity.OnAnyEntityDestroyed += UpdateSelectionList;
        }

        private void Start()
        {
            currentSelectionList = new();
        }

        private void OnEnable()
        {
            selectAction.action.performed += OnSelectPerformed;
            deselectAction.action.performed += OnDeselectPerformed;

            selectAction.action.Enable();
            deselectAction.action.Enable();
        }

        private void OnDisable()
        {
            selectAction.action.performed -= OnSelectPerformed;
            deselectAction.action.performed -= OnDeselectPerformed;

            selectAction.action.Disable();
            deselectAction.action.Disable();
        }

        private void OnDestroy()
        {
            SelectableEntity.OnAnyEntityDestroyed -= UpdateSelectionList;
        }

        private void OnSelectPerformed(InputAction.CallbackContext context)
        {
            TrySelectObject();
        }

        private void OnDeselectPerformed(InputAction.CallbackContext context)
        {
            ClearSelection();
        }

        private void UpdateSelectionList()
        {
            foreach (var selectable in currentSelectionList)
            {
                if (selectable == null)
                {
                    currentSelectionList.Remove(selectable);
                    break;
                }
            }
        }

        private void TrySelectObject()
        {
            if(currentSelectionList.Count >= maxSelectionAmount)
            {
                return;
            }
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = gameplayCamera.ScreenPointToRay(mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, raycastDistance, selectionLayerMask))
            {
                ClearSelection();
                return;
            }

            if (!hit.collider.TryGetComponent(out ISelectable selectable))
            {
                ClearSelection();
                return;
            }

            if (currentSelectionList.Contains(selectable))
            {
                return;
            }

            currentSelectionList.Add(selectable);
            selectable.Select();
        }

        private void ClearSelection()
        {
            if (currentSelectionList == null)
            {
                return;
            }

            foreach (var selectable in currentSelectionList)
            {
                selectable.Deselect();
            }
            currentSelectionList.Clear();
        }
    }
}