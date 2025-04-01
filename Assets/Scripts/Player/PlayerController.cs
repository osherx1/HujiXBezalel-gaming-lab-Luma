using System.Collections;
using Managers;
using Sky;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
        [SerializeField] private float speed = 2f;

       //private Transform startPosition;


        [Header("Die Animation")] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float flashDuration = 1f;
        [SerializeField] private float flashInterval = 0.2f;
        [SerializeField] private PlayerAttack playerAttack;


        [FormerlySerializedAs("_playerParent")] [SerializeField] private Transform playerParent;


        /// <summary>
        /// Distance the player moves during a jump.
        /// </summary>
        public float jumpDistance = 1.5f;

        /// <summary>
        /// Time it takes to complete a jump.
        /// </summary>
        public float jumpTime = 0.2f;

        //TODO DELETE SERIALIZE
        /*[SerializeField] */private CloudTracker _cloudTracker;


        // Input direction for movement
        private Vector2 _moveInput;

        // Rigidbody2D component for physics
        private Rigidbody2D _rb;

        // The direction in which the player will jump
        private Vector2 _directionToMove = Vector2.zero;

        // Indicates if the player is currently in a jump animation
        private bool _isJumping;
        private bool _isDie;
        private bool _isOnCloud = true;
        [FormerlySerializedAs("_collider")] [SerializeField] private Collider2D jumpingActionCollider;

        /// <summary>
        /// Called when the object is initialized.
        /// Grabs a reference to the Rigidbody2D component.
        /// </summary>
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _cloudTracker = new CloudTracker();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            /*
            SetStartingBase(startPosition);            
        */
        }

        private void Update()
        {
            CheckForPlayerDeath();
        }

        private void CheckForPlayerDeath()
        {
            if (!_isOnCloud && !_isJumping && !_isDie)
            {
                _isDie = true;
                transform.SetParent(playerParent,true);
                Die();
                Debug.Log("Player Die");
            }
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

        public void OnAttack(InputValue value)
        {
            playerAttack.Attack();
        }

        /// <summary>
        /// Input callback for jump.
        /// Initiates a jump in the direction the player is moving.
        /// </summary>
        /// <param name="value">The input value (unused, just triggers the action).</param>
        public void OnJump(InputValue value)
        {
            if (_directionToMove == Vector2.zero) return;
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
        IEnumerator JumpTo(Vector2 target)
        {
            _isJumping = true;
            jumpingActionCollider.enabled = false;

            Vector2 start = transform.position;
            float t = 0;
            float height = 0.3f; // שינוי קל בגובה מדומה

            while (t < jumpTime)
            {
                float progress = t / jumpTime;
                Vector2 horizontalPos = Vector2.Lerp(start, target, progress);

                // מוסיפים מעט קפיצה ויזואלית בציר Z או Y לפי סגנון
                float zBump = Mathf.Sin(Mathf.PI * progress) * height;

                transform.position = new Vector3(horizontalPos.x, horizontalPos.y + zBump, transform.position.z);

                t += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
            yield return new WaitForSeconds(0.05f);
            _isJumping = false;
            jumpingActionCollider.enabled = true;

            // אפקטים ויזואליים
            CameraShaker.Instance?.Shake(0.1f, 0.05f); // טלטול מצלמה קטן
            // אפשר גם להוסיף סאונד פה
        }

        /*
        /// <summary>
        /// Coroutine that handles the jump movement over time.
        /// </summary>
        /// <param name="target">The target position to jump to.</param>
        /// <returns>IEnumerator for coroutine handling.</returns>
        IEnumerator JumpTo(Vector2 target)
        {
            _isJumping = true;
            jumpingActionCollider.enabled = false;
            Vector2 start = transform.position;
            float t = 0;

            while (t < jumpTime)
            {
                transform.position = Vector2.Lerp(start, target, t / jumpTime);
                t += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
            yield return new WaitForSeconds(0.05f);
            _isJumping = false;
            jumpingActionCollider.enabled = true;
            /*if (!_isOnCloud)
            {
                Debug.Log("Player fell into the sky!");
                //Die();
            }
            else
            {
                Debug.Log("Player Detect a cloud");
            }#1#
        }*/


        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Cloud"))
            {
                    //transform.SetParent(playerParent, true);
                    _isOnCloud = false;
                    //Debug.Log("Player left all clouds"); 
            }                /*if (transform.parent != playerParent)
                {
                    transform.SetParent(playerParent);
                }*/
            
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Cloud"))
            {
                if (transform.parent != other.transform)
                {
                    transform.SetParent(other.transform,true);
                }
                _isOnCloud = true;
                HandlePlayerOnCloud(other);
            }

            if(other.CompareTag("FinishLine")){
                Debug.Log("Player get a point!");
                //GameManager.Instance.GameOver();
                var startingBase = _cloudTracker.GetStartingBase();
                _cloudTracker.ClearCloudHistory();
                _cloudTracker.PushCloud(startingBase);
                transform.position = startingBase.position;
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


        private void HandlePlayerOnCloud(Collider2D cloudCollider)
        {
            Transform t = cloudCollider.transform;
            Debug.Log("The player is on the cloud in Position " + t.position);
            _cloudTracker.PushCloud(t);
            //_cloudTracker.DebugPrintHistory();
        }

        public void SetStartingBase(Transform startingBase)
        {
            _cloudTracker ??= new CloudTracker();
            if(startingBase != null)
            {
                _cloudTracker.SetStartingBase(startingBase);
            }
        }


        private void Die()
        {
            StartCoroutine(HandleDeathSequence());
        }


        private IEnumerator HandleDeathSequence()
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
                    if (transform.parent != playerParent)
                    {
                        transform.SetParent(playerParent,true);
                    }
                    transform.position = t.position;
                    transform.SetParent(t,true);
                    yield return new WaitForSeconds(0.05f);
                    //TODO CHECK - 
                    _isDie = false;
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