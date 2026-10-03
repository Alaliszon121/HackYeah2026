using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    public class RunGenerator : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private ProductDatabase productDatabase;
        [SerializeField] private GameConfig gameConfig;


        public RunData GenerateRun()
        {
            if (!CanGenerateRun())
            {
                Debug.LogError("RunGenerator: Cannot generate run with current configuration.");
                return null;
            }

            List<ModeType> modeSequence = GenerateModeSequence();

            List<ProductData> runProducts =
                GenerateRunProducts(modeSequence);

            List<SublevelData> sublevels =
                GenerateSublevels(modeSequence, runProducts);

            return new RunData(
                runProducts,
                sublevels
            );
        }


        // -------------------------------------------------------
        // MODE SEQUENCE
        // -------------------------------------------------------

        private List<ModeType> GenerateModeSequence()
        {
            List<ModeType> sequence = new List<ModeType>();

            // First guarantee one occurrence of every enabled mode.
            foreach (ModeWeight modeWeight in gameConfig.ModeWeights)
            {
                if (modeWeight.weight <= 0)
                    continue;

                if (sequence.Count >= gameConfig.NumberOfSublevels)
                    break;

                sequence.Add(modeWeight.mode);
            }

            // Fill the remaining slots using weighted randomness.
            while (sequence.Count < gameConfig.NumberOfSublevels)
            {
                sequence.Add(ChooseWeightedMode());
            }

            // Don't always put the guaranteed modes first.
            Shuffle(sequence);

            return sequence;
        }


        private ModeType ChooseWeightedMode()
        {
            IReadOnlyList<ModeWeight> weights =
                gameConfig.ModeWeights;

            int totalWeight = 0;

            foreach (ModeWeight modeWeight in weights)
            {
                totalWeight += modeWeight.weight;
            }

            int randomValue = Random.Range(0, totalWeight);

            int currentWeight = 0;

            foreach (ModeWeight modeWeight in weights)
            {
                currentWeight += modeWeight.weight;

                if (randomValue < currentWeight)
                {
                    return modeWeight.mode;
                }
            }

            // Should normally never happen.
            return weights[0].mode;
        }


        // -------------------------------------------------------
        // RUN PRODUCTS
        // -------------------------------------------------------

        private List<ProductData> GenerateRunProducts(
            List<ModeType> modeSequence
        )
        {
            List<ProductData> selectedProducts =
                new List<ProductData>();

            bool needsComparisonPair =
                modeSequence.Contains(ModeType.PriceDifference);

            if (needsComparisonPair)
            {
                List<ProductData> pair =
                    FindRandomComparisonPair();

                foreach (ProductData product in pair)
                {
                    AddProductIfMissing(
                        selectedProducts,
                        product
                    );
                }
            }

            FillRemainingProducts(selectedProducts);

            Shuffle(selectedProducts);

            return selectedProducts;
        }


        private void FillRemainingProducts(
            List<ProductData> selectedProducts
        )
        {
            List<ProductData> candidates =
                new List<ProductData>(
                    productDatabase.Products
                );

            Shuffle(candidates);

            foreach (ProductData product in candidates)
            {
                if (
                    selectedProducts.Count >=
                    gameConfig.ProductsPerRun
                )
                {
                    break;
                }

                AddProductIfMissing(
                    selectedProducts,
                    product
                );
            }
        }


        private void AddProductIfMissing(
            List<ProductData> list,
            ProductData product
        )
        {
            if (product == null)
                return;

            if (list.Contains(product))
                return;

            list.Add(product);
        }


        // -------------------------------------------------------
        // SUBLEVEL GENERATION
        // -------------------------------------------------------

        private List<SublevelData> GenerateSublevels(
    List<ModeType> modeSequence,
    List<ProductData> runProducts
)
        {
            List<SublevelData> sublevels =
                new List<SublevelData>();

            HashSet<ProductData> usedGuessPriceProducts =
                new HashSet<ProductData>();

            HashSet<ProductData> usedPriceDifferenceProducts =
                new HashSet<ProductData>();

            HashSet<ProductData> usedSortProducts =
                new HashSet<ProductData>();


            foreach (ModeType mode in modeSequence)
            {
                List<ProductData> products;

                switch (mode)
                {
                    case ModeType.GuessPrice:
                        products = GenerateGuessPriceProducts(
                            runProducts,
                            usedGuessPriceProducts
                        );
                        break;


                    case ModeType.PriceDifference:
                        products = GeneratePriceDifferenceProducts(
                            runProducts,
                            usedPriceDifferenceProducts
                        );
                        break;


                    case ModeType.SortProducts:
                        products = GenerateSortProducts(
                            runProducts,
                            usedSortProducts
                        );
                        break;


                    default:
                        Debug.LogError(
                            $"Unsupported mode: {mode}"
                        );

                        products =
                            new List<ProductData>();

                        break;
                }


                foreach (ProductData product in products)
                {
                    switch (mode)
                    {
                        case ModeType.GuessPrice:
                            usedGuessPriceProducts.Add(product);
                            break;

                        case ModeType.PriceDifference:
                            usedPriceDifferenceProducts.Add(product);
                            break;

                        case ModeType.SortProducts:
                            usedSortProducts.Add(product);
                            break;
                    }
                }


                sublevels.Add(
                    new SublevelData(
                        mode,
                        products
                    )
                );
            }

            return sublevels;
        }


        // -------------------------------------------------------
        // GUESS PRICE
        // -------------------------------------------------------

        private List<ProductData> GenerateGuessPriceProducts(
    List<ProductData> runProducts,
    HashSet<ProductData> usedProducts
)
        {
            List<ProductData> unusedProducts =
                new List<ProductData>();

            foreach (ProductData product in runProducts)
            {
                if (!usedProducts.Contains(product))
                {
                    unusedProducts.Add(product);
                }
            }


            // If everything has already appeared in this mode,
            // repetition becomes allowed again.
            List<ProductData> candidates =
                unusedProducts.Count > 0
                    ? unusedProducts
                    : runProducts;


            ProductData selectedProduct =
                candidates[
                    Random.Range(
                        0,
                        candidates.Count
                    )
                ];


            return new List<ProductData>
            {
                selectedProduct
            };
        }


        // -------------------------------------------------------
        // PRICE DIFFERENCE
        // -------------------------------------------------------

        private List<ProductData> GeneratePriceDifferenceProducts(
    List<ProductData> runProducts,
    HashSet<ProductData> usedProducts
)
        {
            List<List<ProductData>> allPairs =
                FindAllComparisonPairs(
                    runProducts
                );


            if (allPairs.Count == 0)
            {
                Debug.LogError(
                    "No valid comparison pair found in current run."
                );

                return new List<ProductData>();
            }


            List<List<ProductData>> unusedPairs =
                new List<List<ProductData>>();


            foreach (List<ProductData> pair in allPairs)
            {
                bool firstAlreadyUsed =
                    usedProducts.Contains(pair[0]);

                bool secondAlreadyUsed =
                    usedProducts.Contains(pair[1]);


                if (
                    !firstAlreadyUsed &&
                    !secondAlreadyUsed
                )
                {
                    unusedPairs.Add(pair);
                }
            }


            // Prefer completely unused products.
            // If none remain, allow repetition.
            List<List<ProductData>> candidates =
                unusedPairs.Count > 0
                    ? unusedPairs
                    : allPairs;


            return candidates[
                Random.Range(
                    0,
                    candidates.Count
                )
            ];
        }


        private List<ProductData> FindRandomComparisonPair()
        {
            List<ProductData> allProducts =
                new List<ProductData>(
                    productDatabase.Products
                );

            List<List<ProductData>> validPairs =
                FindAllComparisonPairs(
                    allProducts
                );

            if (validPairs.Count == 0)
            {
                Debug.LogError(
                    "Product database contains no valid pink/normal comparison pairs."
                );

                return new List<ProductData>();
            }

            return validPairs[
                Random.Range(
                    0,
                    validPairs.Count
                )
            ];
        }


        private List<List<ProductData>> FindAllComparisonPairs(
            List<ProductData> products
        )
        {
            List<List<ProductData>> pairs =
                new List<List<ProductData>>();

            for (int i = 0; i < products.Count; i++)
            {
                for (
                    int j = i + 1;
                    j < products.Count;
                    j++
                )
                {
                    ProductData first =
                        products[i];

                    ProductData second =
                        products[j];

                    if (
                        first == null ||
                        second == null
                    )
                    {
                        continue;
                    }

                    if (
                        !first.IsComparableWith(second)
                    )
                    {
                        continue;
                    }

                    // For pink-tax comparisons we specifically
                    // want one pink and one non-pink product.
                    if (
                        first.IsPink ==
                        second.IsPink
                    )
                    {
                        continue;
                    }

                    pairs.Add(
                        new List<ProductData>
                        {
                            first,
                            second
                        }
                    );
                }
            }

            return pairs;
        }


        // -------------------------------------------------------
        // SORT PRODUCTS
        // -------------------------------------------------------

        private List<ProductData> GenerateSortProducts(
    List<ProductData> runProducts,
    HashSet<ProductData> usedProducts
)
        {
            List<ProductData> selectedProducts =
                new List<ProductData>();


            List<ProductData> unusedProducts =
                new List<ProductData>();

            List<ProductData> alreadyUsedProducts =
                new List<ProductData>();


            foreach (ProductData product in runProducts)
            {
                if (usedProducts.Contains(product))
                {
                    alreadyUsedProducts.Add(product);
                }
                else
                {
                    unusedProducts.Add(product);
                }
            }


            Shuffle(unusedProducts);
            Shuffle(alreadyUsedProducts);


            int requiredCount =
                gameConfig.SortModeProductCount;


            // First use products that have never appeared
            // in SortProducts before.
            foreach (ProductData product in unusedProducts)
            {
                if (selectedProducts.Count >= requiredCount)
                    break;

                selectedProducts.Add(product);
            }


            // If there aren't enough unused products,
            // fill the remaining slots with previous ones.
            foreach (ProductData product in alreadyUsedProducts)
            {
                if (selectedProducts.Count >= requiredCount)
                    break;

                if (!selectedProducts.Contains(product))
                {
                    selectedProducts.Add(product);
                }
            }


            return selectedProducts;
        }


        // -------------------------------------------------------
        // VALIDATION
        // -------------------------------------------------------

        private bool CanGenerateRun()
        {
            if (productDatabase == null)
            {
                Debug.LogError(
                    "RunGenerator: ProductDatabase is missing."
                );

                return false;
            }

            if (gameConfig == null)
            {
                Debug.LogError(
                    "RunGenerator: GameConfig is missing."
                );

                return false;
            }

            if (
                productDatabase.Products == null ||
                productDatabase.Products.Count == 0
            )
            {
                Debug.LogError(
                    "RunGenerator: Product database is empty."
                );

                return false;
            }

            if (
                gameConfig.ProductsPerRun >
                productDatabase.Products.Count
            )
            {
                Debug.LogError(
                    "RunGenerator: ProductsPerRun is larger than the product database."
                );

                return false;
            }

            if (
                gameConfig.SortModeProductCount >
                gameConfig.ProductsPerRun
            )
            {
                Debug.LogError(
                    "RunGenerator: SortModeProductCount cannot be larger than ProductsPerRun."
                );

                return false;
            }

            if (
                gameConfig.ModeWeights == null ||
                gameConfig.ModeWeights.Count == 0
            )
            {
                Debug.LogError(
                    "RunGenerator: No mode weights configured."
                );

                return false;
            }

            int totalWeight = 0;

            foreach (
                ModeWeight modeWeight
                in gameConfig.ModeWeights
            )
            {
                totalWeight +=
                    modeWeight.weight;
            }

            if (totalWeight <= 0)
            {
                Debug.LogError(
                    "RunGenerator: At least one mode must have a positive weight."
                );

                return false;
            }

            return true;
        }


        // -------------------------------------------------------
        // HELPERS
        // -------------------------------------------------------

        private void Shuffle<T>(
            List<T> list
        )
        {
            for (
                int i = list.Count - 1;
                i > 0;
                i--
            )
            {
                int randomIndex =
                    Random.Range(
                        0,
                        i + 1
                    );

                T temp = list[i];

                list[i] =
                    list[randomIndex];

                list[randomIndex] =
                    temp;
            }
        }
    }
}