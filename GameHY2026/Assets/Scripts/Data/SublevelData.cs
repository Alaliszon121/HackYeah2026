using System;
using System.Collections.Generic;

namespace PinkTaxGame
{
    [Serializable]
    public class SublevelData
    {
        public ModeType modeType;
        public List<ProductData> products = new List<ProductData>();
    }
}
