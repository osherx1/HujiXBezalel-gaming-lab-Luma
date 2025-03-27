using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// Manages players in the game.
/// Responsible for adding new players and placing them at predefined spawn points.
/// </summary>
[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// List of all joined players.
    /// </summary>
    private List<PlayerInput> _players;

    /// <summary>
    /// List of starting positions for newly joined players.
    /// Each player is assigned a spawn point based on their order.
    /// </summary>
    [FormerlySerializedAs("_startingPoints")] [SerializeField]
    private List<Transform> startingPoints;

    /// <summary>
    /// Reference to the PlayerInputManager component,
    /// which is responsible for handling player joining.
    /// </summary>
    private PlayerInputManager _playerInputManager;

    /// <summary>
    /// Called when the object is first initialized.
    /// Initializes the player list and gets the PlayerInputManager component.
    /// </summary>
    private void Awake()
    {
        _playerInputManager = gameObject.GetComponent<PlayerInputManager>();
        _players = new List<PlayerInput>();
    }

    /// <summary>
    /// Subscribes to the player joined event when the object becomes enabled.
    /// </summary>
    private void OnEnable()
    {
        _playerInputManager.onPlayerJoined += AddPlayer;
    }

    /// <summary>
    /// Unsubscribes from the player joined event when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        _playerInputManager.onPlayerJoined -= AddPlayer;
    }

    /// <summary>
    /// Adds a new player to the list and sets their starting position
    /// based on the order they joined.
    /// </summary>
    /// <param name="obj">The PlayerInput component of the newly joined player.</param>
    private void AddPlayer(PlayerInput obj)
    {
        _players.Add(obj);

        // If there are enough starting points, assign one to the player
        if (startingPoints.Count >= _players.Count)
        {
            obj.gameObject.transform.position = startingPoints[_players.Count - 1].position;
            Debug.Log("StartingPoint position: " + startingPoints[_players.Count - 1].position);
        }
    }
}