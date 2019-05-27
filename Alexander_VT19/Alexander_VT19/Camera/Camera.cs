using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public abstract class Camera
    {
        private Matrix _view;
        private Matrix _projection;

        public Matrix Projection
        {
            get { return _projection; }

            protected set
            {
                _projection = value;
                GenerateFrustum();
            }
        }

        public Matrix View
        {
            get { return _view; }

            protected set
            {
                _view = value;
                GenerateFrustum();
            }
        }

        public float AspectRatio { get; private set; }

        public float NearClip { get; set; }
        public float FarClip { get; set; }

        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        /// <summary>
        /// Coordinate the camera should face
        /// </summary>
        public Vector3 Target { get; protected set; }

        public BoundingFrustum Frustum { get; private set; }

        protected GraphicsDevice GraphicsDevice { get; set; }

        public Camera(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;

            GeneratePerspectiveProjectionMatrix(MathHelper.PiOver4);
        }

        private void GeneratePerspectiveProjectionMatrix(float fieldOfView, float near = 0.1f, float far = 1000000.0f)
        {
            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            AspectRatio = (float) pp.BackBufferWidth / (float) pp.BackBufferHeight;

            NearClip = near;
            FarClip = far;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), AspectRatio, NearClip, FarClip);
        }

        public virtual void Update()
        {

        }

        private void GenerateFrustum()
        {
            Matrix viewProjection = View * Projection;
            Frustum = new BoundingFrustum(viewProjection);
        }

        public bool BoundingVolumeIsInView(BoundingSphere sphere)
        {
            return (Frustum.Contains(sphere) != ContainmentType.Disjoint);
        }

        public bool BoundingVolumeIsInView(BoundingBox box)
        {
            return (Frustum.Contains(box) != ContainmentType.Disjoint);
        }


    }
}