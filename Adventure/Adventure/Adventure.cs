using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TileEngine;

namespace Adventure
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Adventure : Microsoft.Xna.Framework.Game
    {
        public const int SCREEN_WIDTH = 640, SCREEN_HEIGHT = 480;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameWorld gameWorld;
        InventoryScreen inventoryScreen;

        private Texture2D emptyHeartIcon;
        private Texture2D heartIconLeft;
        private Texture2D heartIconRight;
        private Texture2D coinIcon;
        private Texture2D keyIcon;
        private Texture2D equippedItemOutlineZ;
        private Texture2D equippedItemOutlineX;
        private SpriteFont font;

        GamePadState gamepadState;
        GamePadState previousGamepadState;

        GameState gameState = GameState.Playing;

        public Adventure()
        { 
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameWorld = new GameWorld();
            gameWorld.Initialize(Content, GraphicsDevice);

            inventoryScreen = new InventoryScreen(gameWorld.Player.Inventory);

            gamepadState = GamePad.GetState(PlayerIndex.One);
            previousGamepadState = gamepadState;

            

            // TODO: Add your initialization logic here
            base.Initialize();

            //graphics.ToggleFullScreen();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameWorld.LoadContent();
            inventoryScreen.LoadContent(Content);

            emptyHeartIcon = Content.Load<Texture2D>("HUD/empty_heart_icon");
            heartIconLeft = Content.Load<Texture2D>("HUD/heart_icon_left");
            heartIconRight = Content.Load<Texture2D>("HUD/heart_icon_right");
            coinIcon = Content.Load<Texture2D>("HUD/coin_icon");
            keyIcon = Content.Load<Texture2D>("HUD/key");
            equippedItemOutlineZ = Content.Load<Texture2D>("HUD/equipped_item_outline_z");
            equippedItemOutlineX = Content.Load<Texture2D>("HUD/equipped_item_outline_x");
            font = Content.Load<SpriteFont>("Fonts/font");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            gamepadState = GamePad.GetState(PlayerIndex.One);

            if (gamepadState.IsButtonDown(Buttons.RightShoulder) && previousGamepadState.IsButtonUp(Buttons.RightShoulder))
                graphics.ToggleFullScreen();
                

            if (gameState == GameState.Playing)
            {
                gameWorld.Update();

                if (gamepadState.IsButtonDown(Buttons.Start) && previousGamepadState.IsButtonUp(Buttons.Start))
                {
                    if (!gameWorld.IsAreaTransitioning && !gameWorld.IsMapTransitioning)
                    {
                        gameState = GameState.InventoryScreen;
                        inventoryScreen.StartDisplaying();
                    }
                }
            }
            else if (gameState == GameState.InventoryScreen)
            {
                inventoryScreen.Update();

                if (gamepadState.IsButtonDown(Buttons.Start) && previousGamepadState.IsButtonUp(Buttons.Start))
                {
                    inventoryScreen.StopDisplaying();
                }

                if (!inventoryScreen.IsDisplaying)
                {
                    gameState = GameState.Playing;
                }
            }

            previousGamepadState = gamepadState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            gameWorld.Draw(spriteBatch);

            if (gameState == GameState.InventoryScreen)
                inventoryScreen.Draw(spriteBatch);

            drawHUD();

            base.Draw(gameTime);
        }

        private void drawHUD()
        {
            spriteBatch.Begin();

            int x = 10, y = 10;
            for (int i = 0; i < (gameWorld.Player.MaxHealth / 2); i++)
            {
                spriteBatch.Draw(emptyHeartIcon, new Vector2(x, y), Color.White);
                x += emptyHeartIcon.Width;
            }

            x = 10;
            for (int i = 0; i < gameWorld.Player.Health; i++)
            {
                int n = i % 2;
                if (n == 0)
                {
                    spriteBatch.Draw(heartIconLeft, new Vector2(x, y), Color.White);
                    x += heartIconLeft.Width;
                }
                else
                {
                    spriteBatch.Draw(heartIconRight, new Vector2(x, y), Color.White);
                    x += heartIconRight.Width;
                }
            }

            spriteBatch.Draw(coinIcon, new Vector2(10, 34), Color.White);
            spriteBatch.DrawString(font, "x " + gameWorld.Player.Inventory.Money, new Vector2(30, 34), Color.White);

            spriteBatch.Draw(equippedItemOutlineX,
                new Vector2(550 - (equippedItemOutlineZ.Width / 2),
                        30 - (equippedItemOutlineZ.Height / 2)),
                    Color.White);
            spriteBatch.Draw(equippedItemOutlineZ,
                new Vector2(600 - (equippedItemOutlineX.Width / 2),
                        30 - (equippedItemOutlineX.Height / 2)),
                    Color.White);

            EquippableItem item = null;
            if ((item = gameWorld.Player.Inventory.GetEquippedItemForButton(Buttons.Y)) != null)
            {
                spriteBatch.Draw(item.InventoryScreenIcon,
                    new Vector2(550 - (item.InventoryScreenIcon.Width / 2),
                        30 - (item.InventoryScreenIcon.Height / 2)),
                    Color.White);
            }
            if ((item = gameWorld.Player.Inventory.GetEquippedItemForButton(Buttons.B)) != null)
            {
                spriteBatch.Draw(item.InventoryScreenIcon,
                    new Vector2(600 - (item.InventoryScreenIcon.Width / 2),
                        30 - (item.InventoryScreenIcon.Height / 2)),
                    Color.White);
            }

            if (gameWorld.CurrentMap is Dungeon)
            {
                spriteBatch.Draw(keyIcon, new Vector2(10, SCREEN_HEIGHT - 25), Color.White);
                spriteBatch.DrawString(font, "x " + gameWorld.Player.Inventory.NumKeys, new Vector2(25, SCREEN_HEIGHT - 25), Color.White);
            }

            spriteBatch.End();
        }
    }

    public enum GameState
    {
        Playing,
        InventoryScreen
    }
}
