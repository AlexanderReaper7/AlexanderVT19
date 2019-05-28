#define DELTA 

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
        GamePad,
    }

    public class Player
    {
        public CustomModel CustomModel;
        public InputMethod PreferredInputMethod;
        private PlayerIndex _playerIndex;
        private KeyBinds _keyBinds;

        private Vector3 RotationInput
        {
            get
            {
                switch (PreferredInputMethod)
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


        public Player(PlayerIndex playerIndex, InputMethod preferredInputMethod, CustomModel model)
        {
            _playerIndex = playerIndex;
            PreferredInputMethod = preferredInputMethod;
            _keyBinds = GetKeyBinds(_playerIndex);
            CustomModel = model;
            CustomModel.Material = new LightingMaterial();
        }


        /// <summary>
        /// Updates Player logic
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
#if DELTA
            // Get input from gamepad/keyboard in pi radians...
            Vector3 deltaRotation = RotationInput * (float) Math.PI;
            // And multiply it by seconds elapsed
            deltaRotation *= (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            Vector3 newRotation = CustomModel.Rotation + deltaRotation;
#endif
#if ABSOLUTE
            // Get input from gamepad/keyboard in pi radians...
            Vector3 newRotation = RotationInput * (float)Math.PI;
#endif
            // "loop" using modulus
            newRotation.X %= MathHelper.TwoPi;
            newRotation.Y %= MathHelper.TwoPi;
            newRotation.Z %= MathHelper.TwoPi;
            // Apply rotation to model
            CustomModel.Rotation = newRotation;

            UpdateColor();
        }

        /// <summary>
        /// Sets the new color to the model based on the current rotation
        /// </summary>
        private void UpdateColor()
        {
            ((LightingMaterial)CustomModel.Material).DiffuseColor = ColorHelper.CalculateColorFromRotation(CustomModel.Rotation).ToVector3();
        }



        public void Draw(Camera camera)
        {
            CustomModel.Draw(camera.View, camera.Projection, camera.Position);
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
            if (keyboard.IsKeyDown(_keyBinds.RollPositive)) roll++;
            if (keyboard.IsKeyDown(_keyBinds.RollNegative)) roll--;
            // Pitch
            if (keyboard.IsKeyDown(_keyBinds.PitchPositive)) pitch++;
            if (keyboard.IsKeyDown(_keyBinds.PitchNegative)) pitch--;
            // Yaw
            if (keyboard.IsKeyDown(_keyBinds.YawPositive)) yaw++;
            if (keyboard.IsKeyDown(_keyBinds.YawNegative)) yaw--;
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
