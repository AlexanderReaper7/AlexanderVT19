using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    class StaticCamera : Camera
    {
        public StaticCamera(Vector3 position, Vector3 target, GraphicsDevice graphics) : base(graphics)
        {
            Position = position;
            Target = target;
        }
        
        public override void Update()
        {
            // Calculate view matrix
            View = Matrix.CreateLookAt(Position, Target, Vector3.Up);
        }

    }
}
