using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexander_VT19.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    class DeferredRenderer
    {
        //Clear Shader
        Effect _clear;
        //GBuffer Shader
        Effect _gBuffer;
        //Directional Light Shader
        Effect _directionalLight;
        //Point Light Shader
        Effect _pointLight;
        //Spot Light Shader
        Effect _spotLight;
        //Composition Shader
        Effect _compose;
        //LightMap BlendState
        BlendState _lightMapBs;
        //GBuffer Targets
        RenderTargetBinding[] _gBufferTargets;
        //GBuffer Texture Size
        Vector2 _gBufferTextureSize;
        //Light Map Target
        RenderTarget2D _lightMap;
        //Fullscreen Quad
        FullscreenQuad _fsq;
        //Point Light Geometry
        Model _pointLightGeometry;
        //Spot Light Geometry
        Model _spotLightGeometry;

        //Get GBuffer
        public RenderTargetBinding[] GetGBuffer => _gBufferTargets;


        public DeferredRenderer(GraphicsDevice graphicsDevice, ContentManager content,
         int width, int height)
        {
            //Load Clear Shader
            _clear = content.Load<Effect>("Effects/Clear");
            _clear.CurrentTechnique = _clear.Techniques[0];

            //Load GBuffer Shader
            _gBuffer = content.Load<Effect>("Effects/GBuffer");
            _gBuffer.CurrentTechnique = _gBuffer.Techniques[0];

            //Load Directional Light Shader
            _directionalLight = content.Load<Effect>("Effects/DirectionalLight");
            _directionalLight.CurrentTechnique = _directionalLight.Techniques[0];

            //Load Point Light Shader
            _pointLight = content.Load<Effect>("Effects/PointLight");
            _pointLight.CurrentTechnique = _pointLight.Techniques[0];

            //Load Spot Light Shader
            _spotLight = content.Load<Effect>("Effects/SpotLight");
            _spotLight.CurrentTechnique = _spotLight.Techniques[0];

            //Load Composition Shader
            _compose = content.Load<Effect>("Effects/Composition");
            _compose.CurrentTechnique = _compose.Techniques[0];

            //Create LightMap BlendState
            _lightMapBs = new BlendState();
            _lightMapBs.ColorSourceBlend = Blend.One;
            _lightMapBs.ColorDestinationBlend = Blend.One;
            _lightMapBs.ColorBlendFunction = BlendFunction.Add;
            _lightMapBs.AlphaSourceBlend = Blend.One;
            _lightMapBs.AlphaDestinationBlend = Blend.One;
            _lightMapBs.AlphaBlendFunction = BlendFunction.Add;

            //Set GBuffer Texture Size
            _gBufferTextureSize = new Vector2(width, height);

            //Initialize GBuffer Targets Array
            _gBufferTargets = new RenderTargetBinding[3];

            //Intialize Each Target of the GBuffer
            _gBufferTargets[0] = new RenderTargetBinding(new RenderTarget2D(graphicsDevice,
           width, height, false,
           SurfaceFormat.Rgba64,
           DepthFormat.Depth24Stencil8));
            _gBufferTargets[1] = new RenderTargetBinding(new RenderTarget2D(graphicsDevice,
           width, height, false,
           SurfaceFormat.Rgba64,
           DepthFormat.Depth24Stencil8));
            _gBufferTargets[2] = new RenderTargetBinding(new RenderTarget2D(graphicsDevice,
           width, height, false,
           SurfaceFormat.Vector2,
           DepthFormat.Depth24Stencil8));

            //Initialize LightMap
            _lightMap = new RenderTarget2D(graphicsDevice, width, height, false,
           SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            //Create Fullscreen Quad
            _fsq = new FullscreenQuad(graphicsDevice);

            //Load Point Light Geometry
            _pointLightGeometry = content.Load<Model>("sphere");

            //Load Spot Light Geometry
            //spotLightGeometry = Content.Load<Model>("SpotLightGeometry");
        }

        //GBuffer Creation
        private void MakeGBuffer(GraphicsDevice graphicsDevice, List<Model> models, Camera camera)
        {
            //Set Depth State
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            //Set up global GBuffer parameters
            _gBuffer.Parameters["View"].SetValue(camera.View);
            _gBuffer.Parameters["Projection"].SetValue(camera.Projection);
            //Draw Each Model
            foreach (Model model in models)
            {
                //Get Transforms
                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);
                //Draw Each ModelMesh
                foreach (ModelMesh mesh in model.Meshes)
                {
                    //Draw Each ModelMeshPart
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        //Set Vertex Buffer
                        graphicsDevice.SetVertexBuffer(part.VertexBuffer, part.VertexOffset);
                        //Set Index Buffer
                        graphicsDevice.Indices = part.IndexBuffer;
                        //Set World
                        _gBuffer.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index]);
                        //Set WorldIT
                        _gBuffer.Parameters["WorldViewIT"].SetValue(Matrix.Transpose(Matrix.Invert(transforms[mesh.ParentBone.Index] * camera.View)));
                        //Set Albedo Texture
                        _gBuffer.Parameters["Texture"].SetValue(part.Effect.Parameters["Texture"].GetValueTexture2D());
                        //Set Normal Texture
                        _gBuffer.Parameters["NormalMap"].SetValue(part.Effect.Parameters["NormalMap"].GetValueTexture2D());
                        //Set Specular Texture
                        _gBuffer.Parameters["SpecularMap"].SetValue(part.Effect.Parameters["SpecularMap"].GetValueTexture2D());
                        //Apply Effect
                        _gBuffer.CurrentTechnique.Passes[0].Apply();
                        //Draw
                        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                            part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
            //Set RenderTargets off
            graphicsDevice.SetRenderTargets(null);
        }

        //Clear GBuffer
        void ClearGBuffer(GraphicsDevice graphicsDevice)
        {
            //Set to ReadOnly depth for now...
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            //Set GBuffer Render Targets
            graphicsDevice.SetRenderTargets(_gBufferTargets);
            //Set Clear Effect
            _clear.CurrentTechnique.Passes[0].Apply();
            //Draw
            _fsq.Draw(graphicsDevice);
        }

        //Draw Scene Deferred
        public void Draw(GraphicsDevice graphicsDevice, List<Model> models,
            LightManager lights, Camera camera, RenderTarget2D output)
        {
            //Set States
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //Clear GBuffer
            ClearGBuffer(graphicsDevice);
            //Make GBuffer
            MakeGBuffer(graphicsDevice, models, camera);
            //Make LightMap
            MakeLightMap(graphicsDevice, lights, camera);
            //Make Final Rendered Scene
            MakeFinal(graphicsDevice, output);
        }

        //Debug
        public void Debug(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            //Begin SpriteBatch
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
                SamplerState.PointClamp, null, null);
            //Width + Height
            int width = 128;
            int height = 128;
            //Set up Drawing Rectangle
            Rectangle rect = new Rectangle(0, 0, width, height);
            //Draw GBuffer 0
            spriteBatch.Draw((Texture2D)_gBufferTargets[0].RenderTarget, rect, Color.White);
            //Draw GBuffer 1
            rect.X += width;
            spriteBatch.Draw((Texture2D)_gBufferTargets[1].RenderTarget, rect, Color.White);
            //Draw GBuffer 2
            rect.X += width;
            spriteBatch.Draw((Texture2D)_gBufferTargets[2].RenderTarget, rect, Color.White);
            //End SpriteBatch
            spriteBatch.End();
        }

        //Light Map Creation
        void MakeLightMap(GraphicsDevice graphicsDevice, LightManager lights,
         Camera camera)
        {
            //Set LightMap Target
            graphicsDevice.SetRenderTarget(_lightMap);
            //Clear to Transperant Black
            graphicsDevice.Clear(Color.Transparent);
            //Set States
            graphicsDevice.BlendState = _lightMapBs;
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            #region Set Global Samplers
            //GBuffer 1 Sampler
            graphicsDevice.Textures[0] = _gBufferTargets[0].RenderTarget;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            //GBuffer 2 Sampler
            graphicsDevice.Textures[1] = _gBufferTargets[1].RenderTarget;
            graphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            //GBuffer 3 Sampler
            graphicsDevice.Textures[2] = _gBufferTargets[2].RenderTarget;
            graphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
            //SpotLight Cookie Sampler
            graphicsDevice.SamplerStates[3] = SamplerState.LinearClamp;
            //ShadowMap Sampler
            graphicsDevice.SamplerStates[4] = SamplerState.PointClamp;

            #endregion
            //Calculate InverseView
            Matrix inverseView = Matrix.Invert(camera.View);

            //Calculate InverseViewProjection
            Matrix inverseViewProjection = Matrix.Invert(camera.View * camera.Projection);

            //Set Directional Lights Globals
            _directionalLight.Parameters["InverseViewProjection"].SetValue(inverseViewProjection);
            _directionalLight.Parameters["inverseView"].SetValue(inverseView);
            _directionalLight.Parameters["CameraPosition"].SetValue(camera.Position);
            _directionalLight.Parameters["GBufferTextureSize"].SetValue(_gBufferTextureSize);

            //Set the Directional Lights Geometry Buffers
            _fsq.ReadyBuffers(graphicsDevice);

            //Draw Directional Lights
            foreach (Lights.DirectionalLight light in lights.GetDirectionalLights())
            {
                //Set Directional Light Parameters
                _directionalLight.Parameters["L"].SetValue(Vector3.Normalize(light.GetDirection()));
                _directionalLight.Parameters["LightColor"].SetValue(light.GetColor());
                _directionalLight.Parameters["LightIntensity"].SetValue(light.GetIntensity());

                //Apply
                _directionalLight.CurrentTechnique.Passes[0].Apply();

                //Draw
                _fsq.JustDraw(graphicsDevice);
            }

            //Set Spot Lights Globals
            _spotLight.Parameters["View"].SetValue(camera.View);
            _spotLight.Parameters["inverseView"].SetValue(inverseView);
            _spotLight.Parameters["Projection"].SetValue(camera.Projection);
            _spotLight.Parameters["InverseViewProjection"].SetValue(inverseViewProjection);
            _spotLight.Parameters["CameraPosition"].SetValue(camera.Position);
            _spotLight.Parameters["GBufferTextureSize"].SetValue(_gBufferTextureSize);

            //Set Spot Lights Geometry Buffers
            graphicsDevice.SetVertexBuffer(_spotLightGeometry.Meshes[0].MeshParts[0].VertexBuffer,
             _spotLightGeometry.Meshes[0].MeshParts[0].VertexOffset);
            graphicsDevice.Indices = _spotLightGeometry.Meshes[0].MeshParts[0].IndexBuffer;

            //Draw Spot Lights
            foreach (Lights.SpotLight light in lights.GetSpotLights())
            {
                //Set Attenuation Cookie Texture and SamplerState
                graphicsDevice.Textures[3] = light.GetAttenuationTexture();

                //Set ShadowMap and SamplerState
                graphicsDevice.Textures[4] = light.GetShadowMap();

                //Set Spot Light Parameters
                _spotLight.Parameters["World"].SetValue(light.GetWorld());
                _spotLight.Parameters["LightViewProjection"].SetValue(light.GetView() *
                light.GetProjection());
                _spotLight.Parameters["LightPosition"].SetValue(light.GetPosition());
                _spotLight.Parameters["LightColor"].SetValue(light.GetColor());
                _spotLight.Parameters["LightIntensity"].SetValue(light.GetIntensity());
                _spotLight.Parameters["S"].SetValue(light.GetDirection());
                _spotLight.Parameters["LightAngleCos"].SetValue(light.LightAngleCos());
                _spotLight.Parameters["LightHeight"].SetValue(light.GetFarPlane());
                _spotLight.Parameters["Shadows"].SetValue(light.GetIsWithShadows());
                _spotLight.Parameters["shadowMapSize"].SetValue(light.GetShadowMapResoloution());
                _spotLight.Parameters["DepthPrecision"].SetValue(light.GetFarPlane());
                _spotLight.Parameters["DepthBias"].SetValue(light.GetDepthBias());

                #region Set Cull Mode
                //Calculate L
                Vector3 l = camera.Position - light.GetPosition();

                //Calculate S.L
                float sl = Math.Abs(Vector3.Dot(l, light.GetDirection()));

                //Check if SL is within the LightAngle, if so then draw the BackFaces, if not
                //then draw the FrontFaces
                if (sl < light.LightAngleCos())
                    graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                else
                    graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                #endregion

                //Apply
                _spotLight.CurrentTechnique.Passes[0].Apply();

                //Draw
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                _spotLightGeometry.Meshes[0].MeshParts[0].NumVertices,
                _spotLightGeometry.Meshes[0].MeshParts[0].StartIndex,
               _spotLightGeometry.Meshes[0].MeshParts[0].PrimitiveCount);
            }

            //Set Point Lights Geometry Buffers
            graphicsDevice.SetVertexBuffer(_pointLightGeometry.Meshes[0].MeshParts[0].VertexBuffer,
             _pointLightGeometry.Meshes[0].MeshParts[0].VertexOffset);
            graphicsDevice.Indices = _pointLightGeometry.Meshes[0].MeshParts[0].IndexBuffer;
            //Set Point Lights Globals
            _pointLight.Parameters["inverseView"].SetValue(inverseView);
            _pointLight.Parameters["View"].SetValue(camera.View);
            _pointLight.Parameters["Projection"].SetValue(camera.Projection);
            _pointLight.Parameters["InverseViewProjection"].SetValue(inverseViewProjection);
            _pointLight.Parameters["CameraPosition"].SetValue(camera.Position);
            _pointLight.Parameters["GBufferTextureSize"].SetValue(_gBufferTextureSize);
            //Draw Point Lights without Shadows
            foreach (Lights.PointLight light in lights.GetPointLights())
            {
                //Set Point Light Sampler
                graphicsDevice.Textures[4] = light.GetShadowMap();
                graphicsDevice.SamplerStates[4] = SamplerState.PointWrap;
                //Set Point Light Parameters
                _pointLight.Parameters["World"].SetValue(light.World());
                _pointLight.Parameters["LightPosition"].SetValue(light.GetPosition());
                _pointLight.Parameters["LightRadius"].SetValue(light.GetRadius());
                _pointLight.Parameters["LightColor"].SetValue(light.GetColor());
                _pointLight.Parameters["LightIntensity"].SetValue(light.GetIntensity()); ;
                _pointLight.Parameters["Shadows"].SetValue(light.GetIsWithShadows());
                _pointLight.Parameters["DepthPrecision"].SetValue(light.GetRadius());
                _pointLight.Parameters["DepthBias"].SetValue(light.GetDepthBias());
                _pointLight.Parameters["shadowMapSize"].SetValue(light.GetShadowMapResoloution());
                //Set Cull Mode
                Vector3 diff = camera.Position - light.GetPosition();
                float cameraToLight = (float)Math.Sqrt((float)Vector3.Dot(diff, diff)) / 100.0f; // WARN: model scaling
                //If the Camera is in the light, render the backfaces, if it's out of the
                //light, render the frontfaces
                if (cameraToLight <= light.GetRadius())
                    graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                else if (cameraToLight > light.GetRadius())
                    graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                //Apply
                _pointLight.CurrentTechnique.Passes[0].Apply();
                //Draw
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                _pointLightGeometry.Meshes[0].MeshParts[0].NumVertices,
                _pointLightGeometry.Meshes[0].MeshParts[0].StartIndex,
               _pointLightGeometry.Meshes[0].MeshParts[0].PrimitiveCount);
            }

            //Set States Off
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        //Composition
        void MakeFinal(GraphicsDevice graphicsDevice, RenderTarget2D output)
        {
            //Set Composition Target
            graphicsDevice.SetRenderTarget(output);
            //Clear
            graphicsDevice.Clear(Color.Transparent);
            //Set Textures
            graphicsDevice.Textures[0] = _gBufferTargets[0].RenderTarget;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            graphicsDevice.Textures[1] = _lightMap;
            graphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            //Set Effect Parameters
            _compose.Parameters["GBufferTextureSize"].SetValue(_gBufferTextureSize);
            //Apply
            _compose.CurrentTechnique.Passes[0].Apply();
            //Draw
            _fsq.Draw(graphicsDevice);
        }
    }
}
