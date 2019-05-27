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
        private GraphicsDevice _graphicsDevice;
        private QuadRenderer _quadRenderer;

        private RenderTargetCube _renderTargetCube;

        private Effect _fillEffect;
        public SkyBox SkyBox;
        private Vector3 _color;

        private NeonBackground _neon;

        public NeonSkybox(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _quadRenderer = new QuadRenderer(graphicsDevice);

            _fillEffect = content.Load<Effect>(@"Effects/Fill");
            _renderTargetCube = new RenderTargetCube(graphicsDevice, 4096, false, SurfaceFormat.Color, DepthFormat.None, 4, RenderTargetUsage.DiscardContents);


            _neon = NeonBackground.CreateNew(content);


            RenderTextureCube();

            SkyBox = new SkyBox(content, graphicsDevice, _renderTargetCube);
        }


        public void Update()
        {
            RenderTextureCube();
        }


        public void Draw(Camera camera, GameTime gameTime)
        {
            _neon.UpdateEffectParameters(gameTime);

            SkyBox.SetTexture(_renderTargetCube);
            SkyBox.Draw(camera);
        }

        private void RenderTextureCube()
        {
            for (int i = 0; i < 6; i++)
            {
                _graphicsDevice.SetRenderTarget(_renderTargetCube, (CubeMapFace)i);
                _quadRenderer.Render(_fillEffect);
                _quadRenderer.Render(_neon.Effect);
            }

            _graphicsDevice.SetRenderTarget(null);
        }
    }
}
