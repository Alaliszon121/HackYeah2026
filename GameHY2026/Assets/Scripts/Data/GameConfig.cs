using System;
using System.Collections.Generic;
using UnityEngine;

namespace PinkTaxGame
{
    [Serializable]
    public struct ModeWeight
    {
        public ModeType mode;

        [Min(0)]
        public int weight;
    }

    [CreateAssetMenu(fileName = "GameConfig", menuName = "Pink Tax Game/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        [Header("Run")]
        [Min(1)]
        [SerializeField] private int numberOfSublevels = 6;

        [Min(1)]
        [SerializeField] private int productsPerRun = 10;

        [Header("Sort Mode")]
        [Min(2)]
        [SerializeField] private int sortModeProductCount = 4;

        [Header("Mode Distribution")]
        [SerializeField] private List<ModeWeight> modeWeights = new List<ModeWeight>();

        [Header("Stars")]
        [Tooltip("Minimum score percentage required for 1, 2, 3, 4 and 5 stars.")]
        [SerializeField] private float[] starThresholds =
        {
            0.20f,
            0.40f,
            0.60f,
            0.80f,
            0.95f
        };

        public int NumberOfSublevels => numberOfSublevels;
        public int ProductsPerRun => productsPerRun;
        public int SortModeProductCount => sortModeProductCount;
        public IReadOnlyList<ModeWeight> ModeWeights => modeWeights;
        public IReadOnlyList<float> StarThresholds => starThresholds;
    }
}
