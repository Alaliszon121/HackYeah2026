using System.Collections;
using UnityEngine;

namespace PinkTaxGame
{
    public class CameraPathTester : MonoBehaviour
    {
        [SerializeField] private CameraController cameraController;
        [SerializeField] private ShelfController shelfController;

        [Header("Test")]
        [Min(1)]
        [SerializeField] private int shelfCount = 6;

        [Min(0.1f)]
        [SerializeField] private float secondsPerStep = 2f;

        private Coroutine testCoroutine;

        [ContextMenu("Run Camera Path Test")]
        public void RunCameraPathTest()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("CameraPathTester: Enter Play Mode first.");
                return;
            }

            if (cameraController == null || shelfController == null)
            {
                Debug.LogError("CameraPathTester: CameraController or ShelfController is not assigned.");
                return;
            }

            if (testCoroutine != null)
                StopCoroutine(testCoroutine);

            testCoroutine = StartCoroutine(TestPath());
        }

        [ContextMenu("Stop Camera Path Test")]
        public void StopCameraPathTest()
        {
            if (testCoroutine == null)
                return;

            StopCoroutine(testCoroutine);
            testCoroutine = null;
        }

        private IEnumerator TestPath()
        {
            if (!shelfController.CreateShelves(shelfCount))
            {
                testCoroutine = null;
                yield break;
            }

            cameraController.ResetToMainMenu();

            yield return new WaitForSeconds(secondsPerStep);

            Debug.Log("CameraPathTester: Main menu -> Shelf 1");
            cameraController.MoveToFirstShelf();

            for (int shelfIndex = 1; shelfIndex < shelfCount; shelfIndex++)
            {
                yield return new WaitForSeconds(secondsPerStep);

                Debug.Log($"CameraPathTester: Shelf {shelfIndex} -> Shelf {shelfIndex + 1}");
                cameraController.MoveToNextShelf();
            }

            yield return new WaitForSeconds(secondsPerStep);

            Debug.Log("CameraPathTester: Last shelf -> Receipt");
            cameraController.MoveToReceipt();

            testCoroutine = null;
        }
    }
}
