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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace ShipShape
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ShmupGame : Microsoft.Xna.Framework.Game
    {
        #region Constants

        const int WindowWidth = 800;
        const int WindowHeight = 600;

        const int PlayerShipSpeed = 3;
        const int PlayerShipStartingX = 320;
        const int PlayerShipStartingY = 240;

        const int PlayerMissileSpeed = 6;

        #endregion

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState lastKeyboardState;

        Texture2D playerShipTexture;
        Vector2 playerShipPosition;

        Texture2D playerMissileTexture;
        IList<Vector2> playerMissilePositions;

        public ShmupGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
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
            // TODO: Add your initialization logic here
            this.lastKeyboardState = Keyboard.GetState();

            this.playerShipPosition = new Vector2(PlayerShipStartingX, PlayerShipStartingY);
            this.playerMissilePositions = new List<Vector2>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            this.playerShipTexture = Content.Load<Texture2D>("pentagon");
            this.playerMissileTexture = Content.Load<Texture2D>("missile");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            UpdateInput(gameTime);

            for (int i = 0; i < playerMissilePositions.Count; i++)
            {
                Vector2 temp = playerMissilePositions[i];
                playerMissilePositions[i] = new Vector2(temp.X + PlayerMissileSpeed, temp.Y);
                if (playerMissilePositions[i].X > WindowWidth)
                {
                    playerMissilePositions.RemoveAt(i);
                    i--;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Performs input processing.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void UpdateInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                this.playerShipPosition.X -= PlayerShipSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                this.playerShipPosition.X += PlayerShipSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                this.playerShipPosition.Y -= PlayerShipSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                this.playerShipPosition.Y += PlayerShipSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Z) && lastKeyboardState.IsKeyUp(Keys.Z))
            {
                this.playerMissilePositions.Add(new Vector2(playerShipPosition.X + playerShipTexture.Width / 2, 
                    playerShipPosition.Y + playerShipTexture.Height * 3 / 8));
            }

            this.lastKeyboardState = Keyboard.GetState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(playerShipTexture, playerShipPosition, Color.White);

            foreach (Vector2 position in playerMissilePositions)
            {
                spriteBatch.Draw(playerMissileTexture, position, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
