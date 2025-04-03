using Enums;
using UnityEngine;

namespace FinishLine
{
    /// <summary>
    /// Represents a slot on the finish line that can be filled with a team's color.
    /// </summary>
    public class FinishSlot : MonoBehaviour
    {
        /// <summary>
        /// Indicates whether this slot has already been filled.
        /// </summary>
        public bool IsFilled { get; private set; } = false;

        // Reference to the Renderer component of the slot
        private Renderer _renderer;

        // Color used when the Sun team fills this slot
        [SerializeField] private Color sunTeamColor = Color.yellow;

        // Color used when the Moon team fills this slot
        [SerializeField] private Color moonTeamColor = Color.blue;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes the Renderer reference.
        /// </summary>
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        /// <summary>
        /// Fills the slot with the color of the specified team.
        /// This can only be done once.
        /// </summary>
        /// <param name="teamType">The team that is filling this slot (Sun or Moon).</param>
        public void Fill(TeamType teamType)
        {
            // Prevents multiple fills
            if (IsFilled) return;

            // Choose the color based on the team type
            Color chosenColor = teamType == TeamType.Sun ? sunTeamColor : moonTeamColor;

            // Apply the chosen color to the material
            _renderer.material.color = chosenColor;

            // Mark the slot as filled
            IsFilled = true;
        }
    }
}