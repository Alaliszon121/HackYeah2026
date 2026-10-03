using System;
using UnityEngine;

namespace PinkTaxGame
{
    public class GuessPriceMode : GameMode
    {
        [SerializeField] private ProductData currentProduct;

        private float playerGuessedPrice;

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
