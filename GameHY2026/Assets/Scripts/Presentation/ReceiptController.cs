using UnityEngine;

namespace PinkTaxGame
{
    public class ReceiptController : MonoBehaviour
    {
        [SerializeField] private GameObject receiptRoot;

        public void ShowReceipt(RunData run) { }
        public void GenerateResultEntries(RunData run) { }

        public int CalculateStars(RunData run)
        {
            return default;
        }

        public void HideReceipt() { }
    }
}
