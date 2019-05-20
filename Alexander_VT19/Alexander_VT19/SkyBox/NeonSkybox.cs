using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    class NeonSkybox
    {
        private TextureCube maskTextureCube;


        private SkyBox skyBox;
        private Vector3 _color;

        public Vector3 Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public NeonSkybox(ContentManager content, GraphicsDevice graphicsDevice)
        {
            RenderTargetCube renderTargetCube = new RenderTargetCube(graphicsDevice, 2048, false, SurfaceFormat.Vector4, DepthFormat.None);
            skyBox = new SkyBox(content, graphicsDevice, renderTargetCube);
        }


    }
}
