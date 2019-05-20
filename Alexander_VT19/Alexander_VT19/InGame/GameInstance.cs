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

namespace Alexander_VT19
{
    class GameInstance
    {
        /// <summary>
        /// The position in world where player will spawn
        /// </summary>
        private static readonly Vector3 playerPosition = new Vector3(0, 10, 0);
        private static readonly float TargetTimeOnTarget = 650;

        private static List<Model> PlayerModels;
        private static Effect simpleEffect;
        private static float maxTargetTime;


        private GraphicsDevice graphicsDevice;
        private Challenge _challenge;
        private SkyBox _skyBox;
        private float _timeOnTarget;
        private int _modelIndex;


        public RenderTarget2D RenderTarget;
        public Player _player;

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

        public GameInstance(PlayerSelectMenu.PlayerData playerData, GraphicsDevice graphics)
        {
            graphicsDevice = graphics;
            
            RenderTarget = new RenderTarget2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            CustomModel playerCustomModel = new CustomModel(PlayerModels[_modelIndex], playerPosition, Vector3.Zero, Vector3.One * 100f, graphics);


            // Create a new player
            _player = new Player(playerIndex, InputMethod.Keyboard, playerCustomModel, graphics);

            _player.customModel.SetModelEffect(simpleEffect, false);
        }



        public static void LoadContent(ContentManager content)
        {
            // Load Models for player
            PlayerModels = new List<Model>();
            PlayerModels.Add(content.Load<Model>(@"Models/newTest"));

            simpleEffect = content.Load<Effect>(@"Effects/SimpleEffect");

        }

        public void Update(GameTime gameTime)
        {
            // Update player
            _player.Update(gameTime, _challenge);
            // Update rotation check
            CheckRotation(gameTime);
        }

        private void CheckRotation(GameTime gameTime)
        {
            bool correctX = false, correctY = false, correctZ = false, all = false;

            if (_player.customModel.Rotation.X < _challenge.DesiredRotation.X + _challenge.Margin &&
                _player.customModel.Rotation.X > _challenge.DesiredRotation.X - _challenge.Margin) correctX = true;

            if (_player.customModel.Rotation.Y < _challenge.DesiredRotation.Y + _challenge.Margin &&
                _player.customModel.Rotation.Y > _challenge.DesiredRotation.Y - _challenge.Margin) correctY = true;

            if (_player.customModel.Rotation.Z < _challenge.DesiredRotation.Z + _challenge.Margin &&
                _player.customModel.Rotation.Z > _challenge.DesiredRotation.Z - _challenge.Margin) correctZ = true;

            all = correctX && correctY && correctZ;


            // If all axis are aligned
            if (all)
            {
                // Add time to timer
                _timeOnTarget += gameTime.ElapsedGameTime.Milliseconds;

                // Shake Camera TODO

                // If time on target elapses target time,
                if (_timeOnTarget >= TargetTimeOnTarget)
                {
                    // Give player points, change model, and spawn particles
                    Hit();
                }
            }
            else
            {
                // Reset Timer
                _timeOnTarget = 0;
            }
        }


        private void Hit()
        {
            // Spawn Particles TODO
            // Add score TODO
            // Next challenge
            // Next model
        }

        private void Miss()
        {
            // Next challenge
            // Next model
        }

        private void UpdateModel()
        {
            _player.customModel.Model = PlayerModels[ModelIndex];
        }

        /// <summary>
        /// New drawing
        /// </summary>
        /// <param name="deferredRenderer"></param>
        /// <param name="camera"></param>
        public void Draw(DeferredRenderer deferredRenderer, Camera camera)
        {
            //deferredRenderer.Draw(graphicsDevice, new List<Model> {_player.customModel.Model}, lightManager, camera, RenderTarget);
        }

        /// <summary>
        /// Old drawing (standard renderer)
        /// </summary>
        /// <param name="camera"></param>
        public void Draw(Camera camera)
        {
            _player.Draw(camera);
        }
    }
}
