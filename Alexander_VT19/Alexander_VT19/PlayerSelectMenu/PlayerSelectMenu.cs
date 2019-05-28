using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Alexander_VT19.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alexander_VT19
{

    public struct GameData
    {
        public PlayerData[] PlayerDatas;
        public double GameDuration;
    }

    public struct PlayerData
    {
        public InputMethod PreferredInputMethod;
        public PlayerIndex PlayerIndex;
        public bool IsActive;
        public Color Color;
        public Difficulty Difficulty;
    }

    public struct Difficulty
    {
        public float Margin;
        public float TimeLimit;

        public static Difficulty Easy => new Difficulty() { Margin = 0.25f, TimeLimit = 60 };
        public static Difficulty Medium => new Difficulty() { Margin = 0.2f, TimeLimit = 60 };
        public static Difficulty Hard => new Difficulty() { Margin = 0.15f, TimeLimit = 90 };
        public static Difficulty Ultra => new Difficulty() { Margin = 0.1f, TimeLimit = 120 };
    }

    public static class PlayerSelectMenu
    {
        private static GraphicsDevice _graphicsDevice;
        private static ExtendedSpriteBatch _spriteBatch;
        private static QuadRenderer _quadRenderer;
        private static SpriteFont _defaultFont;
        private static NeonBackground _neonBackground;

        private static GameData _gameData;
        private static Player[] _players;
        private static StaticCamera _camera;

        private static Texture2D[] _keyboardOverlayTextures;
        private static Texture2D _keyboardTexture;
        private static Texture2D _gamePadTexture;
        private static Texture2D _gamePadOverlayTexture;
        private static Texture2D _gamePadIcon;
        private static Texture2D _keyboardIcon;
        private static Texture2D _aButton;
        private static Texture2D _instructionsTexture;
        private static bool _enableInstructionsOverlay;

        public static void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new ExtendedSpriteBatch(graphicsDevice);
            _quadRenderer = new QuadRenderer(graphicsDevice);

            // Load font
            _defaultFont = content.Load<SpriteFont>(@"DefaultFont");

            // Load neon background
            _neonBackground = NeonBackground.CreateNew(content);
            _neonBackground.Resolution = new Vector2(3840, 2160);

            // Load simple Effect
            Effect effect = content.Load<Effect>(@"Effects/LightingEffect");

            Vector3[] playerPositions = new Vector3[]
            {
                new Vector3(55,-20,0),
                new Vector3(20,-20,0),
                new Vector3(-20,-20,0), 
                new Vector3(-55,-20,0), 
            };

            // Create players
            if (GameInstance.PlayerModels == null) GameInstance.LoadContent(content);
            _players = new Player[4];
            for (int i = 0; i < 4; i++)
            {
                _players[i] = new Player((PlayerIndex)i,
                    InputMethod.Keyboard,
                    new CustomModel(GameInstance.PlayerModels[0], playerPositions[i], Vector3.Zero, Vector3.One * 4, graphicsDevice));
                _players[i].CustomModel.SetModelEffect(effect, false);
                _players[i].CustomModel.Material = new LightingMaterial();
            }

            // Create camera
            _camera = new StaticCamera(Vector3.Forward * 15, Vector3.Zero, graphicsDevice, ProjectionMatrixType.Orthographic);

            // Load Textures
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
            _aButton = content.Load<Texture2D>(@"Images/AButton");
            _instructionsTexture = content.Load<Texture2D>(@"Images/Instructions");
        }



        public static void Update(GameTime gameTime)
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
                        _gameData.PlayerDatas[playerIndex].PreferredInputMethod = InputMethod.Keyboard;
                        _gameData.PlayerDatas[playerIndex].IsActive = true;
                    }
                }
            }

            if (keyboardState.IsKeyDown(Keys.Back)) StartNewSelection(); // Backspace to reset selection
            if (keyboardState.IsKeyDown(Keys.Escape)) Game1.GameState = GameStates.Exit; // Press escape to exit
            if (keyboardState.IsKeyDown(Keys.Enter)) StartGame(); // Press enter to start game with current settings
            _enableInstructionsOverlay = keyboardState.IsKeyDown(Keys.Space); // Hold Enter to view Instructions

            #endregion

            #region Gamepad

            // Get GamePad Input
            for (int playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                GamePadState gamePadState = GamePad.GetState((PlayerIndex) playerIndex);

                // Activate player on A button down
                if (gamePadState.IsButtonDown(Buttons.A))
                {
                    _gameData.PlayerDatas[playerIndex].PreferredInputMethod = InputMethod.GamePad;
                    _gameData.PlayerDatas[playerIndex].IsActive = true;
                }
                // Disable player on B button down
                else if (gamePadState.IsButtonDown(Buttons.B))
                {
                    _gameData.PlayerDatas[playerIndex].PreferredInputMethod = InputMethod.GamePad;
                    _gameData.PlayerDatas[playerIndex].IsActive = false;
                }

                // Start game on Start button down
                if (gamePadState.IsButtonDown(Buttons.Start))
                {
                    StartGame();
                }

                // Dpad up to show insrtuctions
                if (gamePadState.IsButtonDown(Buttons.DPadUp))
                {
                    _enableInstructionsOverlay = true;
                }
            }

            #endregion

            #region Players

            for (int i = 0; i < _players.Length; i++)
            {
                // If the player is active
                if (_gameData.PlayerDatas[i].IsActive)
                {
                    // Update player
                    _players[i].PreferredInputMethod = _gameData.PlayerDatas[i].PreferredInputMethod;
                    _players[i].Update(gameTime);
                }
            }

            #endregion

        }



        public static void StartNewSelection()
        {
            // Create new GameData
            _gameData = new GameData()
            {
                GameDuration = 120,
                PlayerDatas = new PlayerData[4],

            };

            // Create new array of player data
            for (int i = 0; i < 4; i++)
            {
                _gameData.PlayerDatas[i] = new PlayerData
                {
                        PlayerIndex = (PlayerIndex)i,
                        IsActive = false,
                        Color = ColorHelper.HSVToRGB(new ColorHelper.HSV(360 / 4 * i, 1, 0.5)),
                        Difficulty = Difficulty.Easy
                };
            }

        }




        private static void StartGame()
        {
            foreach (PlayerData data in _gameData.PlayerDatas)
            {
                if (data.IsActive)
                {
                    Game1.GameState = GameStates.InGame;
                    InGame.StartNewGame(_gameData);
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

            // Credits
            _spriteBatch.DrawString(_defaultFont,"  Made By: Alexander Oberg ", Vector2.Zero, Color.White);

            #region Instructions

            #region keyboard controlls

            // draw keyboard
            Rectangle keyboardRectangle = new Rectangle(20, 20, 850, 300);
            _spriteBatch.Draw(_keyboardTexture, keyboardRectangle, Color.White);

            // draw keyboard overlays
            for (int i = 0; i < 4; i++)
            {
                _spriteBatch.Draw(_keyboardOverlayTextures[i],
                    keyboardRectangle,
                    _gameData.PlayerDatas[i].Color);
            }

            #endregion

            #region Gamepad controlls

            // Draw gamepad
            Rectangle gamePadRectangle = new Rectangle(keyboardRectangle.Right + 20, 20 ,800, 400);
            _spriteBatch.Draw(_gamePadTexture, gamePadRectangle, Color.White);
            // Draw gamepad overlay
            _spriteBatch.Draw(_gamePadOverlayTexture, gamePadRectangle, ColorHelper.HSVToRGB(new ColorHelper.HSV(gameTime.TotalGameTime.TotalSeconds * 40 % 360 , 1, 0.8)));
            
            #endregion

            Rectangle instructionsRectangle = new Rectangle(LeftMargin, 20, ScreenWidth - 2* LeftMargin, ScreenHeight / 2);
            _spriteBatch.DrawFilledRectangle(instructionsRectangle, BackDropColor);


            string activateKey =
                "Press A on the gamepad or\nany active key on the keyboard to activate player";
            _spriteBatch.DrawString(_defaultFont, activateKey, new Vector2(instructionsRectangle.X, instructionsRectangle.Y) + new Vector2(600, 350), Color.White);

            string deactivateKey = "Press B on the gamepad or\nbackspace key on the keyboard to deactivate player";
            _spriteBatch.DrawString(_defaultFont, deactivateKey, new Vector2(instructionsRectangle.X, instructionsRectangle.Y) + new Vector2(600, 450), Color.White);
            // Draw "press ESC or back to exit"
            string backString = " Press 'ESC' or 'Back' to exit ";
            _spriteBatch.DrawString(_defaultFont, backString, new Vector2(0, ScreenHeight - _defaultFont.MeasureString(backString).Y), Color.White);

            // Draw "press ENTER or START to start game"
            string playString = " Press 'ENTER' or 'Start' to start the game ";
            _spriteBatch.DrawString(_defaultFont, playString, new Vector2(ScreenWidth, ScreenHeight) - _defaultFont.MeasureString(playString), Color.White);

            // Draw instructions tooltip
            string instructionsString = "Hold 'SpaceBar' or 'D-Pad Up' to view instructions";
            _spriteBatch.DrawString(_defaultFont, instructionsString,new Vector2((ScreenWidth - 275 - _defaultFont.MeasureString(playString).X) /2,ScreenHeight - _defaultFont.MeasureString(playString).Y - 20), Color.White, 0, Vector2.Zero, Vector2.One * 1.3f, SpriteEffects.None,0);




            if (_enableInstructionsOverlay) _spriteBatch.Draw(_instructionsTexture, new Rectangle(0,0,ScreenWidth,ScreenHeight), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);
            
            #endregion

            #region For every player
            // Draw Players status
            for (int playerIndex = 0; playerIndex < 4; playerIndex++)
            {



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

                if (_gameData.PlayerDatas[playerIndex].IsActive)
                {
                    // draw game pad icon
                    _spriteBatch.Draw(_gamePadIcon,
                        new Rectangle(backdropRectangle.X + 20, backdropRectangle.Y + 20, 100, 100),
                        _gameData.PlayerDatas[playerIndex].PreferredInputMethod == InputMethod.GamePad ? _gameData.PlayerDatas[playerIndex].Color : BackDropColor); 

                    // draw keyboard icon
                    _spriteBatch.Draw(_keyboardIcon,
                        new Rectangle(backdropRectangle.X + backdropRectangle.Width - 120, backdropRectangle.Y + 20, 100, 100),
                        _gameData.PlayerDatas[playerIndex].PreferredInputMethod == InputMethod.Keyboard ? _gameData.PlayerDatas[playerIndex].Color : BackDropColor);
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

                #region Model

                if (_gameData.PlayerDatas[playerIndex].IsActive)
                {
                    _players[playerIndex].Draw(_camera);
                }

                #endregion
            }
            #endregion

            _spriteBatch.End();
        }
    }
}
