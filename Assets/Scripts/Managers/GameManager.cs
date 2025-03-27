using System;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        
        [SerializeField] private Vector3 playerOneStartPosition;
        [SerializeField] private Vector3 playerTwoStartPosition;
    
        //public static event Action ResetPlayerPlace;


        public void ResetPlayerPosition()
        {
            //ResetPlayerPlace?.Invoke();

        }



    
        public void RestartGame()
        {
            throw new NotImplementedException();
        }


        private void OnEnable()
        {
            //PlayerHealth.OnPlayerDeath -= HandlePlayerDeath;

        }


        private void OnDisable()
        {
            //PlayerHealth.OnPlayerDeath -= HandlePlayerDeath;

        }

        private void HandlePlayerDeath()
        {
            throw new NotImplementedException();
        }


        public void GameOver()
        {
            QuitGame();
        }
    
        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}