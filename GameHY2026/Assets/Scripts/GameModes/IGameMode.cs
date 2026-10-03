using System.Collections.Generic;
using TEMP;

namespace GameModes {
    public interface IGameMode {
        List<ProductTemp> Products {
            set {
                if (ProductListTemp.Instance != null) {
                    Products =  ProductListTemp.Instance.Products;
                };
            }
        }
        int Score { get; set; }
        
        void StartGame();
        void CalculateScore();
        void UpdateScore();
        void EndGame();
        void GetScore();
        void UpdateGlobalScore();
    }
}
