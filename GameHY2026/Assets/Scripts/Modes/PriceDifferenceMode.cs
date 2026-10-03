using UnityEngine;

namespace PinkTaxGame
{
    public class PriceDifferenceMode : GameMode
    {
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

        public override void Setup(SublevelData sublevel)
        {
            base.Setup(sublevel);

            firstProduct = null;
            secondProduct = null;
            playerGuessGrosze = 0;
            hasPlayerGuess = false;

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

            // Later:
            // Display both products and difference-input UI.
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

            if (!hasPlayerGuess)
            {
                Debug.LogWarning("PriceDifferenceMode: Player has not entered a difference yet.");
                return;
            }

            // PriceDifference scoring is intentionally not defined yet.
            // Once agreed, create a ModeResult and send it to GameManager here.
        }
    }
}
