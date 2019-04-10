using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Alexander_VT19
{
    public enum CameraType : byte
    {
        Chase,
        Free
    }

    /// <summary>
    /// Manages camera movement etc.
    /// </summary>
    public class CameraManager
    {
        private ChaseCamera _chaseCamera;
        private FreeCamera _freeCamera;

        /// <summary>
        /// Currently active camera
        /// </summary>
        public CameraType SelectedCamera { get; set; }

        private MouseState _lastMouseState;

        /// <summary>
        /// Returns currently active camera
        /// </summary>
        public Camera Camera
        {
            get
            {
                switch (SelectedCamera)
                {
                    case CameraType.Chase:
                        return _chaseCamera;
                    case CameraType.Free:
                        return _freeCamera;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public CameraManager(ChaseCamera chaseCamera, FreeCamera freeCamera, CameraType initialSelectedCamera)
        {
            _chaseCamera = chaseCamera;
            _freeCamera = freeCamera;
            SelectedCamera = initialSelectedCamera;
            _lastMouseState = Mouse.GetState();
        }

        public void Update(GameTime gameTime, Player player)
        {
            // Update selected 
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) SelectedCamera = CameraType.Chase;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) SelectedCamera = CameraType.Free;

            // Update the selected camera´s relevant methods
            switch (SelectedCamera)
            {
                case CameraType.Chase:
                    UpdateChaseCamera(_chaseCamera, player.customModel);
                    break;
                case CameraType.Free:
                    UpdateFreeCamera(_freeCamera, _lastMouseState, gameTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private static void UpdateChaseCamera(ChaseCamera camera, CustomModel targetModel)
        {
            // Move camera position and rotation relative to box 
            camera.Move(targetModel.Position, targetModel.Rotation);

            // Update camera
            camera.Update();
        }

        /// <summary>
        /// Updates Free camera movement
        /// </summary>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="camera"></param>
        private static void UpdateFreeCamera(FreeCamera camera, MouseState lastMouseState, GameTime gameTime)
        {
            // Get mouse and keyboard state
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            // Calculate how much the camera should rotate
            float deltaX = lastMouseState.X - mouseState.X;
            float deltaY = lastMouseState.Y - mouseState.Y;

            // Rotate camera
            camera.Rotate(deltaX * 0.01f, deltaY * 0.01f);

            Vector3 translation = Vector3.Zero;
            // Get camera movement
            if (keyState.IsKeyDown(Keys.W)) translation += Vector3.Forward * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (keyState.IsKeyDown(Keys.S)) translation += Vector3.Backward * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (keyState.IsKeyDown(Keys.A)) translation += Vector3.Left * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (keyState.IsKeyDown(Keys.D)) translation += Vector3.Right * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (keyState.IsKeyDown(Keys.Space)) translation += Vector3.Up * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (keyState.IsKeyDown(Keys.LeftShift)) translation += Vector3.Down * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Move camera
            camera.Move(translation);

            // Update camera
            camera.Update();
        }
    }
}
