using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PinkTaxGame
{
    public class SortProductsMode : GameMode
    {
        [Header("UI")]
        [SerializeField] private GameObject uiRoot;
        [SerializeField] private Button submitButton;

        [Header("Input Actions")]
        [Tooltip("Assign the pointer-position action, for example Gameplay/Point.")]
        [SerializeField] private InputActionReference pointAction;

        [Tooltip("Assign the press action, for example Gameplay/DragPress.")]
        [SerializeField] private InputActionReference dragPressAction;

        [Header("Camera & Drag")]
        [SerializeField] private Camera puzzleCamera;

        [Min(0f)]
        [SerializeField] private float maxDropDistance = 1.5f;

        private readonly List<Pose> positions = new List<Pose>();
        private readonly List<GameObject> productObjects = new List<GameObject>();
        private readonly List<ProductInteractable> productInteractables = new List<ProductInteractable>();

        private readonly Dictionary<int, ProductInteractable> positionOccupants =
            new Dictionary<int, ProductInteractable>();

        private readonly List<ProductData> playerOrder = new List<ProductData>();
        private readonly List<ProductData> correctOrder = new List<ProductData>();

        private ShelfController shelfController;
        private int currentShelfIndex = -1;

        private ProductInteractable draggedItem;
        private bool inputEnabled;

        public IReadOnlyList<ProductData> PlayerOrder => playerOrder;
        public IReadOnlyList<ProductData> CorrectOrder => correctOrder;

        private void Awake()
        {
            if (puzzleCamera == null)
                puzzleCamera = Camera.main;

            if (submitButton != null)
                submitButton.onClick.AddListener(Submit);

            if (uiRoot != null)
                uiRoot.SetActive(false);
        }

        private void OnDestroy()
        {
            DisableInput();

            if (submitButton != null)
                submitButton.onClick.RemoveListener(Submit);
        }

        private void OnDisable()
        {
            DisableInput();
        }

        private void Update()
        {
            if (inputEnabled)
                HandleDrag();
        }

        public override void Setup(SublevelData sublevel)
        {
            Stop();
            ResetState();
            base.Setup(sublevel);

            if (sublevel == null)
            {
                Debug.LogError("SortProductsMode: SublevelData is null.");
                return;
            }

            if (sublevel.ModeType != ModeType.SortProducts)
            {
                Debug.LogError("SortProductsMode: Received a sublevel for the wrong mode.");
                return;
            }

            if (sublevel.Products.Count < 2)
            {
                Debug.LogError("SortProductsMode: SortProducts requires at least two products.");
                return;
            }

            foreach (ProductData product in sublevel.Products)
            {
                if (product == null)
                {
                    Debug.LogError("SortProductsMode: Sublevel contains a null product.");
                    return;
                }

                correctOrder.Add(product);
            }

            correctOrder.Sort(
                (first, second) => first.PriceGrosze.CompareTo(second.PriceGrosze)
            );
        }

        public void SetShelfContext(ShelfController controller, int shelfIndex)
        {
            shelfController = controller;
            currentShelfIndex = shelfIndex;
        }

        public void SetProductObjects(IReadOnlyList<GameObject> objects)
        {
            productObjects.Clear();

            if (objects == null)
                return;

            foreach (GameObject productObject in objects)
                productObjects.Add(productObject);
        }

        public override void Play()
        {
            if (!CanStart())
                return;

            positions.Clear();

            positions.AddRange(
                shelfController.GetProductPoses(
                    currentShelfIndex,
                    currentSublevel.Products.Count
                )
            );

            if (positions.Count != currentSublevel.Products.Count)
            {
                Debug.LogError(
                    $"SortProductsMode: Expected {currentSublevel.Products.Count} product positions, " +
                    $"but ShelfController returned {positions.Count}."
                );
                return;
            }

            if (!InitializeBoard())
                return;

            uiRoot.SetActive(true);
            EnableInput();
        }

        public override void Submit()
        {
            if (currentSublevel == null || currentSublevel.ModeType != ModeType.SortProducts)
            {
                Debug.LogError(
                    "SortProductsMode: Cannot submit without a valid SortProducts sublevel."
                );
                return;
            }

            if (draggedItem != null)
                EndDragging();

            UpdatePlayerOrder();

            if (playerOrder.Count != currentSublevel.Products.Count)
            {
                Debug.LogWarning(
                    "SortProductsMode: Not every product occupies a sorting position."
                );
                return;
            }

            if (gameManager == null)
            {
                Debug.LogError("SortProductsMode: GameManager is not initialized.");
                return;
            }

            int points = CalculatePoints();

            ModeResult result = ModeResult.CreateSortResult(
                currentSublevel.Products,
                playerOrder,
                correctOrder,
                points,
                MaxPoints
            );

            gameManager.CompleteCurrentSublevel(result);
        }

        public override void Stop()
        {
            DisableInput();
            draggedItem = null;

            if (uiRoot != null)
                uiRoot.SetActive(false);
        }

        public bool VerifySortingOrder()
        {
            UpdatePlayerOrder();

            if (playerOrder.Count != currentSublevel?.Products.Count)
                return false;

            for (int i = 1; i < playerOrder.Count; i++)
            {
                if (playerOrder[i - 1].PriceGrosze > playerOrder[i].PriceGrosze)
                    return false;
            }

            return true;
        }

        private bool CanStart()
        {
            if (currentSublevel == null || currentSublevel.ModeType != ModeType.SortProducts)
            {
                Debug.LogError(
                    "SortProductsMode: Cannot play without a valid SortProducts sublevel."
                );
                return false;
            }

            if (uiRoot == null)
            {
                Debug.LogError("SortProductsMode: UI Root is not assigned.");
                return false;
            }

            if (submitButton == null)
            {
                Debug.LogError("SortProductsMode: Submit Button is not assigned.");
                return false;
            }

            if (pointAction == null || dragPressAction == null)
            {
                Debug.LogError(
                    "SortProductsMode: Point and DragPress input actions must be assigned."
                );
                return false;
            }

            if (puzzleCamera == null)
                puzzleCamera = Camera.main;

            if (puzzleCamera == null)
            {
                Debug.LogError(
                    "SortProductsMode: No puzzle camera is assigned and no Main Camera was found."
                );
                return false;
            }

            if (shelfController == null)
            {
                Debug.LogError(
                    "SortProductsMode: ShelfController context was not provided."
                );
                return false;
            }

            if (currentShelfIndex < 0)
            {
                Debug.LogError(
                    "SortProductsMode: Current shelf index was not provided."
                );
                return false;
            }

            if (productObjects.Count != currentSublevel.Products.Count)
            {
                Debug.LogError(
                    $"SortProductsMode: Expected {currentSublevel.Products.Count} spawned product objects, " +
                    $"but received {productObjects.Count}."
                );
                return false;
            }

            return true;
        }

        private bool InitializeBoard()
        {
            positionOccupants.Clear();
            productInteractables.Clear();
            playerOrder.Clear();
            draggedItem = null;

            List<int> randomizedPositionIndices = new List<int>();

            for (int i = 0; i < positions.Count; i++)
                randomizedPositionIndices.Add(i);

            ShuffleList(randomizedPositionIndices);

            for (int i = 0; i < currentSublevel.Products.Count; i++)
            {
                ProductData product = currentSublevel.Products[i];
                GameObject productObject = productObjects[i];
                int targetPositionIndex = randomizedPositionIndices[i];

                if (productObject == null)
                {
                    Debug.LogError(
                        $"SortProductsMode: Spawned product object {i} is null."
                    );
                    return false;
                }

                ProductInteractable interactable =
                    productObject.GetComponent<ProductInteractable>();

                if (interactable == null)
                    interactable = productObject.AddComponent<ProductInteractable>();

                if (!interactable.Setup(product))
                {
                    Debug.LogError(
                        $"SortProductsMode: Product '{product.ProductName}' " +
                        "could not be made interactable."
                    );
                    return false;
                }

                interactable.CurrentPositionIndex = targetPositionIndex;

                MoveProductToPosition(interactable, targetPositionIndex);

                positionOccupants[targetPositionIndex] = interactable;
                productInteractables.Add(interactable);
            }

            UpdatePlayerOrder();
            return true;
        }

        private void EnableInput()
        {
            if (inputEnabled)
                return;

            dragPressAction.action.performed += OnDragPressPerformed;
            dragPressAction.action.canceled += OnDragPressCanceled;

            pointAction.action.Enable();
            dragPressAction.action.Enable();

            inputEnabled = true;
        }

        private void DisableInput()
        {
            if (!inputEnabled)
                return;

            if (dragPressAction != null)
            {
                dragPressAction.action.performed -= OnDragPressPerformed;
                dragPressAction.action.canceled -= OnDragPressCanceled;
                dragPressAction.action.Disable();
            }

            if (pointAction != null)
                pointAction.action.Disable();

            inputEnabled = false;
        }

        private void OnDragPressPerformed(InputAction.CallbackContext context)
        {
            if (puzzleCamera == null || pointAction == null)
                return;

            Vector2 pointerPosition =
                pointAction.action.ReadValue<Vector2>();

            Ray ray =
                puzzleCamera.ScreenPointToRay(pointerPosition);

            ProductInteractable item =
                FindClosestInteractable(ray);

            if (item != null)
                StartDragging(item);
        }

        private void OnDragPressCanceled(InputAction.CallbackContext context)
        {
            if (draggedItem != null)
                EndDragging();
        }

        private ProductInteractable FindClosestInteractable(Ray ray)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray);

            ProductInteractable closestItem = null;
            float closestDistance = float.MaxValue;

            foreach (RaycastHit hit in hits)
            {
                ProductInteractable item =
                    hit.collider.GetComponentInParent<ProductInteractable>();

                if (item == null || !productInteractables.Contains(item))
                    continue;

                if (hit.distance >= closestDistance)
                    continue;

                closestDistance = hit.distance;
                closestItem = item;
            }

            return closestItem;
        }

        private void StartDragging(ProductInteractable item)
        {
            if (item == null || item.CurrentPositionIndex < 0)
                return;

            draggedItem = item;

            draggedItem.OriginalPositionIndex =
                item.CurrentPositionIndex;

            draggedItem.OriginalPosition =
                item.transform.position;

            draggedItem.OriginalRotation =
                item.transform.rotation;

            if (
                positionOccupants.TryGetValue(
                    draggedItem.OriginalPositionIndex,
                    out ProductInteractable occupant
                ) &&
                occupant == draggedItem
            )
            {
                positionOccupants.Remove(
                    draggedItem.OriginalPositionIndex
                );
            }
        }

        private void HandleDrag()
        {
            if (
                draggedItem == null ||
                pointAction == null ||
                puzzleCamera == null
            )
            {
                return;
            }

            Vector2 pointerPosition =
                pointAction.action.ReadValue<Vector2>();

            Ray ray =
                puzzleCamera.ScreenPointToRay(pointerPosition);

            Plane dragPlane = new Plane(
                -puzzleCamera.transform.forward,
                draggedItem.OriginalPosition
            );

            if (!dragPlane.Raycast(ray, out float enter))
                return;

            draggedItem.transform.position =
                ray.GetPoint(enter);
        }

        private void EndDragging()
        {
            if (draggedItem == null)
                return;

            int targetPositionIndex =
                GetClosestPositionIndex(
                    draggedItem.transform.position
                );

            bool validDrop =
                targetPositionIndex >= 0 &&
                Vector3.Distance(
                    draggedItem.transform.position,
                    positions[targetPositionIndex].position
                ) <= maxDropDistance;

            if (!validDrop)
            {
                ReturnDraggedItemToOriginalPosition();

                draggedItem = null;
                UpdatePlayerOrder();
                return;
            }

            if (
                positionOccupants.TryGetValue(
                    targetPositionIndex,
                    out ProductInteractable occupant
                ) &&
                occupant != null &&
                occupant != draggedItem
            )
            {
                int originalPositionIndex =
                    draggedItem.OriginalPositionIndex;

                occupant.CurrentPositionIndex =
                    originalPositionIndex;

                MoveProductToPosition(
                    occupant,
                    originalPositionIndex
                );

                positionOccupants[originalPositionIndex] =
                    occupant;
            }

            draggedItem.CurrentPositionIndex =
                targetPositionIndex;

            MoveProductToPosition(
                draggedItem,
                targetPositionIndex
            );

            positionOccupants[targetPositionIndex] =
                draggedItem;

            draggedItem = null;

            UpdatePlayerOrder();
        }

        private void ReturnDraggedItemToOriginalPosition()
        {
            int originalPositionIndex =
                draggedItem.OriginalPositionIndex;

            if (
                originalPositionIndex >= 0 &&
                originalPositionIndex < positions.Count
            )
            {
                draggedItem.CurrentPositionIndex =
                    originalPositionIndex;

                MoveProductToPosition(
                    draggedItem,
                    originalPositionIndex
                );

                positionOccupants[originalPositionIndex] =
                    draggedItem;

                return;
            }

            draggedItem.transform.SetPositionAndRotation(
                draggedItem.OriginalPosition,
                draggedItem.OriginalRotation
            );
        }

        private void MoveProductToPosition(
            ProductInteractable item,
            int positionIndex
        )
        {
            Pose pose = positions[positionIndex];

            item.transform.SetPositionAndRotation(
                pose.position,
                pose.rotation
            );
        }

        private void UpdatePlayerOrder()
        {
            playerOrder.Clear();

            for (
                int positionIndex = 0;
                positionIndex < positions.Count;
                positionIndex++
            )
            {
                if (
                    positionOccupants.TryGetValue(
                        positionIndex,
                        out ProductInteractable item
                    ) &&
                    item != null &&
                    item.ProductData != null
                )
                {
                    playerOrder.Add(item.ProductData);
                }
            }
        }

        private int CalculatePoints()
        {
            int productCount = playerOrder.Count;

            if (productCount <= 1)
                return MaxPoints;

            int totalDistance = 0;

            Dictionary<int, List<int>> correctPositionsByPrice =
                new Dictionary<int, List<int>>();

            Dictionary<int, List<int>> playerPositionsByPrice =
                new Dictionary<int, List<int>>();

            for (int i = 0; i < correctOrder.Count; i++)
            {
                int price = correctOrder[i].PriceGrosze;

                if (!correctPositionsByPrice.ContainsKey(price))
                    correctPositionsByPrice[price] = new List<int>();

                correctPositionsByPrice[price].Add(i);
            }

            for (int i = 0; i < playerOrder.Count; i++)
            {
                int price = playerOrder[i].PriceGrosze;

                if (!playerPositionsByPrice.ContainsKey(price))
                    playerPositionsByPrice[price] = new List<int>();

                playerPositionsByPrice[price].Add(i);
            }

            foreach (
                KeyValuePair<int, List<int>> pair
                in correctPositionsByPrice
            )
            {
                List<int> correctPositions = pair.Value;

                if (!playerPositionsByPrice.TryGetValue(
                    pair.Key,
                    out List<int> playerPositions
                ))
                {
                    continue;
                }

                correctPositions.Sort();
                playerPositions.Sort();

                int count = Mathf.Min(
                    correctPositions.Count,
                    playerPositions.Count
                );

                for (int i = 0; i < count; i++)
                {
                    totalDistance += Mathf.Abs(
                        playerPositions[i] -
                        correctPositions[i]
                    );
                }
            }

            int maxDistance =
                productCount * productCount / 2;

            if (maxDistance <= 0)
                return MaxPoints;

            float accuracy =
                1f -
                (float)totalDistance / maxDistance;

            accuracy = Mathf.Clamp01(accuracy);

            return Mathf.RoundToInt(
                accuracy * MaxPoints
            );
        }

        private int GetClosestPositionIndex(
            Vector3 position
        )
        {
            int closestIndex = -1;
            float minimumDistance = float.MaxValue;

            for (int i = 0; i < positions.Count; i++)
            {
                float distance =
                    Vector3.Distance(
                        position,
                        positions[i].position
                    );

                if (distance >= minimumDistance)
                    continue;

                minimumDistance = distance;
                closestIndex = i;
            }

            return closestIndex;
        }

        private void ResetState()
        {
            positions.Clear();
            productObjects.Clear();
            productInteractables.Clear();
            positionOccupants.Clear();
            playerOrder.Clear();
            correctOrder.Clear();

            shelfController = null;
            currentShelfIndex = -1;
            draggedItem = null;
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex =
                    Random.Range(i, list.Count);

                (list[i], list[randomIndex]) =
                    (list[randomIndex], list[i]);
            }
        }
    }
}