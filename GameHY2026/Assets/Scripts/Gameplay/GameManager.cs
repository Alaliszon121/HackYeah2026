using UnityEngine;

namespace PinkTaxGame
{
    public class GameManager : MonoBehaviour
    {
        [Header("Run State")]
        [SerializeField] private RunData currentRun;
        [SerializeField] private int currentSublevelIndex;

        private GameMode currentMode;

        [Header("Core Systems")]
        [SerializeField] private RunGenerator runGenerator;
        [SerializeField] private GameInputController inputController;
        [SerializeField] private ShelfController shelfController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private ReceiptController receiptController;

        [Header("Modes")]
        [SerializeField] private GuessPriceMode guessPriceMode;
        [SerializeField] private PriceDifferenceMode priceDifferenceMode;
        [SerializeField] private SortProductsMode sortProductsMode;

        [Header("Scene Flow")]
        [SerializeField] private Animator shopDoorAnimator;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject endCutscene;

        public RunData CurrentRun => currentRun;

        public void StartGame() { }
        public void StartCurrentSublevel() { }
        public void CompleteCurrentSublevel(ModeResult result) { }
        public void NextSublevel() { }
        public void GoToReceipt() { }
        public void GoToEndCutscene() { }
        public void ReturnToMainMenu() { }

        private GameMode GetModeForType(ModeType modeType)
        {
            return default;
        }
    }
}
