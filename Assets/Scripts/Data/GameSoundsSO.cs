using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using AudioType = Enums.AudioType;

namespace Data
{
    /// <summary>
    /// A ScriptableObject that maps AudioTypes to corresponding AudioClips.
    /// Used by the SoundManager to play sounds by logical type.
    /// </summary>
    [CreateAssetMenu(fileName = "GameSoundsSo", menuName = "Scriptable Objects/GameSoundsSo")]
    public class GameSoundsSo : ScriptableObject
    {
        /// <summary>
        /// List of game sound entries, each mapping an AudioType to an AudioClip.
        /// </summary>
        [FormerlySerializedAs("_gameSounds")]
        [SerializeField]
        private List<GameSound> gameSounds = new List<GameSound>();

        /// <summary>
        /// Internal dictionary for fast lookup of AudioClips by AudioType.
        /// </summary>
        private Dictionary<AudioType, GameSound> _soundsDict;

        /// <summary>
        /// Called when the ScriptableObject is loaded or reloaded.
        /// Initializes the dictionary from the serialized list.
        /// </summary>
        private void OnEnable()
        {
            if (_soundsDict == null || _soundsDict.Count == 0)
            {
                _soundsDict = new Dictionary<AudioType, GameSound>();
                foreach (var gameSound in gameSounds)
                {
                    _soundsDict[gameSound.audioType] = gameSound;
                }
            }
        }

        /// <summary>
        /// Retrieves the AudioClip associated with the given AudioType.
        /// </summary>
        /// <param name="audioType">The AudioType to look up.</param>
        /// <returns>The AudioClip if found; otherwise, null.</returns>
        public AudioClip GetClip(AudioType audioType)
        {
            return _soundsDict.TryGetValue(audioType, out GameSound gameSound) ? gameSound.clip : null;
        }

        /// <summary>
        /// Represents a mapping between an AudioType and a corresponding AudioClip.
        /// </summary>
        [Serializable]
        public class GameSound
        {
            /// <summary>
            /// The logical type of sound (e.g., GameStart, PlayerJump).
            /// </summary>
            [FormerlySerializedAs("AudioType")]
            public AudioType audioType;

            /// <summary>
            /// The audio clip to be played for this type.
            /// </summary>
            [FormerlySerializedAs("Clip")]
            public AudioClip clip;
        }

    }
    
}