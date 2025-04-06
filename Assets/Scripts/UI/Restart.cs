using System;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace UI
{
        public class Restart : MonoBehaviour
        {
            private void Update()
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    SceneManager.LoadScene("GameLevel-WithoutSky");
                }
            }
            
        }
    
}