using System;
using Enums;
using UnityEngine;


namespace Player
{
    [CreateAssetMenu(fileName = "PlayerDataSo", menuName = "Scriptable Objects/PlayerDataSo")]
    [Serializable
    ]
    public class PlayerDataSo : ScriptableObject
    {
        [SerializeField] private Sprite playerIcon;
        [SerializeField] private TeamType teamType;
        [SerializeField] private Vector3 baseTeam;

        public Vector3 BaseTeam
        {
            get => baseTeam;
            set => baseTeam = value;
        }
        public TeamType TeamType
        {
            get => teamType;
            set => teamType = value;
        }

        public Sprite PlayerIcon
        {
            get => playerIcon;
            set => playerIcon = value;
        }
    }
}