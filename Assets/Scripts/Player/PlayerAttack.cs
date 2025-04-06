using System.Collections;
using Managers;
using UnityEngine;
using AudioType = Enums.AudioType;

namespace Player
{
    /// <summary>
    /// Handles the player's attack behavior, including enabling the attack collider
    /// for a short duration and triggering interactions on contact.
    /// </summary>
    [System.Serializable]
    public class PlayerAttack : MonoBehaviour
    {
        /// <summary>
        /// The collider used to detect attack hits.
        /// </summary>
        [SerializeField] private Collider2D attackCollider;

        /// <summary>
        /// Duration for which the attack is active.
        /// </summary>
        [SerializeField] private float attackDuration = 0.2f;

        /// <summary>
        /// Reference to the player controller to set attack state.
        /// </summary>
        [SerializeField] private PlayerController playerController;

        /// <summary>
        /// Gets the current attack duration.
        /// </summary>
        /// <returns>The attack duration in seconds.</returns>
        public float Getattack()
        {
            return attackDuration;
        }

        /// <summary>
        /// Starts the attack sequence by triggering the coroutine.
        /// </summary>
        public void Attack()
        {
            Debug.Log("Using attack");
            StartCoroutine(PerformAttack());
        }

        /// <summary>
        /// Coroutine that handles the timing of enabling and disabling the attack collider.
        /// </summary>
        private IEnumerator PerformAttack()
        {
            //TODO - 
            SoundManager.Instance.PlaySoundByAudioType(AudioType.PlayerAttack);
            // Set player state to attacking
            playerController.SetIsAttacking(true);

            // Optional delay before activating collider
            yield return new WaitForSeconds(0.05f);

            // Enable the attack collider
            attackCollider.enabled = true;
            Debug.Log("Enable collider");

            // Keep the collider active for the attack duration
            yield return new WaitForSeconds(attackDuration);

            // Disable the attack collider
            attackCollider.enabled = false;

            // Optional delay before allowing next attack
            yield return new WaitForSeconds(0.05f);

            // Reset player attack state
            playerController.SetIsAttacking(false);
        }

        /// <summary>
        /// Called when the attack collider enters another collider marked as a trigger.
        /// Used to activate in-game buttons or other triggerable objects.
        /// </summary>
        /// <param name="other">The collider that was entered.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Button"))
            {
                // Attempt to find and activate the trigger component
                var trigger = other.GetComponent<Sky.Trigger>();
                if (trigger != null)
                {
                    trigger.ActivateTrigger();
                }
            }
        }
    }
}
