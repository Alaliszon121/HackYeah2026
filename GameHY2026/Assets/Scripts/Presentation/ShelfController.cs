using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    public class ShelfController : MonoBehaviour
    {
        [Header("Shelf Setup")]
        [SerializeField] private GameObject[] shelfPrefabs;

        [Tooltip("Distance between shelf origins. Should match the common shelf width.")]
        [Min(0.01f)]
        [SerializeField] private float shelfSpacing = 4f;

        private readonly List<GameObject> shelves = new List<GameObject>();

        public int ShelfCount => shelves.Count;

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

                Vector3 position =
                    transform.position +
                    transform.forward * shelfSpacing * -i;

                GameObject shelf = Instantiate(
                    prefab,
                    position,
                    transform.rotation,
                    transform
                );

                shelves.Add(shelf);
            }

            return true;
        }

        public GameObject GetShelf(int index)
        {
            if (index < 0 || index >= shelves.Count)
                return null;

            return shelves[index];
        }

        public void ClearShelves()
        {
            foreach (GameObject shelf in shelves)
            {
                if (shelf != null)
                    Destroy(shelf);
            }

            shelves.Clear();
        }

        private GameObject GetRandomShelfPrefab()
        {
            int index = Random.Range(0, shelfPrefabs.Length);
            return shelfPrefabs[index];
        }
    }
}