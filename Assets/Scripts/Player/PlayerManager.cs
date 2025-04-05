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
            _moonTeamReady = true;
         //   if (teamType == TeamType.Moon) _moonTeamReady = true;
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

