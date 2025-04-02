
using System;
using Enums;
using Player;
using UnityEngine;

namespace Sky
{
    public class FinishLine : MonoBehaviour
    {
        [SerializeField]private SpriteRenderer spriteRenderer;
        private bool[] isFull = new bool[5];
        [SerializeField]private GameObject[] points = new GameObject[5];
        
        private bool isTriggered = false;
        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            PlayerController.TeamGetPoint += AddPoint;

        }

        private void AddPoint(TeamType teamType)
        {
            if (isTriggered) return;
            isTriggered = true;
            Debug.Log("Team " + teamType + " gets a point");
            spriteRenderer.color = teamType == TeamType.Moon ? Color.blue : Color.yellow;
        }

        private void OnDisable()
        {
            PlayerController.TeamGetPoint -= AddPoint;
        }


        // public static event Action<TeamType> TeamGetPoint;

        /*private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var playerComponent = other.transform.GetComponent<PlayerComponent>();

                var teamType = playerComponent.Data.TeamType;
                TeamGetPoint?.Invoke(teamType);
            }
        }

        public static event Action<TeamType> TeamGetPoint;
    }*/
    }
}