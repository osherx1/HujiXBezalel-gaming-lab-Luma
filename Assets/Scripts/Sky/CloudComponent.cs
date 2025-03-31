using System;
using System.Collections;
using Enums;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sky
{
    public class CloudComponent : MonoBehaviour
    {
  
        public static event Action<Transform> PlayerOnCloud;

        [SerializeField] private float speed = 2.3f;
        [SerializeField] private MovementDirection moveDirection = MovementDirection.Up;
        [FormerlySerializedAs("colorType")] [SerializeField] private ColorType color;
        [SerializeField] private float vanishCooldown = 5f;
        [FormerlySerializedAs("_teamType")] [SerializeField] private TeamType teamType;


        private Vector2 _direction;
        private Rigidbody2D _rigidbody2D;
        private float _cloudHeight;

        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider2D;
        private bool _inTrigger ;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();

            _direction = (moveDirection == MovementDirection.Up) ? Vector2.up : Vector2.down;

            if (_rigidbody2D != null)
            {
                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic; // Make sure Rigidbody2D doesn't interfere with movement
            }
        
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider2D = GetComponent<Collider2D>();
            // Calculate the height of the cloud based on its Collider2D
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                _cloudHeight = collider.bounds.size.y;
            }
        }

        private void Update()
        {
            if (_rigidbody2D != null)
            {
                _rigidbody2D.MovePosition(_rigidbody2D.position + _direction * (speed * Time.deltaTime));
            }
            else
            {
                transform.Translate(_direction * (speed * Time.deltaTime));
            }
            Loop(moveDirection);
        }

        private void Loop(MovementDirection direction)
        {
            float screenTopLimit = 3.9f + _cloudHeight / 2;   // Add cloud height to fully disappear
            float screenBottomLimit = -3.9f - _cloudHeight / 2; // Subtract cloud height to fully disappear

            if (direction == MovementDirection.Down)
            {
                if (transform.position.y < screenBottomLimit)
                {
                    transform.position = new Vector3(transform.position.x, screenTopLimit, transform.position.z);
                }
            }

            if (direction == MovementDirection.Up)
            {
                if (transform.position.y > screenTopLimit)
                {
                    transform.position = new Vector3(transform.position.x, screenBottomLimit, transform.position.z);
                }
            }
        
        }
    
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                print("Player Hit");
                OnPlayerOnCloud();
            }
        }
    
        private void OnPlayerOnCloud()
        {
            PlayerOnCloud?.Invoke(this.transform);
        }

        public void SetColor(ColorType colorType)
        {
            this.color = colorType;
        }

        public ColorType GetColor()
        {
            return color;
        }

        public void SetTeamType(TeamType teamType)
        {
            this.teamType = teamType;
        }
    
        public void InTrigger()
        {
            _inTrigger = true;
        }
    
        private void OnEnable()
        {
            if (_inTrigger)
            {
                Trigger.Vanish += VanishCloud;

            }
        }

        private void OnDisable()
        {
            if (_inTrigger)
            {
                Trigger.Vanish -= VanishCloud;

            }
        }

        public void VanishCloud(ColorType colorType,TeamType teamType)
        {
            if ((this.teamType != teamType)&& (color == colorType))
            {
                StartCoroutine(VanishAndReappear());
            }
        }

        private IEnumerator VanishAndReappear()
        {
            // Disable rendering and collision
            _spriteRenderer.enabled = false;
            _collider2D.enabled = false;

            // Wait for vanishTime seconds
            yield return new WaitForSeconds(vanishCooldown);

            // Re-enable rendering and collision
            _spriteRenderer.enabled = true;
            _collider2D.enabled = true;
        }
    }
}