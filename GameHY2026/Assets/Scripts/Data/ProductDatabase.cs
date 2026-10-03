using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    [CreateAssetMenu(
        fileName = "ProductDatabase",
        menuName = "Pink Tax Game/Product Database"
    )]
    public sealed class ProductDatabase : ScriptableObject
    {
        [SerializeField]
        private List<ProductData> products = new List<ProductData>();

        public IReadOnlyList<ProductData> Products => products;
    }
}