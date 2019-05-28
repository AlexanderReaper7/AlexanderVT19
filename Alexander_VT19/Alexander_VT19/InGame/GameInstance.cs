using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Alexander_VT19.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alexander_VT19
{
    class GameInstance
    {
        /// <summary>
        /// The position in world where player will spawn
        /// </summary>
        private static readonly Vector3 PlayerSpawnPosition = new Vector3(0, 10, 0);

        private static SpriteFont _timeFont;

        // Content
        public static List<Model> PlayerModels;
        private static Effect _modelEffect;
        private static SoundEffect _successSoundEffect;

        private GraphicsDevice _graphicsDevice;
        private ExtendedSpriteBatch _spriteBatch;
        public RenderTarget2D RenderTarget;

        private ParticleSystem _particleSystem;

        private LightingMaterial _playerMaterial;
        private Player _player;
        private LightingMaterial _challengeMaterial;
        private Challenge _challenge;
        private double _currentTime;

        public PlayerData PlayerData;
        public double Score;

        private int _modelIndex;
        /// <summary>
        /// Sets a new model to the player
        /// </summary>
        private int ModelIndex
        {
            get { return _modelIndex; }
            set
            {
                _modelIndex = value;
                UpdateModel();
            }
        }

        public GameInstance(PlayerData playerData, GraphicsDevice graphics, ContentManager content)
        {
            // Load Models if they have not already been loaded
            if (PlayerModels == null || _modelEffect == null) LoadContent(content);


            // Initialize graphics & rendering related things
            _graphicsDevice = graphics;
            _spriteBatch = new ExtendedSpriteBatch(graphics);
            RenderTarget = new RenderTarget2D(_graphicsDevice, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

            // Store "PlayerData"
            PlayerData = playerData;

            // Create "Win" particle System
            //Texture2D particleTexture = content.Load<Texture2D>(@""); // TODO
            //particleSystem = new ParticleSystem(graphicsDevice, content, particleTexture, 8, new Vector2(10), 100, Vector3.Zero, 0);

            // Create a new player 
            CustomModel playerCustomModel = new CustomModel(PlayerModels[_modelIndex], PlayerSpawnPosition, Vector3.Zero, Vector3.One * 100f, graphics);
            _playerMaterial = new LightingMaterial();
            playerCustomModel.Material = _playerMaterial;
            playerCustomModel.SetModelEffect(_modelEffect, false);
            _player = new Player(playerData.PlayerIndex, playerData.PreferredInputMethod, playerCustomModel);

            // Create a new challenge
            CustomModel challengeCustomModel = new CustomModel(PlayerModels[_modelIndex], PlayerSpawnPosition, Vector3.Zero, Vector3.One * 100f, graphics);
            _challengeMaterial = new LightingMaterial();
            challengeCustomModel.Material = _challengeMaterial;
            challengeCustomModel.SetModelEffect(_modelEffect, false);
            _challenge = new Challenge(challengeCustomModel, PlayerData.Difficulty.Margin);

            // Set CurrentTime to its difficulty´s time limit
            _currentTime = PlayerData.Difficulty.TimeLimit;

        }



        public static void LoadContent(ContentManager content)
        {
            // Load Models for player
            PlayerModels = new List<Model>
            {
                content.Load<Model>(@"Models/newTest"),
                content.Load<Model>(@"Models/1Cylinder"),
                content.Load<Model>(@"Models/Sphere"),
            };

            _modelEffect = content.Load<Effect>(@"Effects/LightingEffect");
            _timeFont = content.Load<SpriteFont>(@"DefaultFont");
            _successSoundEffect = content.Load<SoundEffect>(@"Sounds/Success");
        }



        public void Update(GameTime gameTime)
        {
            // Update player
            _player.Update(gameTime);


            // If time has run out...
            if (_currentTime <= 0) Next(false); // Continue to next challenge
            else _currentTime -= gameTime.ElapsedGameTime.TotalSeconds; // Else remove elapsed time from current time
                


            // If player color is within margins of the current challenge...
            if (_challenge.CheckCorrectColor(_player))
            {
                // Continue to next challenge
                Next(true);
            }


            //if(Keyboard.GetState().IsKeyDown(Keys.R)) Next(true);
        }



        private void Next(bool result)
        {
             // if target was hit
            if (result)
            {
                // Add score
                Score += _currentTime;
                // play sound effect
                _successSoundEffect.Play();
            }


            // Spawn Particles TODO
            
            // Next challenge
            _challenge = new Challenge(_challenge, PlayerData.Difficulty.Margin);
            // Next model
            ModelIndex = (ModelIndex + 1) % PlayerModels.Count;
            // Reset time
            _currentTime = PlayerData.Difficulty.TimeLimit;
        }

        private void UpdateModel()
        {
            _player.CustomModel.Model = PlayerModels[ModelIndex];
            _challenge.Model = PlayerModels[ModelIndex];
            _player.CustomModel.SetModelEffect(_modelEffect,true);
            _challenge.CustomModel.SetModelEffect(_modelEffect, true);
        }


        /// <summary>
        /// Old drawing (standard renderer)
        /// </summary>
        /// <param name="camera"></param>
        public void Draw(Camera camera)
        {
            // Draw player and challenge models
            _player.Draw(camera);
            _challenge.Draw(camera);

            // Draw Time remaining
            string timeString = Math.Round(_currentTime, 2).ToString();
            Vector2 timeSize = _timeFont.MeasureString(timeString);

            _spriteBatch.Begin();
            // Draw white "shadow" behind text so the text is viewable even on a dark background
            _spriteBatch.DrawString(_timeFont, timeString, (new Vector2(_graphicsDevice.Viewport.Width, 50) - timeSize * 2f) / 2, Color.White, 0,Vector2.Zero, 2f, SpriteEffects.None,0);
            // Draw the text
            _spriteBatch.DrawString(_timeFont, timeString, (new Vector2(_graphicsDevice.Viewport.Width, 50) - timeSize * 1.9f) / 2, PlayerData.Color, 0, Vector2.Zero, 1.9f, SpriteEffects.None,0);
            _spriteBatch.End();

        }

        public void DrawGameOver()
        {
            string scoreString = Math.Round(Score, 2).ToString();
            Vector2 scoreSize = _timeFont.MeasureString(scoreString);

            _spriteBatch.Begin();
            // Draw Score
            _spriteBatch.DrawString(_timeFont, scoreString, (new Vector2(_graphicsDevice.Viewport.Width, 150) - scoreSize) / 2, Color.White);

            _spriteBatch.End();
        }
    }
}
