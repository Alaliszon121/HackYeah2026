using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Pink Tax Game/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Run Settings")]
        public int numberOfSublevels = 6;
        public int productsPerRun = 10;
        public int sortModeProductCount = 4;

        [Header("Modes")]
        public List<ModeType> availableModes = new List<ModeType>();

        [Header("Scoring")]
        public int pointsPerSublevel = 100;

        [Tooltip("Score percentage thresholds used for the 1-5 star rating.")]
        public List<float> starThresholds = new List<float>();
    }
}
