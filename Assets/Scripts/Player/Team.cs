using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    [Serializable] public class Team
    {
        [FormerlySerializedAs("TeamType")] [SerializeField] private TeamType teamType;
        public int CurrentPoints;
        [SerializeField] private List<PlayerComponent> Players = new();
        [SerializeField] private int maxPlayer = 1;
        [SerializeField] private List<Transform> startingBases;

        public static event Action<TeamType> TeamReady;

        public void AddPlayer(PlayerInput obj, PlayerDataSo dataSo)
        {
            var playerComponent = obj.GetComponent<PlayerComponent>();

            if (playerComponent != null&&!Players.Contains(playerComponent))
            {
                Players.Add(playerComponent);
                var index = Players.Count - 1;
                var spriteRenderer = obj.GetComponent<SpriteRenderer>();
                var playerController = playerComponent.GetComponent<PlayerController>();
                if (index < startingBases.Count)
                {
                    playerComponent.Initialize(index, dataSo, obj,startingBases[index],spriteRenderer,playerController);

                }
                
                Debug.Log($"Player added to team {teamType}. Total players: {Players.Count}");
            }

            if (Players.Count == maxPlayer)
            {
                TeamReady?.Invoke(teamType);
            }
            playerComponent.PlaySpawnAnimation();
        }

    
        public void AddPoint()
        {
            CurrentPoints++;
        }
    }
}


  

         
