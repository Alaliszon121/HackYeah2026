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
            if (!ValidateConfiguration())
                return null;

            List<ModeType> modeSequence = GenerateModeSequence();
            List<ProductData> runProducts = GenerateRunProducts(modeSequence);

            if (runProducts == null)
                return null;

            List<SublevelData> sublevels = GenerateSublevels(modeSequence, runProducts);

            if (sublevels == null)
                return null;

            return new RunData(runProducts, sublevels);
        }

        private List<ModeType> GenerateModeSequence()
        {
            List<ModeType> sequence = new List<ModeType>();

            foreach (ModeWeight modeWeight in gameConfig.ModeWeights)
            {
                if (modeWeight.weight > 0)
                    sequence.Add(modeWeight.mode);
            }

            while (sequence.Count < gameConfig.NumberOfSublevels)
                sequence.Add(ChooseWeightedMode());

            Shuffle(sequence);
            return sequence;
        }

        private ModeType ChooseWeightedMode()
        {
            int totalWeight = 0;

            foreach (ModeWeight modeWeight in gameConfig.ModeWeights)
            {
                if (modeWeight.weight > 0)
                    totalWeight += modeWeight.weight;
            }

            int randomValue = Random.Range(0, totalWeight);
            int accumulatedWeight = 0;

            foreach (ModeWeight modeWeight in gameConfig.ModeWeights)
            {
                if (modeWeight.weight <= 0)
                    continue;

                accumulatedWeight += modeWeight.weight;

                if (randomValue < accumulatedWeight)
                    return modeWeight.mode;
            }

            return ModeType.GuessPrice;
        }

        private List<ProductData> GenerateRunProducts(List<ModeType> modeSequence)
        {
            List<ProductData> availableProducts = GetUniqueDatabaseProducts();
            List<ProductData> selectedProducts = new List<ProductData>();

            if (modeSequence.Contains(ModeType.PriceDifference))
            {
                List<List<ProductData>> pairs = FindComparisonPairs(availableProducts);

                if (pairs.Count == 0)
                {
                    Debug.LogError("RunGenerator: No valid comparison pair exists in ProductDatabase.");
                    return null;
                }

                List<ProductData> pair = pairs[Random.Range(0, pairs.Count)];
                selectedProducts.Add(pair[0]);
                selectedProducts.Add(pair[1]);
            }

            Shuffle(availableProducts);

            foreach (ProductData product in availableProducts)
            {
                if (selectedProducts.Count >= gameConfig.ProductsPerRun)
                    break;

                if (!selectedProducts.Contains(product))
                    selectedProducts.Add(product);
            }

            Shuffle(selectedProducts);
            return selectedProducts;
        }

        private List<SublevelData> GenerateSublevels(
            List<ModeType> modeSequence,
            List<ProductData> runProducts
        )
        {
            List<SublevelData> sublevels = new List<SublevelData>();

            HashSet<ProductData> usedGuessPrice = new HashSet<ProductData>();
            HashSet<ProductData> usedPriceDifference = new HashSet<ProductData>();
            HashSet<ProductData> usedSort = new HashSet<ProductData>();

            foreach (ModeType mode in modeSequence)
            {
                List<ProductData> products;

                switch (mode)
                {
                    case ModeType.GuessPrice:
                        products = ChooseGuessPriceProducts(runProducts, usedGuessPrice);
                        break;

                    case ModeType.PriceDifference:
                        products = ChoosePriceDifferenceProducts(runProducts, usedPriceDifference);
                        break;

                    case ModeType.SortProducts:
                        products = ChooseSortProducts(runProducts, usedSort);
                        break;

                    default:
                        Debug.LogError($"RunGenerator: Unsupported mode {mode}.");
                        return null;
                }

                if (products == null || products.Count == 0)
                {
                    Debug.LogError($"RunGenerator: Could not select products for {mode}.");
                    return null;
                }

                foreach (ProductData product in products)
                {
                    switch (mode)
                    {
                        case ModeType.GuessPrice:
                            usedGuessPrice.Add(product);
                            break;

                        case ModeType.PriceDifference:
                            usedPriceDifference.Add(product);
                            break;

                        case ModeType.SortProducts:
                            usedSort.Add(product);
                            break;
                    }
                }

                sublevels.Add(new SublevelData(mode, products));
            }

            return sublevels;
        }

        private List<ProductData> ChooseGuessPriceProducts(
            List<ProductData> runProducts,
            HashSet<ProductData> usedProducts
        )
        {
            List<ProductData> candidates = GetUnusedProducts(runProducts, usedProducts);

            if (candidates.Count == 0)
                candidates = new List<ProductData>(runProducts);

            ProductData product = candidates[Random.Range(0, candidates.Count)];

            return new List<ProductData> { product };
        }

        private List<ProductData> ChoosePriceDifferenceProducts(
            List<ProductData> runProducts,
            HashSet<ProductData> usedProducts
        )
        {
            List<List<ProductData>> allPairs = FindComparisonPairs(runProducts);

            if (allPairs.Count == 0)
                return null;

            List<List<ProductData>> unusedPairs = new List<List<ProductData>>();

            foreach (List<ProductData> pair in allPairs)
            {
                if (!usedProducts.Contains(pair[0]) && !usedProducts.Contains(pair[1]))
                    unusedPairs.Add(pair);
            }

            List<List<ProductData>> candidates = unusedPairs.Count > 0 ? unusedPairs : allPairs;

            return candidates[Random.Range(0, candidates.Count)];
        }

        private List<ProductData> ChooseSortProducts(
            List<ProductData> runProducts,
            HashSet<ProductData> usedProducts
        )
        {
            List<ProductData> result = new List<ProductData>();
            List<ProductData> unused = GetUnusedProducts(runProducts, usedProducts);

            Shuffle(unused);

            foreach (ProductData product in unused)
            {
                if (result.Count >= gameConfig.SortModeProductCount)
                    break;

                result.Add(product);
            }

            if (result.Count < gameConfig.SortModeProductCount)
            {
                List<ProductData> remaining = new List<ProductData>(runProducts);
                Shuffle(remaining);

                foreach (ProductData product in remaining)
                {
                    if (result.Count >= gameConfig.SortModeProductCount)
                        break;

                    if (!result.Contains(product))
                        result.Add(product);
                }
            }

            return result;
        }

        private List<ProductData> GetUniqueDatabaseProducts()
        {
            List<ProductData> result = new List<ProductData>();

            foreach (ProductData product in productDatabase.Products)
            {
                if (product != null && !result.Contains(product))
                    result.Add(product);
            }

            return result;
        }

        private List<ProductData> GetUnusedProducts(
            List<ProductData> products,
            HashSet<ProductData> usedProducts
        )
        {
            List<ProductData> result = new List<ProductData>();

            foreach (ProductData product in products)
            {
                if (!usedProducts.Contains(product))
                    result.Add(product);
            }

            return result;
        }

        private List<List<ProductData>> FindComparisonPairs(IReadOnlyList<ProductData> products)
        {
            List<List<ProductData>> pairs = new List<List<ProductData>>();

            for (int i = 0; i < products.Count; i++)
            {
                for (int j = i + 1; j < products.Count; j++)
                {
                    ProductData first = products[i];
                    ProductData second = products[j];

                    if (first == null || second == null)
                        continue;

                    if (!first.IsComparableWith(second))
                        continue;

                    if (first.IsPink == second.IsPink)
                        continue;

                    pairs.Add(new List<ProductData> { first, second });
                }
            }

            return pairs;
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }

        private bool ValidateConfiguration()
        {
            if (productDatabase == null)
            {
                Debug.LogError("RunGenerator: ProductDatabase is not assigned.");
                return false;
            }

            if (gameConfig == null)
            {
                Debug.LogError("RunGenerator: GameConfig is not assigned.");
                return false;
            }

            if (gameConfig.NumberOfSublevels <= 0)
            {
                Debug.LogError("RunGenerator: NumberOfSublevels must be greater than zero.");
                return false;
            }

            List<ProductData> products = GetUniqueDatabaseProducts();

            if (products.Count == 0)
            {
                Debug.LogError("RunGenerator: ProductDatabase contains no valid products.");
                return false;
            }

            if (gameConfig.ProductsPerRun > products.Count)
            {
                Debug.LogError("RunGenerator: ProductsPerRun is larger than the number of available unique products.");
                return false;
            }

            if (gameConfig.SortModeProductCount > gameConfig.ProductsPerRun)
            {
                Debug.LogError("RunGenerator: SortModeProductCount cannot be larger than ProductsPerRun.");
                return false;
            }

            HashSet<ModeType> seenModes = new HashSet<ModeType>();
            int enabledModeCount = 0;
            int totalWeight = 0;
            bool priceDifferenceEnabled = false;

            foreach (ModeWeight modeWeight in gameConfig.ModeWeights)
            {
                if (!seenModes.Add(modeWeight.mode))
                {
                    Debug.LogError($"RunGenerator: ModeWeights contains {modeWeight.mode} more than once.");
                    return false;
                }

                if (modeWeight.weight <= 0)
                    continue;

                enabledModeCount++;
                totalWeight += modeWeight.weight;

                if (modeWeight.mode == ModeType.PriceDifference)
                    priceDifferenceEnabled = true;
            }

            if (totalWeight <= 0)
            {
                Debug.LogError("RunGenerator: At least one mode must have a positive weight.");
                return false;
            }

            if (gameConfig.NumberOfSublevels < enabledModeCount)
            {
                Debug.LogError("RunGenerator: NumberOfSublevels must be at least the number of enabled modes.");
                return false;
            }

            if (priceDifferenceEnabled && gameConfig.ProductsPerRun < 2)
            {
                Debug.LogError("RunGenerator: PriceDifference requires at least two products per run.");
                return false;
            }

            if (priceDifferenceEnabled && FindComparisonPairs(products).Count == 0)
            {
                Debug.LogError(
                    "RunGenerator: PriceDifference is enabled, but ProductDatabase contains no valid regular/pink comparison pair."
                );
                return false;
            }

            return true;
        }
    }
}
