using UnityEngine;

namespace PinkTaxGame
{
    [RequireComponent(typeof(Collider))]
    public class ProductInteractable : MonoBehaviour
    {
        [SerializeField] private ProductData productData;

        public ProductData ProductData => productData;
        public Transform CurrentSlot { get; set; }
        public Transform OriginalSlot { get; set; }
        public Vector3 OriginalPosition { get; set; }

        public void Setup(ProductData data)
        {
            productData = data;
        }
    }
}