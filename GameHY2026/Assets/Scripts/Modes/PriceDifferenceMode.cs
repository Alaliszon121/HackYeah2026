using UnityEngine;

namespace PinkTaxGame
{
    public class PriceDifferenceMode : GameMode
    {
        [SerializeField] private ProductData firstProduct;
        [SerializeField] private ProductData secondProduct;
        private float playerGuessedDifference;

        public void SetPlayerGuessedDifference(float difference) { }
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
