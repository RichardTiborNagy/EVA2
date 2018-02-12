using System;
using System.Collections.Generic;
using System.IO;
using System.Data;

namespace Snake.Persistence {

    /// <summary>
    /// Text based data access.
    /// </summary>
    public class TextFilePersistence : IPersistence {

        /// <summary>
        /// Stores the size of the map.
        /// </summary>
        public int MapSize { get; private set; }

        /// <summary>
        /// Stores the coordinates of the walls.
        /// </summary>
        public List<Tuple<int, int>> Walls { get; private set; }

        /// <summary>
        /// Loads a level from a text file.
        /// </summary>
        /// <param name="path"></param>
        public void LoadLevel(string path) {
            if (path == null)
                throw new ArgumentNullException("path");
            Walls = new List<Tuple<int, int>>();
            try {
                using (
                    StreamReader reader = new StreamReader(path)) {

                    string[] numbers = reader.ReadToEnd().Split();
                    MapSize = int.Parse(numbers[0]);
                    if (MapSize < 6) throw new DataException();
                    for (int i = 1; i < numbers.Length; i = i + 2) {
                        Walls.Add(new Tuple<int, int>(int.Parse(numbers[i]), int.Parse(numbers[i + 1])));
                    }
                }
            } catch {
                throw new DataException();
            }
        }        
    }
}
