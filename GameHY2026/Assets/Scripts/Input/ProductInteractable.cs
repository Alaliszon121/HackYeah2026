using UnityEngine;
using UnityEngine.EventSystems;

namespace PinkTaxGame
{
    [RequireComponent(typeof(Collider))]
    public class ProductInteractable : MonoBehaviour,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [SerializeField] private ProductData productData;

        public ProductData ProductData => productData;

        public void Setup(ProductData data) { }
        public void OnPointerClick(PointerEventData eventData) { }
        public void OnPointerDown(PointerEventData eventData) { }
        public void OnPointerUp(PointerEventData eventData) { }
        public void OnBeginDrag(PointerEventData eventData) { }
        public void OnDrag(PointerEventData eventData) { }
        public void OnEndDrag(PointerEventData eventData) { }
    }
}
