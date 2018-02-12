using Snake.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Timers;

namespace Snake.Model {

    /// <summary>
    /// The type of the game model.
    /// </summary>
    public class SnakeModel {

        #region Private Fields

        private Tile[,] map;                    //A 2D array of Tiles represents the game map.
        private Tile head;                      //A reference to the tile that contains the head of the snake.
        private Queue<Tile> tail;               //A FIFO collection that contains the rest of the snake.
        private Direction direction;            //The direction the snake is currently facing.
        private bool changedDirection;          //Tells if the snakes direction has been changed since its last move.
        private Timer timer;                    //A timer that moves the snake.
        private const double speed = 150d;      //The interval of the timer.
        private IPersistence persistence;       //Reference to the persistence.

        #endregion

        #region Private Properties

        /// <summary>
        /// Returns the tile that's in front of the snake.
        /// </summary>
        private Tile PreviewTile {
            get {
                try {
                    switch (Direction) {
                        case Direction.Up:
                            return map[head.X - 1, head.Y];
                        case Direction.Down:
                            return map[head.X + 1, head.Y];
                        case Direction.Left:
                            return map[head.X, head.Y - 1];
                        case Direction.Right:
                            return map[head.X, head.Y + 1];
                        default:
                            return null;
                    }
                } catch {
                    return null;
                }
            }
        }

        #endregion

        #region Public Properties 

        /// <summary>
        /// Tells if the game is currently paused.
        /// </summary>
        public bool Paused {
            get {
                if (timer != null)
                    return !timer.Enabled;
                else
                    return true;
            }
        }

        /// <summary>
        /// Gets and sets the direction of the snake.
        /// Only one change per move cycle is allowed.
        /// 180 turns are not allowed.
        /// </summary>
        public Direction Direction {
            get {
                return direction;
            }
            set {
                if (changedDirection) return;
                if (direction == Direction.Up && value == Direction.Down ||
                    direction == Direction.Down && value == Direction.Up ||
                    direction == Direction.Left && value == Direction.Right ||
                    direction == Direction.Right && value == Direction.Left
                    ) return;
                else {
                    changedDirection = true;
                    direction = value;
                }
            }
        }

        /// <summary>
        /// Gets the size of the current map.
        /// </summary>
        public int MapSize { get { return map.GetLength(0); } }

        /// <summary>
        /// Calculates the game score from the length of the snake.
        /// </summary>
        public int Score { get { return tail.Count - 4; } }

        /// <summary>
        /// Returns the type of a tile at specific coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TileType this[int x, int y] {
            get {
                return map[x, y].Type;
            }
        }

        #endregion

        #region Events

