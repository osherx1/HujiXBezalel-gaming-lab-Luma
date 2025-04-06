using System;
using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Handles player-specific initialization including visuals, team data,
    /// and runtime animator controller assignment based on the ScriptableObject.
    /// </summary>
    [Serializable]
    public class PlayerComponent : MonoBehaviour
    {
        /// <summary>
        /// Reference to the player's input system.
        /// </summary>
        private PlayerInput _playerInput;

        /// <summary>
        /// ScriptableObject containing this player's configuration data.
        /// </summary>
        public PlayerDataSo Data { get; set; }

        /// <summary>
        /// The index or ID of the player (e.g., Player 1, Player 2).
        /// </summary>
        private int _playerNumber;

        /// <summary>
        /// Reference to the PlayerController handling movement and gameplay logic.
        /// </summary>
        private PlayerController _playerController;

        /// <summary>
        /// Reference to the SpriteRenderer to display the player's icon.
        /// </summary>
        private SpriteRenderer _spriteRenderer;
        
        
        private Animator _animator;
        /*
        /// <summary>
        /// Animator component responsible for playing animations.
        /// </summary>*/
        //TODO - Add after getting The animations from the art - 
        // private Animator _animator;

        /// <summary>
        /// Called when the object is initialized. Caches required components.
        /// </summary>
        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _spriteRenderer = GetComponent<SpriteRenderer>(); 
            _animator = GetComponent<Animator>();

            Debug.Log(_playerController != null ? "PlayerController found!" : "PlayerController not found.");
            Debug.Log(_spriteRenderer != null ? "SpriteRenderer found!" : "SpriteRenderer not found.");
            Debug.Log(_animator != null ? "Animator found!" : "Animator not found.");

            if (Data != null && _animator != null && Data.AnimatorController != null)
            {
                _animator.runtimeAnimatorController = Data.AnimatorController;  // ✅ Assign the AnimatorController
            }
            //TODO - Add after getting The animations from the art - 
            //Debug.Log(_animator != null ? "Animator found!" : "Animator not found.");
        }

        /// <summary>
        /// Initializes the player with the given settings and applies visual and animation configuration.
        /// </summary>
        /// <param name="playerNumber">The number assigned to the player.</param>
        /// <param name="data">The ScriptableObject holding team and visual data.</param>
        /// <param name="playerInput">The PlayerInput component associated with this player.</param>
        /// <param name="startPosition">The starting position of the player.</param>
        /// <param name="spriteRenderer">The sprite renderer to apply the icon.</param>
        /// <param name="playerController">The controller managing movement and actions.</param>
        public void Initialize(int playerNumber, PlayerDataSo data, PlayerInput playerInput, Transform startPosition,
            SpriteRenderer spriteRenderer, PlayerController playerController)
        {
            PlayerNumber = playerNumber;
            Data = data;
            _playerInput = playerInput;
            _playerController = playerController;
            _spriteRenderer = spriteRenderer;
            //TODO - Add after getting The animations from the art - 
            //_animator = GetComponent<Animator>();

            if (_playerController != null)
            {
                _playerController.SetStartingBase(startPosition);
                transform.position = startPosition.position;
            }

            //TODO - Add after getting The animations from the art - 
            if (Data != null && _animator != null)
            {
                Debug.Log($"Initialized player as {Data.TeamType}.");

                // Ensure the AnimatorController is assigned from the PlayerDataSo
                if (Data.AnimatorController != null)
                {
                    _animator.runtimeAnimatorController = Data.AnimatorController;  // ✅ Assign Animator Controller here
                }
            }

            if (_spriteRenderer != null && Data.PlayerIcon != null)
            {
                Debug.Log("Changing Sprite Icon.");
                _spriteRenderer.sprite = Data.PlayerIcon;
            }
        
            
            

            if (_spriteRenderer != null && Data.PlayerIcon != null)
            {
                Debug.Log("Changing Sprite Icon.");
                _spriteRenderer.sprite = Data.PlayerIcon;
            }
        }

        /// <summary>
        /// Returns the team type assigned to this player.
        /// </summary>
        /// <returns>The player's <see cref="TeamType"/>.</returns>
        public TeamType GetTeamType() => Data.TeamType;

        /// <summary>
        /// Sets the player's base position for resets or respawns.
        /// </summary>
        /// <param name="startPosition">The base position to assign.</param>
        public void SetBasePosition(Transform startPosition)
        {
            _playerController?.SetStartingBase(startPosition);
        }

        // Internal property for PlayerInput access
        private PlayerInput PlayerInput
        {
            get => _playerInput;
            set => _playerInput = value;
        }

        // Internal property for tracking the player's index
        private int PlayerNumber
        {
            get => _playerNumber;
            set => _playerNumber = value;
        }

        /// <summary>
        /// Plays the spawn animation when a player joins the game or respawns.
        /// Includes falling from the sky, blinking, and camera shake.
        /// </summary>
        public void PlaySpawnAnimation()
        {
            StartCoroutine(SpawnAnimationRoutine());
        }

        /// <summary>
        /// Coroutine that animates the player falling from above and blinking.
        /// </summary>
        private IEnumerator SpawnAnimationRoutine()
        {
            Vector3 startPos = transform.position + Vector3.up * 3f;
            Vector3 endPos = transform.position;
            float duration = 0.5f;
            float t = 0f;

            transform.position = startPos;

            while (t < duration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, t / duration);
                t += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;

            // Blink effect
            if (_spriteRenderer != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    _spriteRenderer.color = Color.clear;
                    yield return new WaitForSeconds(0.1f);
                    _spriteRenderer.color = Color.white;
                    yield return new WaitForSeconds(0.1f);
                }
            }

            CameraShaker.Instance.Shake(0.2f, 0.1f);
        }

        /// <summary>
        /// Teleports the player back to their base position with smooth transition and visual effects.
        /// </summary>
        public void ReturnToBaseWithAnimation()
        {
            if (_playerController != null)
            {
                StartCoroutine(ReturnToBaseRoutine(_playerController.GetStartingBase()));
            }
        }

        /// <summary>
        /// Coroutine that fades out the player, moves to the base, and fades back in.
        /// Includes a pop animation and camera shake.
        /// </summary>
        /// <param name="baseTransform">The world position to return to.</param>
        private IEnumerator ReturnToBaseRoutine(Vector3 baseTransform)
        {
            // Fade out
            if (_spriteRenderer != null)
            {
                for (float f = 1f; f >= 0; f -= 0.1f)
                {
                    _spriteRenderer.color = new Color(1, 1, 1, f);
                    yield return new WaitForSeconds(0.02f);
                }
            }

            // Temporarily disable movement
            _playerController?.SetInputEnabled(false);

            // Smooth movement
            Vector3 startPos = transform.position;
            Vector3 endPos = baseTransform;
            float duration = 0.5f;
            float t = 0f;

            while (t < duration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, t / duration);
                t += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;

            // Fade in
            if (_spriteRenderer != null)
            {
                for (float f = 0f; f <= 1f; f += 0.1f)
                {
                    _spriteRenderer.color = new Color(1, 1, 1, f);
                    yield return new WaitForSeconds(0.02f);
                }
            }

            // Pop scale effect
            Vector3 originalScale = transform.localScale;
            transform.localScale = originalScale * 1.3f;
            yield return new WaitForSeconds(0.1f);
            transform.localScale = originalScale;

            // Camera shake
            CameraShaker.Instance.Shake(0.2f, 0.1f);

            // Re-enable movement
            _playerController?.SetInputEnabled(true);
        }
    }
}


