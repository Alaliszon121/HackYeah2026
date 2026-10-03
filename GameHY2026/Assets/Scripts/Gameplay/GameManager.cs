using UnityEngine;

namespace PinkTaxGame
{
    [RequireComponent(typeof(GuessPriceMode))]
    [RequireComponent(typeof(PriceDifferenceMode))]
    [RequireComponent(typeof(SortProductsMode))]
    public class GameManager : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private RunGenerator runGenerator;
        [SerializeField] private ShelfController shelfController;

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
            GetModes();
            InitializeModes();
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
            if (runGenerator == null)
            {
                Debug.LogError("GameManager: RunGenerator is not assigned.");
                return;
            }

            if (shelfController == null)
            {
                Debug.LogError("GameManager: ShelfController is not assigned.");
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

            StartCurrentSublevel();
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

            acceptingResult = true;

            currentMode.Setup(sublevel);
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

            currentRun.AddResult(result);

            if (currentRun.HasNextSublevel())
            {
                currentRun.MoveToNextSublevel();
                StartCurrentSublevel();
                return;
            }

            GoToReceipt();
        }

        public void GoToReceipt()
        {
            currentMode = null;

            Debug.Log(
                $"GameManager: Run finished with {currentRun?.Results.Count ?? 0} results."
            );

            // Step 7:
            // Show receipt.
        }

        public void GoToEndCutscene()
        {
            // Step 7:
            // Start end cutscene.
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
                    Debug.LogError($"GameManager: Unsupported mode {modeType}.");
                    return null;
            }
        }
    }
}