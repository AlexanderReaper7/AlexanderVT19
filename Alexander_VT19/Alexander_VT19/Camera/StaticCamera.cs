using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public enum ProjectionMatrixType
    {
        Perspective,
        Orthographic
    }


    public class StaticCamera : Camera
    {
        public StaticCamera(Vector3 position, Vector3 target, GraphicsDevice graphics, ProjectionMatrixType projectionType) : base(graphics)
        {
            Position = position;
            Target = target;
            View = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            if (projectionType == ProjectionMatrixType.Orthographic)
            {
                Projection = Matrix.CreateOrthographic(1920 / 13, 1080 / 13, 0.0001f, 1000000.0f); //TODO
            }
        }
    }
}
