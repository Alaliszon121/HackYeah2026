using System;
using UnityEngine;

namespace PinkTaxGame
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTarget;

        [Tooltip("Optional. If empty, the controller finds the active ShelfController once in Awake.")]
        [SerializeField] private ShelfController shelfController;

        [Header("Entrance")]
        [Tooltip("Distance the camera target moves along world +X from the main menu to Shelf 1.")]
        [SerializeField] private float entranceMoveDistance = 4f;

        private Vector3 mainMenuPosition;

        private void Awake()
        {
            if (shelfController == null)
                shelfController = FindAnyObjectByType<ShelfController>();

            if (cameraTarget != null)
                mainMenuPosition = cameraTarget.position;
        }

        public void MoveToFirstShelf(Action onComplete = null)
        {
            if (!CanMove())
                return;

            cameraTarget.position += Vector3.right * entranceMoveDistance;
            onComplete?.Invoke();
        }

        public void MoveToNextShelf(Action onComplete = null)
        {
            if (!CanMove())
                return;

            cameraTarget.position -= shelfController.transform.forward * shelfController.ShelfSpacing;
            onComplete?.Invoke();
        }

        public void MoveToReceipt(Action onComplete = null)
        {
            MoveToNextShelf(onComplete);
        }

        public void ResetToMainMenu(Action onComplete = null)
        {
            if (cameraTarget == null)
            {
                Debug.LogError("CameraController: Camera target is not assigned.");
                return;
            }

            cameraTarget.position = mainMenuPosition;
            onComplete?.Invoke();
        }

        private bool CanMove()
        {
            if (cameraTarget == null)
            {
                Debug.LogError("CameraController: Camera target is not assigned.");
                return false;
            }

            if (shelfController == null)
                shelfController = FindAnyObjectByType<ShelfController>();

            if (shelfController == null)
            {
                Debug.LogError("CameraController: No ShelfController exists in the scene.");
                return false;
            }

            return true;
        }
    }
}
