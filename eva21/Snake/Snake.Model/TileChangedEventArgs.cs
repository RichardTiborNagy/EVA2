using System;

namespace Snake.Model {

    /// <summary>
    /// Arguments for tile change events.
    /// </summary>
    public class TileChangedEventArgs : EventArgs {

        /// <summary>
        /// The X coordinate of the changed tile.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The Y coordinate of the changed tile.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// The new type of the changed tile.
        /// </summary>
        public TileType Type { get; private set; }

        /// <summary>
        /// Constructor that sets the coordinates and the new type of the tile.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        public TileChangedEventArgs(int x, int y, TileType type) {
            X = x;
            Y = y;
            Type = type;
        }
    }
}
