using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alexander_VT19
{
    public class PlayerManager
    {
        private List<Player> Players;
        private static List<CustomModel> playerModels;

        public PlayerManager(int numPlayers, ContentManager content, GraphicsDevice graphics)
        {
            // Load all player models
            playerModels = new List<CustomModel>();
            playerModels.Add(new CustomModel(content.Load<Model>("Models/test"), Vector3.Zero, Vector3.Zero, Vector3.One, graphics));
            CustomModel initialModel = playerModels[0];
            // Create players
            Players = new List<Player>(numPlayers);
            for (int i = 1; i < numPlayers; i++)
            {
                // Create new players
                Players.Add(new Player((PlayerIndex)i, initialModel, graphics));
            }
        }


        public void Update()
        {

        }

        /// <summary>
        /// Updates player movement
        /// </summary>
        /// <param name="player"></param>
        /// <param name="gameTime"></param>
        private static void UpdatePlayerMovement(Player player, GameTime gameTime)
        {
            // Get keyboard state
            KeyboardState keyState = Keyboard.GetState();

            // Reset rotation delta
            Vector3 rotChange = Vector3.Zero;

            // Rotate object
            if (keyState.IsKeyDown(Keys.Q)) { rotChange += Vector3.Right; }
            if (keyState.IsKeyDown(Keys.E)) { rotChange += Vector3.Left; }
            if (keyState.IsKeyDown(Keys.A)) { rotChange += Vector3.Up; }
            if (keyState.IsKeyDown(Keys.D)) { rotChange += Vector3.Down; }
            // Set new rotation
            player.Rotation += rotChange * .025f;

            // Move back on S
            if (keyState.IsKeyUp(Keys.S))
            {
                // Calculate what direction the object should move in
                Matrix rotation = Matrix.CreateFromYawPitchRoll(player.Rotation.Y, player.Rotation.X, player.Rotation.Z);
                // Move object in direction given by rotation matrix
                player.Position += Vector3.Transform(Vector3.Forward, rotation * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4);
            }

            // Move forward on W
            if (keyState.IsKeyUp(Keys.W))
            {
                // Calculate what direction the object should move in
                Matrix rotation = Matrix.CreateFromYawPitchRoll(player.Rotation.Y, player.Rotation.X, player.Rotation.Z);
                // Move object in direction given by rotation matrix
                player.Position += Vector3.Transform(Vector3.Backward, rotation * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4);
            }
        }

        public void DrawAllPlayers(params CameraManager[] camera)
        {
            // if the number of elements in player and camera array is not the same throw an ArgumentException
            if (Players.Count != camera.Length) throw new ArgumentException("Number of players and cameras do not match.");

            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].Draw(camera[i].Camera.View, camera[i].Camera.Projection, camera[i].Camera.Position);
            }
        }
    }
}
