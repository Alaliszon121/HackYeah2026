using System.Collections.Generic;
using UnityEngine;

public class SlotGenerator : MonoBehaviour
{
    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color areaGizmoColor = Color.yellow;

    private readonly List<GameObject> generatedSlotObjects = new List<GameObject>();

    private Transform currentShelf;
    private float currentShelfWidth;
    private Vector3 currentRowLocalOffset;

    public List<Transform> GenerateSlots(
        int requiredSlots,
        Transform shelfTransform,
        float shelfWidth,
        Vector3 rowLocalOffset
    )
    {
        ClearGeneratedSlots();

        List<Transform> generatedSlots = new List<Transform>();

        if (requiredSlots <= 0)
            return generatedSlots;

        if (shelfTransform == null)
        {
            Debug.LogError("SlotGenerator: Shelf transform is null.");
            return generatedSlots;
        }

        if (shelfWidth <= 0f)
        {
            Debug.LogError("SlotGenerator: Shelf width must be greater than zero.");
            return generatedSlots;
        }

        currentShelf = shelfTransform;
        currentShelfWidth = shelfWidth;
        currentRowLocalOffset = rowLocalOffset;

        Vector3 rowCenter = shelfTransform.TransformPoint(rowLocalOffset);
        Vector3 halfWidthOffset = shelfTransform.forward * (shelfWidth * 0.5f);

        Vector3 startPosition = rowCenter + halfWidthOffset;
        Vector3 endPosition = rowCenter - halfWidthOffset;
        Vector3 step = (endPosition - startPosition) / (requiredSlots + 1);

        for (int i = 1; i <= requiredSlots; i++)
        {
            GameObject slotObject = new GameObject($"SortSlot_{i - 1}");

            slotObject.transform.SetParent(transform);
            slotObject.transform.SetPositionAndRotation(
                startPosition + step * i,
                shelfTransform.rotation
            );

            generatedSlotObjects.Add(slotObject);
            generatedSlots.Add(slotObject.transform);
        }

        return generatedSlots;
    }

    public void ClearGeneratedSlots()
    {
        foreach (GameObject slotObject in generatedSlotObjects)
        {
            if (slotObject != null)
                Destroy(slotObject);
        }

        generatedSlotObjects.Clear();
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || currentShelf == null || currentShelfWidth <= 0f)
            return;

        Vector3 rowCenter = currentShelf.TransformPoint(currentRowLocalOffset);
        Vector3 halfWidthOffset = currentShelf.forward * (currentShelfWidth * 0.5f);

        Vector3 startPosition = rowCenter + halfWidthOffset;
        Vector3 endPosition = rowCenter - halfWidthOffset;

        Gizmos.color = areaGizmoColor;
        Gizmos.DrawLine(startPosition, endPosition);
        Gizmos.DrawWireSphere(startPosition, 0.15f);
        Gizmos.DrawWireSphere(endPosition, 0.15f);
    }
}
