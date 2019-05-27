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
        //private static SkyBox _skyBox;

        public static void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _contentManager = content;

            //deferredRenderer = new DeferredRenderer(graphicsDevice, content, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            //lightManager = new LightManager(content);
            //ssao = new SSAO(graphicsDevice,content, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
            //scene = new RenderTarget2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            //lightManager.AddLight(new SpotLight(graphicsDevice,Vector3.One * 10f, Vector3.Backward, Color.White.ToVector4(),0.2f, true,2048, null));

            LoadCamera(content);
            LoadSkyBox(content);


        }

        private static void LoadSkyBox(ContentManager content)
        {
            // Load sky texture
            TextureCube skyTexture = content.Load<TextureCube>("SkyBox/clouds");
            // Create new SkyBox
            _skyBox = new NeonSkybox(content, _graphicsDevice);
            //_skyBox = new SkyBox(content, _graphicsDevice, skyTexture);
        }

        private static void LoadCamera(ContentManager content)
        {
            // Create new cameras
            FreeCamera freeCamera = new FreeCamera(_graphicsDevice, 0f, 0f, new Vector3(10f));
            StaticCamera stc1 = new StaticCamera(Vector3.Backward * 600f, Vector3.Zero, _graphicsDevice);
            _cameraManager = new CameraManager(freeCamera, stc1, Cameras.Static1);
        }


        /// <summary>
        /// Entry point for starting a new game, creates new game instances
        /// </summary>
        /// <param name="playerDatas"></param>
        public static void StartNewGame(PlayerSelectMenu.PlayerData[] playerDatas)
        {
            // Parse playerdata
            // Remove inactive players
            List<int> activePlayers = new List<int>();
            for (int i = 0; i < playerDatas.Length; i++)
            {
                if (playerDatas[i].IsActive) activePlayers.Add(i);
            }

            _gameInstances = new List<GameInstance>(activePlayers.Count);
            foreach (int p in activePlayers)
            {
                _gameInstances.Add(new GameInstance(playerDatas[p], _graphicsDevice, _contentManager));
            }
        }

        public static void Update(GameTime gameTime)
        {
            _skyBox.Update();

            // Update instances
            foreach (GameInstance instance in _gameInstances)
            {
                instance.Update(gameTime);
            }
            // Update camera
            _cameraManager.Update(gameTime);
            // Update Challenge

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

                _gameInstances[i].Draw(_cameraManager.Camera);

                // Collect every instance´s RenderTarget
                screens[i] = _gameInstances[i].RenderTarget;
            }

            // Set render target to the back buffer
            _graphicsDevice.SetRenderTarget(null);
            // Draw The screens
            SplitScreenHelper.DrawSplitScreen(_graphicsDevice, screens);
        }
    }
}
