using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    public class ShelfController : MonoBehaviour
    {
        [Header("Shelf Setup")]
        [SerializeField] private GameObject[] shelfPrefabs;

        [Tooltip("Distance between shelf origins.")]
        [Min(0.01f)]
        [SerializeField] private float shelfSpacing = 4f;

        [Header("Product Display")]
        [Tooltip("Shared local offset from each shelf root to the product row.")]
        [SerializeField] private Vector3 productRowLocalOffset;

        private readonly List<GameObject> shelves = new List<GameObject>();
        private readonly List<List<GameObject>> spawnedProducts = new List<List<GameObject>>();

        public int ShelfCount => shelves.Count;
        public float ShelfSpacing => shelfSpacing;
        public float ShelfWidth => shelfSpacing * 0.75f;
        public Vector3 ProductRowLocalOffset => productRowLocalOffset;

        public bool CreateShelves(int count)
        {
            ClearShelves();

            if (count <= 0)
            {
                Debug.LogError("ShelfController: Shelf count must be greater than zero.");
                return false;
            }

            if (shelfPrefabs == null || shelfPrefabs.Length == 0)
            {
                Debug.LogError("ShelfController: No shelf prefabs are assigned.");
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                GameObject prefab = GetRandomShelfPrefab();

                if (prefab == null)
                {
                    Debug.LogError("ShelfController: Shelf prefabs contain a null entry.");
                    ClearShelves();
                    return false;
                }

                Vector3 position = transform.position - transform.forward * shelfSpacing * i;
                GameObject shelf = Instantiate(prefab, position, transform.rotation, transform);

                shelves.Add(shelf);
                spawnedProducts.Add(new List<GameObject>());
            }

            return true;
        }

        public bool PopulateShelves(IReadOnlyList<SublevelData> sublevels)
        {
            if (sublevels == null)
            {
                Debug.LogError("ShelfController: Sublevels are null.");
                return false;
            }

            if (sublevels.Count != shelves.Count)
            {
                Debug.LogError(
                    $"ShelfController: Received {sublevels.Count} sublevels for {shelves.Count} shelves."
                );
                return false;
            }

            for (int i = 0; i < sublevels.Count; i++)
            {
                SublevelData sublevel = sublevels[i];

                if (sublevel == null)
                {
                    Debug.LogError($"ShelfController: Sublevel {i} is null.");
                    return false;
                }

                if (!ShowProducts(i, sublevel.Products))
                    return false;
            }

            return true;
        }

        public bool ShowProducts(int shelfIndex, IReadOnlyList<ProductData> products)
        {
            if (products == null || products.Count == 0)
            {
                Debug.LogError($"ShelfController: Shelf {shelfIndex} received no products.");
                return false;
            }

            for (int i = 0; i < products.Count; i++)
            {
                ProductData product = products[i];

                if (product == null)
                {
                    Debug.LogError($"ShelfController: Product {i} on shelf {shelfIndex} is null.");
                    return false;
                }

                if (product.ModelPrefab == null)
                {
                    Debug.LogError($"ShelfController: Product '{product.ProductName}' has no model prefab.");
                    return false;
                }
            }

            List<Pose> productPoses = GetProductPoses(shelfIndex, products.Count);

            if (productPoses.Count != products.Count)
                return false;

            ClearProducts(shelfIndex);

            GameObject shelf = shelves[shelfIndex];

            for (int i = 0; i < products.Count; i++)
            {
                ProductData product = products[i];
                Pose pose = productPoses[i];

                GameObject productObject = Instantiate(
                    product.ModelPrefab,
                    pose.position,
                    pose.rotation,
                    shelf.transform
                );

                productObject.name = $"{product.ProductName}_Shelf_{shelfIndex}_Product_{i}";
                spawnedProducts[shelfIndex].Add(productObject);
            }

            return true;
        }

        public List<Pose> GetProductPoses(int shelfIndex, int productCount)
        {
            List<Pose> poses = new List<Pose>();

            if (shelfIndex < 0 || shelfIndex >= shelves.Count)
            {
                Debug.LogError($"ShelfController: Shelf index {shelfIndex} is out of range.");
                return poses;
            }

            if (productCount <= 0)
            {
                Debug.LogError("ShelfController: Product count must be greater than zero.");
                return poses;
            }

            Transform shelf = shelves[shelfIndex].transform;

            Vector3 rowCenter = shelf.TransformPoint(productRowLocalOffset);
            Vector3 halfWidthOffset = shelf.forward * (ShelfWidth * 0.5f);

            Vector3 startPosition = rowCenter + halfWidthOffset;
            Vector3 endPosition = rowCenter - halfWidthOffset;
            Vector3 step = (endPosition - startPosition) / (productCount + 1);

            for (int i = 0; i < productCount; i++)
            {
                Vector3 position = startPosition + step * (i + 1);
                poses.Add(new Pose(position, shelf.rotation));
            }

            return poses;
        }

        public GameObject GetShelf(int index)
        {
            if (index < 0 || index >= shelves.Count)
                return null;

            return shelves[index];
        }

        public IReadOnlyList<GameObject> GetSpawnedProducts(int shelfIndex)
        {
            if (shelfIndex < 0 || shelfIndex >= spawnedProducts.Count)
                return null;

            return spawnedProducts[shelfIndex];
        }

        public void ClearProducts(int shelfIndex)
        {
            if (shelfIndex < 0 || shelfIndex >= spawnedProducts.Count)
                return;

            foreach (GameObject productObject in spawnedProducts[shelfIndex])
            {
                if (productObject != null)
                    Destroy(productObject);
            }

            spawnedProducts[shelfIndex].Clear();
        }

        public void ClearShelves()
        {
            foreach (GameObject shelf in shelves)
            {
                if (shelf != null)
                    Destroy(shelf);
            }

            shelves.Clear();
            spawnedProducts.Clear();
        }

        private GameObject GetRandomShelfPrefab()
        {
            int index = Random.Range(0, shelfPrefabs.Length);
            return shelfPrefabs[index];
        }
    }
}
