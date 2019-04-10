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

        private Player _player;
        private Obstacle _obstacle;

        public GameInstance(PlayerIndex playerIndex, GraphicsDevice graphics)
        {
            CustomModel playerCustomModel = new CustomModel(PlayerModels[0], playerPosition, Vector3.Zero, Vector3.One, graphics);
            _player = new Player(playerIndex, playerCustomModel, graphics);

            CustomModel obstacleCustomModel = new CustomModel(obstacleModel, obstaclePosition, Vector3.Zero, Vector3.One, graphics);
            _obstacle = new Obstacle(obstacleCustomModel);
        }

        public static void LoadContent(ContentManager content)
        {
            // Load Models for player
            PlayerModels = new List<Model>();
            PlayerModels.Add(content.Load<Model>("Model/test"));

        }
    }
}
