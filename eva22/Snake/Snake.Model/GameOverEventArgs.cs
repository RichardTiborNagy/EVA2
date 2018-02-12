using System;

namespace Snake.Model {

    /// <summary>
    /// Arguments for game over events.
    /// </summary>
    public class GameOverEventArgs : EventArgs {
        
        /// <summary>
        /// The achieved game score.
        /// </summary>
        public int Score { get; private set; }
        
        /// <summary>
        /// Constructor that sets the score.
        /// </summary>
        /// <param name="score"></param>
        public GameOverEventArgs(int score) {
            Score = score;
        }

    }
}
