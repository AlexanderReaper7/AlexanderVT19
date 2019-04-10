using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAGameConsole;

namespace Alexander_VT19
{
    public abstract class Camera
    {
        private Matrix view;
        private Matrix projection;

        public Matrix Projection
        {
            get { return projection; }

            protected set
            {
                projection = value;
                generateFrustrum();
            }
        }

        public Matrix View
        {
            get { return view; }

            set
            {
                view = value;
                generateFrustrum();
            }
        }

        public Vector3 Position { get; set; }

        public BoundingFrustum Frustum { get; private set; }

        protected GraphicsDevice GraphicsDevice { get; set; }

        public Camera(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;

            GeneratePerspectiveProjectionMatrix(MathHelper.PiOver4);
        }

        private void GeneratePerspectiveProjectionMatrix(float fieldOfView)
        {
            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            float aspectRatio = (float) pp.BackBufferWidth / (float) pp.BackBufferHeight;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspectRatio, 0.1f, 1000000.0f);
        }

        public virtual void Update()
        {

        }

        private void generateFrustrum()
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