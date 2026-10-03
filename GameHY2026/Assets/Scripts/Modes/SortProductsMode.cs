using System;
using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    public class SortProductsMode : GameMode
    {
        [SerializeField] private List<ProductData> displayedProducts = new List<ProductData>();

        private List<ProductData> playerOrder = new List<ProductData>();

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
