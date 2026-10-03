using System;
using UnityEngine;

namespace PinkTaxGame
{
    public class CameraController : MonoBehaviour
    {
        public void MoveToShelf(int shelfIndex, Action onComplete = null)
        {
            // Step 5:
            // Move the camera to the requested shelf.
            onComplete?.Invoke();
        }

        public void MoveToReceipt(Action onComplete = null)
        {
            // Step 5/7:
            // Move the camera to the receipt.
            onComplete?.Invoke();
        }

        public void MoveToEndCutscene(Action onComplete = null)
        {
            // Step 5/7:
            // Move the camera to the end-cutscene position.
            onComplete?.Invoke();
        }
    }
}
