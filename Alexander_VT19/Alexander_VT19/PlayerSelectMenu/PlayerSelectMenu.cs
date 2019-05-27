using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexander_VT19.Materials;
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
            public Color Color;
        }


        private static GraphicsDevice _graphicsDevice;
        private static ExtendedSpriteBatch _spriteBatch;
        private static QuadRenderer _quadRenderer;

        private static PlayerData[] _players;

        private static SpriteFont _defaultFont;

        private static NeonBackground _neonBackground;

        private static Texture2D _pixel;

        private static Texture2D[] _keyboardOverlayTextures;
        private static Texture2D _keyboardTexture; // TODO:
        private static Texture2D _gamePadTexture;
        private static Texture2D _gamePadOverlayTexture;

        private static Texture2D _gamePadIcon;
        private static Texture2D _keyboardIcon;

        public static void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new ExtendedSpriteBatch(graphicsDevice);
            _quadRenderer = new QuadRenderer(graphicsDevice);

            _defaultFont = content.Load<SpriteFont>(@"DefaultFont");

            _neonBackground = NeonBackground.CreateNew(content);
            _neonBackground.Resolution = new Vector2(3840, 2160);

            _pixel = new Texture2D(_graphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });


            _keyboardTexture = content.Load<Texture2D>(@"Images/keyboard");
            _keyboardOverlayTextures = new Texture2D[4];
            _keyboardOverlayTextures[0] = content.Load<Texture2D>(@"Images/KeyboardOverlayPlayer1");
            _keyboardOverlayTextures[1] = content.Load<Texture2D>(@"Images/KeyboardOverlayPlayer2");
            _keyboardOverlayTextures[2] = content.Load<Texture2D>(@"Images/KeyboardOverlayPlayer3");
            _keyboardOverlayTextures[3] = content.Load<Texture2D>(@"Images/KeyboardOverlayPlayer4");

            _gamePadTexture = content.Load<Texture2D>(@"Images/360_controller");
            _gamePadOverlayTexture = content.Load<Texture2D>(@"Images/360_controller_overlay");

            _keyboardIcon = content.Load<Texture2D>(@"Images/keyboard_icon");
            _gamePadIcon = content.Load<Texture2D>(@"Images/gamepad_icon");
        }



        public static void Update()
        {
            // TODO: this is temp
            if (_players == null) { StartNewSelection();}

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

            if (keyboardState.IsKeyDown(Keys.Escape)) Game1.GameState = GameStates.Exit; // Press escape to exit
            if (keyboardState.IsKeyDown(Keys.Enter)) StartGame(); // Press enter to start game with current settings
            #endregion

            #region Gamepad

            // Get GamePad Input
            for (int playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                GamePadState gamePadState = GamePad.GetState((PlayerIndex) playerIndex);

                // Activate player on A button down
                if (gamePadState.IsButtonDown(Buttons.A))
                {
                    _players[playerIndex].PreferredInputMethod = InputMethod.GamePad;
                    _players[playerIndex].IsActive = true;
                }
                // Disable player on B button down
                else if (gamePadState.IsButtonDown(Buttons.B))
                {
                    _players[playerIndex].PreferredInputMethod = InputMethod.GamePad;
                    _players[playerIndex].IsActive = false;
                }

                // Start game on Start button down
                if (gamePadState.IsButtonDown(Buttons.Start))
                {
                    StartGame();
                }
            }
            
            #endregion


        }



        public static void StartNewSelection()
        {
            const int capacity = 4; // Capacity should not exceed 4
            // Create new array of player data
            _players = new PlayerData[capacity];
            for (int i = 0; i < capacity; i++)
            {
                _players[i] = new PlayerData
                {
                        PlayerIndex = (PlayerIndex)i,
                        IsActive = false,
                        Color = ColorHelper.HSVToRGB(new ColorHelper.HSV(360 / capacity * i, 1f, 0.5))
                };
            }
        }




        private static void StartGame()
        {
            foreach (PlayerData data in _players)
            {
                if (data.IsActive)
                {
                    Game1.GameState = GameStates.InGame;
                    InGame.StartNewGame(_players);
                }
            }
        }

        #region UIconstants
        private const int ScreenWidth = 1920; // TODO
        private const int ScreenHeight = 1080;

        private const int LeftMargin = ScreenWidth / 100;
        private const int Width = ScreenWidth / 4 - (2 * LeftMargin);
        private const int TopMargin = ScreenHeight / 2 + 50;
        private const int Height = ScreenHeight / 3;

        private const int TotalSpacer = 2* LeftMargin + Width;
        private const int TotalTop = TopMargin - LeftMargin;

        private static readonly Color BackDropColor = new Color(33,33,33,20);

        #endregion


        public static void Draw(GameTime gameTime)
        {
            // Clear buffer
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            #region Background
            // Update and render background
            _neonBackground.UpdateEffectParameters(gameTime);
            _quadRenderer.Render(_neonBackground.Effect); // TODO: fix aliasing problem for player select background

            #endregion

            #region Instructions

            Rectangle instructionsRectangle = new Rectangle(LeftMargin, 20, ScreenWidth - 2* LeftMargin, ScreenHeight / 2);
            _spriteBatch.DrawFilledRectangle(instructionsRectangle, BackDropColor);

            // Draw "press ESc to return to main menu"
            string backString = "Press 'ESC' to return to main menu";
            _spriteBatch.DrawString(_defaultFont, backString, new Vector2(0, ScreenHeight - _defaultFont.MeasureString(backString).Y), Color.White);

            // Draw "press ENTER or START to start game"
            string playString = "Press 'ENTER' or 'Start' to start the game";
            _spriteBatch.DrawString(_defaultFont, playString, new Vector2(ScreenWidth, ScreenHeight) - _defaultFont.MeasureString(playString), Color.White);

            #region keyboard controlls

            // draw keyboard
            Rectangle keyboardRectangle = new Rectangle(20, 20, 850, 300);
            _spriteBatch.Draw(_keyboardTexture, keyboardRectangle, Color.White);

            #endregion

            #region Gamepad controlls

            Rectangle gamePadRectangle = new Rectangle(keyboardRectangle.Right + 20, 20 ,800, 400);

            _spriteBatch.Draw(_gamePadTexture, gamePadRectangle, Color.White);
            _spriteBatch.Draw(_gamePadOverlayTexture, gamePadRectangle, ColorHelper.HSVToRGB(new ColorHelper.HSV(gameTime.TotalGameTime.TotalSeconds * 40 % 360 , 1, 0.8)));
            
            #endregion
            #endregion

            #region For every player
            // Draw Players status
            for (int playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                // draw keyboard overlays
                    _spriteBatch.Draw(_keyboardOverlayTextures[playerIndex],
                        keyboardRectangle,
                        _players[playerIndex].Color);



                #region Back drop
                // Status rectangles
                Rectangle backdropRectangle = new Rectangle(
                    LeftMargin + playerIndex * TotalSpacer,
                    TotalTop,
                    Width,
                    Height);

                // Draw backdrop 
                _spriteBatch.DrawFilledRectangle(backdropRectangle, BackDropColor);

                #endregion

                #region status

                if (_players[playerIndex].IsActive)
                {
                    // draw game pad icon
                    _spriteBatch.Draw(_gamePadIcon,
                        new Rectangle(backdropRectangle.X + 20, backdropRectangle.Y + 20, 100, 100),
                        _players[playerIndex].PreferredInputMethod == InputMethod.GamePad ? _players[playerIndex].Color : BackDropColor); 

                    // draw keyboard icon
                    _spriteBatch.Draw(_keyboardIcon,
                        new Rectangle(backdropRectangle.X + backdropRectangle.Width - 120, backdropRectangle.Y + 20, 100, 100),
                        _players[playerIndex].PreferredInputMethod == InputMethod.Keyboard ? _players[playerIndex].Color : BackDropColor);
                }
                else
                { 
                    // Draw "inactive text"
                    _spriteBatch.DrawString(_defaultFont, "Inactive", new Vector2(backdropRectangle.X + backdropRectangle.Width / 2 - _defaultFont.MeasureString("Inactive").X / 2, backdropRectangle.Y + backdropRectangle.Height / 2), Color.LightGray);
                }

                string playerNumString = $"Player {((PlayerIndex) playerIndex).ToString()}";
                Vector2 playerNumSize = _defaultFont.MeasureString(playerNumString);
                // Draw player index number
                _spriteBatch.DrawString(_defaultFont, playerNumString, new Vector2(backdropRectangle.X + (backdropRectangle.Width - playerNumSize.X) / 2, backdropRectangle.Y + playerNumSize.Y + 4), Color.White);
                #endregion

            }
            #endregion

            _spriteBatch.End();
        }
    }
}
