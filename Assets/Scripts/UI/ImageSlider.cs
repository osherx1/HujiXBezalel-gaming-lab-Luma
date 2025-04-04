using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class ImageSlider : MonoBehaviour
    {
        public Image[] images; // Array of images to display
        public float delayBetweenSlides = 2f; // Delay before showing each image

        // Start is called before the first frame update
        private void Start()
        {
          
        }
        
        // Start displaying and removing images when called
        public System.Collections.IEnumerator DisplayImages()
        {
            // Show each image one by one at its original position
            for (int i = 0; i < images.Length; i++)
            {
                images[i].enabled = true; // Show image
                yield return new WaitForSeconds(delayBetweenSlides); // Wait before showing the next image
            }
        }

        public void RemoveAllImages()
        {
            // Remove all images at once
            foreach (Image img in images)
            {
                img.enabled = false;
            }
        }
    }
    
}