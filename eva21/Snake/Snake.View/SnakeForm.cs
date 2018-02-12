using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Snake.Model;
using Snake.Persistence;

namespace Snake.View {
    public partial class SnakeForm : Form {
        
        #region Private fields

        private SnakeModel model;           //The model of the game.
        private Panel[,] panelGrid;         //The grid of field panel.
        private const int panelSize = 12;   //The size of individual field panels.

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the Form window. Creates a new model and subscribes to its events.
        /// </summary>
        public SnakeForm() {
            InitializeComponent();

            model = new SnakeModel(new TextFilePersistence());

            model.MapChanged += new EventHandler<MapChangedEventArgs>(Model_MapChanged);
            model.TileChanged += new EventHandler<TileChangedEventArgs>(Model_TileChanged);
            model.GameOver += new EventHandler<GameOverEventArgs>(Model_GameOver);
        }

        #endregion

        #region Private methods


        /// <summary>
        /// Generates a new map display
        /// </summary>
        /// <param name="n"></param>
        private void GenerateMap(int n) {
            tableLayoutPanel.SuspendLayout();
            tableLayoutPanel.Controls.Clear();
            panelGrid = new Panel[n, n];
            tableLayoutPanel.RowCount = n;
            tableLayoutPanel.ColumnCount = n;
            tableLayoutPanel.RowStyles.Clear();
            tableLayoutPanel.ColumnStyles.Clear();
            for (int i = 0; i < n; i++) {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 1 / n));
            }
            for (int j = 0; j < n; j++) {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1 / n));
            }

            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    panelGrid[i, j] = new Panel();
                    //pictureGrid[i, j].Location = new Point(5 * i, 5 * j);
                    panelGrid[i, j].Size = new Size(panelSize, panelSize);
                    //pictureGrid[i, j].TabIndex = 100 + i * n + j;
                    panelGrid[i, j].Dock = DockStyle.Fill;
                    //pictureGrid[i, j].Padding = new Padding(0);
                    panelGrid[i, j].Margin = new Padding(0);
                    tableLayoutPanel.Controls.Add(panelGrid[i, j], j, i);
                }
            }
            tableLayoutPanel.ResumeLayout();
            Size = new Size(n * panelSize + 25, n * panelSize + 70);
        }

        /// <summary>
        /// Changes the look of a tile according to the type of it.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        private void SetColor(int x, int y, TileType type) {
            switch (type) {
                case TileType.Empty:
                    panelGrid[x, y].BackColor = Color.Black;
                    break;
                case TileType.Snake:
                    panelGrid[x, y].BackColor = Color.Green;
                    break;
                case TileType.Food:
                    panelGrid[x, y].BackColor = Color.Red;
                    break;
                case TileType.Wall:
                    panelGrid[x, y].BackColor = Color.Blue;
                    break;
            }
        }

        #endregion

        #region Model event handlers

        /// <summary>
        /// Event handler for map changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_MapChanged(object sender, MapChangedEventArgs e) {
            GenerateMap(e.Size);
        }

        /// <summary>
        /// Event handler for tile changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_TileChanged(object sender, TileChangedEventArgs e) {
            SetColor(e.X, e.Y, e.Type);
        }

        /// <summary>
        /// Event handler for game over.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_GameOver(object sender, GameOverEventArgs e) {
            MessageBox.Show("Score: " + e.Score, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            model.RestartLevel();
        }

        #endregion

        #region Form event handlers

        /// <summary>
        /// Opens a load level dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadLevelToolStripMenuItem_Click(object sender, EventArgs e) {
            model.Pause(true);
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    model.LoadLevel(openFileDialog.FileName);
                } catch (DataException){
                    MessageBox.Show("An error occured while loading the level", "Snake", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Pauses/resumes the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!model.Paused) model.Pause(true);
            else model.Pause(false);
        }

        /// <summary>
        /// Closes the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

        /// <summary>
        /// Handles keystrokes, changes snake direction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SnakeForm_KeyDown(object sender, KeyEventArgs e) {
            if (model.Paused) return;
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) model.Direction = Direction.Up;
            else if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) model.Direction = Direction.Down;
            else if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) model.Direction = Direction.Left;
            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) model.Direction = Direction.Right;
        }

        #endregion
    }
}
