using UnityEngine;

namespace PinkTaxGame
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera controlledCamera;

        [Header("Scene Positions")]
        [SerializeField] private Transform receiptPosition;
        [SerializeField] private Transform endCutscenePosition;
        [SerializeField] private Transform mainMenuPosition;

        public void MoveToShelf(ShelfSpot shelf)
        {
        }

        public void MoveToReceipt()
        {
        }

        public void MoveToEndCutscene()
        {
        }

        public void MoveToMainMenu()
        {
        }
    }
}
