using System;
using System.Collections.Generic;

namespace PinkTaxGame
{
    [Serializable]
    public sealed class SublevelData
    {
        private ModeType modeType;

        private List<ProductData> products =
            new List<ProductData>();

        public ModeType ModeType => modeType;

        public IReadOnlyList<ProductData> Products =>
            products;

        public SublevelData(
            ModeType modeType,
            IEnumerable<ProductData> products
        )
        {
            this.modeType = modeType;

            if (products != null)
                this.products = new List<ProductData>(products);
        }
    }
}