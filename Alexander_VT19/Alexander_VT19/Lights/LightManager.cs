using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19.Lights
{
    /// <summary>
    /// A Manager for all the light types
    /// </summary>
    class LightManager
    {
        //Depth Writing Shader
        Effect _depthWriter;
        //Directional Lights
        List<DirectionalLight> _directionalLights;
        //Spot Lights
        List<SpotLight> _spotLights;
        //Point Lights
        List<PointLight> _pointLights;
        #region Get Functions
        //Get Directional Lights
        public List<DirectionalLight> GetDirectionalLights() { return _directionalLights; }
        //Get Spot Lights
        public List<SpotLight> GetSpotLights() { return _spotLights; }
        //Get Point Lights
        public List<PointLight> GetPointLights() { return _pointLights; }
        #endregion
        //Constructor
        public LightManager(ContentManager content)
        {
            //Initialize Directional Lights
            _directionalLights = new List<DirectionalLight>();

            //Initialize Spot Lights
            _spotLights = new List<SpotLight>();

            //Initialize Point Lights
            _pointLights = new List<PointLight>();

            //Load the Depth Writing Shader
            _depthWriter = content.Load<Effect>("Effects/DepthWriter");
            _depthWriter.CurrentTechnique = _depthWriter.Techniques[0];
        }
        //Add a Point Light
        public void AddLight(PointLight light)
        {
            _pointLights.Add(light);
        }
        //Add a Directional Light
        public void AddLight(DirectionalLight light)
        {
            _directionalLights.Add(light);
        }
        //Add a Spot Light
        public void AddLight(SpotLight light)
        {
            _spotLights.Add(light);
        }
        //Remove a Point Light
        public void RemoveLight(PointLight light)
        {
            _pointLights.Remove(light);
        }
        //Remove a Directional Light
        public void RemoveLight(DirectionalLight light)
        {
            _directionalLights.Remove(light);
        }
        //Remove a Spot Light
        public void RemoveLight(SpotLight light)
        {
            _spotLights.Remove(light);
        }

        //Draw Shadow Maps
        public void DrawShadowMaps(GraphicsDevice graphicsDevice, List<Model> models)
        {
            //Set States
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //Foreach SpotLight with Shadows
            foreach (SpotLight light in _spotLights)
            {
                //Update it
                light.Update();
                //Draw it's Shadow Map
                if (light.GetIsWithShadows()) DrawShadowMap(graphicsDevice, light, models);
            }
            //Foreach PointLight with Shadows
            foreach (PointLight light in _pointLights)
            {
                //Draw it's Shadow Map
                if (light.GetIsWithShadows()) DrawShadowMap(graphicsDevice, light, models);
            }
        }

        //Draw a Shadow Map for a Spot Light
        void DrawShadowMap(GraphicsDevice graphicsDevice, SpotLight light, List<Model> models)
        {
            //Set Light's Target onto the Graphics Device
            graphicsDevice.SetRenderTarget(light.GetShadowMap());
            //Clear Target
            graphicsDevice.Clear(Color.Transparent);
            //Set global Effect parameters
            _depthWriter.Parameters["View"].SetValue(light.GetView());
            _depthWriter.Parameters["Projection"].SetValue(light.GetProjection());
            _depthWriter.Parameters["LightPosition"].SetValue(light.GetPosition());
            _depthWriter.Parameters["DepthPrecision"].SetValue(light.GetFarPlane());
            //Draw Models
            DrawModels(graphicsDevice, models);
        }

        //Draw a Shadow Map for a Point Light
        void DrawShadowMap(GraphicsDevice graphicsDevice, PointLight light, List<Model>
        models)
        {
            //Initialize View Matrices Array
            Matrix[] views = new Matrix[6];
            //Create View Matrices
            views[0] = Matrix.CreateLookAt(light.GetPosition(), light.GetPosition() +
            Vector3.Forward, Vector3.Up);

            views[1] = Matrix.CreateLookAt(light.GetPosition(), light.GetPosition() +
           Vector3.Backward, Vector3.Up);

            views[2] = Matrix.CreateLookAt(light.GetPosition(), light.GetPosition() +
           Vector3.Left, Vector3.Up);

            views[3] = Matrix.CreateLookAt(light.GetPosition(), light.GetPosition() +
           Vector3.Right, Vector3.Up);

            views[4] = Matrix.CreateLookAt(light.GetPosition(), light.GetPosition() +
           Vector3.Down, Vector3.Forward);

            views[5] = Matrix.CreateLookAt(light.GetPosition(), light.GetPosition() +
           Vector3.Up, Vector3.Backward);
            //Create Projection Matrix
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f),
            1.0f, 1.0f, light.GetRadius());
            //Set Global Effect Values
            _depthWriter.Parameters["Projection"].SetValue(projection);
            _depthWriter.Parameters["LightPosition"].SetValue(light.GetPosition());
            _depthWriter.Parameters["DepthPrecision"].SetValue(light.GetRadius());
            #region Forward
            graphicsDevice.SetRenderTarget(light.GetShadowMap(), CubeMapFace.PositiveZ);
            //Clear Target
            graphicsDevice.Clear(Color.Transparent);
            //Set global Effect parameters
            _depthWriter.Parameters["View"].SetValue(views[0]);
            //Draw Models
            DrawModels(graphicsDevice, models);
            #endregion
            #region Backward
            graphicsDevice.SetRenderTarget(light.GetShadowMap(), CubeMapFace.NegativeZ);
            //Clear Target
            graphicsDevice.Clear(Color.Transparent);
            //Set global Effect parameters
            _depthWriter.Parameters["View"].SetValue(views[1]);
            //Draw Models
            DrawModels(graphicsDevice, models);
            #endregion
            #region Left
            graphicsDevice.SetRenderTarget(light.GetShadowMap(), CubeMapFace.NegativeX);
            //Clear Target
            graphicsDevice.Clear(Color.Transparent);
            //Set global Effect parameters
            _depthWriter.Parameters["View"].SetValue(views[2]);
            //Draw Models
            DrawModels(graphicsDevice, models);
            #endregion
            #region Right
            graphicsDevice.SetRenderTarget(light.GetShadowMap(), CubeMapFace.PositiveX);
            //Clear Target
            graphicsDevice.Clear(Color.Transparent);
            //Set global Effect parameters
            _depthWriter.Parameters["View"].SetValue(views[3]);
            //Draw Models
            DrawModels(graphicsDevice, models);
            #endregion
            #region Down
            graphicsDevice.SetRenderTarget(light.GetShadowMap(), CubeMapFace.NegativeY);
            //Clear Target
            graphicsDevice.Clear(Color.Transparent);
            //Set global Effect parameters
            _depthWriter.Parameters["View"].SetValue(views[4]);
            //Draw Models
            DrawModels(graphicsDevice, models);
            #endregion
            #region Up
            graphicsDevice.SetRenderTarget(light.GetShadowMap(), CubeMapFace.PositiveY);
            //Clear Target
            graphicsDevice.Clear(Color.Transparent);
            //Set global Effect parameters
            _depthWriter.Parameters["View"].SetValue(views[5]);
            //Draw Models
            DrawModels(graphicsDevice, models);
            #endregion
        }

        //Draw Models
        void DrawModels(GraphicsDevice graphicsDevice, List<Model> models)
        {
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
                        _depthWriter.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index]);
                        //Apply Effect
                        _depthWriter.CurrentTechnique.Passes[0].Apply();
                        //Draw
                        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                            part.NumVertices, part.StartIndex,
                            part.PrimitiveCount);
                    }
                }
            }
        }
    }
}
