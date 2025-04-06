using System;
using System.Collections;
using Enums;
using Sky;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    /// <summary>
    /// Controls player input, movement, jumping, attack actions,
    /// death/reset behavior, and interaction with clouds and finish line.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Happy = Animator.StringToHash("Happy");
        private static readonly int Sad = Animator.StringToHash("Sad");
        private static readonly int Surprised = Animator.StringToHash("Surprised");

        /// <summary>
        /// Invoked when the player reaches the finish line to notify the game.
        /// </summary>
        public static event Action<TeamType> TeamGetPoint;

        /// <summary>
        /// Movement speed of the player.
        /// </summary>
        [SerializeField] private float speed = 2f;

        [Header("Die Animation")]
        
        //TODO: adding animation.
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private float flashDuration = 1f;
        [SerializeField] private float flashInterval = 0.2f;

        /// <summary>
        /// Component responsible for attack behavior.
        /// </summary>
        [SerializeField] private PlayerAttack playerAttack;

        /// <summary>
        /// Parent object to reset to upon death.
        /// </summary>
        [FormerlySerializedAs("_playerParent")]
        [SerializeField] private Transform playerParent;

        /// <summary>
        /// Distance the player jumps per action.
        /// </summary>
        public float jumpDistance = 1.5f;

        /// <summary>
        /// Time it takes to complete a jump.
        /// </summary>
        public float jumpTime = 0.2f;

        [SerializeField] private CloudTracker _cloudTracker = new();

        // --- Input and movement state ---
        private Vector2 _moveInput;
        private Vector2 _directionToMove = Vector2.zero;
        private Rigidbody2D _rb;
        [SerializeField]private Animator _animator;

        private bool _isJumping;
        private bool _isDie;
        private bool _isAttack;
        private bool _isOnCloud = true;
        private bool _inputEnabled = true;

        /// <summary>
        /// Collider used during jumps to prevent unwanted triggers.
        /// </summary>
        [FormerlySerializedAs("_collider")]
        [SerializeField] private Collider2D jumpingActionCollider;

        [FormerlySerializedAs("_playerComponent")] [SerializeField] private PlayerComponent playerComponent;

        private Vector3 _startPosition;

        /// <summary>
        /// Enables or disables player input.
        /// </summary>
        public void SetInputEnabled(bool enabled) => _inputEnabled = enabled;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
           _animator = GetComponent<Animator>();

            if (playerComponent != null && playerComponent.Data != null)
            {
                _animator.runtimeAnimatorController = playerComponent.Data.AnimatorController;
            }
        }

        private void Update()
        {
            CheckForPlayerDeath();
        }

        /// <summary>
        /// Checks whether the player is off a cloud and not jumping — triggers death.
        /// </summary>
        private void CheckForPlayerDeath()
        {
            if (!_isOnCloud && !_isJumping && !_isDie)
            {
                _isDie = true;
                transform.SetParent(playerParent, true);
                Die();
                Debug.Log("Player Die");
            }
        }

        /// <summary>
        /// Handles movement input.
        /// </summary>
        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
            _directionToMove = _moveInput;
        }

        /// <summary>
        /// Handles attack input.
        /// </summary>
        public void OnAttack(InputValue value)
        {
            _isAttack = true;
            playerAttack.Attack();
            _isAttack = false;
        }

        /// <summary>
        /// Handles jump input.
        /// </summary>
        public void OnJump(InputValue value)
        {
            if (_directionToMove == Vector2.zero) return;
            Jump(_directionToMove);
        }

        /// <summary>
        /// Starts a jump in the given direction.
        /// </summary>
        private void Jump(Vector2 dir)
        {
            if (_isJumping) return;

            Vector2 target = (Vector2)transform.position + dir * jumpDistance;
            StartCoroutine(JumpTo(target));
        }

        /// <summary>
        /// Coroutine for smooth jump movement with arc and slight camera shake.
        /// </summary>
        private IEnumerator JumpTo(Vector2 target)
        {
            _isJumping = true;
            jumpingActionCollider.enabled = false;

            Vector2 start = transform.position;
            float t = 0f;
            float height = 0.3f;

            while (t < jumpTime)
            {
                float progress = t / jumpTime;
                Vector2 horizontalPos = Vector2.Lerp(start, target, progress);
                float zBump = Mathf.Sin(Mathf.PI * progress) * height;

                transform.position = new Vector3(horizontalPos.x, horizontalPos.y + zBump, transform.position.z);
                t += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
            yield return new WaitForSeconds(0.05f);
            _isJumping = false;
            jumpingActionCollider.enabled = true;

            CameraShaker.Instance?.Shake(0.1f, 0.05f);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Cloud") && !_isAttack)
            {
                _isOnCloud = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Cloud"))
            {
                if (transform.parent != other.transform)
                {
                    transform.SetParent(other.transform, true);
                }

                _isOnCloud = true;
                HandlePlayerOnCloud(other);
            }

            if (other.CompareTag("FinishLine"))
            {
                Debug.Log("Player get a point!");
                TeamGetPoint?.Invoke(playerComponent.Data.TeamType);

                // Trigger Happy Animation for collecting a point
                PlayHappyAnimation();
                
                
            }
        }

        /// <summary>
        /// Resets the player to the last known good position on a cloud.
        /// </summary>
        private void ResetPlayer()
        {
            _isOnCloud = true;
            transform.position = _startPosition;
            _isJumping = false;
            _isDie = false;
            jumpingActionCollider.enabled = true;
            _cloudTracker.ClearCloudHistory();
        }

        /// <summary>
        /// Called every physics update. Applies velocity based on player input.
        /// </summary>
        private void FixedUpdate()
        {
            _rb.linearVelocity = _inputEnabled ? _moveInput * speed : Vector2.zero;
        }

        /// <summary>
        /// Adds the current cloud to the cloud tracker history.
        /// </summary>
        private void HandlePlayerOnCloud(Collider2D cloudCollider)
        {
            Transform t = cloudCollider.transform;
            Debug.Log("The player is on the cloud in Position " + t.position);
            _cloudTracker.PushCloud(t);
        }

        /// <summary>
        /// Sets the starting position of the player and initializes the cloud tracker.
        /// </summary>
        public void SetStartingBase(Transform startingBase)
        {
            if (_cloudTracker == null)
                _cloudTracker = new CloudTracker();

            _cloudTracker.SetStartingBase(startingBase);
            _startPosition = startingBase.position;
        }

        /// <summary>
        /// Begins the player’s death sequence.
        /// </summary>
        private void Die()
        {
            PlaySurprisedAnimation();
            //Transform t = _cloudTracker.PeekLastCloud();
            StartCoroutine(HandleDeathSequence());
        }
        

        /// <summary>
        /// Coroutine to handle the reset process after dying.
        /// </summary>
        private IEnumerator HandleDeathSequence()
        {
            if (_cloudTracker != null)
            {
                _rb.bodyType = RigidbodyType2D.Kinematic;
                _rb.simulated = false;

                Transform t = _cloudTracker.PopLastCloud();
                if (t != null)
                {
                    _moveInput = Vector2.zero;
                    _rb.linearVelocity = Vector2.zero;

                    if (transform.parent != playerParent)
                        transform.SetParent(playerParent, true);

                    transform.position = t.position;
                    transform.SetParent(t, true);

                    yield return new WaitForSeconds(0.05f);

                    _rb.simulated = true;
                    _rb.bodyType = RigidbodyType2D.Dynamic;

                    yield return new WaitForSeconds(0.05f);

                    _isDie = false;
                    Debug.Log("Player returned to " + t.position);
                    yield break;
                }

                Debug.Log("Cloud tracker is empty");
                _rb.simulated = true;
                _rb.bodyType = RigidbodyType2D.Dynamic;
                yield break;
            }

            Debug.Log("Cloud tracker is null");
        }

        /// <summary>
        /// Sets whether the player is currently in attack state.
        /// </summary>
        public void SetIsAttacking(bool isAttacking)
        {
            _isAttack = isAttacking;
        }

        /// <summary>
        /// Returns the player’s starting position.
        /// </summary>
        public Vector3 GetStartingBase()
        {
            return _startPosition;
        }
        
        public void PlayHappyAnimation()
        {
            _animator?.SetTrigger(Happy);
        }

        public void PlaySadAnimation()
        {
            _animator?.SetTrigger(Sad);
        }

        public void PlaySurprisedAnimation()
        {
            _animator?.SetTrigger(Surprised);
        }
        
    }
}
