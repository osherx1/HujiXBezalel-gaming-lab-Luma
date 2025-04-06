using Managers;
using System;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers { 
    public class UIManager : MonoBehaviour 
    {
        private static readonly int Happy = Animator.StringToHash("Happy");
        [SerializeField] private ImageSlider startScreen;
        [SerializeField] private ImageSlider instructionsScreen;
        [SerializeField] private ImageSlider moonScreen;
        [SerializeField] private ImageSlider sunScreen;
      //  [SerializeField] private Animator sunAnimator;
      //  [SerializeField] private Animator moonAnimator;
      //  [SerializeField] private Animator sunAn;
        private void Start()
        {
         //   sunAn.SetTrigger(Happy);
            StartCoroutine(startScreen.DisplayImages());
        }

        public void DisplayScreen(String screenName)
        {
            if (screenName == "startScreen")
            {
                StartCoroutine(startScreen.DisplayImages());
            }

            if (screenName == "instructions")
            {
                StartCoroutine(instructionsScreen.DisplayImages());
            }

            if (screenName == "moon")
            {
                SceneManager.LoadScene("MoonScreen");
            }

            if (screenName == "sun")
            {
                SceneManager.LoadScene("SunScreen");
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
                    instructionsScreen.RemoveAllImages();
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