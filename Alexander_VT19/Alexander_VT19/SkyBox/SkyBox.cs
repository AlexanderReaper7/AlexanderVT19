using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public class SkyBox
    {
        private CustomModel _model;
        private Effect _skySphereEffect;
        private GraphicsDevice _graphics;

        /// <summary>
        /// Creates a new SkyBox
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="texture"></param>
        public SkyBox(ContentManager content, GraphicsDevice graphicsDevice, TextureCube texture)
        {
            // Create a new model
            _model = new CustomModel(content.Load<Model>("SkyBox/skysphere_mesh"), Vector3.Zero, Vector3.Zero, Vector3.One, graphicsDevice);
            // Load the effect
            _skySphereEffect = content.Load<Effect>("SkyBox/skysphere_effect");
            // Set the texture to the effect
            _skySphereEffect.Parameters["CubeMap"].SetValue(texture);
            // Set the effect to the model
            _model.SetModelEffect(_skySphereEffect,false);

            _graphics = graphicsDevice;
        }

        /// <summary>
        /// Loads a new texture to the SkyBox
        /// </summary>
        /// <param name="texture"></param>
        public void SetTexture(TextureCube texture)
        {
            _skySphereEffect.Parameters["CubeMap"].SetValue(texture);
            _model.SetModelEffect(_skySphereEffect, true);
        }

        /// <summary>
        /// Draws the SkyBox
        /// </summary>
        public void Draw(Camera camera)
        {
            // Disable the DepthStencil
            _graphics.DepthStencilState = DepthStencilState.None;

            // Move the model with the sphere
            _model.Position = camera.Position;

            // Draw the sphere (SkyBox)
            _model.Draw(camera.View, camera.Projection, camera.Position);

            // Reset the DepthStencil
            _graphics.DepthStencilState = DepthStencilState.Default;
        }
    }
}
