using System;
using UnityEngine;

namespace PinkTaxGame
{
    public class PriceDifferenceMode : GameMode
    {
        [SerializeField] private ProductData firstProduct;
        [SerializeField] private ProductData secondProduct;

        private float playerGuessedDifference;

        public override void Setup(SublevelData sublevel)
        {
        }

        public override void Play()
        {
        }

        public override void Submit()
        {
        }

        public override ModeResult CalculateResult()
        {
            throw new NotImplementedException();
        }

        public override ModeResult GetResult()
        {
            throw new NotImplementedException();
        }
    }
}
