using Sky;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Controls the player character's movement and jumping behavior.
    /// Handles input events and movement physics.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Movement speed of the player.
        /// </summary>
        [SerializeField] private float speed = 5f;
        /*[SerializeField] */private Transform startPosition;


        [Header("Die Animation")]
        private SpriteRenderer _spriteRenderer;
       [SerializeField] private float flashDuration = 1.5f;
       [SerializeField] private float flashInterval = 0.2f;

        
        /// <summary>
        /// Distance the player moves during a jump.
        /// </summary>
        public float jumpDistance = 1.5f;

        /// <summary>
        /// Time it takes to complete a jump.
        /// </summary>
        public float jumpTime = 0.2f;

        private CloudTracker _cloudTracker;
        

        // Input direction for movement
        private Vector2 _moveInput;

        // Rigidbody2D component for physics
        private Rigidbody2D _rb;

        // The direction in which the player will jump
        private Vector2 _directionToMove = Vector2.zero;

        // Indicates if the player is currently in a jump animation
        private bool _isJumping;
        private bool _isOnCloud;


        /// <summary>
        /// Called when the object is initialized.
        /// Grabs a reference to the Rigidbody2D component.
        /// </summary>
        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _cloudTracker = new CloudTracker();
            _cloudTracker.SetStartingBase(startPosition);
            _spriteRenderer = GetComponent<SpriteRenderer>();

        }

        /// <summary>
        /// Input callback for movement.
        /// Called by the Input System when movement input is received.
        /// </summary>
        /// <param name="value">The 2D vector input from the player.</param>
        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
            _directionToMove = _moveInput;
        }

        /// <summary>
        /// Input callback for jump.
        /// Initiates a jump in the direction the player is moving.
        /// </summary>
        /// <param name="value">The input value (unused, just triggers the action).</param>
        public void OnJump(InputValue value)
        {
            Jump(_directionToMove);
        }

        /// <summary>
        /// Begins a jump action if not already jumping.
        /// </summary>
        /// <param name="dir">The direction to jump toward.</param>
        void Jump(Vector2 dir)
        {
            if (_isJumping) return;

            Vector2 target = (Vector2)transform.position + dir * jumpDistance;
            StartCoroutine(JumpTo(target));
        }

        /// <summary>
        /// Coroutine that handles the jump movement over time.
        /// </summary>
        /// <param name="target">The target position to jump to.</param>
        /// <returns>IEnumerator for coroutine handling.</returns>
        System.Collections.IEnumerator JumpTo(Vector2 target)
        {
            _isJumping = true;
            Vector2 start = transform.position;
            float t = 0;

            while (t < jumpTime)
            {
                transform.position = Vector2.Lerp(start, target, t / jumpTime);
                t += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
            _isJumping = false;
            yield return new WaitForSeconds(0.05f); 
            if (!_isOnCloud)
            {
                Debug.Log("Player fell into the sky!");
                //Die();
            }
            else
            {
                Debug.Log("Player Detect a cloud");
            }
        }



        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Cloud"))
            {
                _isOnCloud = false;
            }
        }
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Cloud"))
            {
                _isOnCloud = true;
                HandlePlayerOnCloud(other);
            }
        }

        /// <summary>
        /// Physics update loop.
        /// Moves the player based on input direction.
        /// </summary>
        void FixedUpdate()
        {
            _rb.linearVelocity = _moveInput * speed;
        }
        
        
        private void HandlePlayerOnCloud(Collider2D collider)
        {
            Transform t = collider.transform;
            Debug.Log("The player is on the cloud in Position " + t.position);
            _cloudTracker.PushCloud(t);
        }

        public void SetStartingBase(Transform startingBase)
        {
            if (_cloudTracker != null)
            {
                _cloudTracker.SetStartingBase(startingBase);

            }
        }

        
        private void Die()
        {
            StartCoroutine(HandleDeathSequence());
        }

        private System.Collections.IEnumerator HandleDeathSequence()
        {
            if (_cloudTracker != null)
            {
                Transform t = _cloudTracker.PopLastCloud();

                if (t != null)
                {
                    // Freeze movement
                    _moveInput = Vector2.zero;
                    _rb.linearVelocity = Vector2.zero;

                    // Flash red for 1.5 seconds (every 0.2s toggle)

                    float elapsed = 0f;

                    while (elapsed < flashDuration)
                    {
                        if (_spriteRenderer != null)
                        {
                            _spriteRenderer.color = Color.red;
                            yield return new WaitForSeconds(flashInterval / 2);
                            _spriteRenderer.color = Color.white;
                            yield return new WaitForSeconds(flashInterval / 2);
                        }
                        elapsed += flashInterval;
                    }
                    // Move to last cloud
                    transform.position = t.position;
                    Debug.Log("Player returned to " + t.position);
                    yield break;
                }

                Debug.Log("Cloud tracker is empty");
                yield break;
            }

            Debug.Log("Cloud tracker is null");
        }
    }
}
/*
private void Die()
{


    //Get last Cloud3 from Cloud3 Tracker -
    if (_cloudTracker != null)
    {
        Transform t = _cloudTracker.PopLastCloud();
        if (t != null)
        {
            transform.position = t.position;
            Debug.Log("Player Change is position to "+ t.position);
            return;
        }
        Debug.Log("Cloud3 tracker is Empty");
        return;
    }
    Debug.Log("Cloud3 tracker is Empty");



}
*/

