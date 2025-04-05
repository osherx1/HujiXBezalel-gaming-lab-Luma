using System;
using Enums;
using Player;
using Sky;
using UI;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        
        //[SerializeField] private Vector3 playerOneStartPosition;
      //  [SerializeField] private Vector3 playerTwoStartPosition;
    
        [SerializeField] private UIManager uIManager;
        [SerializeField] private PlayerManager playerManager;

     
        //public static event Action ResetPlayerPlace;


        // TODO: he start the start screen wite to the buyyen and then call the player Manager. 
        // when the player when he calls the game manger and the game manager call the ui manager to holld this.

        private Board _board;
        private void Start()
        {
            _board = GameObject.Find("Board").GetComponent<Board>();
            _board.gameObject.SetActive(false);
            uIManager.DisplayScreen("startScreen"); 
        }

        //add button.
        public void OnStartGameClicked()
        {
            uIManager.RemoveScreen("start");
            _board.gameObject.SetActive(true);
            
        }
        
        public void ResetPlayerPosition()
        {
            //ResetPlayerPlace?.Invoke();

        }

        
        

        //TODO:button.
        public void RestartGame()
        {
            
          //  uIManager.Restart();
        }


        private void OnEnable()
        {
            //PlayerHealth.OnPlayerDeath -= HandlePlayerDeath;
            Team.TheWinner += ShowTheWinner;
        }

        private void OnDisable()
        {
            Team.TheWinner -= ShowTheWinner;
            //PlayerHealth.OnPlayerDeath -= HandlePlayerDeath;

        }

        private void ShowTheWinner(string screenName)
        {
            _board.gameObject.SetActive(false);
            uIManager.DisplayScreen(screenName);
        }

        

        private void HandlePlayerDeath()
        {
            throw new NotImplementedException();
        }


        public void GameOver()
        {
            uIManager.Quit();
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

        public void ResetGame()
        {
            
        }
    }
}