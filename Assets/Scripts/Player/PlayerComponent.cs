using System;
using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Handles individual player configuration, appearance, animations, and base resetting logic.
    /// </summary>
    [Serializable]
    public class PlayerComponent : MonoBehaviour
    {
        /// <summary>
        /// The PlayerInput component used for handling controls.
        /// </summary>
        private PlayerInput _playerInput;

        /// <summary>
        /// ScriptableObject containing this player's team data, color, and icon.
        /// </summary>
        public PlayerDataSo Data { get; set; }

        /// <summary>
        /// Index number of the player (e.g., player 1, 2, etc.).
        /// </summary>
        private int _playerNumber;

        /// <summary>
        /// Reference to the PlayerController responsible for movement and actions.
        /// </summary>
        private PlayerController _playerController;

        /// <summary>
        /// The sprite renderer used to display the player's visual.
        /// </summary>
        private SpriteRenderer _spriteRenderer;

        /// <summary>
        /// (Optional) Reference to the player's base platform.
        /// </summary>
        private Transform _baseTransform;

        /// <summary>
        /// Called on game start. Attempts to find required components automatically.
        /// </summary>
        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            Debug.Log(_playerController != null ? "PlayerController found!" : "PlayerController not found.");
            Debug.Log(_spriteRenderer != null ? "SpriteRenderer found!" : "SpriteRenderer not found.");
        }

        /// <summary>
        /// Fully initializes the player with input, data, visuals, and starting location.
        /// </summary>
        /// <param name="playerNumber">The player's number (e.g., Player 1, Player 2).</param>
        /// <param name="data">ScriptableObject containing the player's team data.</param>
        /// <param name="playerInput">Input object used for controlling the player.</param>
        /// <param name="startPosition">Initial position to spawn the player at.</param>
        /// <param name="spriteRenderer">Sprite renderer for visuals.</param>
        /// <param name="playerController">Reference to the main PlayerController script.</param>
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

            if (Data != null)
                Debug.Log($"Initialized player as {Data.TeamType}.");

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