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
    }
}

/*using System;
using System.Collections;
using Enums;
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
        
        public static event Action<TeamType> TeamGetPoint;

        /// <summary>
        /// Movement speed of the player.
        /// </summary>
        [SerializeField] private float speed = 2f;
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
        [SerializeField] private CloudTracker _cloudTracker = new();


        // Input direction for movement
        private Vector2 _moveInput;

        // Rigidbody2D component for physics
        private Rigidbody2D _rb;

        // The direction in which the player will jump
        private Vector2 _directionToMove = Vector2.zero;

        // Indicates if the player is currently in a jump animation
        private bool _isJumping;
        private bool _isDie;
        private bool _isattack;
        private bool _isOnCloud = true;
        [FormerlySerializedAs("_collider")] [SerializeField] private Collider2D jumpingActionCollider;
        private Vector3 _startPosition;
        [SerializeField] private PlayerComponent _playerComponent;

        private bool _inputEnabled = true;

        public void SetInputEnabled(bool enabled)
        {
            _inputEnabled = enabled;
        }
        /// <summary>
        /// Called when the object is initialized.
        /// Grabs a reference to the Rigidbody2D component.
        /// </summary>
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>(); 
            _spriteRenderer = GetComponent<SpriteRenderer>();
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
            _isattack = true;
            playerAttack.Attack();
            _isattack = false;
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
            }#2#
        }#1#


        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Cloud") && !_isattack)
            {
                    //transform.SetParent(playerParent, true);
                    _isOnCloud = false;
                    //Debug.Log("Player left all clouds"); 
            }                /*if (transform.parent != playerParent)
                {
                    transform.SetParent(playerParent);
                }#1#
            
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
                //ResetPlayer();
                TeamGetPoint?.Invoke(_playerComponent.Data.TeamType);
                //GameManager.Instance.GameOver();
            }
        }
        
      

        

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
        /// Physics update loop.
        /// Moves the player based on input direction.
        /// </summary>
        void FixedUpdate()
        {
            if (_inputEnabled)
            {
                _rb.linearVelocity = _moveInput * speed;
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
            }        }


        private void HandlePlayerOnCloud(Collider2D cloudCollider)
        {
            Transform t = cloudCollider.transform;
            Debug.Log("The player is on the cloud in Position " + t.position);
            _cloudTracker.PushCloud(t);
            //_cloudTracker.DebugPrintHistory();
        }

        public void SetStartingBase(Transform startingBase)
        {
            if (_cloudTracker == null)
            {
                _cloudTracker = new CloudTracker();
            }
            _cloudTracker.SetStartingBase(startingBase);
            _startPosition = startingBase.position;
        }


        private void Die()
        {
            Transform t = _cloudTracker.PeekLastCloud();
            StartCoroutine(HandleDeathSequence());
            
            
        }

        private IEnumerator AnimateDeathSequence()
        {
            float elapsed = 0f;

            while (elapsed < flashDuration)
            {
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.color = Color.gray;
                    yield return new WaitForSeconds(flashInterval / 2);
                    _spriteRenderer.color = Color.white;
                    yield return new WaitForSeconds(flashInterval / 2);
                }

                elapsed += flashInterval;
            }
        }


        private IEnumerator HandleDeathSequence()
        {
            //jumpingActionCollider.enabled = false;

            if (_cloudTracker != null)
            {
                _rb.bodyType = RigidbodyType2D.Kinematic;
                _rb.simulated = false;
                Transform t = _cloudTracker.PopLastCloud();

                if (t != null)
                {
                    // Freeze movement
                    _moveInput = Vector2.zero;
                    _rb.linearVelocity = Vector2.zero;
                    
                    // Move to last cloud
                    if (transform.parent != playerParent)
                    {
                        transform.SetParent(playerParent,true);
                    }
                    transform.position = t.position;
                    transform.SetParent(t,true);
                    yield return new WaitForSeconds(0.05f);
                    
                    // Flash red for 1.5 seconds (every 0.2s toggle)

                   

                    yield return new WaitForSeconds(0.05f);
                    _rb.simulated = true;
                    _rb.bodyType = RigidbodyType2D.Dynamic;
                    yield return new WaitForSeconds(0.05f);
          
                    //TODO CHECK - 
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


        /*
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
        #1#


        public void SetIsAttacking(bool isAttacking)
        {
            _isattack = isAttacking;
        }

        public Vector3 GetStartingBase()
        {
            return _startPosition;
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
#1#*/