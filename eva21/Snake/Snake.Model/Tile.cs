namespace Snake.Model {

    /// <summary>
    /// A type that represents a tile on the game map.
    /// </summary>
    public class Tile {

        /// <summary>
        /// The type of the tile.
        /// </summary>
        public TileType Type { get; set; }

        /// <summary>
        /// X coordinate of the tile.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Y coordinate of the tile.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Constructor of a tile, it sets its coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Tile(int x, int y) {
            X = x;
            Y = y;
            Type = TileType.Empty;
        }
    }
}