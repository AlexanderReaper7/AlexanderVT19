using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Alexander_VT19.Lights;
using Microsoft.Xna.Framework;
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
        private static List<Model> _playerModels;
        private static Effect _simpleEffect;

        private const float EasyMargin = 0.25f;
        private const float MediumMargin = 0.2f;
        private const float HardMargin = 0.15f;
        private const float UltraMargin = 0.1f;

        // Time limit in seconds TODO
        private const float EasyTimeLimit = 60f;
        private const float MediumTimeLimit = 60f;
        private const float HardTimeLimit = 90f;
        private const float UltraTimeLimit = 120f;



        private GraphicsDevice _graphicsDevice;
        private ExtendedSpriteBatch _spriteBatch;
        public RenderTarget2D RenderTarget;

        private ParticleSystem _particleSystem;

        private Challenge _challenge;
        private Player _player;
        private PlayerSelectMenu.PlayerData _playerData;
        private double _currentTime;
        public double Score;

        private int _modelIndex;
        /// <summary>
        /// Sets a new model to the player
        /// </summary>
        public int ModelIndex
        {
            get { return _modelIndex; }
            set
            {
                _modelIndex = value;
                UpdateModel();
            }
        }

        public GameInstance(PlayerSelectMenu.PlayerData playerData, GraphicsDevice graphics, ContentManager content)
        {
            // Load Models if they have not already been loaded
            if (_playerModels == null || _simpleEffect == null) LoadContent(content);


            // Initialize graphics & rendering related things
            _graphicsDevice = graphics;
            _spriteBatch = new ExtendedSpriteBatch(graphics);
            RenderTarget = new RenderTarget2D(_graphicsDevice, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

            // Store "PlayerData"
            _playerData = playerData;

            // Create "Win" particle System
            //Texture2D particleTexture = content.Load<Texture2D>(@""); // TODO
            //particleSystem = new ParticleSystem(graphicsDevice, content, particleTexture, 8, new Vector2(10), 100, Vector3.Zero, 0);

            // Create a new player 
            CustomModel playerCustomModel = new CustomModel(_playerModels[_modelIndex], PlayerSpawnPosition, Vector3.Zero, Vector3.One * 100f, graphics);
            playerCustomModel.SetModelEffect(_simpleEffect, false);
            _player = new Player(playerData.PlayerIndex, playerData.PreferredInputMethod, playerCustomModel, graphics);
            // Create a new challenge
            CustomModel challengeCustomModel = new CustomModel(_playerModels[_modelIndex], PlayerSpawnPosition, Vector3.Zero, Vector3.One * 100f, graphics);
            challengeCustomModel.SetModelEffect(_simpleEffect, false);
            _challenge = new Challenge(challengeCustomModel, EasyMargin);

            // Set CurrentTime to its difficulty´s time limit TODO
            _currentTime = EasyTimeLimit;

        }



        private static void LoadContent(ContentManager content)
        {
            // Load Models for player
            _playerModels = new List<Model>
            {
                content.Load<Model>(@"Models/newTest"),
            };

            _simpleEffect = content.Load<Effect>(@"Effects/SimpleEffect");
            _timeFont = content.Load<SpriteFont>(@"DefaultFont");
        }



        public void Update(GameTime gameTime)
        {
            // Update player
            _player.Update(gameTime);

            // Remove elapsed time from current time
            _currentTime -= gameTime.ElapsedGameTime.TotalSeconds;

            // If time has run out...
            if (_currentTime <= 0)
            {
                // Continue to next challenge
                Next(false);
            }

            // If player color is within margins of the current challenge...
            if (_challenge.CheckCorrectColor(_player))
            {
                // Continue to next challenge
                Next(true);
            }


            if(Keyboard.GetState().IsKeyDown(Keys.R)) _challenge = new Challenge(_challenge, EasyMargin);
        }

        //private void CheckRotation(GameTime gameTime)
        //{
        //    bool correctX = false, correctY = false, correctZ = false, all = false;

        //    if (_player.customModel.Rotation.X < _challenge.DesiredRotation.X + _challenge.Margin &&
        //        _player.customModel.Rotation.X > _challenge.DesiredRotation.X - _challenge.Margin) correctX = true;

        //    if (_player.customModel.Rotation.Y < _challenge.DesiredRotation.Y + _challenge.Margin &&
        //        _player.customModel.Rotation.Y > _challenge.DesiredRotation.Y - _challenge.Margin) correctY = true;

        //    if (_player.customModel.Rotation.Z < _challenge.DesiredRotation.Z + _challenge.Margin &&
        //        _player.customModel.Rotation.Z > _challenge.DesiredRotation.Z - _challenge.Margin) correctZ = true;

        //    all = correctX && correctY && correctZ;


        //    // If all axis are aligned
        //    if (all)
        //    {
        //        // Add time to timer
        //        _timeOnTarget += gameTime.ElapsedGameTime.Milliseconds;

        //        // Shake Camera TODO

        //        // If time on target elapses target time,
        //        if (_timeOnTarget >= TargetTimeOnTarget)
        //        {
        //            // Give player points, change model, and spawn particles
        //            Next();
        //        }
        //    }
        //    else
        //    {
        //        // Reset Timer
        //        _timeOnTarget = 0;
        //    }
        //}


        private void Next(bool result)
        {
            // Spawn Particles TODO
            
            // Add score 
            Score += _currentTime;
            // Next challenge
            _challenge = new Challenge(_challenge, EasyMargin);
            // Next model
            ModelIndex = (ModelIndex + 1) % _playerModels.Count;
        }

        private void UpdateModel()
        {
            _player.CustomModel.Model = _playerModels[ModelIndex];
            _challenge.Model = _playerModels[ModelIndex];
        }



        ///// <summary>
        ///// New drawing Method
        ///// </summary>
        ///// <param name="deferredRenderer"></param>
        ///// <param name="camera"></param>
        //public void Draw(DeferredRenderer deferredRenderer, Camera camera)
        //{
        //    deferredRenderer.Draw(graphicsDevice, new List<Model> {_player.customModel.Model}, lightManager, camera, RenderTarget);
        //}

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
            _spriteBatch.DrawString(_timeFont, timeString, (new Vector2(_graphicsDevice.Viewport.Width, 50) - timeSize * 1.1f) / 2, Color.White, 0,Vector2.Zero, 1.1f, SpriteEffects.None,0);
            // Draw the text
            _spriteBatch.DrawString(_timeFont, timeString, (new Vector2(_graphicsDevice.Viewport.Width, 50) - timeSize) / 2, _playerData.Color);
            _spriteBatch.End();

        }
    }
}
