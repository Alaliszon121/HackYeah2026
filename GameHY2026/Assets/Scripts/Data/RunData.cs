using System;
using System.Collections.Generic;

namespace PinkTaxGame
{
    [Serializable]
    public sealed class RunData
    {
        private readonly List<ProductData> productsInCurrentRun;
        private readonly List<SublevelData> sublevels;
        private readonly List<ModeResult> results = new List<ModeResult>();

        private int currentSublevelIndex;

        public IReadOnlyList<ProductData> ProductsInCurrentRun => productsInCurrentRun;
        public IReadOnlyList<SublevelData> Sublevels => sublevels;
        public IReadOnlyList<ModeResult> Results => results;

        public int CurrentSublevelIndex => currentSublevelIndex;

        public SublevelData CurrentSublevel
        {
            get
            {
                if (currentSublevelIndex < 0 || currentSublevelIndex >= sublevels.Count)
                    return null;

                return sublevels[currentSublevelIndex];
            }
        }

        public int TotalPoints
        {
            get
            {
                int total = 0;

                foreach (ModeResult result in results)
                    total += result.Points;

                return total;
            }
        }

        public int MaximumPossiblePoints
        {
            get
            {
                int total = 0;

                foreach (ModeResult result in results)
                    total += result.MaxPoints;

                return total;
            }
        }

        public float ScorePercentage
        {
            get
            {
                if (MaximumPossiblePoints <= 0)
                    return 0f;

                return (float)TotalPoints / MaximumPossiblePoints;
            }
        }

        public RunData(IEnumerable<ProductData> products, IEnumerable<SublevelData> sublevels)
        {
            productsInCurrentRun = products != null
                ? new List<ProductData>(products)
                : new List<ProductData>();

            this.sublevels = sublevels != null
                ? new List<SublevelData>(sublevels)
                : new List<SublevelData>();

            currentSublevelIndex = 0;
        }

        public void AddResult(ModeResult result)
        {
            if (result != null)
                results.Add(result);
        }

        public bool HasNextSublevel()
        {
            return currentSublevelIndex + 1 < sublevels.Count;
        }

        public bool MoveToNextSublevel()
        {
            if (!HasNextSublevel())
                return false;

            currentSublevelIndex++;
            return true;
        }
    }
}
