using UnityEngine;

namespace PinkTaxGame
{
    public class ShelfSpot : MonoBehaviour
    {
        [Header("Products")]
        [SerializeField] private Transform[] productPositions;

        [Header("UI")]
        [SerializeField] private Transform uiAnchor;

        public Transform[] ProductPositions => productPositions;
        public Transform UIAnchor => uiAnchor;
    }
}
