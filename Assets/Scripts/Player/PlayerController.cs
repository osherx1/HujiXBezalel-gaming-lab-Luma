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

        /// <summary>
        /// Distance the player moves during a jump.
        /// </summary>
        public float jumpDistance = 1.5f;

        /// <summary>
        /// Time it takes to complete a jump.
        /// </summary>
        public float jumpTime = 0.2f;

        /// <summary>
        /// Layer mask to identify what counts as ground (currently unused).
        /// </summary>
        [SerializeField] public LayerMask groundLayer;

        // Input direction for movement
        private Vector2 _moveInput;

        // Rigidbody2D component for physics
        private Rigidbody2D _rb;

        // The direction in which the player will jump
        private Vector2 _directionToMove = Vector2.zero;

        // Indicates if the player is currently in a jump animation
        private bool _isJumping;


        /// <summary>
        /// Called when the object is initialized.
        /// Grabs a reference to the Rigidbody2D component.
        /// </summary>
        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
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
        }

        /// <summary>
        /// Physics update loop.
        /// Moves the player based on input direction.
        /// </summary>
        void FixedUpdate()
        {
            _rb.linearVelocity = _moveInput * speed;
        }
    }
}
