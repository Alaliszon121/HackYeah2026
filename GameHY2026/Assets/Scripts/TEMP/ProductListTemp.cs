using System;
using System.Collections.Generic;
using UnityEngine;

namespace TEMP {
    public class ProductListTemp : MonoBehaviour
    {
        public static ProductListTemp Instance;
        public List<ProductTemp> Products = new List<ProductTemp>();

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            } else {
                Instance = this;
            }
        }
    }
}
