using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [Serializable] public class PlayerComponent : MonoBehaviour
    {
        private PlayerInput _playerInput;
        [SerializeField] public PlayerDataSo Data { get; set; }

        private int _playerNumber;
        private PlayerController _playerController;
        private SpriteRenderer _spriteRenderer; 
        

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
            if (_playerController != null) _playerController.SetStartingBase(startPosition);
            if (Data != null) Debug.Log($"Initialized player as {Data.TeamType}.");
            if (_spriteRenderer != null && Data.PlayerIcon != null)
            {
                Debug.Log("Changing Sprite Color.");
                _spriteRenderer.sprite = Data.PlayerIcon;
            }
            
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


   
    }
}