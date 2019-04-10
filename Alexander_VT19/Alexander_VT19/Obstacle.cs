using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    class Obstacle
    {
        public CustomModel CustomModel;
        public Vector3 CorrectRotation { get; private set; }

        public Obstacle(CustomModel model)
        {
            CustomModel = model
        }

        /// <summary>
        /// Generates a new random obstacle for the respective player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Obstacle GenerateNewObstacle(Player player)
        {

        }
    }
}
