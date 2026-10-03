using UnityEngine;

namespace PinkTaxGame
{
    public abstract class GameMode : MonoBehaviour
    {
        protected GameManager gameManager;
        protected SublevelData currentSublevel;
        protected ShelfController shelfController;
        protected bool isCompleted;

        public bool IsCompleted => isCompleted;

        public virtual void Initialize(GameManager manager) {
            gameManager = manager;
            currentSublevel = manager.CurrentRun.sublevels[manager.CurrentRun.currentSublevelIndex];
            
        }
        public abstract void Setup(SublevelData sublevel);
        public abstract void Play();
        public abstract void Submit();
        public abstract ModeResult CalculateResult();
        public abstract ModeResult GetResult();
    }
}
