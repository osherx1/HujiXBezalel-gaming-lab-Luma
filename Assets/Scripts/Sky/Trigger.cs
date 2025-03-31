using System;
using System.Collections;
using Enums;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sky
{
    public class Trigger : MonoBehaviour
    {
        [Header("CloudComponent to follow")]
     
        // the associatedClouds why public?
        [SerializeField] private int state = 1;
        [SerializeField] private float vanishCooldown = 5f;

        [FormerlySerializedAs("triggerdColor")] [SerializeField] private ColorType triggerdColorType;
        // [SerializeField] public CloudComponent[] associatedClouds;
        public static event Action<ColorType,TeamType> Vanish;
        private Transform cloudTransform;
        [FormerlySerializedAs("_teamType")] [SerializeField] private TeamType teamType;
        public void SetupCloud(Transform cloudTransform)
        {
            this.cloudTransform = cloudTransform;
        }
        public void SetupTeam(TeamType teamType)
        {
            this.teamType = teamType;
        }
    
        public void SetColor(ColorType colorType)
        {
            triggerdColorType = colorType;
        }
    
        private void Update()
        {
            if (cloudTransform != null)
            {
                transform.position = new Vector3(cloudTransform.position.x,
                    cloudTransform.position.y, cloudTransform.position.z - 3);
            }
        }
    
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") && state != 0)
            {
                state = 0; // Set the state to 0 to indicate it's triggered
        
                // Invoke the Vanish event with the color and team type
                Vanish?.Invoke(triggerdColorType, teamType);

                // Start the coroutine to reset the state after a delay
                StartCoroutine(ResetStateAfterDelay(vanishCooldown)); // Adjust the delay time (in seconds) as needed
            }
        }
    
        private IEnumerator ResetStateAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay); // Wait for the specified delay time
            state = 1; // Reset the state to 1
        }

    }
}