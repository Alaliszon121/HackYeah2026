using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PinkTaxGame
{
    [RequireComponent(typeof(GuessPriceMode))]
    [RequireComponent(typeof(PriceDifferenceMode))]
    [RequireComponent(typeof(SortProductsMode))]
    public class GameManager : MonoBehaviour
    {
        [Header("Optional Scene Overrides")]
        [SerializeField] private RunGenerator runGenerator;
        [SerializeField] private ShelfController shelfController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private ReceiptController receiptController;

        private RunData currentRun;
        private GameMode currentMode;

        private GuessPriceMode guessPriceMode;
        private PriceDifferenceMode priceDifferenceMode;
        private SortProductsMode sortProductsMode;

        private bool acceptingResult;

        public RunData CurrentRun => currentRun;
        public GameMode CurrentMode => currentMode;

        private void Awake()
        {
            ResolveSceneReferences();
            GetModes();
            InitializeModes();
        }

        private void ResolveSceneReferences()
        {
            if (runGenerator == null)
                runGenerator = FindAnyObjectByType<RunGenerator>();

            if (shelfController == null)
                shelfController = FindAnyObjectByType<ShelfController>();

            if (cameraController == null)
                cameraController = FindAnyObjectByType<CameraController>();

            if (receiptController == null)
                receiptController = GetComponent<ReceiptController>();

            if (receiptController == null)
                receiptController = FindAnyObjectByType<ReceiptController>();
        }

        private void GetModes()
        {
            guessPriceMode = GetComponent<GuessPriceMode>();
            priceDifferenceMode = GetComponent<PriceDifferenceMode>();
            sortProductsMode = GetComponent<SortProductsMode>();
        }

        private void InitializeModes()
        {
            guessPriceMode.Initialize(this);
            priceDifferenceMode.Initialize(this);
            sortProductsMode.Initialize(this);
        }

        public void StartGame()
        {
            currentMode?.Stop();
            currentMode = null;
            currentRun = null;
            acceptingResult = false;

            ResolveSceneReferences();
            receiptController?.HideReceipt();

            if (runGenerator == null)
            {
                Debug.LogError("GameManager: No RunGenerator exists in the scene.");
                return;
            }

            if (shelfController == null)
            {
                Debug.LogError("GameManager: No ShelfController exists in the scene.");
                return;
            }

            if (cameraController == null)
            {
                Debug.LogError("GameManager: No CameraController exists in the scene.");
                return;
            }

            currentRun = runGenerator.GenerateRun();

            if (currentRun == null)
            {
                Debug.LogError("GameManager: RunGenerator failed to create a run.");
                return;
            }

            if (!shelfController.CreateShelves(currentRun.Sublevels.Count))
            {
                Debug.LogError("GameManager: ShelfController failed to create shelves.");
                currentRun = null;
                return;
            }

            if (!shelfController.PopulateShelves(currentRun.Sublevels))
            {
                Debug.LogError("GameManager: ShelfController failed to populate shelves.");
                shelfController.ClearShelves();
                currentRun = null;
                return;
            }

            cameraController.ResetToMainMenu();
            cameraController.MoveToFirstShelf(StartCurrentSublevel);
        }

        private void StartCurrentSublevel()
        {
            if (currentRun == null)
                return;

            SublevelData sublevel = currentRun.CurrentSublevel;

            if (sublevel == null)
            {
                Debug.LogError("GameManager: Current sublevel is null.");
                return;
            }

            currentMode = GetMode(sublevel.ModeType);

            if (currentMode == null)
            {
                Debug.LogError($"GameManager: No mode exists for {sublevel.ModeType}.");
                return;
            }

            currentMode.Setup(sublevel);

            if (currentMode is SortProductsMode sortMode)
            {
                int shelfIndex = currentRun.CurrentSublevelIndex;
                IReadOnlyList<GameObject> currentProducts =
                    shelfController.GetSpawnedProducts(shelfIndex);

                if (currentProducts == null)
                {
                    Debug.LogError(
                        $"GameManager: Shelf {shelfIndex} has no spawned product list."
                    );
                    return;
                }

                sortMode.SetShelfContext(shelfController, shelfIndex);
                sortMode.SetProductObjects(currentProducts);
            }

            acceptingResult = true;
            currentMode.Play();
        }

        public void CompleteCurrentSublevel(ModeResult result)
        {
            if (!acceptingResult)
                return;

            if (currentRun == null)
            {
                Debug.LogError("GameManager: No active run.");
                return;
            }

            if (result == null)
            {
                Debug.LogError("GameManager: Received null ModeResult.");
                return;
            }

            acceptingResult = false;
            currentMode?.Stop();
            currentRun.AddResult(result);

            if (currentRun.HasNextSublevel())
            {
                currentRun.MoveToNextSublevel();
                cameraController.MoveToNextShelf(StartCurrentSublevel);
                return;
            }

            cameraController.MoveToReceipt(GoToReceipt);
        }

        public void GoToReceipt()
        {
            acceptingResult = false;
            currentMode?.Stop();
            currentMode = null;

            if (currentRun == null)
            {
                Debug.LogError("GameManager: Cannot show receipt without an active run.");
                return;
            }

            if (receiptController == null)
            {
                ResolveSceneReferences();

                if (receiptController == null)
                {
                    Debug.LogError(
                        "GameManager: No ReceiptController exists in the scene."
                    );
                    return;
                }
            }

            receiptController.ShowReceipt(
                currentRun,
                runGenerator != null ? runGenerator.Config : null
            );
        }

        public void GoToEndCutscene()
        {
            receiptController?.HideReceipt();

            Debug.Log("GameManager: End cutscene requested.");

            // Next step:
            // Trigger the end cutscene here.
            // After the cutscene, reset/reload back to the main menu.
        }

        private GameMode GetMode(ModeType modeType)
        {
            switch (modeType)
            {
                case ModeType.GuessPrice:
                    return guessPriceMode;

                case ModeType.PriceDifference:
                    return priceDifferenceMode;

                case ModeType.SortProducts:
                    return sortProductsMode;

                default:
                    Debug.LogError(
                        $"GameManager: Unsupported mode {modeType}."
                    );
                    return null;
            }
        }
    }
}
