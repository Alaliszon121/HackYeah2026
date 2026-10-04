using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PinkTaxGame
{
    public class ReceiptItemUI : MonoBehaviour
    {
        [SerializeField] private Image productIcon;
        [SerializeField] private TMP_Text productNameText;
        [SerializeField] private TMP_Text priceText;

        public void Setup(ProductData product)
        {
            if (product == null)
            {
                Debug.LogError("ReceiptItemUI: Product is null.");
                return;
            }

            if (productIcon != null)
            {
                productIcon.sprite = product.Icon;
                productIcon.enabled = product.Icon != null;
            }

            if (productNameText != null)
                productNameText.text = product.ProductName;

            if (priceText != null)
                priceText.text = FormatPrice(product.PriceGrosze);
        }

        private string FormatPrice(int grosze)
        {
            int absoluteGrosze = Mathf.Abs(grosze);
            int zloty = absoluteGrosze / 100;
            int remainder = absoluteGrosze % 100;

            string sign = grosze < 0 ? "-" : string.Empty;
            return $"{sign}{zloty}.{remainder:00} zł";
        }
    }
}
