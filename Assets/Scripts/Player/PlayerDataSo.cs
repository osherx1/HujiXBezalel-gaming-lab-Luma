using System;
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