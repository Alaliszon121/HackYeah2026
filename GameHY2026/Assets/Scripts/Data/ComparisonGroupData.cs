using UnityEngine;

namespace PinkTaxGame
{
    [CreateAssetMenu(
        fileName = "ComparisonGroup",
        menuName = "Pink Tax Game/Comparison Group"
    )]
    public sealed class ComparisonGroupData : ScriptableObject
    {
        [SerializeField]
        private string displayName;

        public string DisplayName => displayName;
    }
}
