using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    public class GameManagerTester : MonoBehaviour
    {
        private const int MaxPoints = 100;

        [SerializeField] private GameManager gameManager;

        [ContextMenu("Run Fake Progression Test")]
        public void RunFakeProgressionTest()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("GameManagerTester: Enter Play Mode first.");
                return;
            }

            if (gameManager == null)
            {
                Debug.LogError("GameManagerTester: GameManager is not assigned.");
                return;
            }

            gameManager.StartGame();

            if (gameManager.CurrentRun == null)
            {
                Debug.LogError("GameManagerTester: StartGame did not create a run.");
                return;
            }

            int expectedResults = gameManager.CurrentRun.Sublevels.Count;

            while (gameManager.CurrentRun.Results.Count < expectedResults)
            {
                SublevelData sublevel = gameManager.CurrentRun.CurrentSublevel;

                if (sublevel == null)
                {
                    Debug.LogError("GameManagerTester: CurrentSublevel is null.");
                    return;
                }

                ModeResult result = CreateFakeResult(sublevel);

                if (result == null)
                    return;

                gameManager.CompleteCurrentSublevel(result);
            }

            Debug.Log(
                $"GameManagerTester: PASS. " +
                $"{gameManager.CurrentRun.Results.Count}/{expectedResults} results stored. " +
                $"Total score: {gameManager.CurrentRun.TotalPoints}/" +
                $"{gameManager.CurrentRun.MaximumPossiblePoints}."
            );
        }

        private ModeResult CreateFakeResult(SublevelData sublevel)
        {
            switch (sublevel.ModeType)
            {
                case ModeType.GuessPrice:
                {
                    int correct = sublevel.Products[0].PriceGrosze;

                    return ModeResult.CreateNumericResult(
                        ModeType.GuessPrice,
                        sublevel.Products,
                        correct,
                        correct,
                        MaxPoints,
                        MaxPoints
                    );
                }

                case ModeType.PriceDifference:
                {
                    int correct = Mathf.Abs(
                        sublevel.Products[0].PriceGrosze -
                        sublevel.Products[1].PriceGrosze
                    );

                    return ModeResult.CreateNumericResult(
                        ModeType.PriceDifference,
                        sublevel.Products,
                        correct,
                        correct,
                        MaxPoints,
                        MaxPoints
                    );
                }

                case ModeType.SortProducts:
                {
                    List<ProductData> correctOrder = new List<ProductData>(sublevel.Products);
                    correctOrder.Sort((a, b) => a.PriceGrosze.CompareTo(b.PriceGrosze));

                    return ModeResult.CreateSortResult(
                        sublevel.Products,
                        correctOrder,
                        correctOrder,
                        MaxPoints,
                        MaxPoints
                    );
                }

                default:
                    Debug.LogError($"GameManagerTester: Unsupported mode {sublevel.ModeType}.");
                    return null;
            }
        }
    }
}
