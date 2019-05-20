using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public class StaticCamera : Camera
    {
        public StaticCamera(Vector3 position, Vector3 target, GraphicsDevice graphics) : base(graphics)
        {
            Position = position;
            Target = target;
            View = Matrix.CreateLookAt(Position, Target, Vector3.Up);
        }
    }
}
