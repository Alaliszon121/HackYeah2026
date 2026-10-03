using TMPro;
using UnityEngine;

namespace PinkTaxGame
{
    public class GuessPriceMode : GameMode
    {
        [SerializeField] private ProductData currentProduct;
        [SerializeField] private TMP_InputField playerInputZloty;
        [SerializeField] private TMP_InputField playerInputGrosze;
        
        private int playerGuessPrice;

        public void SetPlayerGuessedPrice(float price) { }
        public override void Setup(SublevelData sublevel) { }
        public override void Play() { }

        public override void Submit() {
            CalculateResult();
        }

        public override void CalculateResult() {
            result = new ModeResult();
            result.modeType = ModeType.GuessPrice;
            
            playerGuessPrice = int.Parse(playerInputZloty.text) * 100 + int.Parse(playerInputGrosze.text);
            result.playerAnswer = playerGuessPrice.ToString();
            result.correctAnswer = currentProduct.pricePLN.ToString();
            
            Debug.Log("Calculating result: " + playerGuessPrice);
            
        }

        public override ModeResult GetResult()
        {
            CalculateResult();
            return result;
        }
    }
}
