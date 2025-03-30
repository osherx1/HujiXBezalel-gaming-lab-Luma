/*using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cloud
{
    public class Cloud3 : MonoBehaviour
    {
        public static event Action<Transform,TeamType> PlayerOnCloud;
        //public static event Action<Transform> PlayerOutFromCloud;
    

        [SerializeField] private float speed = 2.5f;
        [SerializeField] private MovementDirection moveHorizontally = MovementDirection.Up;
        [FormerlySerializedAs("color")] [SerializeField] private ColorType colorType;
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Transform endPosition;
        [SerializeField] private float vanishCooldown = 5f;
        [FormerlySerializedAs("playerType")] [SerializeField] private TeamType teamType;
    
    

        private Vector2 _direction;

        private void Start()
        {
            _direction = (moveHorizontally == MovementDirection.Up) ?  Vector2.up :Vector2.down;
        }

        private void Update()
        {
            transform.Translate((speed * Time.deltaTime)*_direction);
        }
    


        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                OnPlayerOnCloud();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
     
            if (other.gameObject.CompareTag("Walls"))
            {
                transform.position = startPosition;
            }
        }

        public enum MovementDirection
        {
            Up = 0,
            Down = 1
        }

        private void OnPlayerOnCloud()
        {
            print("player is on cloud at position " + transform.position);
            PlayerOnCloud?.Invoke(this.transform,teamType);
        }
    
    
        //TODO - Check if "PlayerOutFromCloud" event is necessary
        /*private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            OnPlayerOutFromCloud();
                  }
    }#1#
        /*private void OnPlayerOutFromCloud()
{
    PlayerOutFromCloud?.Invoke(this.transform);
}#1#

    }
}*/