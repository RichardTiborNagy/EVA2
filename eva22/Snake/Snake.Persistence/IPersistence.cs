using System;
using System.Collections.Generic;

namespace Snake.Persistence {

    /// <summary>
    /// Interface for data access
    /// </summary>
    public interface IPersistence {

        /// <summary>
        /// Gets the size of the loaded map.
        /// </summary>
        int MapSize { get; }

        /// <summary>
        /// Gets the locations of the walls.
        /// </summary>
        List<Tuple<int, int>> Walls { get; }

        /// <summary>
        /// Loads a map from a specified text file.
        /// </summary>
        /// <param name="path"></param>
        void LoadLevel(string path);
    }
}