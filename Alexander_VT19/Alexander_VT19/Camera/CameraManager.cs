using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Alexander_VT19
{
    public enum Cameras : byte
    {
        Free,
        Static1
    }

    /// <summary>
    /// Manages camera movement etc.
    /// </summary>
    public class CameraManager
    {
        private ChaseCamera _chaseCamera;
        private FreeCamera _freeCamera;
        private StaticCamera _staticCamera1;

        /// <summary>
        /// Currently active camera
        /// </summary>
        public Cameras SelectedCamera { get; set; }

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
                    case Cameras.Free:
                        return _freeCamera;
                    case Cameras.Static1:
                        return _staticCamera1;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public CameraManager( FreeCamera freeCamera, StaticCamera stc1, Cameras initialSelectedCamera)
        {
            _freeCamera = freeCamera;
            _staticCamera1 = stc1;
            SelectedCamera = initialSelectedCamera;
            _lastMouseState = Mouse.GetState();
        }

        public void Update(GameTime gameTime)
        {
            // Update selected 
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) SelectedCamera = Cameras.Free;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) SelectedCamera = Cameras.Static1;

            // Update the selected camera´s relevant methods
            switch (SelectedCamera)
            {
                case Cameras.Free:
                    UpdateFreeCamera(_freeCamera, _lastMouseState, gameTime);
                    break;
                case Cameras.Static1:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void UpdateChaseCamera(ChaseCamera camera, CustomModel targetModel)
        {
            // Move camera position and rotation relative to box 
            camera.Move(targetModel.Position, Vector3.Zero);

            // Update camera
            camera.Update();
        }

        /// <summary>
        /// Updates Free camera movement
        /// </summary>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="camera"></param>
        private void UpdateFreeCamera(FreeCamera camera, MouseState lastMouseState, GameTime gameTime)
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

            _lastMouseState = mouseState;
        }
    }
}
