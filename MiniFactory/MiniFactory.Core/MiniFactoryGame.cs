using System;
using MiniFactory.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;

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
        private OreDeposit IronDeposit;

        //textures and resources
        private Texture2D pixel;
        private SpriteFont fontSmall;
        private SpriteFont fontMedium;
        private SpriteFont fontLarge;

        //mouse
        private ResourceTile currentResourceTile;
        private Vector2 tileCenter;
        private bool inRange;
        private Vector2 currentMousePos;
        private Point currentMousePosGrid;

        //keyboard
        private KeyboardState previousKeyboard;

        //mining
        private float miningProgress = 0f;

        //border
        const int borderThickness = 4;

        //progress bar
        const int barWidth = 60;
        const int barHeight = 10;
        const int barVerticalOffset = 20;

        //inventory
        private bool inventoryOpen = false;
        private const int slotSize = 36;
        private const int slotSpacing = 8;
        private Texture2D panelCornerTexture;
        private float inventorySlide = 0f;
        private float slideSpeed = 4f;

        //colors
        Color panelBackground = new Color(27, 38, 59);    // tmavě námořnická modrá — pozadí panelu
        Color slotColor = new Color(42, 63, 90);    // střední modrá — prázdný slot
        Color slotBorder = new Color(62, 92, 118);   // světlejší modrá — okraj slotu / hover
        Color textColor = new Color(232, 236, 239); // téměř bílá — text, kontrast na tmavém pozadí
        Color accent = new Color(255, 190, 60);  // teplá zlatá/oranžová — zvýraznění (progress bar, vybraný slot)

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

            oreDeposit = new OreDeposit(0, 0);
            oreDeposit.RegisterInWorld(world);

            IronDeposit = new OreDeposit(5, 5, 5, 5, ItemType.IronOre);
            IronDeposit.RegisterInWorld(world);

            panelCornerTexture = Content.Load<Texture2D>("square");

            fontSmall = Content.Load<SpriteFont>("Fonts/HudSmall");
            fontMedium = Content.Load<SpriteFont>("Fonts/HudMedium");
            fontLarge = Content.Load<SpriteFont>("Fonts/Hud");

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
                        player.Inventory.TryAddItem(currentResourceTile.Type);
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

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Tab) && previousKeyboard.IsKeyUp(Keys.Tab))
            {
                inventoryOpen = !inventoryOpen;
            }
            if (inventoryOpen)
                inventorySlide = Math.Min(1f, inventorySlide + slideSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            else
                inventorySlide = Math.Max(0f, inventorySlide - slideSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            previousKeyboard = keyboard;

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

            _spriteBatch.Begin();

            world.Draw(_spriteBatch, pixel);
            player.Draw(_spriteBatch);

            if (currentResourceTile != null)
            {
                _spriteBatch.DrawString(fontMedium, currentResourceTile.Type.ToString(), new Vector2(10, 10), textColor);
                _spriteBatch.DrawString(fontMedium, currentResourceTile.Remaining.ToString(), new Vector2(10, 60), textColor);

                Vector2 corner = World.GridToPixel(currentMousePosGrid);

                if (inRange)
                {
                    _spriteBatch.DrawHollowRect(pixel, corner, accent, borderThickness);
                }
            }

            if (miningProgress > 0)
            {
                _spriteBatch.Draw(pixel, new Rectangle((int)currentMousePos.X - (barWidth / 2) - 4, (int)currentMousePos.Y - barVerticalOffset - 4, barWidth + 8, barHeight + 8), Color.Black);
                _spriteBatch.Draw(pixel, new Rectangle((int)currentMousePos.X - (barWidth / 2), (int)currentMousePos.Y - barVerticalOffset, (int)(barWidth * (miningProgress / currentResourceTile.MiningDuration)), barHeight), Color.LimeGreen);
            }

            if (inventorySlide > 0f)
            {
                int rows = player.Inventory.Slots.GetLength(0);
                int columns = player.Inventory.Slots.GetLength(1);

                int panelWidth = columns * slotSize + (columns + 1) * slotSpacing;
                int panelHeight = rows * slotSize + (rows + 1) * slotSpacing;

                float y = MathHelper.Lerp(GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Height - panelHeight, inventorySlide);
                Point inventoryTopLeft = new Point((GraphicsDevice.Viewport.Width / 2) - (panelWidth / 2), (int)y);

                _spriteBatch.DrawRoundedRect(pixel, panelCornerTexture, new Rectangle(inventoryTopLeft.X, inventoryTopLeft.Y, panelWidth, panelHeight), panelBackground);
                
                ItemDefinition hoveredItem = null;
                for (int i = 0; i < columns; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        InventorySlot slot = player.Inventory.Slots[j, i];
                        Rectangle slotRectangle = new Rectangle(inventoryTopLeft.X + slotSpacing + i * (slotSize + slotSpacing), inventoryTopLeft.Y + slotSpacing + j * (slotSize + slotSpacing), slotSize, slotSize);
                        _spriteBatch.DrawRoundedRect(pixel, panelCornerTexture, slotRectangle, slotColor);
                        if (slot.Count > 0)
                        {
                            Texture2D itemTexture = ItemDatabase.Get(slot.Type).ItemTexture;
                            Vector2 textSize = fontSmall.MeasureString(slot.Count.ToString());
                            float textY = slotRectangle.Bottom - textSize.Y;
                            _spriteBatch.Draw(itemTexture, slotRectangle, Color.White);
                            _spriteBatch.DrawString(fontSmall, slot.Count.ToString(), new Vector2(slotRectangle.X, (int)textY), textColor);
                        }

                        if (slotRectangle.Contains(currentMousePos) && slot.Count > 0)
                            hoveredItem = ItemDatabase.Get(slot.Type);
                    }
                }
                if (hoveredItem != null)
                {
                    Vector2 boxTextSize = fontMedium.MeasureString(hoveredItem.Name);
                    _spriteBatch.Draw(pixel, new Rectangle((int)currentMousePos.X - ((int)boxTextSize.X / 2), (int)currentMousePos.Y, (int)boxTextSize.X, (int)boxTextSize.Y), new Color(0, 0, 0, 180));
                    _spriteBatch.DrawString(fontMedium, hoveredItem.Name, new Vector2((int)currentMousePos.X - ((int)boxTextSize.X / 2), (int)currentMousePos.Y), textColor);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}