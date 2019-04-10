using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alexander_VT19
{
    public class Player
    {
        public CustomModel customModel;
        private PlayerIndex _controllerIndex;

        public Player(PlayerIndex controllerIndex, CustomModel model, GraphicsDevice graphicsDevice)
        {
            _controllerIndex = controllerIndex;
            customModel = model;
        }

        public void Update(GameTime gameTime)
        {
            UpdateRotationVector(gameTime);
        }

        private const float sensitivity = 0.001f;

        private void UpdateRotationVector(GameTime gameTime)
        {
            // Get input from game pad or keyboard TODO: implement keyboard controls
            // Get controller input
            GamePadState gamePad = GamePad.GetState(_controllerIndex);
            // Get input vector
            Vector2 rotation = gamePad.ThumbSticks.Right * gameTime.ElapsedGameTime.Milliseconds * (float)Math.PI * sensitivity;
            // Create rotation matrix from 
            Matrix matrix = Matrix.CreateFromYawPitchRoll(customModel.Rotation.Y, customModel.Rotation.X, customModel.Rotation.Z);

            customModel.Rotation += Vector3.TransformNormal(new Vector3(rotation,0), matrix);
            customModel.Position += Vector3.TransformNormal(new Vector3(gamePad.ThumbSticks.Left, 0), matrix);
            // Reset rotation on right thumbstick click
            if (gamePad.Buttons.RightStick == ButtonState.Pressed) { customModel.Rotation = Vector3.Zero;}
        }

        public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition)
        {
            customModel.Draw(view, projection, cameraPosition);
        }
    }
}
