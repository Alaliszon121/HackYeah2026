using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PinkTaxGame
{
    public class ReceiptController : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject receiptRoot;

        [Header("Score")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Image[] starImages;
        [SerializeField] private Sprite filledStarSprite;
        [SerializeField] private Sprite emptyStarSprite;

        [Header("History")]
        [SerializeField] private Transform historyContent;
        [SerializeField] private HistoryEntryUI historyEntryPrefab;

        [Header("Receipt")]
        [SerializeField] private Transform receiptItemsContent;
        [SerializeField] private ReceiptItemUI receiptItemPrefab;
        [SerializeField] private TMP_Text totalText;

        [Header("Navigation")]
        [SerializeField] private Button continueButton;

        private readonly List<HistoryEntryUI> historyEntries = new List<HistoryEntryUI>();
        private readonly List<ReceiptItemUI> receiptItems = new List<ReceiptItemUI>();

        private GameManager gameManager;

        private void Awake()
        {
            gameManager = GetComponent<GameManager>();

            if (gameManager == null)
                gameManager = FindAnyObjectByType<GameManager>();

            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinuePressed);

            HideReceipt();
        }

        private void OnDestroy()
        {
            if (continueButton != null)
                continueButton.onClick.RemoveListener(OnContinuePressed);
        }

        public void ShowReceipt(RunData run, GameConfig config)
        {
            if (run == null)
            {
                Debug.LogError("ReceiptController: RunData is null.");
                return;
            }

            if (receiptRoot == null)
            {
                Debug.LogError("ReceiptController: Receipt Root is not assigned.");
                return;
            }

            ClearGeneratedUI();

            receiptRoot.SetActive(true);

            ShowScore(run, config);
            ShowHistory(run);
            ShowReceiptItems(run);
        }

        public void HideReceipt()
        {
            if (receiptRoot != null)
                receiptRoot.SetActive(false);
        }

        private void ShowScore(RunData run, GameConfig config)
        {
            if (scoreText != null)
            {
                scoreText.text =
                    $"{run.TotalPoints}/{run.MaximumPossiblePoints}";
            }

            int starCount = CalculateStarCount(
                run.ScorePercentage,
                config
            );

            for (int i = 0; i < starImages.Length; i++)
            {
                Image star = starImages[i];

                if (star == null)
                    continue;

                bool filled = i < starCount;

                if (filled && filledStarSprite != null)
                    star.sprite = filledStarSprite;
                else if (!filled && emptyStarSprite != null)
                    star.sprite = emptyStarSprite;

                star.enabled =
                    filledStarSprite != null ||
                    emptyStarSprite != null;
            }
        }

        private int CalculateStarCount(
            float scorePercentage,
            GameConfig config
        )
        {
            if (config == null)
            {
                Debug.LogWarning(
                    "ReceiptController: GameConfig is null. Showing 0 stars."
                );
                return 0;
            }

            int starCount = 0;

            foreach (float threshold in config.StarThresholds)
            {
                if (scorePercentage >= threshold)
                    starCount++;
            }

            return Mathf.Clamp(starCount, 0, 5);
        }

        private void ShowHistory(RunData run)
        {
            if (historyContent == null || historyEntryPrefab == null)
            {
                Debug.LogWarning(
                    "ReceiptController: History UI is not fully assigned."
                );
                return;
            }

            foreach (ModeResult result in run.Results)
            {
                HistoryEntryUI entry =
                    Instantiate(historyEntryPrefab, historyContent);

                entry.Setup(result);
                historyEntries.Add(entry);
            }
        }

        private void ShowReceiptItems(RunData run)
        {
            if (
                receiptItemsContent == null ||
                receiptItemPrefab == null
            )
            {
                Debug.LogWarning(
                    "ReceiptController: Receipt item UI is not fully assigned."
                );
                return;
            }

            int totalGrosze = 0;

            foreach (ProductData product in run.ProductsInCurrentRun)
            {
                if (product == null)
                    continue;

                ReceiptItemUI item =
                    Instantiate(receiptItemPrefab, receiptItemsContent);

                item.Setup(product);
                receiptItems.Add(item);

                totalGrosze += product.PriceGrosze;
            }

            if (totalText != null)
                totalText.text = $"TOTAL: {FormatPrice(totalGrosze)}";
        }

        private void OnContinuePressed()
        {
            if (gameManager == null)
                gameManager = FindAnyObjectByType<GameManager>();

            if (gameManager == null)
            {
                Debug.LogError(
                    "ReceiptController: No GameManager exists in the scene."
                );
                return;
            }

            gameManager.GoToEndCutscene();
        }

        private void ClearGeneratedUI()
        {
            foreach (HistoryEntryUI entry in historyEntries)
            {
                if (entry != null)
                    Destroy(entry.gameObject);
            }

            historyEntries.Clear();

            foreach (ReceiptItemUI item in receiptItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }

            receiptItems.Clear();
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
