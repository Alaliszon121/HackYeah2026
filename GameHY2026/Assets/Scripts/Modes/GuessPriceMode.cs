using UnityEngine;

namespace PinkTaxGame
{
    public class GuessPriceMode : GameMode
    {
        [SerializeField] private ProductData currentProduct;
        private float playerGuessedPrice;

        public void SetPlayerGuessedPrice(float price) { }
        public override void Setup(SublevelData sublevel) { }
        public override void Play() { }
        public override void Submit() { }

        public override ModeResult CalculateResult()
        {
            return default;
        }

        public override ModeResult GetResult()
        {
            return default;
        }
    }
}
