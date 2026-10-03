using UnityEngine;

namespace PinkTaxGame
{
    public class RunGeneratorTester : MonoBehaviour
    {
        [SerializeField]
        private RunGenerator runGenerator;

        [ContextMenu("Generate Test Run")]
        private void GenerateTestRun()
        {
            RunData run =
                runGenerator.GenerateRun();

            if (run == null)
                return;

            Debug.Log(
                $"Generated run with {run.Sublevels.Count} sublevels."
            );

            for (
                int i = 0;
                i < run.Sublevels.Count;
                i++
            )
            {
                SublevelData sublevel =
                    run.Sublevels[i];

                string products = "";

                foreach (ProductData product in sublevel.Products)
                {
                    products +=
                        $"{product.ProductName} " +
                        $"[{(product.IsPink ? "Pink" : "Regular")}] " +
                        $"({product.ComparisonGroup}) " +
                        $"{product.PricePLN:0.00} PLN | ";
                }

                Debug.Log(
                    $"{i + 1}. {sublevel.ModeType}: {products}"
                );
            }
        }
    }
}