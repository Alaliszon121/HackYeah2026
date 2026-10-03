using TMPro;
using UnityEngine;

namespace PinkTaxGame
{
    public class GuessPriceMode : GameMode
    {
        [SerializeField] private ProductData currentProduct;
        [SerializeField] private TMP_InputField playerInputZloty;
        [SerializeField] private TMP_InputField playerInputGrosze;
        
        private ProductData product;
        private int playerGuessGrosze;
        private bool hasPlayerGuess;

        public ProductData Product => product;
        public int PlayerGuessGrosze => playerGuessGrosze;
        public bool HasPlayerGuess => hasPlayerGuess;

        public override void Setup(SublevelData sublevel)
        {
            base.Setup(sublevel);

            product = null;
            playerGuessGrosze = 0;
            hasPlayerGuess = false;

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

            // Later:
            // Display the product and price-input UI.
        }

        public void SetPlayerGuess(int priceGrosze)
        {
            playerGuessGrosze = Mathf.Max(0, priceGrosze);
            hasPlayerGuess = true;
        }
        
        public void ExtractPlayerGuess() {
            playerGuessGrosze = int.Parse(playerInputZloty.text) * 100 + int.Parse(playerInputGrosze.text);
            hasPlayerGuess = true;
        }

        public override void Submit()
        {
            if (product == null)
            {
                Debug.LogError("GuessPriceMode: Cannot submit without a product.");
                return;
            }

            if (!hasPlayerGuess)
            {
                Debug.LogWarning("GuessPriceMode: Player has not entered a price yet.");
                return;
            }

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
    }
}
