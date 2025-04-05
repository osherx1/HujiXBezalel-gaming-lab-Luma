using Managers;
using System;
using UnityEditor;
using UnityEngine;

namespace UI { 
    public class UIManager : MonoBehaviour 
    { 
        [SerializeField] private ImageSlider startScreen; 
        [SerializeField] private ImageSlider moonScreen;
        [SerializeField] private ImageSlider sunScreen;
        //   [SerializeField] private ImageSlider endScreen;
        [SerializeField] private GameManager gameManager; // Reference to GameManager to call ResetGame()
        //TODO: have a Image slider to the start and thr end and display them,he contact with the game Manger.

        private void Start()
        {
            StartCoroutine(startScreen.DisplayImages());
        }

        public void DisplayScreen(String screenName)
        {
            if (screenName == "startScreen")
            {
                StartCoroutine(startScreen.DisplayImages());
            }

            if (screenName == "moon")
            {
                StartCoroutine(moonScreen.DisplayImages());

            }

            if (screenName == "sun")
            {
                StartCoroutine(sunScreen.DisplayImages());
            }
        }

        public void ShowEndScreen()
        {
            //    StartCoroutine(endScreen.DisplayImages()); 
           
        }

        public void RemoveScreen(String screenName)
        {
            if (screenName == "start")
            {
                startScreen.RemoveAllImages();
               
            }
            else
            {
                //   endScreen.RemoveAllImages();
            }
        } 
        //TODO: button to each one.from the Game manager.
        public void Restart()
        {
            if (gameManager == null)
            {
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            } 
            RemoveScreen("end"); 
            gameManager.ResetGame();
        }
        //TODO: remove if we dont need this?
        public void Quit()
        {
            RemoveScreen("end");
        }
        
    } 
}