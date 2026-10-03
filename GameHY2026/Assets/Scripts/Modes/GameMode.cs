using UnityEngine;

namespace PinkTaxGame
{
    public abstract class GameMode : MonoBehaviour
    {
        protected GameManager gameManager;
        protected SublevelData currentSublevel;
        protected bool isCompleted;

        public virtual void Initialize(GameManager manager)
        {
        }

        public abstract void Setup(SublevelData sublevel);

        public abstract void Play();

        public abstract void Submit();

        public abstract ModeResult CalculateResult();

        public abstract ModeResult GetResult();
    }
}
