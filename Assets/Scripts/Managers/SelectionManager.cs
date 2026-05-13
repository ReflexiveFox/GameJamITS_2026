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

        [SerializeField] private List<SelectableEntity> currentSelectionList;

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
            ClearWholeSelection();
        }

        private void UpdateSelectionList()
        {
            currentSelectionList.RemoveAll(item => item == null);
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
                ClearWholeSelection();
                return;
            }

            if (!hit.collider.TryGetComponent(out SelectableEntity selectable))
            {
                ClearWholeSelection();
                return;
            }

            if (currentSelectionList.Contains(selectable))
            {
                return;
            }
            currentSelectionList.Add(selectable);
            selectable.Select();
        }

        private void ClearWholeSelection()
        {
            if (currentSelectionList == null)
            {
                return;
            }

            foreach (var selectable in currentSelectionList)
            {
                selectable?.Deselect();
            }
            currentSelectionList.Clear();
        }
    }
}