using System;
using UnityEngine;
using UnityEngine.UI;
namespace UI 
{
    public class ImageSlider : MonoBehaviour 
    {
        public Image[] images; // Array of images to display public
        float delayBetweenSlides = 0.5f; // Delay before showing each image
     
        private void Start()
        {
            // Initially hide all images
       //     HideAllImages();
        }

        // This method will be called by UIManager to hide all images
        public void HideAllImages()
        {
            foreach (Image img in images)
            {
                img.enabled = false; // Disable all images initially
            }
        }

        public System.Collections.IEnumerator DisplayImages() 
        {
            float fadeDuration = 1f; // Duration of the fade-in effect

            for (int i = 0; i < images.Length; i++) 
            {
                // Ensure the image has a CanvasGroup component attached
                CanvasGroup canvasGroup = images[i].GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = images[i].gameObject.AddComponent<CanvasGroup>();
                }

                // Reset alpha and enable the image
                canvasGroup.alpha = 0;
                images[i].enabled = true;

                // Gradually increase alpha to 1 over the fadeDuration
                float elapsedTime = 0;
                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                    yield return null; // Wait for next frame
                }

                // Make sure the alpha is exactly 1 after the loop
                canvasGroup.alpha = 1;

                yield return new WaitForSeconds(delayBetweenSlides); // Wait before showing the next image
            }
        }

      // Remove all images at once
      public void RemoveAllImages() 
      { 
          foreach (Image img in images)
          {
              img.enabled = false;
          }
      } 
    } 
}