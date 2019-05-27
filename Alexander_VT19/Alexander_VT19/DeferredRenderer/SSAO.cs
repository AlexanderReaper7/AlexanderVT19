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
    class Ssao
    {
        //SSAO effect
        Effect _ssao;
        //SSAO Blur Effect
        Effect _ssaoBlur;
        //SSAO Composition effect
        Effect _composer;
        //Random Normal Texture
        Texture2D _randomNormals;
        //Sample Radius
        float _sampleRadius;
        //Distance Scale
        float _distanceScale;
        //SSAO Target
        RenderTarget2D _ssaoTarget;
        //Blue Target
        RenderTarget2D _blurTarget;
        //FSQ
        FullscreenQuad _fsq;
        #region Get Methods
        //Get Sample Radius
        float GetSampleRadius() { return _sampleRadius; }
        //Get Distance Scale
        float GetDistanceScale() { return _distanceScale; }
        #endregion
        #region Set Methods
        //Set Sample Radius
        void SetSampleRadius(float radius) { this._sampleRadius = radius; }
        //Set Distance Scale
        void SetDistanceScale(float scale) { this._distanceScale = scale; }
        #endregion
        //Constructor
        public Ssao(GraphicsDevice graphicsDevice, ContentManager content,
            int width, int height)
        {
            //Load SSAO effect
            _ssao = content.Load<Effect>("Effects/SSAO");
            _ssao.CurrentTechnique = _ssao.Techniques[0];
            //Load SSAO Blur effect
            _ssaoBlur = content.Load<Effect>("Effects/SSAOBlur");
            _ssaoBlur.CurrentTechnique = _ssaoBlur.Techniques[0];
            //Load SSAO composition effect
            _composer = content.Load<Effect>("Effects/SSAOFinal");
            _composer.CurrentTechnique = _composer.Techniques[0];
            //Create SSAO Target
            _ssaoTarget = new RenderTarget2D(graphicsDevice, width, height, false,
                SurfaceFormat.Color, DepthFormat.None);
            //Create SSAO Blur Target
            _blurTarget = new RenderTarget2D(graphicsDevice, width, height, false,
                SurfaceFormat.Color, DepthFormat.None);
            //Create FSQ
            _fsq = new FullscreenQuad(graphicsDevice);
            //Load Random Normal Texture
            _randomNormals = content.Load<Texture2D>("null_normal");

            //Set Sample Radius to Default
            _sampleRadius = 0;
            //Set Distance Scale to Default
            _distanceScale = 0;
        }

        //Draw
        public void Draw(GraphicsDevice graphicsDevice, RenderTargetBinding[] gBuffer,
            RenderTarget2D scene, Camera camera, RenderTarget2D output)
        {
            //Set States
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //Render SSAO
            RenderSsao(graphicsDevice, gBuffer, camera);
            //Blur SSAO
            BlurSsao(graphicsDevice);
            //Compose final
            Compose(graphicsDevice, scene, output, true);
        }

        //Render SSAO
        void RenderSsao(GraphicsDevice graphicsDevice, RenderTargetBinding[] gBuffer,
            Camera camera)
        {
            //Set SSAO Target
            graphicsDevice.SetRenderTarget(_ssaoTarget);
            //Clear
            graphicsDevice.Clear(Color.White);
            //Set Samplers
            graphicsDevice.Textures[1] = gBuffer[1].RenderTarget;
            graphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            graphicsDevice.Textures[2] = gBuffer[1].RenderTarget;
            graphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
            graphicsDevice.Textures[3] = _randomNormals;
            graphicsDevice.SamplerStates[3] = SamplerState.LinearWrap;
            //Calculate Frustum Corner of the Camera
            Vector3 cornerFrustum = Vector3.Zero;
            cornerFrustum.Y = (float)Math.Tan(Math.PI / 3.0 / 2.0) * camera.FarClip;
            cornerFrustum.X = cornerFrustum.Y * camera.AspectRatio;
            cornerFrustum.Z = camera.FarClip;
            //Set SSAO parameters
            _ssao.Parameters["Projection"].SetValue(camera.Projection);
            _ssao.Parameters["cornerFustrum"].SetValue(cornerFrustum);
            _ssao.Parameters["sampleRadius"].SetValue(_sampleRadius);
            _ssao.Parameters["distanceScale"].SetValue(_distanceScale);
            _ssao.Parameters["GBufferTextureSize"].SetValue(new Vector2(_ssaoTarget.Width,
                _ssaoTarget.Height));
            //Apply
            _ssao.CurrentTechnique.Passes[0].Apply();
            //Draw
            _fsq.Draw(graphicsDevice);
        }

        //Blur SSAO
        void BlurSsao(GraphicsDevice graphicsDevice)
        {
            //Set Blur Target
            graphicsDevice.SetRenderTarget(_blurTarget);
            //Clear
            graphicsDevice.Clear(Color.White);
            //Set Samplers, GBuffer was set before so no need to reset...
            graphicsDevice.Textures[3] = _ssaoTarget;
            graphicsDevice.SamplerStates[3] = SamplerState.LinearClamp;
            //Set SSAO parameters
            _ssaoBlur.Parameters["blurDirection"].SetValue(Vector2.One);
            _ssaoBlur.Parameters["targetSize"].SetValue(new Vector2(_ssaoTarget.Width,
                _ssaoTarget.Height));
            //Apply
            _ssaoBlur.CurrentTechnique.Passes[0].Apply();
            //Draw
            _fsq.Draw(graphicsDevice);
        }

        //Compose
        void Compose(GraphicsDevice graphicsDevice, RenderTarget2D scene, RenderTarget2D
            output, bool useBlurredSsao)
        {
            //Set Output Target
            graphicsDevice.SetRenderTarget(output);
            //Clear
            graphicsDevice.Clear(Color.White);
            //Set Samplers
            graphicsDevice.Textures[0] = scene;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            if (useBlurredSsao) graphicsDevice.Textures[1] = _blurTarget;
            else graphicsDevice.Textures[1] = _ssaoTarget;
            graphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            //Set Effect Parameters
            _composer.Parameters["halfPixel"].SetValue(new Vector2(1.0f / _ssaoTarget.Width,
                1.0f / _ssaoTarget.Height));
            //Apply
            _composer.CurrentTechnique.Passes[0].Apply();
            //Draw
            _fsq.Draw(graphicsDevice);
        }

        //Modify
        public void Modify(KeyboardState current)
        {
            float speed = 0.01f;
            if (current.IsKeyDown(Keys.Z)) _sampleRadius -= speed;
            if (current.IsKeyDown(Keys.X)) _sampleRadius += speed;
            if (current.IsKeyDown(Keys.C)) _distanceScale -= speed;
            if (current.IsKeyDown(Keys.V)) _distanceScale += speed;
        }
        //Debug Values
        public void Debug(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            //Width + Height
            int width = 128;
            int height = 128;
            //Set up Drawing Rectangle
            Rectangle rect = new Rectangle(384, 0, width, height);
            //Begin SpriteBatch for Buffer
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
                SamplerState.LinearClamp, null, null);
            //Draw SSAO buffer
            spriteBatch.Draw((Texture2D)_ssaoTarget, rect, Color.White);
            //Draw SSAO Blurred
            rect.X += 128;
            spriteBatch.Draw((Texture2D)_blurTarget, rect, Color.White);
            //End SpriteBatch
            spriteBatch.End();
            //Begin SpriteBatch for Text
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //Draw sampleRadius
            spriteBatch.DrawString(spriteFont, "Sample Radius: " + _sampleRadius.ToString(),
                new Vector2(0, 128), Color.Red);
            //Draw distanceScale
            spriteBatch.DrawString(spriteFont, "Distance Scale: " + _distanceScale.ToString(),
                new Vector2(0, 148), Color.Blue);
            //End SpriteBatch
            spriteBatch.End();
        }
    }
}
