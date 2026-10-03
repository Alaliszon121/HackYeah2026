using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PinkTaxGame
{
    public class GuessPriceMode : GameMode
    {
        [Header("UI")]
        [SerializeField] private GameObject uiRoot;
        [SerializeField] private TMP_InputField playerInputZloty;
        [SerializeField] private TMP_InputField playerInputGrosze;
        [SerializeField] private Button submitButton;

        private ProductData product;
        private int playerGuessGrosze;
        private bool hasPlayerGuess;

        public ProductData Product => product;
        public int PlayerGuessGrosze => playerGuessGrosze;
        public bool HasPlayerGuess => hasPlayerGuess;

        private void Awake()
        {
            ConfigureInputFields();

            if (submitButton != null)
                submitButton.onClick.AddListener(Submit);

            if (uiRoot != null)
                uiRoot.SetActive(false);
        }

        private void OnDestroy()
        {
            if (submitButton != null)
                submitButton.onClick.RemoveListener(Submit);

            if (playerInputZloty != null)
                playerInputZloty.onValueChanged.RemoveListener(OnZlotyValueChanged);

            if (playerInputGrosze != null)
                playerInputGrosze.onValueChanged.RemoveListener(OnGroszeValueChanged);
        }

        public override void Setup(SublevelData sublevel)
        {
            base.Setup(sublevel);

            product = null;
            playerGuessGrosze = 0;
            hasPlayerGuess = false;

            ClearInputFields();

            if (sublevel == null)
            {
                Debug.LogError("GuessPriceMode: SublevelData is null.");
                return;
            }

            if (sublevel.ModeType != ModeType.GuessPrice)
            {
                Debug.LogError("GuessPriceMode: Received a sublevel for the wrong mode.");
                return;
            }

            if (sublevel.Products.Count != 1)
            {
                Debug.LogError("GuessPriceMode: GuessPrice requires exactly one product.");
                return;
            }

            product = sublevel.Products[0];

            if (product == null)
                Debug.LogError("GuessPriceMode: Product is null.");
        }

        public override void Play()
        {
            if (product == null)
            {
                Debug.LogError("GuessPriceMode: Cannot play without a product.");
                return;
            }

            if (uiRoot == null)
            {
                Debug.LogError("GuessPriceMode: UI Root is not assigned.");
                return;
            }

            if (playerInputZloty == null || playerInputGrosze == null)
            {
                Debug.LogError("GuessPriceMode: Both price input fields must be assigned.");
                return;
            }

            if (submitButton == null)
            {
                Debug.LogError("GuessPriceMode: Submit Button is not assigned.");
                return;
            }

            ClearInputFields();
            hasPlayerGuess = false;

            uiRoot.SetActive(true);

            playerInputZloty.Select();
            playerInputZloty.ActivateInputField();
        }

        public override void Stop()
        {
            if (uiRoot != null)
                uiRoot.SetActive(false);

            hasPlayerGuess = false;
        }

        public void SetPlayerGuess(int priceGrosze)
        {
            playerGuessGrosze = Mathf.Max(0, priceGrosze);
            hasPlayerGuess = true;
        }

        public override void Submit()
        {
            if (product == null)
            {
                Debug.LogError("GuessPriceMode: Cannot submit without a product.");
                return;
            }

            if (!TryReadPlayerGuess(out int guessGrosze))
            {
                hasPlayerGuess = false;
                Debug.LogWarning("GuessPriceMode: Enter a valid non-negative price.");
                return;
            }

            SetPlayerGuess(guessGrosze);

            if (gameManager == null)
            {
                Debug.LogError("GuessPriceMode: GameManager is not initialized.");
                return;
            }

            int correctPriceGrosze = product.PriceGrosze;
            int points = CalculatePoints(correctPriceGrosze, playerGuessGrosze);

            ModeResult result = ModeResult.CreateNumericResult(
                ModeType.GuessPrice,
                currentSublevel.Products,
                playerGuessGrosze,
                correctPriceGrosze,
                points,
                MaxPoints
            );

            gameManager.CompleteCurrentSublevel(result);
        }

        private void ConfigureInputFields()
        {
            if (playerInputZloty != null)
            {
                playerInputZloty.contentType = TMP_InputField.ContentType.IntegerNumber;
                playerInputZloty.onValueChanged.AddListener(OnZlotyValueChanged);
            }

            if (playerInputGrosze != null)
            {
                playerInputGrosze.contentType = TMP_InputField.ContentType.IntegerNumber;
                playerInputGrosze.characterLimit = 2;
                playerInputGrosze.onValueChanged.AddListener(OnGroszeValueChanged);
            }
        }

        private void OnZlotyValueChanged(string value)
        {
            SanitizeDigitsOnly(playerInputZloty, value);
        }

        private void OnGroszeValueChanged(string value)
        {
            SanitizeDigitsOnly(playerInputGrosze, value);
        }

        private void SanitizeDigitsOnly(TMP_InputField inputField, string value)
        {
            if (inputField == null || string.IsNullOrEmpty(value))
                return;

            char[] digits = new char[value.Length];
            int digitCount = 0;

            foreach (char character in value)
            {
                if (char.IsDigit(character))
                {
                    digits[digitCount] = character;
                    digitCount++;
                }
            }

            string sanitized = new string(digits, 0, digitCount);

            if (sanitized != value)
                inputField.SetTextWithoutNotify(sanitized);
        }

        private bool TryReadPlayerGuess(out int totalGrosze)
        {
            totalGrosze = 0;

            if (playerInputZloty == null || playerInputGrosze == null)
                return false;

            string zlotyText = playerInputZloty.text.Trim();
            string groszeText = playerInputGrosze.text.Trim();

            if (string.IsNullOrEmpty(zlotyText) && string.IsNullOrEmpty(groszeText))
                return false;

            int zloty = 0;
            int grosze = 0;

            if (!string.IsNullOrEmpty(zlotyText) && !int.TryParse(zlotyText, out zloty))
                return false;

            if (!string.IsNullOrEmpty(groszeText) && !int.TryParse(groszeText, out grosze))
                return false;

            if (zloty < 0 || grosze < 0 || grosze > 99)
                return false;

            long combinedValue = (long)zloty * 100 + grosze;

            if (combinedValue > int.MaxValue)
                return false;

            totalGrosze = (int)combinedValue;
            return true;
        }

        private int CalculatePoints(int correctPriceGrosze, int guessedPriceGrosze)
        {
            int delta = Mathf.Abs(guessedPriceGrosze - correctPriceGrosze);

            if (delta == 0)
                return MaxPoints;

            if (correctPriceGrosze <= 0 || delta >= correctPriceGrosze)
                return 0;

            float relativeError = (float)delta / correctPriceGrosze;
            return Mathf.RoundToInt((1f - relativeError) * MaxPoints);
        }

        private void ClearInputFields()
        {
            if (playerInputZloty != null)
                playerInputZloty.SetTextWithoutNotify(string.Empty);

            if (playerInputGrosze != null)
                playerInputGrosze.SetTextWithoutNotify(string.Empty);
        }
    }
}
