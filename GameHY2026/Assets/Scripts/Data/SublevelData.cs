using System;
using System.Collections.Generic;

namespace PinkTaxGame
{
    [Serializable]
    public sealed class SublevelData
    {
        private readonly ModeType modeType;
        private readonly List<ProductData> products;

        public ModeType ModeType => modeType;
        public IReadOnlyList<ProductData> Products => products;

        public SublevelData(ModeType modeType, IEnumerable<ProductData> products)
        {
            this.modeType = modeType;
            this.products = products != null
                ? new List<ProductData>(products)
                : new List<ProductData>();
        }
    }
}
