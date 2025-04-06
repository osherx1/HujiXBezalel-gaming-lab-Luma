using Managers;
using System;
using UI;
using UnityEditor;
using UnityEngine;

namespace Managers { 
    public class UIManager : MonoBehaviour 
    { 
        [SerializeField] private ImageSlider startScreen; 
        [SerializeField] private ImageSlider moonScreen;
        [SerializeField] private ImageSlider sunScreen;

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
        

        public void RemoveScreen(String screenName)
        {
            if (screenName == "start")
            {
                startScreen.RemoveAllImages();
               
            }
            else
            {
                   sunScreen.RemoveAllImages();
                   moonScreen.RemoveAllImages();
            }
        } 
        public void Restart()
        {
            RemoveScreen("end"); 
        }
        
        
        public void Quit()
        {
            RemoveScreen("end");
        }
        
    } 
}