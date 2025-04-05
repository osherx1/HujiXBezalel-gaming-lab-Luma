using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    [Serializable]
    public class Team
    {
        [FormerlySerializedAs("TeamType")] [SerializeField]
        private TeamType teamType;

        [FormerlySerializedAs("CurrentPoints")] [SerializeField]
        private int currentPoints = 0;

        [FormerlySerializedAs("_players")] [SerializeField]
        private List<PlayerComponent> players = new();

        [SerializeField] private int maxPlayer = 1;
        [SerializeField] private List<Transform> startingBases = new();


        public static event Action<TeamType> TeamReady;
        public static event Action<string> TheWinner;

        public void AddPlayer(PlayerInput obj, PlayerDataSo dataSo)
        {
            var playerComponent = obj.GetComponent<PlayerComponent>();

            if ((playerComponent != null) && (!players.Contains(playerComponent)))
            {
                players.Add(playerComponent);
                var index = players.Count - 1;

                var spriteRenderer = obj.GetComponent<SpriteRenderer>();
                var playerController = playerComponent.GetComponent<PlayerController>();
                if (index < startingBases.Count)
                {
                    playerComponent.Initialize(index, dataSo, obj, startingBases[index], spriteRenderer,
                        playerController);
                }

                Debug.Log($"Player added to team {teamType}. Total players: {players.Count}");
            }

            if (players.Count == maxPlayer)
            {
                TeamReady?.Invoke(teamType);
            }

            playerComponent.PlaySpawnAnimation();
        }

        public int GetPoint()
        {
            return currentPoints;
        }


        public void AddPoint()
        {
            Debug.Log("Team " + teamType + " get point");
            currentPoints++;
            if (currentPoints == 3)
            {
                TheWinner?.Invoke(teamType.ToString());
            }
            ReturnAllPlayersToBase();
        }


        /// <summary>
        /// Sends all players in the team back to their base with animation.
        /// </summary>
        private void ReturnAllPlayersToBase()
        {
            foreach (var player in players)
            {
                if (player != null)
                {
                    player.ReturnToBaseWithAnimation();
                }
            }
        }
    }
}