using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public class ChaseCamera : Camera
    {
        public Vector3 FollowTargetPosition { get; private set; }
        public Vector3 FollowTargetRotation { get; private set; }

        public Vector3 PositionOffset { get; set; }
        public Vector3 TargetOffset { get; set; }

        public Vector3 RelativeCameraRotation { get; set; }


        private float _springiness = .15f;

        public float Springiness
        {
            get { return _springiness; }
            set { _springiness = MathHelper.Clamp(value, 0, 1); }
        }

        public ChaseCamera(Vector3 positionOffset, Vector3 targetOffset, Vector3 relativeCameraRotation, GraphicsDevice graphicsDevice) 
            : base(graphicsDevice)
        {
            PositionOffset = positionOffset;
            TargetOffset = targetOffset;
            RelativeCameraRotation = relativeCameraRotation;
        }

        public void Move(Vector3 newFollowTargetPosition, Vector3 newFollowTargetRotation)
        {
            FollowTargetPosition = newFollowTargetPosition;
            FollowTargetRotation = newFollowTargetRotation;
        }

        public void Rotate(Vector3 rotationChange)
        {
            RelativeCameraRotation = rotationChange;
        }

        public override void Update()
        {
            Vector3 combinedRotation = FollowTargetRotation + RelativeCameraRotation;

            // Calculate rotation matrix for camera
            Matrix rotation = Matrix.CreateFromYawPitchRoll(combinedRotation.Y, combinedRotation.X, combinedRotation.Z);

            Vector3 desiredPosition = FollowTargetPosition + Vector3.Transform(PositionOffset, rotation);

            // Interpolate between desiredPosition and Position
            Position = Vector3.Lerp(Position, desiredPosition, Springiness);

            // Calculate new target from rotation matrix
            Target = FollowTargetPosition + Vector3.Transform(TargetOffset, rotation);

            // Set vector for matrix
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            View = Matrix.CreateLookAt(Position, Target, up);
        }
    }
}
