using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sky
{
    /// <summary>
    /// Tracks the sequence of clouds a single player steps on using a fixed-size LinkedList (FIFO).
    /// Should be attached to each player individually.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    [Serializable]
    public class CloudTracker
    {
        private const int MaxClouds = 5;
        private LinkedList<Transform> _cloudHistory = new LinkedList<Transform>();

        [SerializeField] private Transform startingBase;

        
        /// <summary>
        /// Adds a new cloud to the history. If the history is full, removes the oldest entry.
        /// Ignores duplicates if the latest cloud is the same as the new one.
        /// </summary>
        /// <param name="cloudTransform">The cloud the player just stepped on.</param>
        public void PushCloud(Transform cloudTransform)
        {
            if (_cloudHistory.Count == 0 || _cloudHistory.Last.Value != cloudTransform)
            {
                if (_cloudHistory.Count >= MaxClouds)
                {
                    _cloudHistory.RemoveFirst();
                }

                _cloudHistory.AddLast(cloudTransform);
            }
        }

        /// <summary>
        /// Returns the most recent cloud the player stepped on.
        /// </summary>
        /// <returns>The latest cloud transform or the starting base if empty.</returns>
        public Transform PeekLastCloud()
        {
            return _cloudHistory.Count > 0 ? _cloudHistory.Last.Value : startingBase;
        }

        /// <summary>
        /// Removes and returns the most recent cloud from the history.
        /// </summary>
        /// <returns>The last cloud transform or the starting base if empty.</returns>
        public Transform PopLastCloud()
        {
            if (_cloudHistory.Count > 0)
            {
                Transform last = _cloudHistory.Last.Value;
                _cloudHistory.RemoveLast();
                return last;
            }

            return startingBase;
        }

        /// <summary>
        /// Clears all cloud history.
        /// </summary>
        public void ClearCloudHistory()
        {
            _cloudHistory.Clear();
        }

        /// <summary>
        /// Returns the current number of clouds in the history.
        /// </summary>
        /// <returns>The number of clouds tracked.</returns>
        public int GetCloudCount()
        {
            return _cloudHistory.Count;
        }

        /// <summary>
        /// Sets the initial base transform for fallback.
        /// </summary>
        /// <param name="baseTransform">Transform to use as default.</param>
        public void SetStartingBase(Transform baseTransform)
        {
            startingBase = baseTransform;
        }
        public Transform GetStartingBase()
        {
            return startingBase;
        }

        /// <summary>
        /// Returns a list of all cloud transforms in the history (from oldest to newest).
        /// </summary>
        /// <returns>List of cloud transforms.</returns>
        public List<Transform> GetCloudHistory()
        {
            return new List<Transform>(_cloudHistory);
        }

        /// <summary>
        /// Logs the full cloud history to the console for debugging purposes.
        /// </summary>
        public void DebugPrintHistory()
        {
            Debug.Log("--- Cloud History ---");
            int i = 1;
            foreach (var cloud in _cloudHistory)
            {
                Debug.Log($"{i++}. {cloud.name}");
            }
        }
        
    }
} 




/*using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks the sequence of clouds a single player steps on using a Stack.
/// Should be attached to each player individually.
/// </summary>
[RequireComponent(typeof(Transform))]
[Serializable]
public class CloudTracker
{
     private Stack<Transform> _cloudHistory = new Stack<Transform>();
    
    [SerializeField] private Transform startingBase;

    public void PushCloud(Transform cloudTransform)
    {
        if (_cloudHistory.Count == 0 || _cloudHistory.Peek() != cloudTransform)
        {
            _cloudHistory.Push(cloudTransform);
            /*
            Debug.Log($"Player {gameObject.name} stepped on cloud: {cloudTransform.name}");
        #1#
        }
    }

    public Transform PeekLastCloud()
    {
        return _cloudHistory.Count > 0 ? _cloudHistory.Peek() : startingBase;
    }

    public Transform PopLastCloud()
    {
        return _cloudHistory.Count > 0 ? _cloudHistory.Pop() : startingBase;
    }

    public void ClearCloudHistory()
    {
        _cloudHistory.Clear();
    }

    public int GetCloudCount()
    {
        return _cloudHistory.Count;
    }

    public void SetStartingBase(Transform baseTransform)
    {
        startingBase = baseTransform;
    }
}*/