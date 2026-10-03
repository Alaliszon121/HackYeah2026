using System.Collections.Generic;
using TEMP;
using UnityEngine;

namespace GameModes {
    public class GameModeProductSort : MonoBehaviour, IGameMode
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        public int Score { get; set; }
        
        public void StartGame() {
            throw new System.NotImplementedException();
        }

        public void CalculateScore() {
            throw new System.NotImplementedException();
        }

        public void UpdateScore() {
            throw new System.NotImplementedException();
        }

        public void EndGame() {
            throw new System.NotImplementedException();
        }

        public void GetScore() {
            throw new System.NotImplementedException();
        }

        public void UpdateGlobalScore() {
            throw new System.NotImplementedException();
        }
    }
}
