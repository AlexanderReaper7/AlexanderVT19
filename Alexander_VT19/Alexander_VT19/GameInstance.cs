using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    class GameInstance
    {
        private static List<Model> PlayerModels;
        private static Model obstacleModel;
        /// <summary>
        /// The position in world where player will spawn
        /// </summary>
        private static readonly Vector3 playerPosition = new Vector3(0, 10, 0);
        private static readonly Vector3 obstaclePosition = new Vector3(10f);



        public RenderTarget2D RenderTarget;

        public Player _player;
        private Obstacle _obstacle;

        public GameInstance(PlayerIndex playerIndex, GraphicsDevice graphics)
        {
            CustomModel playerCustomModel = new CustomModel(PlayerModels[2], playerPosition, Vector3.Zero, Vector3.One * 100f, graphics);

            CustomModel[] axismodels = new CustomModel[3];
            for (int i = 0; i < 3; i++)
            {
                axismodels[i] = new CustomModel(PlayerModels[1], playerPosition, Vector3.Zero, Vector3.One * 100f, graphics);
            }

            axismodels[0].Material = new LightingMaterial() { AmbientColor = Color.Green.ToVector3(), LightColor = Color.Green.ToVector3()};
            axismodels[1].Material = new LightingMaterial() { AmbientColor = Color.Blue.ToVector3(), LightColor = Color.Blue.ToVector3()};
            axismodels[2].Material = new LightingMaterial() { AmbientColor = Color.Red.ToVector3(), LightColor = Color.Red.ToVector3()};

            _player = new Player(playerIndex, InputMethod.Keyboard, playerCustomModel, axismodels);

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

        public void Draw(Matrix cameraView, Matrix cameraProjection, Vector3 cameraPosition)
        {
            
            _player.Draw(cameraView,cameraProjection,cameraPosition);
        }
    }
}
