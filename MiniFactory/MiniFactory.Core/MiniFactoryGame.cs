using System;
using MiniFactory.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

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

        //entities
        private Player player;
        private World world;
        private OreDeposit oreDeposit;

        //textures and resources
        private Texture2D pixel;
        private SpriteFont font;

        //mouse
        private ResourceTile currentResourceTile;
        private Vector2 tileCenter;
        private bool inRange;
        private Vector2 currentMousePos;
        private Point currentMousePosGrid;

        //mining
        private float miningProgress = 0f;

        //border
        const int borderThickness = 4;
        Color borderColor = Color.Yellow;

        //progress bar
        const int barWidth = 60;
        const int barHeight = 10;
        const int barVerticalOffset = 20;

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
            graphicsDeviceManager = new GraphicsDeviceManager(this);

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

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData([Color.White]);

            player = new Player(new Vector2(400, 240), pixel);
            world = new World();
            oreDeposit = new OreDeposit(2, 2);
            oreDeposit.RegisterInWorld(world);

            font = Content.Load<SpriteFont>("Fonts/Hud");

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
            // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime);
            MouseState mouse = Mouse.GetState();
            world.TryGetResourceAt(new Vector2(mouse.X, mouse.Y), out ResourceTile tile);
            if (tile != currentResourceTile)
                miningProgress = 0;
            currentResourceTile = tile;

            currentMousePos = new Vector2(mouse.X, mouse.Y);
            currentMousePosGrid = World.PixelToGrid(currentMousePos);

            if (currentResourceTile != null)
            {
                Vector2 current = World.GridToPixel(currentMousePosGrid);
                tileCenter = new Vector2(current.X + (World.TileSize / 2), current.Y + (World.TileSize / 2));
                inRange = Vector2.Distance(player.Position, tileCenter) <= player.MiningRange;

                if (mouse.LeftButton == ButtonState.Pressed && inRange)
                {
                    miningProgress += player.MiningSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (miningProgress >= currentResourceTile.MiningDuration)
                    {
                        miningProgress = 0;
                        currentResourceTile.Remaining -= 1;
                        if (currentResourceTile.Remaining == 0)
                        {
                            world.Resources.Remove(currentMousePosGrid);
                        }
                    }
                }
                else
                {
                    miningProgress = 0;
                }
            }
            else
            {
                tileCenter = new Vector2();
                inRange = false;
            }

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
            GraphicsDevice.Clear(Color.LightBlue);

            _spriteBatch.Begin();

            world.Draw(_spriteBatch, pixel);
            player.Draw(_spriteBatch);

            if (currentResourceTile != null)
            {
                _spriteBatch.DrawString(font, currentResourceTile.Type.ToString(), new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(font, currentResourceTile.Remaining.ToString(), new Vector2(10, 40), Color.White);
                _spriteBatch.DrawString(font, "In range: " + inRange, new Vector2(10, 70), Color.White);

                Vector2 corner = World.GridToPixel(currentMousePosGrid);

                if (inRange)
                {
                    // horní hrana
                    _spriteBatch.Draw(pixel, new Rectangle((int)corner.X, (int)corner.Y, World.TileSize, borderThickness), borderColor);
                    // dolní hrana
                    _spriteBatch.Draw(pixel, new Rectangle((int)corner.X, (int)corner.Y + World.TileSize - borderThickness, World.TileSize, borderThickness), borderColor);
                    // levá hrana
                    _spriteBatch.Draw(pixel, new Rectangle((int)corner.X, (int)corner.Y, borderThickness, World.TileSize), borderColor);
                    // pravá hrana
                    _spriteBatch.Draw(pixel, new Rectangle((int)corner.X + World.TileSize - borderThickness, (int)corner.Y, borderThickness, World.TileSize), borderColor);
                }
            }

            if (miningProgress > 0)
            {
                _spriteBatch.Draw(pixel, new Rectangle((int)currentMousePos.X - (barWidth / 2) - 4, (int)currentMousePos.Y - barVerticalOffset - 4, barWidth + 8, barHeight + 8), Color.Black);
                _spriteBatch.Draw(pixel, new Rectangle((int)currentMousePos.X - (barWidth / 2), (int)currentMousePos.Y - barVerticalOffset, (int)(barWidth * (miningProgress / currentResourceTile.MiningDuration)), barHeight), Color.LimeGreen);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}