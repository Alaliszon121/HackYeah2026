using System;
using System.Collections.Generic;

namespace PinkTaxGame
{
    [Serializable]
    public sealed class ModeResult
    {
        private ModeType modeType;
        private List<ProductData> products = new List<ProductData>();

        private int points;
        private int maxPoints;

        private int playerNumericAnswerGrosze;
        private int correctNumericAnswerGrosze;

        private List<ProductData> playerProductOrder = new List<ProductData>();
        private List<ProductData> correctProductOrder = new List<ProductData>();

        public ModeType ModeType => modeType;
        public IReadOnlyList<ProductData> Products => products;

        public int Points => points;
        public int MaxPoints => maxPoints;

        public int PlayerNumericAnswerGrosze => playerNumericAnswerGrosze;
        public int CorrectNumericAnswerGrosze => correctNumericAnswerGrosze;

        public IReadOnlyList<ProductData> PlayerProductOrder => playerProductOrder;
        public IReadOnlyList<ProductData> CorrectProductOrder => correctProductOrder;

        public static ModeResult CreateNumericResult(
            ModeType modeType,
            IEnumerable<ProductData> products,
            int playerAnswerGrosze,
            int correctAnswerGrosze,
            int points,
            int maxPoints
        )
        {
            return new ModeResult
            {
                modeType = modeType,
                products = products != null ? new List<ProductData>(products) : new List<ProductData>(),
                playerNumericAnswerGrosze = playerAnswerGrosze,
                correctNumericAnswerGrosze = correctAnswerGrosze,
                points = points,
                maxPoints = maxPoints
            };
        }

        public static ModeResult CreateSortResult(
            IEnumerable<ProductData> products,
            IEnumerable<ProductData> playerOrder,
            IEnumerable<ProductData> correctOrder,
            int points,
            int maxPoints
        )
        {
            return new ModeResult
            {
                modeType = ModeType.SortProducts,
                products = products != null ? new List<ProductData>(products) : new List<ProductData>(),
                playerProductOrder = playerOrder != null ? new List<ProductData>(playerOrder) : new List<ProductData>(),
                correctProductOrder = correctOrder != null ? new List<ProductData>(correctOrder) : new List<ProductData>(),
                points = points,
                maxPoints = maxPoints
            };
        }
    }
}
