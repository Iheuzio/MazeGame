using Maze;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using MessageBox = System.Windows.Forms.MessageBox;
using NLog.Config;
using System;

namespace MazeGame
{
    /// <summary>
    /// The main class for the Maze Game program.
    /// </summary>
    public class MazeGameProgram : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private Color menuTextColor = Color.Black;

        private Map _maze;
        private Texture2D _goalTexture;
        private Texture2D _wallTexture;
        private Texture2D _playerTexture;
        private Texture2D _pathTexture;
        private InputManager _inputManager;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private Block[,] _mazeStructure;
        private Vector2 _goalPosition;
        private Texture2D _completeMazeTexture;
        private bool _movementOccured = false;
        private bool _playerRotated = false;
        private float _playerRotation = 0.0f;
        private MapVector _previousPlayerPosition;
        private bool _mazeCreated = false;
        private int scaleMaze = 32;
        private bool _activated = false;

        private enum MenuState
        {
            LoadFromFile,
            Recursive,
            HuntKill
        }

        private MenuState currentMenuState = MenuState.LoadFromFile;
        private bool menuActive = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MazeGameProgram"/> class.
        /// </summary>
        public MazeGameProgram()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // occupy the entire screen using values not fullscreen (calculate the size of the screen)
            _graphics.PreferredBackBufferWidth = Screen.PrimaryScreen.Bounds.Width;
            _graphics.PreferredBackBufferHeight = Screen.PrimaryScreen.Bounds.Height;
            _graphics.ApplyChanges();