        public event EventHandler<MapChangedEventArgs> MapChanged;          //The event for a map change.
        public event EventHandler<TileChangedEventArgs> TileChanged;        //The event for a tile change.
        public event EventHandler<GameOverEventArgs> GameOver;              //The event for game over.

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor that sets the persistence.
        /// </summary>
        /// <param name="dataAccess"></param>
        public SnakeModel(IPersistence dataAccess) {
            persistence = dataAccess;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pauses or resumes the game, depending on the parameter.
        /// </summary>
        /// <param name="pause"></param>
        public void Pause(bool pause) {
            if (pause) timer?.Stop();
            else timer?.Start();
        }
                
        /// <summary>
        /// Restarts the current level, setting everything to its original state.
        /// </summary>
        public void RestartLevel() {
            //Clear all the snake and food tiles.
            foreach (Tile tile in map) {
                if (tile.Type == TileType.Snake || tile.Type == TileType.Food) {
                    tile.Type = TileType.Empty;
                    OnTileChanged(tile);
                }
            }
            //Put a new snake in the middle of the map, facing right.
            int middle = map.GetLength(0) / 2;
            tail = new Queue<Tile>();
            for (int i = middle - 2; i < middle + 2; i++) {
                Tile tile = map[middle, i];
                tile.Type = TileType.Snake;
                tail.Enqueue(tile);
                OnTileChanged(tile);
            }
            map[middle, middle + 2].Type = TileType.Snake;
            head = map[middle, middle + 2];
            OnTileChanged(head);
            direction = Direction.Right;
            //Spawn a new food.
            SpawnFood();
            //Start the game.
            Pause(false);
        }

        /// <summary>
        /// Load a map from a text file.
        /// </summary>
        /// <param name="path"></param>
        public void LoadLevel(string path) {
            if (persistence == null) return;
            try {
                persistence.LoadLevel(path);
            } catch (DataException) {
                throw new DataException();
            }
            NewGame(persistence.MapSize, persistence.Walls);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Start a new game.
        /// </summary>
        /// <param name="n">The size of the map</param>
        /// <param name="walls">A list of wall coordinates.</param>
        private void NewGame(int n, List<Tuple<int, int>> walls) {
            //Pause the game.
            Pause(true);
            //Create a new timer, hook up its events.
            timer = new Timer(speed);
            timer.Elapsed += new ElapsedEventHandler(OnTick);
            //Create a new map, fill it up with tiles.
            map = new Tile[n, n];
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    map[i, j] = new Tile(i, j);
                }
            }
            OnMapChanged(n);
            //Place the walls down.
            foreach (Tuple<int, int> coords in walls) {
                map[coords.Item1, coords.Item2].Type = TileType.Wall;
            }
            //Put down the snake.
            int middle = n / 2;
            tail = new Queue<Tile>();
            for (int i = middle - 2; i < middle + 2; i++) {
                map[middle, i].Type = TileType.Snake;
                tail.Enqueue(map[middle, i]);
            }
            map[middle, middle + 2].Type = TileType.Snake;
            head = map[middle, middle + 2];
            direction = Direction.Right;
            //Spawn the first food.
            SpawnFood();
            foreach (Tile tile in map) {
                OnTileChanged(tile);
            }
            //Start the game.
            Pause(false);
        }

        /// <summary>
        /// Moves the snake, handles collisions.
        /// </summary>
        protected void MoveSnake() {
            //Get the tile in front of the snake.
            Tile destination = PreviewTile;
            changedDirection = false;
            if (destination == null || destination.Type == TileType.Snake || destination.Type == TileType.Wall) {
                //The snake hit the edge of the map, a wall, or itself. Game over.
                OnGameOver();
            } else if (destination.Type == TileType.Food) {
                //The snake ate a food, it grows and moves. A new food is spawned.
                tail.Enqueue(head);
                head = destination;
                head.Type = TileType.Snake;
                SpawnFood();
                OnTileChanged(head);
            } else if (destination.Type == TileType.Empty) {
                //The snake moved to an empty tile.
                tail.Enqueue(head);
                head = destination;
                head.Type = TileType.Snake;
                Tile removed = tail.Dequeue();
                removed.Type = TileType.Empty;
                OnTileChanged(head);
                OnTileChanged(removed);
            }
        }

        /// <summary>
        /// Spawns a new food to and empty tile, if there is one.
        /// </summary>
        protected void SpawnFood() {
            var r = new Random();
            List<Tile> validTiles = new List<Tile>();
            foreach (Tile tile in map) {
                if (tile.Type == TileType.Empty)
                    validTiles.Add(tile);
            }
            if (validTiles.Any()) {
                Tile chosenTile = validTiles[r.Next(0, validTiles.Count)];
                chosenTile.Type = TileType.Food;
                OnTileChanged(chosenTile);
            } else {
                return;
            }
        }

        #endregion

        #region Event Triggers

        /// <summary>
        /// Raising a map changed event.
        /// </summary>
        /// <param name="n"></param>
        private void OnMapChanged(int n) => MapChanged?.Invoke(this, new MapChangedEventArgs(n));

        /// <summary>
        /// Raising a game over event.
        /// </summary>
        private void OnGameOver() {
            Pause(true);
            GameOver?.Invoke(this, new GameOverEventArgs(Score));
        }

        /// <summary>
        /// Raising a tile change event.
        /// </summary>
        /// <param name="tile"></param>
        private void OnTileChanged(Tile tile) => TileChanged?.Invoke(this, new TileChangedEventArgs(tile.X, tile.Y, tile.Type));

        /// <summary>
        /// The event of the timer.
        /// Moves the snake.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, ElapsedEventArgs e) {
            MoveSnake();
        }

        #endregion
                
    }
}
