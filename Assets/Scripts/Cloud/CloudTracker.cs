using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for tracking the sequence of clouds the player steps on using a Stack.
/// Need to be a "CloudTracker" for each Player 
/// </summary>
[RequireComponent(typeof(Transform))]
public class CloudTracker : MonoBehaviour
{
    /// <summary>
    /// Reference to the player transform.
    /// </summary>
    [SerializeField] private Transform player;

    /// <summary>
    /// Fallback base (e.g., ground/platform) used if the stack is empty.
    /// </summary>
    [SerializeField] private Transform startingBase;

    /// <summary>
    /// Stack holding the history of clouds the player has stepped on.
    /// </summary>
    private Stack<Transform> _cloudHistory = new Stack<Transform>();

    /// <summary>
    /// Subscribes to the PlayerOnCloud event when this component is enabled.
    /// </summary>
    private void OnEnable()
    {
        Cloud.PlayerOnCloud += HandlePlayerOnCloud;
    }

    /// <summary>
    /// Unsubscribes from the PlayerOnCloud event when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        Cloud.PlayerOnCloud -= HandlePlayerOnCloud;
    }

    /// <summary>
    /// Handles the event triggered when the player steps on a cloud.
    /// Adds the cloud to the stack if it's not already the last one recorded.
    /// </summary>
    /// <param name="cloudTransform">Transform of the cloud stepped on.</param>
    private void HandlePlayerOnCloud(Transform cloudTransform)
    {
        if (_cloudHistory.Count == 0 || _cloudHistory.Peek() != cloudTransform)
        {
            _cloudHistory.Push(cloudTransform);
            Debug.Log($"Player stepped on cloud: {cloudTransform.name}");
        }
    }

    /// <summary>
    /// Returns the last cloud the player stepped on without removing it.
    /// If the stack is empty, returns the starting base.
    /// </summary>
    public Transform PeekLastCloud()
    {
        return _cloudHistory.Count > 0 ? _cloudHistory.Peek() : startingBase;
    }

    /// <summary>
    /// Removes and returns the last cloud from the stack.
    /// If the stack is empty, returns the starting base.
    /// </summary>
    public Transform PopLastCloud()
    {
        return _cloudHistory.Count > 0 ? _cloudHistory.Pop() : startingBase;
    }

    /// <summary>
    /// Clears the entire cloud history stack.
    /// </summary>
    public void ClearCloudHistory()
    {
        _cloudHistory.Clear();
    }

    /// <summary>
    /// Returns the number of clouds currently stored in the history stack.
    /// </summary>
    public int GetCloudCount()
    {
        return _cloudHistory.Count;
    }
}
