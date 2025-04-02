using System.Collections;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private Collider2D attackCollider;
        [SerializeField] private float attackDuration = 0.2f;

        public float Getattack()
        {
            return attackDuration;
        }

        public void Attack()
        {
            Debug.Log("Using attack");
            StartCoroutine(PerformAttack());
            
        }

        private IEnumerator PerformAttack()
        {
            attackCollider.enabled = true;
            Debug.Log("Enable collider");
            yield return new WaitForSeconds(attackDuration);
            attackCollider.enabled = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Button"))
            {
                var trigger = other.GetComponent<Sky.Trigger>();
                if (trigger != null)
                {
                    trigger.ActivateTrigger();
                }
            }
   
        }


    }
}