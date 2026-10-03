using System;
using System.Collections.Generic;

namespace PinkTaxGame
{
    [Serializable]
    public class ModeResult
    {
        public ModeType modeType;
        public List<ProductData> products = new List<ProductData>();

        public string playerAnswer;
        public string correctAnswer;

        public int points;
        public int maxPoints;
    }
}
