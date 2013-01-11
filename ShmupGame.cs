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

        private const int WindowWidth = 800;
        private const int WindowHeight = 600;

        private const int PlayerShipSpeed = 3;
        private const int PlayerShipStartingX = 320;
        private const int PlayerShipStartingY = 240;

        private const int PlayerMissileSpeed = 6;

        private const double BaseEnemySpawnRate = .25;
        private const double EnemySpawnChance = 0.6;
        private const int EnemyShipSpeed = 2;

        #endregion

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private KeyboardState lastKeyboardState;

        private Random random;

        private Texture2D playerShipTexture;
        private Vector2 playerShipPosition;

        private Texture2D playerMissileTexture;
        private IList<Vector2> playerMissilePositions;

        private double enemySpawnTimer;
        private Texture2D enemyShipTexture;
        private IList<Vector2> enemyShipPositions;

        public ShmupGame()
        {
            random = new Random();

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
            this.enemyShipPositions = new List<Vector2>();

            this.enemySpawnTimer = BaseEnemySpawnRate;

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
            this.enemyShipTexture = Content.Load<Texture2D>("enemy1");
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
            {
                this.Exit();
            }

            // TODO: Add your update logic here
            UpdateInput(gameTime);

            enemySpawnTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (enemySpawnTimer <= 0)
            {
                if (EnemySpawnChance <= random.NextDouble())
                {
                    enemyShipPositions.Add(new Vector2(WindowWidth, random.Next(0, 
                        WindowHeight - enemyShipTexture.Height)));
                }
                enemySpawnTimer = BaseEnemySpawnRate;
            }

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

            for (int i = 0; i < enemyShipPositions.Count; i++)
            {
                Vector2 temp = enemyShipPositions[i];
                enemyShipPositions[i] = new Vector2(temp.X - EnemyShipSpeed, temp.Y);

                for (int j = 0; j < playerMissilePositions.Count; j++)
                {
                    Vector2 enemy = enemyShipPositions[i];
                    Vector2 missile = playerMissilePositions[j];
                    if (enemy.X < missile.X + playerMissileTexture.Width && 
                        enemy.X + enemyShipTexture.Width > missile.X &&
                        enemy.Y < missile.Y + playerMissileTexture.Height &&
                        enemy.Y + enemyShipTexture.Height > missile.Y)
                    {
                        playerMissilePositions.RemoveAt(j);
                        j--;
                        enemyShipPositions[i] = new Vector2(-2000, 0);
                    }
                }

                if (enemyShipPositions[i].X < 0 - enemyShipTexture.Width)
                {
                    enemyShipPositions.RemoveAt(i);
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

            foreach (Vector2 position in enemyShipPositions)
            {
                spriteBatch.Draw(enemyShipTexture, position, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
