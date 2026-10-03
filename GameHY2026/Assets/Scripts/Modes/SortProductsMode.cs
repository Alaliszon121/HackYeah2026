using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace PinkTaxGame
{
    public class SortProductsMode : GameMode
    {
        [Header("Input Actions")]
        [Tooltip("Assign 'Gameplay/Point' from PinkTaxGame Input Actions.")]
        [SerializeField] private InputActionReference pointAction;
        [Tooltip("Assign 'Gameplay/DragPress' from PinkTaxGame Input Actions.")]
        [SerializeField] private InputActionReference dragPressAction;

        [Header("Camera & Mechanics")]
        [SerializeField] private Camera puzzleCamera;
        [SerializeField] private float maxDropDistance = 1.5f;

        [Header("Products & Layout")]
        [SerializeField] private List<ProductData> displayedProducts = new List<ProductData>();
        [SerializeField] private List<Transform> slots = new List<Transform>();
        [SerializeField] private SlotGenerator slotGenerator;

        private Dictionary<Transform, ProductInteractable> slotOccupants = new Dictionary<Transform, ProductInteractable>();
        private List<ProductInteractable> spawnedInteractables = new List<ProductInteractable>();
        private List<ProductData> playerOrder = new List<ProductData>();

        private ProductInteractable draggedItem;

        private void Awake()
        {
            if (puzzleCamera == null)
            {
                puzzleCamera = Camera.main;
            }
        }

        private void OnEnable()
        {
            if (dragPressAction != null)
            {
                dragPressAction.action.Enable();
                dragPressAction.action.performed += OnDragPressPerformed;
                dragPressAction.action.canceled += OnDragPressCanceled;
            }

            if (pointAction != null)
            {
                pointAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (dragPressAction != null)
            {
                dragPressAction.action.performed -= OnDragPressPerformed;
                dragPressAction.action.canceled -= OnDragPressCanceled;
                dragPressAction.action.Disable();
            }

            if (pointAction != null)
            {
                pointAction.action.Disable();
            }
        }

        private void Update()
        {
            HandleDrag();
        }

        public void SetPlayerOrder(List<ProductData> orderedProducts)
        {
            playerOrder = new List<ProductData>(orderedProducts);
        }

        public override void Setup(SublevelData sublevel)
        {
            InitializeBoard();
        }

        private void Start() {
            InitializeBoard();
        }

        public override void Play() { }

        public override void Submit()
        {
            CalculateResult();
        }

        public void InitializeBoard()
        {
            ClearBoard();

            if (displayedProducts == null || displayedProducts.Count == 0) return;

            if (slotGenerator != null)
            {
                slots = slotGenerator.GenerateSlots(displayedProducts.Count);
            }

            if (slots == null || slots.Count == 0)
            {
                Debug.LogWarning("SortProductsMode: No slots available!");
                return;
            }

            List<Transform> randomizedSlots = new List<Transform>(slots);
            ShuffleList(randomizedSlots);

            for (int i = 0; i < Mathf.Min(displayedProducts.Count, randomizedSlots.Count); i++)
            {
                ProductData data = displayedProducts[i];
                Transform targetSlot = randomizedSlots[i];

                GameObject itemObj = data.modelPrefab != null 
                    ? Instantiate(data.modelPrefab, targetSlot.position, targetSlot.rotation)
                    : new GameObject($"Product_{data.productName}");

                ProductInteractable interactable = itemObj.GetComponent<ProductInteractable>();
                if (interactable == null)
                {
                    interactable = itemObj.AddComponent<ProductInteractable>();
                }

                interactable.Setup(data);
                interactable.CurrentSlot = targetSlot;

                slotOccupants[targetSlot] = interactable;
                spawnedInteractables.Add(interactable);
            }

            UpdatePlayerOrder();
        }

        private void OnDragPressPerformed(InputAction.CallbackContext context)
        {
            if (puzzleCamera == null || pointAction == null) return;

            Vector2 pointerPos = pointAction.action.ReadValue<Vector2>();
            Ray ray = puzzleCamera.ScreenPointToRay(pointerPos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ProductInteractable item = hit.collider.GetComponentInParent<ProductInteractable>();
                if (item != null && spawnedInteractables.Contains(item))
                {
                    StartDragging(item);
                }
            }
        }

        private void OnDragPressCanceled(InputAction.CallbackContext context)
        {
            if (draggedItem != null)
            {
                EndDragging();
            }
        }

        private void StartDragging(ProductInteractable item)
        {
            draggedItem = item;
            draggedItem.OriginalSlot = item.CurrentSlot;
            draggedItem.OriginalPosition = item.transform.position;

            if (draggedItem.OriginalSlot != null && slotOccupants.ContainsKey(draggedItem.OriginalSlot))
            {
                if (slotOccupants[draggedItem.OriginalSlot] == draggedItem)
                {
                    slotOccupants.Remove(draggedItem.OriginalSlot);
                }
            }
        }

        private void HandleDrag()
        {
            if (draggedItem == null || pointAction == null || puzzleCamera == null) return;

            Vector2 pointerPos = pointAction.action.ReadValue<Vector2>();
            Ray ray = puzzleCamera.ScreenPointToRay(pointerPos);

            Plane dragPlane = new Plane(-puzzleCamera.transform.forward, draggedItem.OriginalPosition);

            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                draggedItem.transform.position = new Vector3(hitPoint.x, hitPoint.y, draggedItem.OriginalPosition.z);
            }
        }

        private void EndDragging()
        {
            Transform targetSlot = GetClosestSlot(draggedItem.transform.position);

            if (targetSlot != null && Vector3.Distance(draggedItem.transform.position, targetSlot.position) <= maxDropDistance)
            {
                // If target slot is occupied, move occupant to the dragged item's original slot
                if (slotOccupants.TryGetValue(targetSlot, out ProductInteractable occupant) && occupant != null)
                {
                    occupant.CurrentSlot = draggedItem.OriginalSlot;
                    occupant.transform.position = draggedItem.OriginalSlot.position;

                    if (draggedItem.OriginalSlot != null)
                    {
                        slotOccupants[draggedItem.OriginalSlot] = occupant;
                    }
                }

                // Snap dragged item into target slot
                draggedItem.CurrentSlot = targetSlot;
                draggedItem.transform.position = targetSlot.position;
                slotOccupants[targetSlot] = draggedItem;
            }
            else
            {
                // Snap back to original slot
                draggedItem.CurrentSlot = draggedItem.OriginalSlot;
                draggedItem.transform.position = draggedItem.OriginalPosition;

                if (draggedItem.OriginalSlot != null)
                {
                    slotOccupants[draggedItem.OriginalSlot] = draggedItem;
                }
            }

            draggedItem = null;
            UpdatePlayerOrder();
        }

        public override ModeResult CalculateResult()
        {
            bool isCorrectlySorted = VerifySortingOrder();
            ModeResult result = new ModeResult();
            return result;
        }

        public override ModeResult GetResult()
        {
            return CalculateResult();
        }

        public bool VerifySortingOrder()
        {
            if (playerOrder == null || playerOrder.Count <= 1) return true;

            for (int i = 0; i < playerOrder.Count - 1; i++)
            {
                if (playerOrder[i] == null || playerOrder[i + 1] == null) return false;

                if (playerOrder[i].pricePLN > playerOrder[i + 1].pricePLN)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdatePlayerOrder()
        {
            playerOrder.Clear();
            foreach (Transform slot in slots)
            {
                if (slotOccupants.TryGetValue(slot, out ProductInteractable item) && item != null && item.ProductData != null)
                {
                    playerOrder.Add(item.ProductData);
                }
            }
        }

        private Transform GetClosestSlot(Vector3 position)
        {
            Transform closest = null;
            float minDistance = float.MaxValue;

            foreach (Transform slot in slots)
            {
                float dist = Vector3.Distance(position, slot.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = slot;
                }
            }

            return closest;
        }

        private void ClearBoard()
        {
            foreach (var item in spawnedInteractables)
            {
                if (item != null) Destroy(item.gameObject);
            }
            spawnedInteractables.Clear();
            slotOccupants.Clear();
            playerOrder.Clear();
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}