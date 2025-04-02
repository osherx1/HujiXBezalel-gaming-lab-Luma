using System;
using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sky
{
    public class Trigger : MonoBehaviour
    {
        [Header("CloudComponent to follow")]
     
        // the associatedClouds why public?
        [SerializeField] private int state = 1;
        [SerializeField] private float vanishCooldown = 2f;

        [FormerlySerializedAs("triggerdColor")] [SerializeField] private ColorType triggerdColorType;
        // [SerializeField] public CloudComponent[] associatedClouds;
        public static event Action<ColorType,TeamType> Vanish;
        private Transform _cloudTransform;
        [FormerlySerializedAs("_teamType")] [SerializeField] private TeamType teamType;
        public void SetupCloud(Transform cloudTransform)
        {
            this._cloudTransform = cloudTransform;
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
            if (_cloudTransform != null)
            {
                transform.position = new Vector3(_cloudTransform.position.x,
                    _cloudTransform.position.y, _cloudTransform.position.z - 3);
            }
        }
    
        /*private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Trigger entered by player");
                ActivateTrigger();
            }
        }*/

    
        private IEnumerator ResetStateAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay); // Wait for the specified delay time
            state = 1; // Reset the state to 1
        }
        
        public void ActivateTrigger()
        {
            
            //if (state == 0) return;

            state = 0;
            Debug.Log("Trigger activated");
            Vanish?.Invoke(triggerdColorType, teamType);
            Debug.Log("Vanish Invoked");
            StartCoroutine(ResetStateAfterDelay(vanishCooldown));
        }


    }
}