/*using System;
using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [Serializable] public class PlayerComponent : MonoBehaviour
    {
        private PlayerInput _playerInput;
        public PlayerDataSo Data { get; set; }
        private int _playerNumber;
        private PlayerController _playerController;
        private SpriteRenderer _spriteRenderer; 
        private Transform _baseTransform;
        
        

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            Debug.Log(_playerController != null ? "PlayerController found!" : "PlayerController not found.");
            Debug.Log(_spriteRenderer != null ? "SpriteRenderer found!" : "SpriteRenderer not found.");
        }

        public void Initialize(int playerNumber, PlayerDataSo data, PlayerInput playerInput, Transform startPosition,
            SpriteRenderer spriteRenderer, PlayerController playerController)
        {
            PlayerNumber = playerNumber;
            Data = data;
            _playerInput = playerInput;
            _playerController = playerController; 
            _spriteRenderer = spriteRenderer;
            if (_playerController != null)
            {
                _playerController.SetStartingBase(startPosition);
                transform.position = startPosition.position;
            }
            if (Data != null) Debug.Log($"Initialized player as {Data.TeamType}.");
            if (_spriteRenderer != null && Data.PlayerIcon != null)
            {
                Debug.Log("Changing Sprite Color.");
                _spriteRenderer.sprite = Data.PlayerIcon;
            }
            
        }

        public TeamType GetTeamType()
        {
            return Data.TeamType;
        }

        public void SetBasePosition(Transform startPosition)
        {
            _playerController.SetStartingBase(startPosition);
        }

        private PlayerInput PlayerInput
        {
            get => _playerInput;
            set => _playerInput = value;
        }
        private int PlayerNumber
        {
            get => _playerNumber;
            set => _playerNumber = value;
        }
        
        
        public void PlaySpawnAnimation()
        {
            StartCoroutine(SpawnAnimationRoutine());
        }

        private IEnumerator SpawnAnimationRoutine()
        {
            Vector3 startPos = transform.position + Vector3.up * 3f; // כאילו נופל מהשמיים
            Vector3 endPos = transform.position;
            float duration = 0.5f;
            float t = 0f;

            transform.position = startPos;

            while (t < duration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, t / duration);
                t += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;

            // הבהוב
            if (_spriteRenderer != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    _spriteRenderer.color = Color.clear;
                    yield return new WaitForSeconds(0.1f);
                    _spriteRenderer.color = Color.white;
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // Shake camera
            CameraShaker.Instance.Shake(0.2f, 0.1f);
        }
        
        
        public void ReturnToBaseWithAnimation()
        {
            if (_playerController != null)
            {
                StartCoroutine(ReturnToBaseRoutine(_playerController.GetStartingBase()));
            }
        }
        private IEnumerator ReturnToBaseRoutine(Vector3 baseTransform)
        {
            // Disappear effect
            if (_spriteRenderer != null)
            {
                for (float f = 1f; f >= 0; f -= 0.1f)
                {
                    _spriteRenderer.color = new Color(1, 1, 1, f);
                    yield return new WaitForSeconds(0.02f);
                }
            }

            // Optional: disable movement (if needed)
            if (_playerController != null)
                _playerController.SetInputEnabled(false);

            // Smooth move to base
            Vector3 startPos = transform.position;
            Vector3 endPos = baseTransform;
            float duration = 0.5f;
            float t = 0f;

            while (t < duration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, t / duration);
                t += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;

            // Appear effect
            if (_spriteRenderer != null)
            {
                for (float f = 0f; f <= 1f; f += 0.1f)
                {
                    _spriteRenderer.color = new Color(1, 1, 1, f);
                    yield return new WaitForSeconds(0.02f);
                }
            }

            // Optional: small pop animation
            Vector3 originalScale = transform.localScale;
            transform.localScale = originalScale * 1.3f;
            yield return new WaitForSeconds(0.1f);
            transform.localScale = originalScale;

            // Shake camera
            CameraShaker.Instance.Shake(0.2f, 0.1f);

            // Re-enable input
            if (_playerController != null)
                _playerController.SetInputEnabled(true);
        }



   
    }
}*/