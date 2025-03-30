using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

[Serializable] public class Team
{
    [SerializeField] private TeamType TeamType;
    [SerializeField] private int currentPoints { get; set; }
    [SerializeField] private List<PlayerComponent> Players;
    [SerializeField]  private int maxPlayer = 1;
    public static event Action<TeamType> TeamReady;

    public void AddPlayer(PlayerComponent player)
    {
        if (!Players.Contains(player))
        {
            Players.Add(player);
            Debug.Log($"Player added to team {TeamType}. Total players: {Players.Count}");
        }

        if (Players.Count == maxPlayer)
        {
            TeamReady?.Invoke(TeamType);
        }
    }

    public void AddPoint()
    {
        currentPoints++;
    }
}