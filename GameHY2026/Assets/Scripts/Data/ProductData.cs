using UnityEngine;

namespace PinkTaxGame
{
    [CreateAssetMenu(fileName = "ProductData", menuName = "Pink Tax Game/Product")]
    public class ProductData : ScriptableObject
    {
        [Header("Identity")]
        public string productName;

        [Header("Price")]
        public bool isPink;
        public float pricePLN;

        [Header("Visuals")]
        public GameObject modelPrefab;
        public Sprite icon;

        [Header("Comparison")]
        public string comparisonGroup;
    }
}
