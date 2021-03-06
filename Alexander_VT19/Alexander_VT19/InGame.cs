﻿using System;
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
        private static GraphicsDevice _graphics;

        //private static DeferredRenderer deferredRenderer;
        //private static SSAO ssao;
        //private static RenderTarget2D scene;
        //private static SpriteFont defaultFont;
        //private static LightManager lightManager;

        private static CameraManager _cameraManager;

        private static List<GameInstance> _gameInstances;

        private static SkyBox _skyBox;


        public static void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _graphics = graphicsDevice;

            //deferredRenderer = new DeferredRenderer(graphicsDevice, content, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            //lightManager = new LightManager(content);
            //ssao = new SSAO(graphicsDevice,content, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
            //scene = new RenderTarget2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            //lightManager.AddLight(new SpotLight(graphicsDevice,Vector3.One * 10f, Vector3.Backward, Color.White.ToVector4(),0.2f, true,2048, null));

            LoadCamera(content);
            LoadSkyBox(content);
            GameInstance.LoadContent(content);


            StartNewGame(1);
        }

        private static void LoadSkyBox(ContentManager content)
        {
            // Load sky texture
            TextureCube skyTexture = content.Load<TextureCube>("SkyBox/clouds");
            // Create new SkyBox
            _skyBox = new SkyBox(content, _graphics, skyTexture);
        }

        private static void LoadCamera(ContentManager content)
        {
            // Create new cameras
            ChaseCamera chaseCamera = new ChaseCamera(new Vector3(0, 100f, 30f), Vector3.Zero, Vector3.Zero, _graphics) { Springiness = 1 };
            chaseCamera.Move(Vector3.Zero, Vector3.Zero);
            FreeCamera freeCamera = new FreeCamera(_graphics, 0f, 0f, new Vector3(10f));
            _cameraManager = new CameraManager(chaseCamera, freeCamera, CameraType.Chase);
        }

        /// <summary>
        /// Entry point for starting a new game, creates new players etc
        /// </summary>
        /// <param name="numPlayers"></param>
        public static void StartNewGame(int numPlayers)
        {
            _gameInstances = new List<GameInstance>(numPlayers);
            for (int i = 0; i < numPlayers; i++)
            {
                _gameInstances.Add(new GameInstance((PlayerIndex)i, _graphics));
            }
        }

        public static void Update(GameTime gameTime)
        {
            foreach (GameInstance instance in _gameInstances)
            {
                instance.Update(gameTime, _cameraManager.Camera);
            }
            _cameraManager.Update(gameTime, _gameInstances[0]._player);
        }




        public static void Draw()
        {
            _graphics.Clear(Color.Black);
            RenderTarget2D[] screens = new RenderTarget2D[_gameInstances.Count];
            // Render each instance 
            for (int i = 0; i < _gameInstances.Count; i++)
            {
                _graphics.SetRenderTarget(_gameInstances[i].RenderTarget);
                _skyBox.Draw(_cameraManager.Camera.View, _cameraManager.Camera.Projection, _cameraManager.Camera.Position);
                _gameInstances[i].Draw(_cameraManager.Camera);
                // Collect every instance´s RenderTarget
                screens[i] = _gameInstances[i].RenderTarget;
            }

            _graphics.SetRenderTarget(null);
            SplitScreenHelper.DrawSplitScreen(_graphics, screens);
        }
    }
}
