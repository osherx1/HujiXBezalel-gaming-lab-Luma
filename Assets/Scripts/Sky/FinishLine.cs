using System;
using Enums;
using Player;
using UnityEngine;

namespace Sky
{
    public class FinishLine : MonoBehaviour
    {
        public static event Action<TeamType> TeamGetPoint;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var playerComponent = other.transform.GetComponent<PlayerComponent>();
                var teamType = playerComponent.Data.TeamType;
                TeamGetPoint?.Invoke(teamType);
            }
        }
    }
}