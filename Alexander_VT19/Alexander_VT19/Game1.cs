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

namespace Alexander_VT19
{
    public enum GameStates
    {
        InGame,
        PlayerSelection,
        Exit
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;

        private static GameStates _gameState;
        private static GameStates _nextGameState;

        public static GameStates GameState
        {
            get { return _gameState; }
            set { _nextGameState = value; }
        }


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.PreferMultiSampling = true;
            IsFixedTimeStep = false;

            //_graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            const GameStates firstGameState = GameStates.PlayerSelection;
            _gameState = firstGameState;
            _nextGameState = firstGameState;

            // Load InGame
            InGame.LoadContent(Content, GraphicsDevice);

            PlayerSelectMenu.LoadContent(Content, GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.End))
            {
                Exit();
            }

            switch (GameState)
            {
                case GameStates.InGame:
                    InGame.Update(gameTime);
                    break;
                case GameStates.Exit:
                    Exit();
                    break;
                case GameStates.PlayerSelection:
                    PlayerSelectMenu.Update();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (GameState)
            {
                case GameStates.InGame:
                    InGame.Draw(gameTime);
                    break;
                case GameStates.Exit:
                    break;
                case GameStates.PlayerSelection:
                    PlayerSelectMenu.Draw(gameTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.Draw(gameTime);
            FinalUpdate();
        }

        private void FinalUpdate()
        {
            // Update Game State
            _gameState = _nextGameState;
        }
    }
}
