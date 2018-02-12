using System;
using System.Windows.Media;
using Snake.Model;
using System.Collections.ObjectModel;

namespace Snake.ViewModel {

    public class SnakeViewModel {

        #region Private Fields

        private SnakeModel model;       //The model of the game

        #endregion

        #region Public properties

        /// <summary>
        /// Stores the fields in an observable collection.
        /// </summary>
        public ObservableCollection<ViewField> Fields { get; private set; }

        /// <summary>
        /// Gets the size of the current map from the model.
        /// </summary>
        public int MapSize { get { return model.MapSize; } }

        public DelegateCommand UpCommand { get; private set; }      //Command for changing the direction up.
        public DelegateCommand DownCommand { get; private set; }    //Command for changing the direction down.
        public DelegateCommand LeftCommand { get; private set; }    //Command for changing the direction left.
        public DelegateCommand RightCommand { get; private set; }   //Command for changing the direction right.
        public DelegateCommand ExitCommand { get; private set; }    //Command for exiting the game.
        public DelegateCommand RestartCommand { get; private set; } //Command for restarting the level.
        public DelegateCommand PauseCommand { get; private set; }   //Command for pausing the game.
        public DelegateCommand LoadCommand { get; private set; }    //Command for loading a new level.

        #endregion

        #region Events

        public event EventHandler ExitGame;        //Event for exiting the game.
        public event EventHandler LoadLevel;       //Event for loading a new level.

        #endregion

        #region Constructors

        /// <summary>
        /// SnakeViewModel constructor
        /// </summary>
        /// <param name="model"></param>
        public SnakeViewModel(SnakeModel model) {
            //Set the model.
            this.model = model;
            //Subscribe to the events of the model.
            model.MapChanged += new EventHandler<MapChangedEventArgs>(Model_MapChanged);
            model.TileChanged += new EventHandler<TileChangedEventArgs>(Model_TileChanged);

            //Define the commands.
            UpCommand = new DelegateCommand(param => model.Direction = Direction.Up);
            DownCommand = new DelegateCommand(param => model.Direction = Direction.Down);
            LeftCommand = new DelegateCommand(param => model.Direction = Direction.Left);
            RightCommand = new DelegateCommand(param => model.Direction = Direction.Right);
            ExitCommand = new DelegateCommand(param => OnExit());
            RestartCommand = new DelegateCommand(param => model.RestartLevel());
            PauseCommand = new DelegateCommand(param => OnPause());
            LoadCommand = new DelegateCommand(param => OnLoad());

            //Initialize the fields collection.
            Fields = new ObservableCollection<ViewField>();
        }

        #endregion

        #region Model event handlers

        /// <summary>
        /// Handles map changes. Updates field representations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_MapChanged(object sender, MapChangedEventArgs e) {
            Fields.Clear();
            for (int i = 0; i < e.Size; i++) {
                for (int j = 0; j < e.Size; j++) {
                    Fields.Add(new ViewField());
                }
            }
        }

        /// <summary>
        /// Handles tile changes. Sets their new colors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_TileChanged(object sender, TileChangedEventArgs e) {
            switch (e.Type) {
                case TileType.Empty:
                    Fields[e.X * model.MapSize + e.Y].Color = Color.FromRgb(0, 0, 0);
                    break;                            
                case TileType.Snake:                  
                    Fields[e.X * model.MapSize + e.Y].Color = Color.FromRgb(255, 0, 0);
                    break;                            
                case TileType.Food:                   
                    Fields[e.X * model.MapSize + e.Y].Color = Color.FromRgb(0, 255, 0);
                    break;                            
                case TileType.Wall:                   
                    Fields[e.X * model.MapSize + e.Y].Color = Color.FromRgb(0, 0, 255);
                    break;
            }
        }

        #endregion

        #region Event methods

        /// <summary>
        /// Pauses/resumes the game.
        /// </summary>
        private void OnPause() {
            if (!model.Paused) model.Pause(true);
            else model.Pause(false);
        }

        /// <summary>
        /// Loads a new level.
        /// </summary>
        private void OnLoad() {
            LoadLevel?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Exits the game.
        /// </summary>
        private void OnExit() {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        #endregion

    }
}