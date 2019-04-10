using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexander_VT19;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools_MultipleCamera
{
    public class FreeCamera : Camera
    {
        /// <summary>
        /// Rotation around Z axis
        /// </summary>
        public float Yaw { get; set; }
        /// <summary>
        /// Rotation around Y axis
        /// </summary>
        public float Pitch { get; set; }

        /// <summary>
        /// Coordinate the camera should face
        /// </summary>
        public Vector3 Target { get; set; }

        /// <summary>
        /// Change in position this update
        /// </summary>
        private Vector3 _translation;

        /// <summary>
        /// Instantiates a new FreeCamera
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="position"></param>
        public FreeCamera(GraphicsDevice graphicsDevice, float yaw, float pitch, Vector3 position) : base(graphicsDevice)
        {
            Yaw = yaw;
            Pitch = pitch;
            Position = position;

            _translation = Vector3.Zero;
        }

        /// <summary>
        /// Add rotation
        /// </summary>
        /// <param name="yawChange"></param>
        /// <param name="pitchChange"></param>
        public void Rotate(float yawChange, float pitchChange)
        {
            Yaw += yawChange;
            Pitch += pitchChange;
        }
        /// <summary>
        /// Adds a change in position this update,
        /// moves camera
        /// </summary>
        /// <param name="translation"></param>
        public void Move(Vector3 translation)
        {
            _translation += translation;
        }

        public override void Update()
        {
            // Calculate rotation matrix
            Matrix rotation = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, 0);
            // Transform _translation with rotation
            _translation = Vector3.Transform(_translation,  rotation);
            // Move position
            Position += _translation;
            // Reset _translation
            _translation = Vector3.Zero;

            // Calculate forward vector
            Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
            // Set new target
            Target = Position + forward;

            // Calculate up vector 
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            // Calculate view matrix
            View = Matrix.CreateLookAt(Position, Target, up);
        }
    }
}
