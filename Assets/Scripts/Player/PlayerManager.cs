using System.Collections;
using System.Collections.Generic;
using Enums;
using FinishLine;
using Managers;
using Sky;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    /// <summary>
    /// Manages players in the game.
    /// Responsible for adding new players and placing them at predefined spawn points.
    /// Also manages game start and point tracking.
    /// </summary>
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerManager : MonoBehaviour
    {
        #region Game Configuration

        [Header("Player Settings")]
        [Tooltip("Maximum number of players allowed in the game.")]
        [SerializeField] private int maxPlayer = 2;

        [Header("Scoring Settings")]
        [Tooltip("Cooldown time between allowed point scoring (in seconds).")]
        [SerializeField] private float pointCooldown = 2f;

        [Tooltip("Number  of points required for a team to win.")]
        [FormerlySerializedAs("_pointsToWin")]
        [SerializeField] private int pointsToWin = 3;

        [Tooltip("UI area that visually displays team points.")]
        [SerializeField] private SlotsArea pointArea;

        [Header("Team Setup")]
        [Tooltip("Player data for the Sun team.")]
        [SerializeField] private PlayerDataSo sunDataSo;

        [Tooltip("Player data for the Moon team.")]
        [SerializeField] private PlayerDataSo moonDataSo;

        [Tooltip("Reference to the Sun team object.")]
        [SerializeField] private Team sunTeam;

        [Tooltip("Reference to the Moon team object.")]
        [SerializeField] private Team moonTeam;

        [Header("Game Start")]
        [Tooltip("Delay before the game starts once both teams are ready.")]
        [SerializeField] private float startGameDelay = 2f;

        [Tooltip("UI object shown when the game starts.")]
        [SerializeField] private GameObject startGameObject;

        #endregion

        #region Internal State

        /// <summary>
        /// Internal list of all joined players.
        /// </summary>
        private List<PlayerInput> _players;

        /// <summary>
        /// Reference to the Unity PlayerInputManager component.
        /// </summary>
        private PlayerInputManager _playerInputManager;

        private float _lastPointTimeSun = -Mathf.Infinity;
        private float _lastPointTimeMoon = -Mathf.Infinity;
        private bool _moonTeamReady;
        private bool _sunTeamReady;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _playerInputManager = gameObject.GetComponent<PlayerInputManager>();
            _players = new List<PlayerInput>();

            if (startGameObject != null)
            {
                startGameObject.SetActive(false);
            }
            else
            {
                Debug.Log("StartGameObject is not initialized");
            }
        }

        private void OnEnable()
        {
            _playerInputManager.onPlayerJoined += AddPlayer;
            Team.TeamReady += HandleTeamReady;
            PlayerController.TeamGetPoint += AddPoint;
        }

        private void OnDisable()
        {
            _playerInputManager.onPlayerJoined -= AddPlayer;
            Team.TeamReady -= HandleTeamReady;
            PlayerController.TeamGetPoint -= AddPoint;
        }

        #endregion

        #region Gameplay Logic

        private void AddPoint(TeamType teamType)
        {
            float currentTime = Time.time;

            if (teamType == TeamType.Moon)
            {
                if (currentTime - _lastPointTimeMoon < pointCooldown) return;
                _lastPointTimeMoon = currentTime;

                pointArea.FillNextSlot(teamType);
                moonTeam.AddPoint();

                if (moonTeam.GetPoint() >= pointsToWin)
                {
                    HandleVictory(teamType);
                }
            }

            if (teamType == TeamType.Sun)
            {
                if (currentTime - _lastPointTimeSun < pointCooldown) return;
                _lastPointTimeSun = currentTime;

                pointArea.FillNextSlot(teamType);
                sunTeam.AddPoint();

                if (sunTeam.GetPoint() >= pointsToWin)
                {
                    HandleVictory(teamType);
                }
            }
        }

        private void HandleVictory(TeamType teamType)
        {
            Debug.Log("Victory! " + teamType + " won!");
            GameManager.Instance.GameOver();
        }

        private void HandleTeamReady(TeamType teamType)
        {
            if (teamType == TeamType.Moon) _moonTeamReady = true;
            if (teamType == TeamType.Sun) _sunTeamReady = true;

            if (_moonTeamReady && _sunTeamReady)
            {
                StartCoroutine(StartGameWithDelay());
            }
        }

        private IEnumerator StartGameWithDelay()
        {
            Debug.Log($"Game will start in {startGameDelay} seconds...");
            yield return new WaitForSeconds(startGameDelay);
            StartTheGame();
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

        #endregion

        #region Player Management

        private void AddPlayer(PlayerInput obj)
        {
            _players.Add(obj);
            int index = _players.Count - 1;

            // Alternate team assignment: even -> Sun, odd -> Moon
            var teamType = index % 2 == 0 ? TeamType.Sun : TeamType.Moon;
            PlayerDataSo dataSo = teamType == TeamType.Sun ? sunDataSo : moonDataSo;

            if (teamType == TeamType.Sun)
            {
                sunTeam.AddPlayer(obj, dataSo);
            }
            else
            {
                moonTeam.AddPlayer(obj, dataSo);
            }
        }

        #endregion
    }
}



