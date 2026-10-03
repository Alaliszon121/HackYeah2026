using UnityEngine;

namespace PinkTaxGame
{
    public class ShelfSpot : MonoBehaviour
    {
        [Header("Camera")]
        [Range(0f, 1f)]
        public float cameraSplinePosition;
        public Transform cameraLookTarget;

        [Header("Products")]
        public Transform[] productPositions;

        [Header("UI")]
        public Transform uiAnchor;
    }
}
