using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

namespace PinkTaxGame
{
    public class CameraController : MonoBehaviour
    {
        [Header("Cinemachine 3")]
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private CinemachineSplineDolly splineDolly;

        [Header("Unity Spline")]
        [SerializeField] private SplineContainer shopSpline;

        [Header("Special Spline Positions")]
        [Range(0f, 1f)]
        [SerializeField] private float mainMenuSplinePosition;

        [Range(0f, 1f)]
        [SerializeField] private float receiptSplinePosition;

        [Range(0f, 1f)]
        [SerializeField] private float endCutsceneSplinePosition;

        [Header("Transition")]
        [SerializeField] private float moveDuration = 1f;

        public void MoveToShelf(ShelfSpot shelf) { }
        public void MoveToSplinePosition(float normalizedPosition, Transform lookTarget = null) { }
        public void MoveToReceipt() { }
        public void MoveToEndCutscene() { }
        public void MoveToMainMenu() { }
    }
}
