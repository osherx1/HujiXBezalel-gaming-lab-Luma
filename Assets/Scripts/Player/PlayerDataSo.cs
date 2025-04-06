using System;
using Enums;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// A ScriptableObject that holds configuration data for a player,
    /// such as their team type, icon, and starting base position.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerDataSo", menuName = "Scriptable Objects/PlayerDataSo")]
    [Serializable]
    public class PlayerDataSo : ScriptableObject
    {
        /// <summary>
        /// The sprite icon representing the player in the game.
        /// </summary>
        [SerializeField] private Sprite playerIcon;

        /// <summary>
        /// 
        /// The team that this player belongs to (e.g., Sun or Moon).
        /// </summary>
        [SerializeField] private TeamType teamType;
        [SerializeField] private RuntimeAnimatorController animatorController; 

        [SerializeField] private string playerName;
        /// <summary>
        /// The default starting position (base) for the player, specific to their team.
        /// </summary>
        [SerializeField] private Vector3 baseTeam;

        
        /// <summary>
        /// /
        /// </summary>

        /// <summary>
        /// Gets or sets the animator controller.
        /// </summary>
        public RuntimeAnimatorController AnimatorController 
        {
            get => animatorController;
            set => animatorController = value;
        }
        
        //TODO - Add after getting the animations from the art - 
        /*/// <summary>
        /// Animator controller to assign to the player's Animator.
        /// </summary>
        [SerializeField] private RuntimeAnimatorController animatorController;*/

        /// <summary>
        /// Gets or sets the base (starting) position for the player.
        /// </summary>
        public Vector3 BaseTeam
        {
            get => baseTeam;
            set => baseTeam = value;
        }

        /// <summary>
        /// Gets or sets the player's team type.
        /// </summary>
        public TeamType TeamType
        {
            get => teamType;
            set => teamType = value;
        }

        /// <summary>
        /// Gets or sets the icon used to visually represent the player.
        /// </summary>
        public Sprite PlayerIcon
        {
            get => playerIcon;
            set => playerIcon = value;
        }
    }
}