using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class FadeIn : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 1.5f; // Duration for each fade-in
        [SerializeField] private bool fadeInOnStart = true; // Automatically start fade-in on scene load

        private void Start()
        {
            if (fadeInOnStart)
            {
                StartCoroutine(FadeInObjectsByLayer());
            }
        }

        private IEnumerator FadeInObjectsByLayer()
        {
            // Get all unique layers present in the scene
            HashSet<int> layersInScene = new HashSet<int>();
            Renderer[] renderers = FindObjectsOfType<Renderer>();
            CanvasGroup[] canvasGroups = FindObjectsOfType<CanvasGroup>();

            foreach (var renderer in renderers)
            {
                layersInScene.Add(renderer.gameObject.layer);
            }

            foreach (var canvasGroup in canvasGroups)
            {
                layersInScene.Add(canvasGroup.gameObject.layer);
            }

            // Sort layers from lowest to highest
            List<int> sortedLayers = new List<int>(layersInScene);
            sortedLayers.Sort();

            // Fade in objects layer by layer
            foreach (int layer in sortedLayers)
            {
                yield return StartCoroutine(FadeInLayer(layer));
            }
        }

        private IEnumerator FadeInLayer(int layer)
        {
            float elapsedTime = 0f;

            Renderer[] renderers = FindObjectsOfType<Renderer>();
            CanvasGroup[] canvasGroups = FindObjectsOfType<CanvasGroup>();

            // Collect objects in the current layer
            List<Renderer> layerRenderers = new List<Renderer>();
            List<CanvasGroup> layerCanvasGroups = new List<CanvasGroup>();

            foreach (Renderer renderer in renderers)
            {
                if (renderer.gameObject.layer == layer)
                {
                    layerRenderers.Add(renderer);
                }
            }

            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                if (canvasGroup.gameObject.layer == layer)
                {
                    layerCanvasGroups.Add(canvasGroup);
                    canvasGroup.alpha = 0f; // Ensure CanvasGroup starts fully transparent
                }
            }

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

                foreach (Renderer renderer in layerRenderers)
                {
                    if (renderer.material.HasProperty("_Color"))
                    {
                        Color color = renderer.material.color;
                        color.a = alpha;
                        renderer.material.color = color;
                    }
                }

                foreach (CanvasGroup canvasGroup in layerCanvasGroups)
                {
                    canvasGroup.alpha = alpha;
                }

                yield return null;
            }

            // Ensure all objects are fully visible at the end
            foreach (Renderer renderer in layerRenderers)
            {
                if (renderer.material.HasProperty("_Color"))
                {
                    Color color = renderer.material.color;
                    color.a = 1f;
                    renderer.material.color = color;
                }
            }

            foreach (CanvasGroup canvasGroup in layerCanvasGroups)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }
}
