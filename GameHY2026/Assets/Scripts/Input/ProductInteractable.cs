using UnityEngine;

namespace PinkTaxGame
{
    public class ProductInteractable : MonoBehaviour
    {
        [SerializeField] private ProductData productData;

        public ProductData ProductData => productData;

        public int CurrentPositionIndex { get; set; } = -1;
        public int OriginalPositionIndex { get; set; } = -1;

        public Vector3 OriginalPosition { get; set; }
        public Quaternion OriginalRotation { get; set; }

        public bool Setup(ProductData data)
        {
            productData = data;

            if (productData == null)
            {
                Debug.LogError("ProductInteractable: ProductData is null.");
                return false;
            }

            return EnsureCollider();
        }

        private bool EnsureCollider()
        {
            if (GetComponentInChildren<Collider>() != null)
                return true;

            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                Debug.LogError(
                    $"ProductInteractable: '{name}' has no Collider and no Renderer to build one from."
                );
                return false;
            }

            Bounds worldBounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
                worldBounds.Encapsulate(renderers[i].bounds);

            Vector3[] worldCorners =
            {
                new Vector3(worldBounds.min.x, worldBounds.min.y, worldBounds.min.z),
                new Vector3(worldBounds.min.x, worldBounds.min.y, worldBounds.max.z),
                new Vector3(worldBounds.min.x, worldBounds.max.y, worldBounds.min.z),
                new Vector3(worldBounds.min.x, worldBounds.max.y, worldBounds.max.z),
                new Vector3(worldBounds.max.x, worldBounds.min.y, worldBounds.min.z),
                new Vector3(worldBounds.max.x, worldBounds.min.y, worldBounds.max.z),
                new Vector3(worldBounds.max.x, worldBounds.max.y, worldBounds.min.z),
                new Vector3(worldBounds.max.x, worldBounds.max.y, worldBounds.max.z)
            };

            Vector3 firstLocalCorner = transform.InverseTransformPoint(worldCorners[0]);
            Bounds localBounds = new Bounds(firstLocalCorner, Vector3.zero);

            for (int i = 1; i < worldCorners.Length; i++)
                localBounds.Encapsulate(transform.InverseTransformPoint(worldCorners[i]));

            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = localBounds.center;
            boxCollider.size = localBounds.size;

            return true;
        }
    }
}
