using Managers;

using System;
using UnityEditor;
using UnityEngine;

namespace UI {
   

        public class UIManager : MonoBehaviour
        {
            [SerializeField] private GameObject showEndScreen;
            [SerializeField] private GameManager gameManager; // Reference to GameManager to call ResetGame()

            //TODO: have a Image slider to the start and thr end and display them,he contact with the game Manger.
            private void Start()
            {
                showEndScreen.SetActive(false);
            }
        
            
            public void ShowEndScreen()
            {
                showEndScreen.SetActive(true);
            }

 
            
            
            //TODO: button to each one.
            public void OnRestartButtonPressed()
            {
                if (gameManager == null)
                {
                    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                }
                gameManager.ResetGame();
            
            } 

            //TODO: remove if we dont need this?
            public void OnQuitButtonPressed()
            {
                if (gameManager == null)
                {
                    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                }
                gameManager.GameOver();
            }
          
        }
    
}