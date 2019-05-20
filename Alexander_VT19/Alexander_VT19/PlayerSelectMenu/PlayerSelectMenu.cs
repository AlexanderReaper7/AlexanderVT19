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
    public static class PlayerSelectMenu
    {
        public struct PlayerData
        {
            public InputMethod PreferredInputMethod;
            public PlayerIndex PlayerIndex;
            public bool IsActive;
        }


        private static GraphicsDevice _graphicsDevice;
        private static SpriteBatch _spriteBatch;

        private static PlayerData[] _players;

        private static Texture2D[] _keyboardOverlayTextures;

        public static void LoadContent(ContentManager content, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;

            _keyboardOverlayTextures = new Texture2D[4];
            _keyboardOverlayTextures[0] = content.Load<Texture2D>(@"Images/KeyboardOverlayPlayer1");
            _keyboardOverlayTextures[1] = content.Load<Texture2D>(@"Images/KeyboardOverlayPlayer2");
            _keyboardOverlayTextures[2] = content.Load<Texture2D>(@"Images/KeyboardOverlayPlayer3");
            _keyboardOverlayTextures[3] = content.Load<Texture2D>(@"Images/KeyboardOverlayPlayer4");


        }



        public static void Update()
        {
            #region Keyboard

            // Get keyboard Input
            KeyboardState keyboardState = Keyboard.GetState();

            // for every player...
            for (int playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                // Get key binds
                Keys[] keyBinds = Player.GetKeyBinds((PlayerIndex) playerIndex).KeyArray;

                // and every key...
                foreach (Keys key in keyBinds)
                {
                    // if the key is down
                    if (keyboardState.IsKeyDown(key))
                    {
                        // then set that players input method to keyboard
                        _players[playerIndex].PreferredInputMethod = InputMethod.Keyboard;
                        _players[playerIndex].IsActive = true;
                    }
                }
            }
            

            #endregion

            #region Gamepad

            // Get GamePad Input
            for (int playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                GamePadState gamePadState = GamePad.GetState((PlayerIndex) playerIndex);

                if (gamePadState.IsButtonDown(Buttons.Start))
                {
                    _players[playerIndex].PreferredInputMethod = InputMethod.GamePad;
                    _players[playerIndex].IsActive = true;
                }
            }
            
            #endregion
        }



        public static void StartNewSelection()
        {
            // Create new list of player data
            _players = new PlayerData[4]
            {
                new PlayerData() {PlayerIndex = PlayerIndex.One, IsActive = false},
                new PlayerData() {PlayerIndex = PlayerIndex.Two, IsActive = false},
                new PlayerData() {PlayerIndex = PlayerIndex.Three, IsActive = false},
                new PlayerData() {PlayerIndex = PlayerIndex.Four, IsActive = false},
            };
            
        }




        private static void StartGame()
        {
            Game1.GameState = Game1.GameStates.InGame;
            InGame.StartNewGame(_players);
        }

        public static void Draw()
        {
            // Draw Player status
            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).IsConnected)
                {
                    // Connected
                    _spriteBatch.Draw();
                }
                else
                {
                    // Disconnected
                    _spriteBatch.Draw();
                }
            }

            
        }

        
    }
}
