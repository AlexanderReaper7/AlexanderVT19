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

    #region TODO´s

    // TODO: unique Fonts that fit the theme and purpose
    // TODO: higher quality fonts
    // TODO: more models
    // TODO: Replace the CheckCorrectColor method with DeltaE color checking, see ColorHelper.cs for more
    // TODO: more sound effect
    // TODO: background music for the menu
    // TODO: InGame music
    // TODO: Secret menu for debugging stuff
    // TODO: Research SMAA
    // TODO: Implement Deferred rendering
    // TODO: Implement a blurring effect on the background in PlayerSelectMenu
    // TODO: Fail, Success and Win Particles
    // TODO: display resolution scaling
    // TODO: Additional visual effects in InGame (represent speed etc.)
    // TODO: finalize difficulties implementation
    // TODO: custom game parameters ( game duration etc.)


    #endregion


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager Graphics;

        private static GameStates _currentGameState;
        private static GameStates _nextGameState;

        public static GameStates GameState
        {
            get { return _currentGameState; }
            set { _nextGameState = value; }
        }


        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.PreferMultiSampling = true;
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
            _currentGameState = firstGameState;
            _nextGameState = firstGameState;

            // Load InGame
            InGame.LoadContent(Content, GraphicsDevice);
            PlayerSelectMenu.LoadContent(Content, GraphicsDevice);
            PlayerSelectMenu.StartNewSelection();
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
                case GameStates.PlayerSelection:
                    PlayerSelectMenu.Update(gameTime);
                    break;
                case GameStates.Exit:
                    Exit();
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
                case GameStates.PlayerSelection:
                    PlayerSelectMenu.Draw(gameTime);
                    break;
                case GameStates.Exit:
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
            _currentGameState = _nextGameState;
        }
    }
}
