using Enums;
using UnityEngine;

namespace FinishLine
{
    /// <summary>
    /// Represents a slot on the finish line that can be filled with a team's image.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class FinishSlot : MonoBehaviour
    {
        /// <summary>
        /// Indicates whether this slot has already been filled.
        /// </summary>
        public bool IsFilled { get; private set; } = false;

        // Reference to the SpriteRenderer component of the slot
        private SpriteRenderer _spriteRenderer;

        // Sprite used when the Sun team fills this slot
        [SerializeField] private Sprite sunTeamSprite;

        // Sprite used when the Moon team fills this slot
        [SerializeField] private Sprite moonTeamSprite;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes the SpriteRenderer reference.
        /// </summary>
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Fills the slot with the image of the specified team.
        /// This can only be done once.
        /// </summary>
        /// <param name="teamType">The team that is filling this slot (Sun or Moon).</param>
        public void Fill(TeamType teamType)
        {
            // Prevents multiple fills
            if (IsFilled) return;

            // Choose the sprite based on the team type
            Sprite chosenSprite = teamType == TeamType.Sun ? sunTeamSprite : moonTeamSprite;

            // Apply the chosen sprite to the SpriteRenderer
            _spriteRenderer.sprite = chosenSprite;

            // Mark the slot as filled
            IsFilled = true;
        }
    }
}