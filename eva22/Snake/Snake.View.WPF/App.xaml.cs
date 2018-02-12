using System;
using System.Data;
using System.Windows;
using Snake.ViewModel;
using Snake.Model;
using Snake.Persistence;
using Microsoft.Win32;

namespace Snake.View.WPF {

    /// <summary>
    /// The type of the application
    /// </summary>
    public partial class App : Application {

        #region Private Fields

        private IPersistence dataAccess;
        private SnakeModel model;
        private SnakeViewModel viewModel;
        private SnakeWindow window;
        private OpenFileDialog openFileDialog;

        #endregion

        /// <summary>
        /// Application constructor
        /// </summary>
        public App() {
            Startup += new StartupEventHandler(App_Startup);
        }

        /// <summary>
        /// Startup eventhandler. Creates components, hooks up events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_Startup(object sender, StartupEventArgs e) {
            dataAccess = new TextFilePersistence();

            model = new SnakeModel(dataAccess);
            model.GameOver += new EventHandler<GameOverEventArgs>(Model_GameOver);

            viewModel = new SnakeViewModel(model);
            viewModel.ExitGame += new EventHandler(ViewModel_Exit);
            viewModel.LoadLevel += new EventHandler(ViewModel_LoadLevel);

            window = new SnakeWindow();
            window.DataContext = viewModel;
            window.Show();
        }

        #region ViewModel event handlers

        /// <summary>
        /// Opens a dialog box to load a level.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModel_LoadLevel(object sender, EventArgs e) {
            model.Pause(true);

            if (openFileDialog == null) {
                openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Snake - Level Loading";
                openFileDialog.Filter = "Text files|*.txt";
            }
            
            if (openFileDialog.ShowDialog() == true) {
                try {
                    model.LoadLevel(openFileDialog.FileName);
                } catch (DataException) {
                    MessageBox.Show("An error occured while loading the level", "Snake", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Eventhandler for exiting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModel_Exit(object sender, EventArgs e) {
            Shutdown();
        }

        #endregion

        #region Model event handlers

        /// <summary>
        /// Handles the end of the game, opens a messagebox to show the score.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_GameOver(object sender, GameOverEventArgs e) {
            MessageBox.Show("Score: " + e.Score, "Game Over", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            model.RestartLevel();
        }

        #endregion

    }
}
