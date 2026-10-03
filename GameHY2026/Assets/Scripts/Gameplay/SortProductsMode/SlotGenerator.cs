using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public class SlotGenerator : MonoBehaviour
{
    [Header("Border Boundaries")]
    [Tooltip("Left edge boundary (outer limit, excluded from slot spawning).")]
    [SerializeField] private Transform leftBorder;

    [Tooltip("Right edge boundary (outer limit, excluded from slot spawning).")]
    [SerializeField] private Transform rightBorder;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color borderGizmoColor = Color.yellow;

    /// <summary>
    /// Calculates dynamic spacing between leftBorder and rightBorder and generates 
    /// slot positions evenly between them, excluding the borders themselves.
    /// </summary>
    public List<Transform> GenerateSlots(int requiredSlots)
    {
        List<Transform> generatedSlots = new List<Transform>();

        if (requiredSlots <= 0) return generatedSlots;

        if (leftBorder == null || rightBorder == null)
        {
            Debug.LogError("LineSlotGenerator: Please assign both Left Border and Right Border in the Inspector!");
            return generatedSlots;
        }

        Vector3 startPos = leftBorder.position;
        Vector3 endPos = rightBorder.position;

        // Dividing the space into (requiredSlots + 1) segments creates 
        // requiredSlots inner points between startPos and endPos.
        int totalSegments = requiredSlots + 1;
        Vector3 stepVector = (endPos - startPos) / totalSegments;

        // Loop through internal points only (excluding i = 0 [startPos] and i = totalSegments [endPos])
        for (int i = 1; i <= requiredSlots; i++)
        {
            Vector3 worldPos = startPos + (stepVector * i);

            GameObject slotObj = new GameObject($"Slot_{i - 1}");
            slotObj.transform.position = worldPos;
            slotObj.transform.SetParent(transform);

            generatedSlots.Add(slotObj.transform);
        }

        return generatedSlots;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || leftBorder == null || rightBorder == null) return;

        Gizmos.color = borderGizmoColor;
        Gizmos.DrawLine(leftBorder.position, rightBorder.position);
        Gizmos.DrawWireSphere(leftBorder.position, 0.15f);
        Gizmos.DrawWireSphere(rightBorder.position, 0.15f);
    }
}