            InitializeLogging(); // Initialize the logger
        }

        /// <summary>
        /// Starts the logging process and sets the log file to be used
        /// </summary>
        private void InitializeLogging()
        {
            try
            {
                // starts in the debug-6 folder so we go up to root
                // easier to maange this way since I can clone into different machines without copying this file to the folder
                var path = "nlog.config.xml";
                try
                {
                    LogManager.Configuration = new XmlLoggingConfiguration(path);
                }
                catch
                {
                    path = "../../../../nlog.config.xml";
                    LogManager.Configuration = new XmlLoggingConfiguration(path);
                }
                // Temp directory is where it is stored
                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MazeGame.log");

                if (File.Exists(logFilePath))
                {
                    FileInfo logFileInfo = new FileInfo(logFilePath);
                    // if file is larger than 5MB, delete it
                    if (logFileInfo.Length > (5 * 1024 * 1024))
                        File.Delete(logFilePath);
                }
                _logger.Info("---------------------------------------------");
                _logger.Info($"Date {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                _logger.Warn("Application started.");
            }
            catch
            {
                _logger.Error("Files needed for logging not found, if deleted, must be added again.");
                MessageBox.Show("Files needed for logging not found, if deleted, must be added again. Add to debug6.0-windows folder", "Logging Files Not Found", MessageBoxButtons.OK);
                Exit();
            }
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Loads the content for the game.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("fonts/font");
            LoadTextures();
        }
           
        /// <summary>
        /// Loads the textures for the game.
        /// </summary>
        private void LoadTextures()
        {
            _goalTexture = Content.Load<Texture2D>("game_objects/Goal");
            _wallTexture = Content.Load<Texture2D>("game_objects/Wall");
            _playerTexture = Content.Load<Texture2D>("game_objects/Player");
            _pathTexture = Content.Load<Texture2D>("game_objects/Path");
        }

        /// <summary>
        /// Updates the game with a clock cycle.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)
                && !menuActive) // Don't show the exit prompt if the menu is active
            {
                var result = MessageBox.Show("Are you sure you want to exit?", "Exit Game", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    _logger.Warn("Exit: User selected Yes in window.");
                    Exit();
                }
                else
                {
                    _logger.Warn("Exit: User selected No in window, continuing.");
                }
            }

            if (menuActive)
            {
                HandleMenuInput();
            }

            // don't run past if the maze wasn't created yet
            else if (!_mazeCreated)
            {
                return;
            }

            else
            {
                _inputManager.Update();
                CheckGoalAttainment();

                if (_maze.Player.Position != _previousPlayerPosition)
                {
                    _movementOccured = true;
                    _previousPlayerPosition = _maze.Player.Position;
                    _logger.Info($"Player moved to position:|\tX: |{_maze.Player.Position.X}| Y: |{_maze.Player.Position.Y}|\t |");
                }
                if (_maze.Player.GetRotation() != _playerRotation)
                {
                    _playerRotated = true;
                    _playerRotation = _maze.Player.GetRotation();
                    _logger.Info($"Player rotated to angle: {_playerRotation}");
                }
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// Handles the input for the menu.
        /// </summary>
        private void HandleMenuInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                menuActive = false;
                if (currentMenuState == MenuState.LoadFromFile)
                {
                    InitializeMaze();
                    _logger.Info("Maze loaded from file.");
                }
                else if (currentMenuState == MenuState.Recursive)
                {
                    InitializeRecursiveMaze("Recursive");
                    _logger.Info("Maze loaded from recursive algorithm.");
                }
                else if (currentMenuState == MenuState.HuntKill)
                {
                    InitializeRecursiveMaze("HuntKill");
                    _logger.Info("Maze loaded from hunt and kill algorithm.");
                }
                InitializeInputHandlers();
            }
            else if (!_activated)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    if (currentMenuState == MenuState.LoadFromFile)
                    {
                        currentMenuState = MenuState.Recursive;
                    }
                    else if (currentMenuState == MenuState.Recursive)
                    {
                        currentMenuState = MenuState.HuntKill;
                    }
                    else if (currentMenuState == MenuState.HuntKill)
                    {
                        currentMenuState = MenuState.LoadFromFile;
                    }
                    _activated = true;
                    return;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    if (currentMenuState == MenuState.LoadFromFile)
                    {
                        currentMenuState = MenuState.HuntKill;
                    }
                    else if (currentMenuState == MenuState.Recursive)
                    {
                        currentMenuState = MenuState.LoadFromFile;
                    }
                    else if (currentMenuState == MenuState.HuntKill)
                    {
                        currentMenuState = MenuState.Recursive;
                    }
                    _activated = true;
                    return;
                }
            }
            if (Keyboard.GetState().GetPressedKeys().Length == 0)
            {
                _activated = false;
            }
        }

        /// <summary>
        /// Initializes the maze creation based on the file selection.
        /// </summary>
        private void InitializeMaze()
        {
            List<string> selectedFiles = OpenFileSelectionDialog();

            if (selectedFiles.Count > 0)
            {
                string selectedMapFilePath = selectedFiles[0];
                IMapProvider mazeProvider = new MazeFromFile.MazeFromFile(selectedMapFilePath);
                _maze = new Map(mazeProvider);
                _maze.CreateMap();
               

                _mazeStructure = _maze.MapGrid.Clone() as Block[,];
                _goalPosition = new Vector2(_maze.Goal.X, _maze.Goal.Y);

                // Create the complete maze texture here
                CreateCompleteMazeTexture(_mazeStructure, _goalPosition);
                _mazeCreated = true;
            }
            else
            {
                ShowNoFileSelectedDialog();
            }
        }

        /// <summary>
        /// Initializes the maze creation based on the recursive algorithm.
        /// </summary>
        private void InitializeRecursiveMaze(String type)
        {
            IMapProvider mazeProvider = null;
            switch (type)
            {
                case "Recursive":
                    mazeProvider = new MazeRecursion.RecursiveMazeGen();
                    _logger.Info("Maze loaded from recursive algorithm.");
                    break;
                case "HuntKill":
                    mazeProvider = new MazeHuntKill.HuntKillMazeGen();
                    _logger.Info("Maze loaded from hunt and kill algorithm.");
                    break;
            }
            if (mazeProvider == null)
            {
                _logger.Error("Maze provider is null, cannot create maze.");
                throw new Exception("Maze provider is null, cannot create maze.");
            }
            _maze = new Map(mazeProvider);

            // show popup to get width and height
            int width = 0;
            int height = 0;
            string widthInput = "";
            string heightInput = "";
            bool validWidth = false;
            bool validHeight = false;
            while (!validWidth)
            {
                widthInput = Microsoft.VisualBasic.Interaction.InputBox("Please enter the width of the maze (must be odd):", "Maze Width", "15");
                if (widthInput == "")
                {
                    break;
                }
                validWidth = int.TryParse(widthInput, out width);
                if (width % 2 == 0)
                {
                    MessageBox.Show("Width must be odd.");
                    validWidth = false;
                }
                else if (width < 5)
                {
                    MessageBox.Show("Width must be at least 5.");
                    validWidth = false;

                }
            }
            while (!validHeight)
            {
                heightInput = Microsoft.VisualBasic.Interaction.InputBox("Please enter the height of the maze (must be odd):", "Maze Height", "15");
                if (heightInput == "")
                {
                    break;
                }
                validHeight = int.TryParse(heightInput, out height);
                if (height % 2 == 0)
                {
                    MessageBox.Show("Height must be odd.");
                    validHeight = false;
                }
                else if (height < 5)
                {
                    MessageBox.Show("Height must be at least 5.");
                    validHeight = false;
                }
               
            }

            if (widthInput == "" || heightInput == "")
            {
                _maze.CreateMap();
                _logger.Info("Maze loaded with size 5x5.");
            }
            else
            {
                _maze.CreateMap(width, height);
                _logger.Info($"Maze loaded with size {width}x{height}.");
            }

            _mazeStructure = _maze.MapGrid.Clone() as Block[,];
            _goalPosition = new Vector2(_maze.Goal.X, _maze.Goal.Y);
            
            // Create the complete maze texture here
            CreateCompleteMazeTexture(_mazeStructure, _goalPosition);
            _mazeCreated = true;

        }

        /// <summary>
        /// Opens the file selection dialog.
        /// </summary>
        /// <returns>A list of selected files.</returns>
        private List<string> OpenFileSelectionDialog()
        {
            List<string> selectedFiles = new List<string>();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Maps");
                openFileDialog.Filter = "Map Files (*.txt)|*.txt|All Files (*.*)|*.*";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFiles = openFileDialog.FileNames.ToList();
                }
            }

            return selectedFiles;
        }

        /// <summary>
        /// Shows the no file selected dialog when the user does not select a file.
        /// </summary>
        private void ShowNoFileSelectedDialog()
        {
            var result = MessageBox.Show("No file was selected, would you like to try again?", "No File Selected", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                _logger.Warn("Exit: User retried their file.");
                InitializeMaze();
            }
            else
            {
                _logger.Warn("Exit: User selected No to retry their file.");
                Exit();
            }
        }

        /// <summary>
        /// Draws the game with a clock cycle.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            if (menuActive)
            {

                GraphicsDevice.Clear(Color.CornflowerBlue);
                _spriteBatch.Begin();

                DrawMenu();
                _spriteBatch.End();
            }
            // else if input is detected or player rotated
            else if (_movementOccured || _playerRotated)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                _spriteBatch.Begin();

                DrawGameScreen();
                _spriteBatch.End();
            }


            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws the menu screen.
        /// </summary>
        private void DrawMenu()
        {
            _spriteBatch.DrawString(font, "Menu", new Vector2(10, 10), menuTextColor);
            _spriteBatch.DrawString(font, "Load from file", new Vector2(10, 60), currentMenuState == MenuState.LoadFromFile ? Color.White : Color.Black);
            _spriteBatch.DrawString(font, "Recursive", new Vector2(10, 110), currentMenuState == MenuState.Recursive ? Color.White : Color.Black);
            _spriteBatch.DrawString(font, "Hunt and Kill", new Vector2(10, 160), currentMenuState == MenuState.HuntKill ? Color.White : Color.Black);
        }

        /// <summary>
        /// Draws the game screen.
        /// </summary>
        private void DrawGameScreen()
        {
            if (_completeMazeTexture != null)
                _spriteBatch.Draw(_completeMazeTexture, Vector2.Zero, Color.White);

            if (_movementOccured)
            {
                DrawPathAndPlayer(_maze);
                _previousPlayerPosition = _maze.Player.Position;
                _movementOccured = false;
            }
            if (_playerRotated)
            {
                DrawPathAndPlayer(_maze);
                _playerRotated = false;
            }

        }

        /// <summary>
        /// Draws the path and player on the game screen.
        /// </summary>
        /// <param name="maze">The maze to draw.</param>
        private void DrawPathAndPlayer(Map maze)
        {
            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    Rectangle destinationRect = new Rectangle(x * scaleMaze, y * scaleMaze, scaleMaze, scaleMaze);

                    if (x == maze.Player.Position.X && y == maze.Player.Position.Y)
                    {
                        float rotation = maze.Player.GetRotation();
                        Vector2 origin = new Vector2(_playerTexture.Width / 2, _playerTexture.Height / 2);
                        Vector2 position = new Vector2(destinationRect.X + destinationRect.Width / 2, destinationRect.Y + destinationRect.Height / 2);

                        _logger.Info($"Drawing player at:|\tX: |{x}| Y: |{y}|\t |");
                        float playerScale = (float)scaleMaze / (float)_playerTexture.Width;
                        _spriteBatch.Draw(_playerTexture, position, null, Color.White, rotation, origin, playerScale, SpriteEffects.None, 0);
                    }
                    else if (_mazeStructure[x, y] == Block.Empty && new Vector2(x, y) != _goalPosition)
                    {
                        _logger.Info($"Drawing path at:|\tX: |{x}| Y: |{y}|\t |");
                        _spriteBatch.Draw(_pathTexture, destinationRect, Color.White);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the complete maze texture.
        /// </summary>
        /// <param name="mazeStructure">The maze structure to use.</param>
        /// <param name="goalPosition">The position of the goal.</param>
        /// Creates the complete maze texture.
        /// </summary>
        /// <param name="mazeStructure">The maze structure to use.</param>
        /// <param name="goalPosition">The position of the goal.</param>
        private void CreateCompleteMazeTexture(Block[,] mazeStructure, Vector2 goalPosition)
        {

            // scaleMaze based on the size of the monitor for maximum coverage of placement for the objects: 32 works when the monitor is 1920x1080 at 67x33 mazes. Scale it like that
            // Get the screen width and height
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // Calculate the scale factors for width and height
            double widthScaleFactor = (double)screenWidth / mazeStructure.GetLength(0);
            double heightScaleFactor = (double)screenHeight / mazeStructure.GetLength(1);

            // Use the smaller of the two scale factors to maintain aspect ratio
            scaleMaze = (int)Math.Ceiling(Math.Min(widthScaleFactor, heightScaleFactor));

            if (mazeStructure == null)
            {
                _logger.Warn("Maze structure is null, cannot create complete maze texture.");
                return;
            }
            int mazeWidth = mazeStructure.GetLength(0);
            int mazeHeight = mazeStructure.GetLength(1);

            // Create a RenderTarget to draw the complete maze
            RenderTarget2D mazeRenderTarget = new RenderTarget2D(GraphicsDevice, mazeWidth * scaleMaze, mazeHeight * scaleMaze);

            GraphicsDevice.SetRenderTarget(mazeRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();

            // Draw the maze structure and goal
            for (int y = 0; y < mazeHeight; y++)
            {
                for (int x = 0; x < mazeWidth; x++)
                {
                    Rectangle destinationRect = new Rectangle(x * scaleMaze, y * scaleMaze, scaleMaze, scaleMaze);

                    if (x == (int)goalPosition.X && y == (int)goalPosition.Y)
                    {
                        _logger.Info($"Drawing goal at:|\tX: |{x}| Y: |{y}|\t |");
                        _spriteBatch.Draw(_goalTexture, destinationRect, Color.White);
                    }
                    else if (mazeStructure[x, y] == Block.Solid) // Check for walls in mazeStructure
                    {
                        _logger.Info($"Drawing wall at:|\tX: |{x}| Y: |{y}|\t |");
                        _spriteBatch.Draw(_wallTexture, destinationRect, Color.White);
                    }
                    else
                    {
                        // Draw path initially
                        _logger.Info($"Drawing path at:|\tX: |{x}| Y: |{y}|\t |");
                        _spriteBatch.Draw(_pathTexture, destinationRect, Color.White);
                    }
                }
            }

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            // Set the complete maze texture
            _completeMazeTexture = mazeRenderTarget;
        }

        /// <summary>
        /// Initializes the input handlers for getting input from user.
        /// </summary>
        private void InitializeInputHandlers()
        {
            _inputManager = InputManager.Instance;
            _inputManager.AddKeyHandler(Keys.Up, () => _maze.Player.MoveForward());
            _inputManager.AddKeyHandler(Keys.Down, () => _maze.Player.MoveBackward());
            _inputManager.AddKeyHandler(Keys.Left, () => _maze.Player.TurnLeft());
            _inputManager.AddKeyHandler(Keys.Right, () => _maze.Player.TurnRight());
            _logger.Info("Input handlers initialized.");
        }

        /// <summary>
        /// Checks if the goal was reached
        /// </summary>
        private void CheckGoalAttainment()
        {
            if (_maze.Goal.X == _maze.Player.Position.X && _maze.Goal.Y == _maze.Player.Position.Y)
            {
                var result = MessageBox.Show("Goal Reached! Would you like to play again with a different map?", "Congratulations!", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    _logger.Warn("Exit: User selected Yes in window after completion.");
                    Exit();
                }
                else
                {
                    _logger.Warn("Exit: User selected No in window after completion.");
                    Exit();
                }
            }
        }

    }
}
