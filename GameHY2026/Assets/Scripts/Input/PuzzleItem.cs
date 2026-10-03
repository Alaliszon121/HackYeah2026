using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuzzleItem : MonoBehaviour
{
    [HideInInspector]
    public Transform CurrentSlot;
}