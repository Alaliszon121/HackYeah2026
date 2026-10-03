using System;
using System.Collections.Generic;

namespace PinkTaxGame
{
    [Serializable]
    public class RunData
    {
        public List<ProductData> productsInCurrentRun = new List<ProductData>();
        public List<SublevelData> sublevels = new List<SublevelData>();
        public List<ModeResult> results = new List<ModeResult>();

        public int currentSublevelIndex;

        public int TotalPoints => default;
        public int MaximumPossiblePoints => default;
        public float ScorePercentage => default;
    }
}
