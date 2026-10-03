using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    public class RunGenerator : MonoBehaviour
    {
        [SerializeField] private ProductDatabase productDatabase;
        [SerializeField] private GameConfig gameConfig;

        public RunData GenerateRun()
        {
            return default;
        }

        public List<ProductData> DrawProducts(int count)
        {
            return default;
        }

        public List<SublevelData> GenerateSublevels(List<ProductData> runProducts)
        {
            return default;
        }

        public List<ProductData> FindValidComparisonPair(List<ProductData> candidates)
        {
            return default;
        }

        public List<ProductData> GenerateProductsForMode(ModeType modeType, List<ProductData> candidates, int productCount)
        {
            return default;
        }
    }
}
