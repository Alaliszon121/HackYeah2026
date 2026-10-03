using UnityEngine;

namespace PinkTaxGame
{
    public abstract class GameMode : MonoBehaviour
    {
        protected const int MaxPoints = 100;

        protected GameManager gameManager;
        protected SublevelData currentSublevel;

        public virtual void Initialize(GameManager manager)
        {
            gameManager = manager;
        }

        public virtual void Setup(SublevelData sublevel)
        {
            currentSublevel = sublevel;
        }

        public abstract void Play();
        public abstract void Submit();
    }
}
