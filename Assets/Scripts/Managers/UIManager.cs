
using Managers;
using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers { 
    public class UIManager : MonoBehaviour 
    {
        private static readonly int Happy = Animator.StringToHash("Happy");
        [SerializeField] private ImageSlider startScreen;
        [SerializeField] private ImageSlider instructionsScreen;
        [SerializeField] private ImageSlider moonScreen;
        [SerializeField] private ImageSlider sunScreen;
        [SerializeField] private float fadeDuration = 0.01f;
        [SerializeField] private Image fadeImage;  // A fullscreen Image component instead of CanvasGroup

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
            else if (screenName == "moon")
            {
                StartCoroutine(LoadSceneWithFade("MoonScreen"));
            }
            else if (screenName == "sun")
            {
                StartCoroutine(LoadSceneWithFade("SunScreen"));
            }
        }

        private IEnumerator LoadSceneWithFade(string sceneName)
        {
            if (fadeImage != null)
            {
                yield return StartCoroutine(FadeOut());  // Fade out before changing scenes
            }

            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        private IEnumerator FadeIn()
        {
            float elapsedTime = 0f;
            Color imageColor = fadeImage.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                imageColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                fadeImage.color = imageColor;
                yield return null;
            }

            fadeImage.color = new Color(0f, 0f, 0f, 0f); // Fully transparent at the end
        }

        private IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            Color imageColor = fadeImage.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                imageColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                fadeImage.color = imageColor;
                yield return null;
            }

            fadeImage.color = new Color(0f, 0f, 0f, 1f); // Fully opaque at the end
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