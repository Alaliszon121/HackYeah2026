using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SortProductsGameManager : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("Reference to the Input Action for selecting/clicking (e.g., Mouse/Press).")]
    [SerializeField] private InputActionReference selectAction;
    [Tooltip("Reference to the Input Action for pointer position (e.g., Mouse/Position).")]
    [SerializeField] private InputActionReference pointerPositionAction;

    [Header("Puzzle References")]
    [Tooltip("List of all available slots in the puzzle.")]
    [SerializeField] private List<Transform> slots = new List<Transform>();
    [Tooltip("Prefabs or references of the items to be spawned and sorted.")]
    [SerializeField] private List<GameObject> itemPrefabs = new List<GameObject>();
    
    [Header("Grid Generation")]
    [Tooltip("Reference to the script that builds the layout.")]
    [SerializeField] private SlotGenerator slotGenerator;

    [Header("Settings")]
    [Tooltip("Camera being used (Cinemachine Brain camera). Leave null to auto-detect Camera.main.")]
    [SerializeField] private Camera puzzleCamera;

    private Dictionary<Transform, PuzzleItem> slotOccupants = new Dictionary<Transform, PuzzleItem>();
    private List<PuzzleItem> spawnedItems = new List<PuzzleItem>();

    private PuzzleItem draggedItem;
    private Transform originalSlot;
    private Vector3 originalPosition;

    private void Awake()
    {
        if (puzzleCamera == null)
        {
            puzzleCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        if (selectAction != null)
        {
            selectAction.action.Enable();
            selectAction.action.performed += OnSelectPerformed;
            selectAction.action.canceled += OnSelectCanceled;
        }

        if (pointerPositionAction != null)
        {
            pointerPositionAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (selectAction != null)
        {
            selectAction.action.performed -= OnSelectPerformed;
            selectAction.action.canceled -= OnSelectCanceled;
            selectAction.action.Disable();
        }

        if (pointerPositionAction != null)
        {
            pointerPositionAction.action.Disable();
        }
    }

    private void Start()
    {
        // Intercept initialization to build the grid first
        if (slotGenerator != null && itemPrefabs.Count > 0) //
        {
            // Override the slots list dynamically based on prefab count
            slots = slotGenerator.GenerateSlots(itemPrefabs.Count); //
        }
        else if (slots.Count == 0) //[cite: 1]
        {
            Debug.LogWarning("No slots available and no SlotGenerator assigned!");
        }

        InitializePuzzle(); //[cite: 1]
    }

    private void Update()
    {
        HandleDrag();
    }

    /// <summary>
    /// Randomly spawns the item prefabs onto the available slots.
    /// </summary>
    public void InitializePuzzle()
    {
        ClearPuzzle();

        if (itemPrefabs.Count > slots.Count)
        {
            Debug.LogWarning("PuzzleManager: More item prefabs than available slots! Some items won't be spawned.");
        }

        // Create a randomized copy of the slots list
        List<Transform> randomizedSlots = new List<Transform>(slots);
        ShuffleList(randomizedSlots);

        for (int i = 0; i < Mathf.Min(itemPrefabs.Count, randomizedSlots.Count); i++)
        {
            Transform targetSlot = randomizedSlots[i];
            GameObject itemObj = Instantiate(itemPrefabs[i], targetSlot.position, targetSlot.rotation);
            PuzzleItem puzzleItem = itemObj.GetComponent<PuzzleItem>();

            if (puzzleItem == null)
            {
                puzzleItem = itemObj.AddComponent<PuzzleItem>();
            }

            puzzleItem.CurrentSlot = targetSlot;
            slotOccupants[targetSlot] = puzzleItem;
            spawnedItems.Add(puzzleItem);
        }
    }

    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        Vector2 pointerPos = pointerPositionAction != null ? pointerPositionAction.action.ReadValue<Vector2>() : (Vector2)Input.mousePosition;
        Ray ray = puzzleCamera.ScreenPointToRay(pointerPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PuzzleItem item = hit.collider.GetComponent<PuzzleItem>();
            if (item != null && spawnedItems.Contains(item))
            {
                StartDragging(item);
            }
        }
    }

    private void OnSelectCanceled(InputAction.CallbackContext context)
    {
        if (draggedItem != null)
        {
            EndDragging();
        }
    }

    // private void StartDragging(PuzzleItem item)
    // {
    //     draggedItem = item;
    //     originalSlot = item.CurrentSlot;
    //     originalPosition = item.transform.position;
    //
    //     // Temporarily clear current slot occupant so slots can accept new items during drag
    //     if (slotOccupants.ContainsKey(originalSlot) && slotOccupants[originalSlot] == draggedItem)
    //     {
    //         slotOccupants.Remove(originalSlot);
    //     }
    //
    //     // Optional: Raise item slightly off the board during drag
    //     draggedItem.transform.position += Vector3.up * 0.5f;
    // }
    
    private void StartDragging(PuzzleItem item)
    {
        draggedItem = item;
        originalSlot = item.CurrentSlot;
        originalPosition = item.transform.position;

        if (slotOccupants.ContainsKey(originalSlot) && slotOccupants[originalSlot] == draggedItem)
        {
            slotOccupants.Remove(originalSlot);
        }

        // REMOVED: draggedItem.transform.position += Vector3.up * 0.5f; 
        // Item now stays right on the slot plane when clicked.
    }

    // private void HandleDrag()
    // {
    //     if (draggedItem == null) return;
    //
    //     Vector2 pointerPos = pointerPositionAction != null ? pointerPositionAction.action.ReadValue<Vector2>() : (Vector2)Input.mousePosition;
    //     Ray ray = puzzleCamera.ScreenPointToRay(pointerPos);
    //
    //     // Plane or ground cast to follow pointer smoothly
    //     // Plane dragPlane = new Plane(Vector3.up, originalPosition);
    //     Plane dragPlane = new Plane(-puzzleCamera.transform.forward, originalPosition);
    //     if (dragPlane.Raycast(ray, out float enter))
    //     {
    //         Vector3 hitPoint = ray.GetPoint(enter);
    //         draggedItem.transform.position = new Vector3(hitPoint.x, originalPosition.y + 0.5f, hitPoint.z);
    //     }
    // }
    
    private void HandleDrag()
    {
        if (draggedItem == null) return;

        Vector2 pointerPos = pointerPositionAction != null ? pointerPositionAction.action.ReadValue<Vector2>() : (Vector2)Input.mousePosition;
        Ray ray = puzzleCamera.ScreenPointToRay(pointerPos);

        // Use a plane facing the camera, anchored at the original position
        Plane dragPlane = new Plane(-puzzleCamera.transform.forward, originalPosition);
    
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
        
            // Allow X and Y to move freely with the mouse, but lock Z to the slot's depth
            draggedItem.transform.position = new Vector3(hitPoint.x, hitPoint.y, originalPosition.z);
        }
    }

    private void EndDragging()
    {
        Transform targetSlot = GetClosestSlot(draggedItem.transform.position);

        if (targetSlot != null && Vector3.Distance(draggedItem.transform.position, targetSlot.position) < 1.5f)
        {
            // Valid drop slot found
            if (slotOccupants.ContainsKey(targetSlot) && slotOccupants[targetSlot] != null)
            {
                // Slot is occupied -> Swap places with the occupant
                PuzzleItem occupant = slotOccupants[targetSlot];
                occupant.CurrentSlot = originalSlot;
                occupant.transform.position = originalSlot.position;
                slotOccupants[originalSlot] = occupant;
            }
            else
            {
                // Slot is empty
                originalSlot = null; // Clear old slot reference as it's now empty
            }

            // Place item into target slot
            draggedItem.CurrentSlot = targetSlot;
            draggedItem.transform.position = targetSlot.position;
            slotOccupants[targetSlot] = draggedItem;
        }
        else
        {
            // Dropped into invalid space -> Return to previous slot
            draggedItem.CurrentSlot = originalSlot;
            draggedItem.transform.position = originalSlot.position;
            slotOccupants[originalSlot] = draggedItem;
        }

        draggedItem = null;
        CheckPuzzleCompletion();
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

    private void CheckPuzzleCompletion()
    {
        // Add your win condition check here (e.g., comparing current slot occupants with correct target IDs)
    }

    private void ClearPuzzle()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        spawnedItems.Clear();
        slotOccupants.Clear();
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