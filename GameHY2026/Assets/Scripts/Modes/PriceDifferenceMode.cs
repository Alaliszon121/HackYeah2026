using TMPro;
using UnityEngine;

namespace PinkTaxGame
{
    public class PriceDifferenceMode : GameMode
    {
        [SerializeField] private ProductData firstProduct;
        [SerializeField] private ProductData secondProduct;
        [SerializeField] private TMP_InputField playerInputZloty;
        [SerializeField] private TMP_InputField playerInputGrosze;
        
        private int playerGuessedDifference;
        private ModeResult currentResult;
        
        public override void Setup(SublevelData sublevel) { }
        public override void Play() { }

        public override void Submit() {
            CalculateResult();
        }

        public override void CalculateResult() {
            result = new ModeResult();
            result.modeType = ModeType.PriceDifference;
            
            playerGuessedDifference = int.Parse(playerInputZloty.text) * 100 + int.Parse(playerInputGrosze.text);
            result.correctAnswer = Mathf.Abs(secondProduct.pricePLN - firstProduct.pricePLN).ToString();
            result.playerAnswer = playerGuessedDifference.ToString();
        }

        public override ModeResult GetResult()
        {
            CalculateResult();
            return result;
        }
    }
}
