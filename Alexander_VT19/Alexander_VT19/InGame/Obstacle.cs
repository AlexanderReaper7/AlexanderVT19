using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexander_VT19;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public struct Challenge
    {
        public Vector3 DesiredRotation;
        public float Margin;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desiredRotation">Rotation Vector in radians</param>
        /// <param name="margin">Margin of error</param>
        public Challenge(Vector3 desiredRotation, float margin)
        {
            DesiredRotation = desiredRotation;
            Margin = margin;
        }

        /// <summary>
        /// Creates a new random Challenge with the specified margin
        /// </summary>
        /// <param name="margin"></param>
        public Challenge(float margin)
        {
            Random r = new Random();
            Vector3 v = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
            v *= (float)Math.PI * 2f;
            DesiredRotation = v;
            Margin = margin;
        }
    }

    class Obstacle
    {
        public CustomModel CustomModel;
        public Vector3 CorrectRotation { get; private set; }

        public Obstacle(CustomModel model)
        {
            CustomModel = model;
        }

        /// <summary>
        /// Generates a new random obstacle for the respective player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Obstacle GenerateNewObstacle(Player player)
        {
            return null;
        }
    }
}
