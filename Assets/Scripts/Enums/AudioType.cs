namespace Enums
{
    /// <summary>
    /// Enumeration of different sound types used in the game.
    /// </summary>
    public enum AudioType
    {
        // General game events
        /// <summary>No sound / default value.</summary>
        None = -1,
        /// <summary>Sound for game start.</summary>
        GameStart = 0,
        /// <summary>Sound for game over screen.</summary>
        GameOver = 1,
        /// <summary>Sound for victory screen.</summary>
        VictoryScreen = 2,

        // Player-related actions
        /// <summary>Sound for player movement.</summary>
        PlayerMove = 10,
        /// <summary>Sound for player jump.</summary>
        PlayerJump = 11,
        /// <summary>Sound for player death.</summary>
        PlayerDie = 12,
    }
}