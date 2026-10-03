using System;
using UnityEngine;

namespace PinkTaxGame
{
    public class ReceiptController : MonoBehaviour
    {
        [SerializeField] private GameObject receiptRoot;

        public void ShowReceipt(RunData run)
        {
        }

        public void GenerateResultEntries(RunData run)
        {
        }

        public int CalculateStars(RunData run)
        {
            throw new NotImplementedException();
        }

        public void HideReceipt()
        {
        }
    }
}
