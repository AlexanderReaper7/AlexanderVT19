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

        private Vector3 RotationInput
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


        public Player(PlayerIndex playerIndex, InputMethod preferredInputMethod, CustomModel model, GraphicsDevice graphicsDevice)
        {
            _playerIndex = playerIndex;
            _preferredInputMethod = preferredInputMethod;
            keyBinds = GetKeyBinds(_playerIndex);
            customModel = model;
            customModel.Material = new SimpleMaterial();

            
        }



        public void Update(GameTime gameTime, Challenge challenge)
        {
            // Get input from gamepad/keyboard in pi radians...
            Vector3 deltaRotation = RotationInput * (float) Math.PI;
            // And multiply it by seconds elapsed
            deltaRotation *= (float)gameTime.ElapsedGameTime.Milliseconds * 0.0001f;

            // Transform rotation direction for easier controlling ERROR: Gimbal lock
            Vector3.Transform(deltaRotation, Matrix.CreateFromYawPitchRoll(customModel.Rotation.Y, customModel.Rotation.X,
                0) * Matrix.CreateFromAxisAngle(Vector3.UnitZ, customModel.Rotation.Z));

            // Add Mod and Apply rotation
            Vector3 r = customModel.Rotation + deltaRotation;
            r.X %= MathHelper.TwoPi;
            r.Y %= MathHelper.TwoPi;
            r.Z %= MathHelper.TwoPi;
            customModel.Rotation = r;

            UpdateColor();
        }

        private void UpdateColor()
        {
            // Change Color from rotation
            // Y X Z becomes H S V
            float H, S, V;
            H = Math.Abs( MathHelper.ToDegrees(customModel.Rotation.Y));

            //S = (float) (Math.Cos(customModel.Rotation.X) + 1) / 2f;
            //V = (float) (Math.Cos(customModel.Rotation.Z) + 1) / 2f;

            S = LinearCosine(customModel.Rotation.X);
            V = LinearCosine(customModel.Rotation.Z);

            ((SimpleMaterial)customModel.Material).DiffuseColor = ColorHelper.HSVToRGB(new ColorHelper.HSV(H, S, V)).ToColor().ToVector3();
        }

        private static float LinearCosine(float value)
        {
            return Math.Abs((float) (1 - Math.Abs(value % MathHelper.TwoPi) / Math.PI));
        }




        public void Draw(Camera camera)
        {
            customModel.Draw(camera.View, camera.Projection, camera.Position);
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
        public struct KeyBinds
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

            public Keys[] KeyArray =>
                new Keys[6]
                {
                    RollPositive,
                    RollNegative,
                    PitchPositive,
                    PitchNegative,
                    YawPositive,
                    YawNegative
                };
        }

        public static KeyBinds GetKeyBinds(PlayerIndex playerIndex)
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
                    output = new KeyBinds(Keys.RightControl, Keys.NumPad0, Keys.Up, Keys.Down, Keys.Left, Keys.Right);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerIndex), playerIndex, null);
            }
            return output;
        }
        #endregion
    }
}
