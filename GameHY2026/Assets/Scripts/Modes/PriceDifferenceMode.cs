using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PinkTaxGame
{
    public class PriceDifferenceMode : GameMode
    {
        [Header("UI")]
        [SerializeField] private GameObject uiRoot;
        [SerializeField] private TMP_InputField playerInputZloty;
        [SerializeField] private TMP_InputField playerInputGrosze;
        [SerializeField] private Button submitButton;

        private ProductData firstProduct;
        private ProductData secondProduct;

        private int playerGuessGrosze;
        private bool hasPlayerGuess;

        public ProductData FirstProduct => firstProduct;
        public ProductData SecondProduct => secondProduct;
        public int PlayerGuessGrosze => playerGuessGrosze;
        public bool HasPlayerGuess => hasPlayerGuess;

        public int CorrectDifferenceGrosze
        {
            get
            {
                if (firstProduct == null || secondProduct == null)
                    return 0;

                return Mathf.Abs(firstProduct.PriceGrosze - secondProduct.PriceGrosze);
            }
        }

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

            firstProduct = null;
            secondProduct = null;
            playerGuessGrosze = 0;
            hasPlayerGuess = false;

            ClearInputFields();

            if (sublevel == null)
            {
                Debug.LogError("PriceDifferenceMode: SublevelData is null.");
                return;
            }

            if (sublevel.ModeType != ModeType.PriceDifference)
            {
                Debug.LogError("PriceDifferenceMode: Received a sublevel for the wrong mode.");
                return;
            }

            if (sublevel.Products.Count != 2)
            {
                Debug.LogError("PriceDifferenceMode: PriceDifference requires exactly two products.");
                return;
            }

            ProductData first = sublevel.Products[0];
            ProductData second = sublevel.Products[1];

            if (first == null || second == null)
            {
                Debug.LogError("PriceDifferenceMode: One or both products are null.");
                return;
            }

            if (!first.IsComparableWith(second))
            {
                Debug.LogError("PriceDifferenceMode: Products are not from the same comparison group.");
                return;
            }

            if (first.IsPink == second.IsPink)
            {
                Debug.LogError("PriceDifferenceMode: Comparison requires one pink and one regular product.");
                return;
            }

            firstProduct = first;
            secondProduct = second;
        }

        public override void Play()
        {
            if (firstProduct == null || secondProduct == null)
            {
                Debug.LogError("PriceDifferenceMode: Cannot play without two valid products.");
                return;
            }

            if (uiRoot == null)
            {
                Debug.LogError("PriceDifferenceMode: UI Root is not assigned.");
                return;
            }

            if (playerInputZloty == null || playerInputGrosze == null)
            {
                Debug.LogError("PriceDifferenceMode: Both price input fields must be assigned.");
                return;
            }

            if (submitButton == null)
            {
                Debug.LogError("PriceDifferenceMode: Submit Button is not assigned.");
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

        public void SetPlayerGuess(int differenceGrosze)
        {
            playerGuessGrosze = Mathf.Max(0, differenceGrosze);
            hasPlayerGuess = true;
        }

        public override void Submit()
        {
            if (firstProduct == null || secondProduct == null)
            {
                Debug.LogError("PriceDifferenceMode: Cannot submit without valid products.");
                return;
            }

            if (!TryReadPlayerGuess(out int guessGrosze))
            {
                hasPlayerGuess = false;
                Debug.LogWarning("PriceDifferenceMode: Enter a valid non-negative price difference.");
                return;
            }

            SetPlayerGuess(guessGrosze);

            if (gameManager == null)
            {
                Debug.LogError("PriceDifferenceMode: GameManager is not initialized.");
                return;
            }

            int correctDifferenceGrosze = CorrectDifferenceGrosze;
            int points = CalculatePoints(correctDifferenceGrosze, playerGuessGrosze);

            ModeResult result = ModeResult.CreateNumericResult(
                ModeType.PriceDifference,
                currentSublevel.Products,
                playerGuessGrosze,
                correctDifferenceGrosze,
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

        private int CalculatePoints(int correctDifferenceGrosze, int guessedDifferenceGrosze)
        {
            int delta = Mathf.Abs(guessedDifferenceGrosze - correctDifferenceGrosze);

            if (delta == 0)
                return MaxPoints;

            if (correctDifferenceGrosze <= 0 || delta >= correctDifferenceGrosze)
                return 0;

            float relativeError = (float)delta / correctDifferenceGrosze;
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
