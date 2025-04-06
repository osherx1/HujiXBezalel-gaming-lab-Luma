using System;
using Enums;
using Player;
using Sky;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        
        //[SerializeField] private Vector3 playerOneStartPosition;
      //  [SerializeField] private Vector3 playerTwoStartPosition;
    
        [SerializeField] private UIManager uIManager;
        [SerializeField] private PlayerManager playerManager;
        private bool check = false;
     
        //public static event Action ResetPlayerPlace;
        private PlayerInputManager _playerInputManager;

        private Board _board;
        private void Start()
        {
            _playerInputManager = FindObjectOfType<PlayerInputManager>();

            if (_playerInputManager != null)
            {
                _playerInputManager.DisableJoining(); // Disable player joining at the start
                Debug.Log("PlayerInputManager: Joining is disabled at the start.");
            }
            else
            {
                Debug.LogError("PlayerInputManager not found in the scene!");
            }
            _board = GameObject.Find("Board").GetComponent<Board>();//
            _board.gameObject.SetActive(false);//
            uIManager.DisplayScreen("startScreen");
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Debug.Log("Enter Key Pressed");
                OnStartGameClicked();
            }
        }
        public void OnStartGameClicked()
        {
            uIManager.RemoveScreen("start");
           
            if (check == false)
            {
                uIManager.DisplayScreen("instructions"); 
                check = true;
            }
            if (_playerInputManager != null)
            {
                _playerInputManager.EnableJoining(); // Allow players to join now
                Debug.Log("PlayerInputManager: Joining is now enabled.");
            }
            else
            {
                Debug.LogWarning("PlayerInputManager not found or is not properly initialized.");
            }
            _board.gameObject.SetActive(true);
            
        }

        private void OnEnable()
        {
            //PlayerHealth.OnPlayerDeath -= HandlePlayerDeath;
            PlayerManager.TheWinner += ShowTheWinner;
            PlayerManager.RemoveInstructions += RemoveInstructions;
        }
        
        private void OnDisable()
        {
            PlayerManager.TheWinner -= ShowTheWinner;
            PlayerManager.RemoveInstructions -= RemoveInstructions;

            //PlayerHealth.OnPlayerDeath -= HandlePlayerDeath;

        }
        private void RemoveInstructions()
        {
            uIManager.RemoveScreen("instructions");
        }
        
        private void ShowTheWinner(string screenName)
        {
            _board.gameObject.SetActive(false);
            uIManager.DisplayScreen(screenName);
        }

        public void ResetGame()
        {
            uIManager.Restart();
            _board.gameObject.SetActive(true);/////////////////
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
        
        
        public void ResetPlayerPosition()
        {
            //ResetPlayerPlace?.Invoke();

        }
    }
}