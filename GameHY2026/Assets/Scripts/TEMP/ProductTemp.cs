using UnityEngine;

namespace TEMP {
    public class ProductTemp : MonoBehaviour {
        [SerializeField] private float price;
        
        float GetPrice() {
            return price;
        }
    }
}
