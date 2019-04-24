using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alexander_VT19
{
    public enum InputMethod
    {
        Keyboard,
        GamePad
    }

    public class Player
    {
        public CustomModel customModel;
        private PlayerIndex _playerIndex;
        private InputMethod _preferredInputMethod;
        private KeyBinds keyBinds;

        private CustomModel[] rotationalAxisModels;

        public Vector3 RotationInput
        {
            get
            {
                switch (_preferredInputMethod)
                {
                    case InputMethod.Keyboard:
                        return GetRotationFromKeyboard();
                    case InputMethod.GamePad:
                        return GetRotationFromGamePad();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }



        public Player(PlayerIndex playerIndex, InputMethod preferredInputMethod, CustomModel model, CustomModel[] axisModels)
        {
            _playerIndex = playerIndex;
            _preferredInputMethod = preferredInputMethod;
            keyBinds = GetKeyBinds(_playerIndex);
            customModel = model;
            rotationalAxisModels = axisModels;
        }



        public void Update(GameTime gameTime, Camera camera)
        {
            // Get input from gamepad/keyboard in pi radians...
            Vector3 deltaRotation = RotationInput * (float) Math.PI;
            // And multiply it by seconds elapsed
            deltaRotation *= (float) gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            //Matrix cameraRotationMatrix = Matrix.CreateFromYawPitchRoll(camera.Rotation.Y, camera.Rotation.X, camera.Rotation.Z);
            //Vector3 cameraRight = Vector3.Transform(Vector3.Right, cameraRotationMatrix);

            // Create a plane through the camera and player
            //Plane plane = new Plane(camera.Position, cameraRight, customModel.Position);
            //Vector3 forward = camera.Position - customModel.Position;
            //forward.Normalize();
            //Vector3 up = plane.Normal;
            //Vector3 right = Vector3.Cross(up, customModel.Position);
            //right.Normalize();

            //Quaternion deltaOrientation = 
            //    Quaternion.CreateFromAxisAngle(up, deltaRotation.Y) * // Yaw
            //    Quaternion.CreateFromAxisAngle(right, deltaRotation.X) * // Pitch
            //    Quaternion.CreateFromAxisAngle(Vector3.Backward, deltaRotation.Z); // Roll

            // Apply rotation
            customModel.Rotation *= Quaternion.CreateFromYawPitchRoll(deltaRotation.Y, deltaRotation.X, 0f) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, deltaRotation.Z); ;

            // Reset rotation on right stick press
            if (GamePad.GetState(_playerIndex).Buttons.RightStick == ButtonState.Pressed)
            {
                customModel.Rotation = Quaternion.CreateFromYawPitchRoll(0f,0f,0f);
            }

            // Update helping models
            //rotationalAxisModels[0].Rotation *= Quaternion.CreateFromYawPitchRoll(deltaRotation.Y, deltaRotation.X, 0f) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, deltaRotation.Z);
            //rotationalAxisModels[1].Rotation *= Quaternion.CreateFromYawPitchRoll(deltaRotation.Y, 0f, deltaRotation.Z);
            //rotationalAxisModels[2].Rotation *= Quaternion.CreateFromYawPitchRoll(0f, deltaRotation.X, deltaRotation.Z);

        }



        public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition)
        {
            customModel.Draw(view, projection, cameraPosition);

            //foreach (CustomModel rotationalAxisModel in rotationalAxisModels)
            //{
            //    rotationalAxisModel.Draw(view, projection, cameraPosition);
            //}
        }



        #region Input
        // TODO: implement left handed and right handed modes
        private Vector3 GetRotationFromGamePad() 
        {
            GamePadState gamePad = GamePad.GetState(_playerIndex);
            // Roll with Left and Right Triggers
            float roll = gamePad.Triggers.Left - gamePad.Triggers.Right;
            // Pitch with ThumbSticks along the Vertical axis (Y)
            float pitch = gamePad.ThumbSticks.Right.Y;
            // Yaw with ThumbSticks along Horizontal axis (X)
            float yaw = gamePad.ThumbSticks.Right.X;
            return new Vector3(yaw,pitch,roll);
        }

        private Vector3 GetRotationFromKeyboard()
        {
            KeyboardState keyboard = Keyboard.GetState();
            float roll = 0f, pitch = 0f, yaw = 0f;
            // Roll
            if (keyboard.IsKeyDown(keyBinds.RollPositive)) roll++;
            if (keyboard.IsKeyDown(keyBinds.RollNegative)) roll--;
            // Pitch
            if (keyboard.IsKeyDown(keyBinds.PitchPositive)) pitch++;
            if (keyboard.IsKeyDown(keyBinds.PitchNegative)) pitch--;
            // Yaw
            if (keyboard.IsKeyDown(keyBinds.YawPositive)) yaw++;
            if (keyboard.IsKeyDown(keyBinds.YawNegative)) yaw--;
            return new Vector3(yaw, pitch, roll);
        }
        #endregion

        #region KeyBinds
        private struct KeyBinds
        {
            public Keys RollPositive;
            public Keys RollNegative;
            public Keys PitchPositive;
            public Keys PitchNegative;
            public Keys YawPositive;
            public Keys YawNegative;

            public KeyBinds(Keys rollPositive, Keys rollNegative, Keys pitchPositive, Keys pitchNegative, Keys yawPositive, Keys yawNegative)
            {
                RollPositive = rollPositive;
                RollNegative = rollNegative;
                PitchPositive = pitchPositive;
                PitchNegative = pitchNegative;
                YawPositive = yawPositive;
                YawNegative = yawNegative;
            }
        }

        private static KeyBinds GetKeyBinds(PlayerIndex playerIndex)
        {
            KeyBinds output;
            switch (playerIndex)
            {
                case PlayerIndex.One:
                    output = new KeyBinds(Keys.Q, Keys.E, Keys.W, Keys.S, Keys.A, Keys.D);
                    break;
                case PlayerIndex.Two:
                    output = new KeyBinds(Keys.R, Keys.Y, Keys.T, Keys.G, Keys.F, Keys.H);
                    break;
                case PlayerIndex.Three:
                    output = new KeyBinds(Keys.U, Keys.O, Keys.I, Keys.K, Keys.J, Keys.L);
                    break;
                case PlayerIndex.Four:
                    output = new KeyBinds(Keys.LeftControl, Keys.NumPad0, Keys.Up, Keys.Down, Keys.Left, Keys.Right);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null);
            }
            return output;
        }
        #endregion
    }
}
