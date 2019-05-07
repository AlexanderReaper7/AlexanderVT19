using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Alexander_VT19.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    class GameInstance
    {
        private static List<Model> PlayerModels;
        private static Model obstacleModel;

        private GraphicsDevice graphicsDevice;
        /// <summary>
        /// The position in world where player will spawn
        /// </summary>
        private static readonly Vector3 playerPosition = new Vector3(0, 10, 0);
        private static readonly Vector3 obstaclePosition = new Vector3(10f);

        public RenderTarget2D RenderTarget;

        //private SkyBox _skyBox;
        public Player _player;
        private Obstacle _obstacle;

        //public List<Model> Models
        //{
        //    get
        //    {
        //        List<Model> output = new List<Model>();
        //        output.Add(_player.customModel.Model);
        //        //output.Add(_obstacle.CustomModel.Model); // TODO: add obstacle to drawing models list
        //        return output;
        //    }
        //}


        public GameInstance(PlayerIndex playerIndex, GraphicsDevice graphics)
        {
            graphicsDevice = graphics;
            
            RenderTarget = new RenderTarget2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            CustomModel playerCustomModel = new CustomModel(PlayerModels[0], playerPosition, Vector3.Zero, Vector3.One * 100f, graphics);

            // Create rotation visualization models
            CustomModel[] axismodels = new CustomModel[3];
            for (int i = 0; i < 3; i++)
            {
                axismodels[i] = new CustomModel(PlayerModels[1], playerPosition, Vector3.Zero, Vector3.One * 100f, graphics);
            }

            axismodels[0].Material = new LightingMaterial() { AmbientColor = Color.Green.ToVector3(), LightColor = Color.Green.ToVector3()};
            axismodels[1].Material = new LightingMaterial() { AmbientColor = Color.Blue.ToVector3(), LightColor = Color.Blue.ToVector3()};
            axismodels[2].Material = new LightingMaterial() { AmbientColor = Color.Red.ToVector3(), LightColor = Color.Red.ToVector3()};


            // Create a new player
            _player = new Player(playerIndex, InputMethod.Keyboard, playerCustomModel, axismodels);

            // Create a new obstacle
            //CustomModel obstacleCustomModel = new CustomModel(obstacleModel, obstaclePosition, Vector3.Zero, Vector3.One, graphics);
            //_obstacle = new Obstacle(obstacleCustomModel);
        }



        public static void LoadContent(ContentManager content)
        {
            // Load Models for player
            PlayerModels = new List<Model>();
            PlayerModels.Add(content.Load<Model>(@"Models/newTest"));
            PlayerModels.Add(content.Load<Model>(@"Models/LowPolyRing"));
            obstacleModel = content.Load<Model>(@"Models/test");
        }

        public void Update(GameTime gameTime, Camera camera)
        {
            _player.Update(gameTime, camera);
        }

        /// <summary>
        /// New drawing
        /// </summary>
        /// <param name="deferredRenderer"></param>
        /// <param name="camera"></param>
        public void Draw(DeferredRenderer deferredRenderer, Camera camera)
        {
            //deferredRenderer.Draw(graphicsDevice, new List<Model> {_player.customModel.Model}, lightManager, camera, RenderTarget);
        }

        /// <summary>
        /// Old drawing (standard renderer)
        /// </summary>
        /// <param name="camera"></param>
        public void Draw(Camera camera)
        {
            _player.Draw(camera.View, camera.Projection, camera.Position);
        }
    }
}
