using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PinkTaxGame
{
    public class HistoryEntryUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private TMP_Text modeTitleText;
        [SerializeField] private TMP_Text scoreText;

        [Header("Products Used")]
        [SerializeField] private Transform productIconsRoot;

        [Header("Numeric Answer")]
        [SerializeField] private GameObject numericSection;
        [SerializeField] private TMP_Text playerNumericAnswerText;
        [SerializeField] private TMP_Text correctNumericAnswerText;

        [Header("Sort Answer")]
        [SerializeField] private GameObject sortSection;
        [SerializeField] private Transform playerOrderRoot;
        [SerializeField] private Transform correctOrderRoot;

        [Header("Shared")]
        [SerializeField] private GameObject iconPrefab;

        private readonly List<GameObject> generatedIcons = new List<GameObject>();

        public void Setup(ModeResult result)
        {
            ClearGeneratedIcons();

            if (result == null)
            {
                Debug.LogError("HistoryEntryUI: ModeResult is null.");
                return;
            }

            if (modeTitleText != null)
                modeTitleText.text = GetModeTitle(result.ModeType);

            if (scoreText != null)
                scoreText.text = $"{result.Points}/{result.MaxPoints}";

            AddIcons(result.Products, productIconsRoot);

            bool isSort = result.ModeType == ModeType.SortProducts;

            if (numericSection != null)
                numericSection.SetActive(!isSort);

            if (sortSection != null)
                sortSection.SetActive(isSort);

            if (isSort)
            {
                AddIcons(result.PlayerProductOrder, playerOrderRoot);
                AddIcons(result.CorrectProductOrder, correctOrderRoot);
                return;
            }

            if (playerNumericAnswerText != null)
                playerNumericAnswerText.text = FormatPrice(result.PlayerNumericAnswerGrosze);

            if (correctNumericAnswerText != null)
                correctNumericAnswerText.text = FormatPrice(result.CorrectNumericAnswerGrosze);
        }

        private void AddIcons(
            IReadOnlyList<ProductData> products,
            Transform root
        )
        {
            if (products == null || root == null || iconPrefab == null)
                return;

            foreach (ProductData product in products)
            {
                if (product == null)
                    continue;

                GameObject iconObject = Instantiate(iconPrefab, root);
                Image image = iconObject.GetComponent<Image>();

                if (image == null)
                {
                    Debug.LogError(
                        "HistoryEntryUI: Icon prefab does not contain an Image component."
                    );

                    Destroy(iconObject);
                    continue;
                }

                image.sprite = product.Icon;
                image.enabled = product.Icon != null;

                generatedIcons.Add(iconObject);
            }
        }

        private void ClearGeneratedIcons()
        {
            foreach (GameObject icon in generatedIcons)
            {
                if (icon != null)
                    Destroy(icon);
            }

            generatedIcons.Clear();
        }

        private string GetModeTitle(ModeType modeType)
        {
            switch (modeType)
            {
                case ModeType.GuessPrice:
                    return "Guess Price";

                case ModeType.PriceDifference:
                    return "Price Difference";

                case ModeType.SortProducts:
                    return "Sort Products";

                default:
                    return modeType.ToString();
            }
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
