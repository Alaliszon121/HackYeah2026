using System;
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
            throw new NotImplementedException();
        }

        public List<ProductData> DrawProducts(int count)
        {
            throw new NotImplementedException();
        }

        public List<SublevelData> GenerateSublevels(List<ProductData> runProducts)
        {
            throw new NotImplementedException();
        }

        public List<ProductData> FindValidComparisonPair(List<ProductData> candidates)
        {
            throw new NotImplementedException();
        }

        public List<ProductData> GenerateProductsForMode(
            ModeType modeType,
            List<ProductData> candidates,
            int productCount)
        {
            throw new NotImplementedException();
        }
    }
}
