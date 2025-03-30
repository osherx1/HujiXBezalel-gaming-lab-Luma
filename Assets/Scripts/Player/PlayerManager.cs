using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Manages players in the game.
    /// Responsible for adding new players and placing them at predefined spawn points.
    /// </summary>
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerManager : MonoBehaviour
    {
        /// <summary>
        /// List of all joined players.
        /// </summary>
        private List<PlayerInput> _players;
    

        /// <summary>
        /// List of starting positions for newly joined players.
        /// Each player is assigned a spawn point based on their order.
        /// </summary>

        [SerializeField] private int maxPlayer = 2;
        /// <summary>
        /// Reference to the PlayerInputManager component,
        /// which is responsible for handling player joining.
        /// </summary>
        private PlayerInputManager _playerInputManager;
        //TODO MOVE TO TEAM CLASS
        [SerializeField] private List<Transform> startingPoints;
        [SerializeField] private PlayerDataSo sunDataSo ;
        [SerializeField] private PlayerDataSo moonDataSo;
        [SerializeField] private Team sunTeam;
        [SerializeField] private Team moonTeam;
        
        [SerializeField] private float startGameDelay = 2f;
        [SerializeField] private GameObject startGameObject;
        private bool _moonTeamReady;
        private bool _sunTeamReady;


        /// <summary>
        /// Called when the object is first initialized.
        /// Initializes the player list and gets the PlayerInputManager component.
        /// </summary>
        private void Awake()
        {
            _playerInputManager = gameObject.GetComponent<PlayerInputManager>();
            _players = new List<PlayerInput>();
            if (startGameObject != null)
            {
                startGameObject.SetActive(false);
            }
            else {Debug.Log("StartGameObject is not initialized");}
            
            
        
        }

        /// <summary>
        /// Subscribes to the player joined event when the object becomes enabled.
        /// </summary>
        private void OnEnable()
        {
            _playerInputManager.onPlayerJoined += AddPlayer;
            Team.TeamReady += HandleTeamReady;
        }

        private void HandleTeamReady(TeamType teamType)
        {
            if (teamType == TeamType.Moon)
            {
                _moonTeamReady = true;
            }
            if (teamType == TeamType.Sun)
            {
                _sunTeamReady = true;
            }

            if (_moonTeamReady && _sunTeamReady)
            {
                StartCoroutine(StartGameWithDelay());
            }
        }
        private void StartTheGame()
        {
            if (startGameObject != null)
            {
                startGameObject.SetActive(true);
                Debug.Log("Game Started!");
            }
            else
            {
                Debug.LogWarning("StartGameObject is null!");
            }
        }
        
        private IEnumerator StartGameWithDelay()
        {
            Debug.Log($"Game will start in {startGameDelay} seconds...");
            yield return new WaitForSeconds(startGameDelay);
            StartTheGame();
        }


        /// <summary>
        /// Unsubscribes from the player joined event when the object is disabled.
        /// </summary>
        private void OnDisable()
        {
            _playerInputManager.onPlayerJoined -= AddPlayer;
        }

        private void AddPlayer(PlayerInput obj)
        {
            _players.Add(obj);

            int index = _players.Count - 1;
            if (index >= startingPoints.Count)
            {
                Debug.LogWarning("No starting point for player " + index);
                return;
            }

            obj.transform.position = startingPoints[index].position;

            var playerComponent = obj.GetComponent<PlayerComponent>();
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            PlayerController playerController = playerComponent.GetComponent<PlayerController>();
            var teamType = index % 2 == 0 ? TeamType.Sun : TeamType.Moon;
            var dataSo = teamType == TeamType.Sun ? sunDataSo : moonDataSo;
            if (playerComponent != null)
            {
                playerComponent.Initialize(_players.Count, dataSo, obj, startingPoints[index],spriteRenderer,playerController);
                playerComponent.PlaySpawnAnimation();
            }

            if (teamType == TeamType.Sun)
            {
                sunTeam.AddPlayer(playerComponent);
            }
            else
            {
                moonTeam.AddPlayer(playerComponent);
            }

            Debug.Log($"Player {teamType} spawned at {startingPoints[index].position}");
        }
    }
}