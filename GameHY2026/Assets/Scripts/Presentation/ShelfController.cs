using System;
using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    public class ShelfController : MonoBehaviour
    {
        [SerializeField] private List<ShelfSpot> shelves = new List<ShelfSpot>();

        public ShelfSpot GetShelf(int index)
        {
            throw new NotImplementedException();
        }

        public void PrepareShelf(ShelfSpot shelf, SublevelData sublevel)
        {
        }

        public void SpawnProducts(ShelfSpot shelf, List<ProductData> products)
        {
        }

        public void ClearShelf()
        {
        }

        public void EnableInteraction()
        {
        }

        public void DisableInteraction()
        {
        }
    }
}
