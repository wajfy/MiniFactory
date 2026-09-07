using System;
using MiniFactory.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MiniFactory.Core
{
    /// <summary>
    /// The main class for the game, responsible for managing game components, settings, 
    /// and platform-specific configurations.
    /// </summary>
    public class MiniFactoryGame : Game
    {
        // Resources for drawing.
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Camera camera;
        private float zoom = 1f;

        //entities
        private Player player;
        private World world;
        private OreDeposit oreDeposit;
        private OreDeposit IronDeposit;

        //mouse
        private Vector2 screenMousePos;
        private Point currentMousePosGrid;
        private Vector2 worldMousePos;

        //UI
        private InventoryUI inventoryUI;
        private UITheme uITheme;

        /// <summary>
        /// Indicates if the game is running on a mobile platform.
        /// </summary>
        public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

        /// <summary>
        /// Indicates if the game is running on a desktop platform.
        /// </summary>
        public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

        /// <summary>
        /// Initializes a new instance of the game. Configures platform-specific settings, 
        /// initializes services like settings and leaderboard managers, and sets up the 
        /// screen manager for screen transitions.
        /// </summary>
        public MiniFactoryGame()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 480
            };
            // Share GraphicsDeviceManager as a service.
            Services.AddService(typeof(GraphicsDeviceManager), graphicsDeviceManager);

            Content.RootDirectory = "Content";

            // Configure screen orientations.
            graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;

            uITheme = new UITheme();
            inventoryUI = new InventoryUI(uITheme);

            base.Initialize();

            // Load supported languages and set the default language.
            List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
            var languages = new List<CultureInfo>();
            for (int i = 0; i < cultures.Count; i++)
            {
                languages.Add(cultures[i]);
            }

            // TODO You should load this from a settings file or similar,
            // based on what the user or operating system selected.
            var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
            LocalizationManager.SetCulture(selectedLanguage);
        }

        /// <summary>
        /// Loads game content, such as textures and particle systems.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            uITheme.LoadContent(GraphicsDevice, Content);
            player = new Player(new Vector2(400, 240), uITheme.Pixel);
            world = new World();
            camera = new Camera(player.Position, zoom);

            oreDeposit = new OreDeposit(0, 0);
            oreDeposit.RegisterInWorld(world);

            IronDeposit = new OreDeposit(5, 5, 5, 5, ItemType.IronOre);
            IronDeposit.RegisterInWorld(world);

            ItemDatabase.LoadContent(Content);
            base.LoadContent();
        }

        /// <summary>
        /// Updates the game's logic, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for game updates.
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            screenMousePos = new Vector2(mouse.X, mouse.Y);

            camera.Position = player.Position;

            Matrix inverse = Matrix.Invert(camera.GetTransformMatrix(GraphicsDevice.Viewport));
            worldMousePos = Vector2.Transform(screenMousePos, inverse);
            currentMousePosGrid = World.PixelToGrid(worldMousePos);

            player.Update(gameTime, world, mouse, worldMousePos);

            inventoryUI.Update(gameTime, player.Inventory, mouse, keyboard, GraphicsDevice.Viewport);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game's graphics, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for rendering.
        /// </param>
        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the MonoGame orange color before drawing.
            GraphicsDevice.Clear(Color.Green);

            _spriteBatch.Begin(transformMatrix: camera.GetTransformMatrix(GraphicsDevice.Viewport));

            world.Draw(_spriteBatch, uITheme.Pixel);
            player.DrawWorld(_spriteBatch, currentMousePosGrid, uITheme);  

            _spriteBatch.End();

            _spriteBatch.Begin();

            inventoryUI.Draw(_spriteBatch, player.Inventory, screenMousePos);
            player.DrawScreen(_spriteBatch, screenMousePos, currentMousePosGrid, uITheme);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}