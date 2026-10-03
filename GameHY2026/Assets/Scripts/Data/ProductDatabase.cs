using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    [CreateAssetMenu(fileName = "ProductDatabase", menuName = "Pink Tax Game/Product Database")]
    public class ProductDatabase : ScriptableObject
    {
        public List<ProductData> products = new List<ProductData>();
    }
}
