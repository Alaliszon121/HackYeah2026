using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    public class RunWorldTester : MonoBehaviour
    {
        [SerializeField] private RunGenerator runGenerator;
        [SerializeField] private ShelfController shelfController;
        [SerializeField] private CameraController cameraController;

        [Header("Test")]
        [Min(0.1f)]
        [SerializeField] private float secondsPerStep = 2f;

        private Coroutine testCoroutine;

        [ContextMenu("Run Generated World Test")]
        public void RunGeneratedWorldTest()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("RunWorldTester: Enter Play Mode first.");
                return;
            }

            if (runGenerator == null || shelfController == null || cameraController == null)
            {
                Debug.LogError(
                    "RunWorldTester: RunGenerator, ShelfController and CameraController must all be assigned."
                );
                return;
            }

            if (testCoroutine != null)
                StopCoroutine(testCoroutine);

            testCoroutine = StartCoroutine(TestGeneratedWorld());
        }

        [ContextMenu("Stop Generated World Test")]
        public void StopGeneratedWorldTest()
        {
            if (testCoroutine == null)
                return;

            StopCoroutine(testCoroutine);
            testCoroutine = null;
        }

        private IEnumerator TestGeneratedWorld()
        {
            RunData run = runGenerator.GenerateRun();

            if (run == null)
            {
                Debug.LogError("RunWorldTester: RunGenerator returned null.");
                testCoroutine = null;
                yield break;
            }

            if (!shelfController.CreateShelves(run.Sublevels.Count))
            {
                testCoroutine = null;
                yield break;
            }

            if (!shelfController.PopulateShelves(run.Sublevels))
            {
                testCoroutine = null;
                yield break;
            }

            if (!ValidateGeneratedWorld(run))
            {
                testCoroutine = null;
                yield break;
            }

            cameraController.ResetToMainMenu();

            Debug.Log(
                $"RunWorldTester: PASS world build. Generated {run.Sublevels.Count} shelves " +
                "and all assigned product models."
            );

            yield return new WaitForSeconds(secondsPerStep);

            for (int i = 0; i < run.Sublevels.Count; i++)
            {
                if (i == 0)
                    cameraController.MoveToFirstShelf();
                else
                    cameraController.MoveToNextShelf();

                LogSublevel(i, run.Sublevels[i]);

                yield return new WaitForSeconds(secondsPerStep);
            }

            cameraController.MoveToReceipt();

            Debug.Log(
                "RunWorldTester: PASS camera path. Reached every generated shelf and moved to receipt."
            );

            testCoroutine = null;
        }

        private bool ValidateGeneratedWorld(RunData run)
        {
            if (shelfController.ShelfCount != run.Sublevels.Count)
            {
                Debug.LogError(
                    $"RunWorldTester: Expected {run.Sublevels.Count} shelves, " +
                    $"but found {shelfController.ShelfCount}."
                );
                return false;
            }

            for (int i = 0; i < run.Sublevels.Count; i++)
            {
                SublevelData sublevel = run.Sublevels[i];
                GameObject shelf = shelfController.GetShelf(i);
                IReadOnlyList<GameObject> productObjects = shelfController.GetSpawnedProducts(i);

                if (shelf == null)
                {
                    Debug.LogError($"RunWorldTester: Shelf {i} is null.");
                    return false;
                }

                if (productObjects == null)
                {
                    Debug.LogError($"RunWorldTester: Shelf {i} has no spawned product list.");
                    return false;
                }

                if (productObjects.Count != sublevel.Products.Count)
                {
                    Debug.LogError(
                        $"RunWorldTester: Shelf {i} expected {sublevel.Products.Count} products, " +
                        $"but has {productObjects.Count}."
                    );
                    return false;
                }

                for (int productIndex = 0; productIndex < productObjects.Count; productIndex++)
                {
                    if (productObjects[productIndex] == null)
                    {
                        Debug.LogError(
                            $"RunWorldTester: Spawned product {productIndex} on shelf {i} is null."
                        );
                        return false;
                    }
                }
            }

            return true;
        }

        private void LogSublevel(int index, SublevelData sublevel)
        {
            List<string> productDescriptions = new List<string>();

            foreach (ProductData product in sublevel.Products)
            {
                productDescriptions.Add(
                    $"{product.ProductName} ({product.PricePLN:F2} PLN)"
                );
            }

            Debug.Log(
                $"RunWorldTester: Shelf {index + 1}/{shelfController.ShelfCount} | " +
                $"{sublevel.ModeType} | {string.Join(", ", productDescriptions)}"
            );
        }
    }
}
