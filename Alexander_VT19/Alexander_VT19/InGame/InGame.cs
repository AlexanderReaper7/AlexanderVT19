using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Alexander_VT19.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alexander_VT19
{
    public static class InGame
    {
        private static GraphicsDevice _graphicsDevice;
        private static ContentManager _contentManager;
        private static ExtendedSpriteBatch _spriteBatch;
        private static SpriteFont _defaultFont;

        #region DeferredRenderer

        //private static DeferredRenderer deferredRenderer;
        //private static SSAO ssao;
        //private static RenderTarget2D scene;
        //private static SpriteFont defaultFont;
        //private static LightManager lightManager;
        

        #endregion

        private static CameraManager _cameraManager;
        private static List<GameInstance> _gameInstances;
        private static NeonSkybox _skyBox;
        private static double _gametimer; //TODO rename
        private static Tuple<PlayerData?, double> _bestScore;

        public static bool IsGameOver;



        public static void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _contentManager = content;
            _spriteBatch = new ExtendedSpriteBatch(graphicsDevice);
            _defaultFont = content.Load<SpriteFont>(@"DefaultFont");

            //deferredRenderer = new DeferredRenderer(graphicsDevice, content, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
            //lightManager = new LightManager(content);
            //ssao = new SSAO(graphicsDevice,content, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
            //scene = new RenderTarget2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            //lightManager.AddLight(new SpotLight(graphicsDevice,Vector3.One * 10f, Vector3.Backward, Color.White.ToVector4(),0.2f, true,2048, null));

            LoadCamera();
            LoadSkyBox(content);
        }



        private static void LoadSkyBox(ContentManager content)
        {
            // Load sky texture
            //TextureCube skyTexture = content.Load<TextureCube>("SkyBox/clouds");
            //_skyBox = new SkyBox(content, _graphicsDevice, skyTexture);

            // Create new SkyBox
            _skyBox = new NeonSkybox(content, _graphicsDevice);
        }



        private static void LoadCamera()
        {
            // Create new cameras
            FreeCamera freeCamera = new FreeCamera(_graphicsDevice, 0f, 0f, new Vector3(10f));
            StaticCamera stc1 = new StaticCamera(Vector3.Backward * 600f, Vector3.Zero, _graphicsDevice, ProjectionMatrixType.Perspective);
            _cameraManager = new CameraManager(freeCamera, stc1, Cameras.Static1);
        }



        /// <summary>
        /// Entry point for starting a new game, creates new game instances
        /// </summary>
        /// <param name="gameData"></param>
        public static void StartNewGame(GameData gameData)
        {
            // Parse gameData
            // Remove inactive players
            List<int> activePlayers = new List<int>();
            for (int i = 0; i < gameData.PlayerDatas.Length; i++)
            {
                if (gameData.PlayerDatas[i].IsActive) activePlayers.Add(i);
            }
            // Create new game instances for every active player
            _gameInstances = new List<GameInstance>(activePlayers.Count);
            foreach (int p in activePlayers)
            {
                _gameInstances.Add(new GameInstance(gameData.PlayerDatas[p], _graphicsDevice, _contentManager));
            }

            // Reset variables
            _bestScore = new Tuple<PlayerData?, double>(null, 0);
            _gametimer = gameData.GameDuration;
            IsGameOver = false;
        }



        private static void GameOver()
        {
            IsGameOver = true;

            foreach (GameInstance instance in _gameInstances)
            {
                if (instance.Score > _bestScore.Item2)
                {
                    _bestScore = new Tuple<PlayerData?, double>(instance.PlayerData, instance.Score);
                }
            }
        }



        public static void Update(GameTime gameTime)
        {
            // Update skybox
            _skyBox.Update();

            if (!IsGameOver)
            {
                // Update instances
                foreach (GameInstance instance in _gameInstances)
                {
                    instance.Update(gameTime);
                }

                // Update camera
                _cameraManager.Update(gameTime);

                // if time has run out,
                if (_gametimer <= 0)
                {
                    // game over.
                    GameOver();
                }
                else
                {
                    // else subtract elapsed time from gametimer
                    _gametimer -= gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            else
            {
                // Press enter to continue
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    PlayerSelectMenu.StartNewSelection();
                    Game1.GameState = GameStates.PlayerSelection;
                }
            }
        }




        public static void Draw(GameTime gameTime)
        {
            // Clear the graphics device
            _graphicsDevice.Clear(Color.Black);
            // Create a new array to collect the _gameInstances render targets in once rendered
            RenderTarget2D[] screens = new RenderTarget2D[_gameInstances.Count];

            // Render each instance 
            for (int i = 0; i < _gameInstances.Count; i++)
            {
                // Set render target to instance
                _graphicsDevice.SetRenderTarget(_gameInstances[i].RenderTarget);
                _graphicsDevice.Clear(Color.Black);

                // Draw The instance
                _skyBox.Draw(_cameraManager.Camera, gameTime);

                if (!IsGameOver) _gameInstances[i].Draw(_cameraManager.Camera);
                else _gameInstances[i].DrawGameOver();

                // Collect every instance´s RenderTarget
                screens[i] = _gameInstances[i].RenderTarget;
            }

            // Set render target to the back buffer
            _graphicsDevice.SetRenderTarget(null);
            // Draw The screens
            SplitScreenHelper.DrawSplitScreen(_graphicsDevice, screens);

            _spriteBatch.Begin();
            if (IsGameOver)
            {

                Vector2 screenSize = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

                // Draw Best score
                string bestScoreString = _bestScore.Item1.HasValue
                    ? $"Best Score: Player {_bestScore.Item1.Value.PlayerIndex.ToString()} with {Math.Round(_bestScore.Item2, 2)} points!"
                    : "No one got any points! Everyone sucks!";
                _spriteBatch.DrawString(_defaultFont,
                    bestScoreString,
                    (screenSize - _defaultFont.MeasureString(bestScoreString)) / 2, Color.White);

                // Draw "press Enter to continue
                string continueString = "Press Enter or Start to continue ";
                _spriteBatch.DrawString(_defaultFont, continueString, screenSize - _defaultFont.MeasureString(continueString), Color.White);

            }
            else
            {
                _spriteBatch.DrawString(_defaultFont, $"Game time left:{Math.Round(_gametimer, 2).ToString()}", Vector2.Zero, Color.White);
            }
            _spriteBatch.End();
        }
    }
}