/*
using System.Collections;
using System.Collections.Generic;
using Enums;
using FinishLine;
using Managers;
using Sky;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    /// <summary>
    /// Manages players in the game.
    /// Responsible for adding new players and placing them at predefined spawn points.
    /// Also manages game start and point tracking.
    /// </summary>
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerManager : MonoBehaviour
    {
        /// <summary>
        /// List of all joined players.
        /// </summary>
        private List<PlayerInput> _players;

        [SerializeField] private int maxPlayer = 2;

        /// <summary>
        /// Manages player joining events from the input system.
        /// </summary>
        private PlayerInputManager _playerInputManager;

        // Cooldown between points to avoid spamming
        [SerializeField] private float pointCooldown = 2f;
        private float _lastPointTimeSun = -Mathf.Infinity;
        private float _lastPointTimeMoon = -Mathf.Infinity;

        // Player data and team references
        [SerializeField] private PlayerDataSo sunDataSo;
        [SerializeField] private PlayerDataSo moonDataSo;
        [SerializeField] private Team sunTeam;
        [SerializeField] private Team moonTeam;

        // Game start settings
        [SerializeField] private float startGameDelay = 2f;
        [SerializeField] private GameObject startGameObject;
        private bool _moonTeamReady;
        private bool _sunTeamReady;

        [FormerlySerializedAs("_pointsToWin")]
        [SerializeField] private int pointsToWin = 3;

        // UI component for tracking score visually
        [SerializeField] private SlotsArea pointArea;

        /// <summary>
        /// Initialize player list and input manager.
        /// </summary>
        private void Awake()
        {
            _playerInputManager = gameObject.GetComponent<PlayerInputManager>();
            _players = new List<PlayerInput>();

            // Disable start game object initially
            if (startGameObject != null)
            {
                startGameObject.SetActive(false);
            }
            else
            {
                Debug.Log("StartGameObject is not initialized");
            }
        }

        /// <summary>
        /// Subscribes to relevant events.
        /// </summary>
        private void OnEnable()
        {
            _playerInputManager.onPlayerJoined += AddPlayer;
            Team.TeamReady += HandleTeamReady;
            PlayerController.TeamGetPoint += AddPoint;
        }

        /// <summary>
        /// Unsubscribes from events to avoid memory leaks.
        /// </summary>
        private void OnDisable()
        {
            _playerInputManager.onPlayerJoined -= AddPlayer;
            Team.TeamReady -= HandleTeamReady;
            PlayerController.TeamGetPoint -= AddPoint;
        }

        /// <summary>
        /// Handles adding a point to the appropriate team, with cooldown and win check.
        /// </summary>
        private void AddPoint(TeamType teamType)
        {
            float currentTime = Time.time;

            if (teamType == TeamType.Moon)
            {
                // Prevent adding points too quickly
                if (currentTime - _lastPointTimeMoon < pointCooldown) return;

                _lastPointTimeMoon = currentTime;

                pointArea.FillNextSlot(teamType);
                moonTeam.AddPoint();

                if (moonTeam.GetPoint() >= pointsToWin)
                {
                    HandleVictory(teamType);
                }
            }

            if (teamType == TeamType.Sun)
            {
                if (currentTime - _lastPointTimeSun < pointCooldown) return;

                _lastPointTimeSun = currentTime;

                pointArea.FillNextSlot(teamType);
                sunTeam.AddPoint();

                if (sunTeam.GetPoint() >= pointsToWin)
                {
                    HandleVictory(teamType);
                }
            }
        }

        /// <summary>
        /// Called when a team reaches the required points to win.
        /// </summary>
        private void HandleVictory(TeamType teamType)
        {
            Debug.Log("Victory! " + teamType + " won!");
            GameManager.Instance.GameOver();
        }

        /// <summary>
        /// Handles readiness signal from teams and starts the game when both are ready.
        /// </summary>
        private void HandleTeamReady(TeamType teamType)
        {
            if (teamType == TeamType.Moon)
                _moonTeamReady = true;

            if (teamType == TeamType.Sun)
                _sunTeamReady = true;

            // Start game if both teams are ready
            if (_moonTeamReady && _sunTeamReady)
            {
                StartCoroutine(StartGameWithDelay());
            }
        }

        /// <summary>
        /// Actually starts the game by enabling the game object.
        /// </summary>
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

        /// <summary>
        /// Coroutine that delays the game start by a few seconds.
        /// </summary>
        private IEnumerator StartGameWithDelay()
        {
            Debug.Log($"Game will start in {startGameDelay} seconds...");
            yield return new WaitForSeconds(startGameDelay);
            StartTheGame();
        }

        /// <summary>
        /// Adds a new player when they join and assigns them to a team.
        /// </summary>
        private void AddPlayer(PlayerInput obj)
        {
            _players.Add(obj);
            int index = _players.Count - 1;

            // Alternate team assignment: even -> Sun, odd -> Moon
            var teamType = index % 2 == 0 ? TeamType.Sun : TeamType.Moon;
            PlayerDataSo dataSo = teamType == TeamType.Sun ? sunDataSo : moonDataSo;

            // Add player to appropriate team
            if (teamType == TeamType.Sun)
            {
                sunTeam.AddPlayer(obj, dataSo);
            }
            else
            {
                moonTeam.AddPlayer(obj, dataSo);
            }
        }
    }
}
*/


