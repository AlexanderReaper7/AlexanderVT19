using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alexander_VT19
{
    public static class InGame
    {
        private static GraphicsDevice _graphics;


        private static CameraManager _cameraManager;
        private static List<GameInstance> _gameInstances;

        private static SkyBox _skyBox;

        private static Effect _simpleEffect;
        private static LightingMaterial _material;


        public static void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _graphics = graphicsDevice;

            // Create new cameras
            ChaseCamera chaseCamera = new ChaseCamera(new Vector3(0,100f,30f), Vector3.Zero, Vector3.Zero, _graphics) {Springiness = 1};
            chaseCamera.Move(Vector3.Zero, Vector3.Zero);
            FreeCamera freeCamera = new FreeCamera(_graphics, 0f, 0f, new Vector3(10f));
            _cameraManager = new CameraManager(chaseCamera, freeCamera, CameraType.Chase);

            LoadSkyBox(content);
            GameInstance.LoadContent(content);

            _simpleEffect = content.Load<Effect>("Effects/SimpleEffect");
            _material = new LightingMaterial()
            {
                AmbientColor = Color.Red.ToVector3() * .15f,
                LightColor = Color.White.ToVector3() * .85f,
            };

            StartNewGame(1);
        }

        private static void LoadSkyBox(ContentManager content)
        {
            // Load sky texture
            TextureCube skyTexture = content.Load<TextureCube>("SkyBox/clouds");
            // create new SkyBox
            _skyBox = new SkyBox(content, _graphics, skyTexture);

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
                instance.Update(gameTime);
            }
            _cameraManager.Update(gameTime, _gameInstances[0]._player);
        }




        public static void Draw()
        {
            _graphics.Clear(Color.Black);

            foreach (GameInstance instance in _gameInstances)
            {
                //_graphics.SetRenderTarget(instance.RenderTarget);
                _skyBox.Draw(_cameraManager.Camera.View, _cameraManager.Camera.Projection, _cameraManager.Camera.Position);
                instance.Draw(_cameraManager.Camera.View, _cameraManager.Camera.Projection, _cameraManager.Camera.Position);
            }
        }
    }
}
