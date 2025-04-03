using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace FinishLine
{
    /// <summary>
    /// Manages a collection of finish slots that are filled as teams score points.
    /// </summary>
    public class SlotsArea : MonoBehaviour
    {
        /// <summary>
        /// List of finish slots to be filled when a team scores.
        /// </summary>
        [SerializeField] private List<FinishSlot> slots;

        /*
        /// <summary>
        /// Subscribes to the TeamGetPoint event when the object is enabled.
        /// </summary>
        private void OnEnable()
        {
            PlayerController.TeamGetPoint += FillNextSlot;
        }
        */

        /// <summary>
        /// Attempts to fill the next available slot for the specified team.
        /// Logs the result to the console.
        /// </summary>
        /// <param name="teamType">The team that scored a point (Sun or Moon).</param>
        public void FillNextSlot(TeamType teamType)
        {
            // Log whether a slot was successfully filled
            Debug.Log(TryFillNextAvailableSlot(teamType) ? "Slot filled" : "Slot not filled");
        }

        /*
        /// <summary>
        /// Unsubscribes from the TeamGetPoint event when the object is disabled.
        /// </summary>
        private void OnDisable()
        {
            PlayerController.TeamGetPoint -= FillNextSlot;
        }
        */

        /// <summary>
        /// Searches for the first available (unfilled) slot and fills it with the team's color.
        /// </summary>
        /// <param name="teamType">The team to assign to the next available slot.</param>
        /// <returns>True if a slot was successfully filled; false if all slots are already filled.</returns>
        public bool TryFillNextAvailableSlot(TeamType teamType)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsFilled)
                {
                    slot.Fill(teamType);
                    return true;
                }
            }

            // No available slots
            return false;
        }
    }
}
