using System;

namespace Snake.Model {

    /// <summary>
    /// Arguments for map change events.
    /// </summary>
    public class MapChangedEventArgs : EventArgs {

        /// <summary>
        /// The size of the new map.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Constructor that sets the size of the new map.
        /// </summary>
        /// <param name="size">Map size</param>
        public MapChangedEventArgs(int size) {
            Size = size;
        }
    }
}