/*
using System.Collections;
using System.Collections.Generic;
using Enums;
using FinishLine;
using Managers;
using Sky;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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

        [SerializeField] private int maxPlayer = 2;

        /// <summary>
        /// Reference to the PlayerInputManager component,
        /// which is responsible for handling player joining.
        /// </summary>
        private PlayerInputManager _playerInputManager;

        // Cooldown settings
        [SerializeField] private float pointCooldown = 2f;
        private float _lastPointTimeSun = -Mathf.Infinity;
        private float _lastPointTimeMoon = -Mathf.Infinity;

        //TODO MOVE TO TEAM CLASS
        //[SerializeField] private List<Transform> startingPoints;
        [SerializeField] private PlayerDataSo sunDataSo;
        [SerializeField] private PlayerDataSo moonDataSo;
        [SerializeField] private Team sunTeam;
        [SerializeField] private Team moonTeam;

        [SerializeField] private float startGameDelay = 2f;
        [SerializeField] private GameObject startGameObject;
        private bool _moonTeamReady;
        private bool _sunTeamReady;

        [FormerlySerializedAs("_pointsToWin")] [SerializeField]
        private int pointsToWin = 3;

        [SerializeField] private SlotsArea pointArea;


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
            else
            {
                Debug.Log("StartGameObject is not initialized");
            }
        }

        /// <summary>
        /// Subscribes to the player joined event when the object becomes enabled.
        /// </summary>
        private void OnEnable()
        {
            _playerInputManager.onPlayerJoined += AddPlayer;
            Team.TeamReady += HandleTeamReady;
            PlayerController.TeamGetPoint += AddPoint;
        }

        /// <summary>
        /// Unsubscribes from the player joined event when the object is disabled.
        /// </summary>
        private void OnDisable()
        {
            _playerInputManager.onPlayerJoined -= AddPlayer;
            Team.TeamReady -= HandleTeamReady;
            PlayerController.TeamGetPoint -= AddPoint;
        }

        private void AddPoint(TeamType teamType)
        {
            float currentTime = Time.time;

            if (teamType == TeamType.Moon)
            {
                // Check cooldown for moon team
                if (currentTime - _lastPointTimeMoon < pointCooldown) return;
                _lastPointTimeMoon = currentTime;
                pointArea.FillNextSlot(teamType);
                moonTeam.AddPoint();
                if (moonTeam.GetPoint() >= pointsToWin)
                {
                    HandleVictory(teamType);
                }
            }

            if (teamType == TeamType.Sun)
            {
                // Check cooldown for sun team
                if (currentTime - _lastPointTimeSun < pointCooldown) return;
                _lastPointTimeSun = currentTime;
                pointArea.FillNextSlot(teamType);
                sunTeam.AddPoint();
                if (sunTeam.GetPoint() >= pointsToWin)
                {
                    HandleVictory(teamType);
                }
            }
        }

        private void HandleVictory(TeamType teamType)
        {
            Debug.Log("Victory! " + teamType+ " won!");
            //
            GameManager.Instance.GameOver();
        }

        /*
        private void AddPoint(TeamType teamType)
        {

            if (teamType == TeamType.Moon)
            {
                moonTeam.AddPoint();
                if (moonTeam.GetPoint()>= pointsToWin)
                {
                    print("sun team won!");

                }
            }

            if (teamType == TeamType.Sun)
            {
                sunTeam.AddPoint();
                if (moonTeam.GetPoint()>= pointsToWin)
                {
                    print("sun team won!");

                }
            }
        }
        #1#

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


        private void AddPlayer(PlayerInput obj)
        {
            _players.Add(obj);

            int index = _players.Count - 1;

            //obj.transform.position = startingPoints[index].position;
            var teamType = index % 2 == 0 ? TeamType.Sun : TeamType.Moon;
            PlayerDataSo dataSo = teamType == TeamType.Sun ? sunDataSo : moonDataSo;


            if (teamType == TeamType.Sun)
            {
                //if (sunTeam.) 
                sunTeam.AddPlayer(obj, dataSo);
            }
            else
            {
                moonTeam.AddPlayer(obj, dataSo);
            }
        }
    }
}*/