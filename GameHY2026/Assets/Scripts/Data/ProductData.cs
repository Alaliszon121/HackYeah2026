using UnityEngine;

namespace PinkTaxGame
{
    [CreateAssetMenu(
        fileName = "ProductData",
        menuName = "Pink Tax Game/Product"
    )]
    public sealed class ProductData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string productName;

        [Header("Price")]
        [SerializeField] private bool isPink;

        [Tooltip("Price stored in grosze. Example: 12.99 PLN = 1299.")]
        [Min(0)]
        [SerializeField] private int priceGrosze;

        [Header("Visuals")]
        [SerializeField] private GameObject modelPrefab;
        [SerializeField] private Sprite icon;

        [Header("Comparison")]
        [Tooltip("Products with the same comparison group can be compared. Example: razor.")]
        [SerializeField] private string comparisonGroup;

        public string ProductName => productName;
        public bool IsPink => isPink;

        public int PriceGrosze => priceGrosze;

        public float PricePLN => priceGrosze / 100f;

        public GameObject ModelPrefab => modelPrefab;
        public Sprite Icon => icon;

        public string ComparisonGroup => comparisonGroup;

        public bool IsComparableWith(ProductData other)
        {
            if (other == null)
                return false;

            if (other == this)
                return false;

            if (string.IsNullOrWhiteSpace(comparisonGroup))
                return false;

            return comparisonGroup == other.comparisonGroup;
        }
    }
}