using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tools_MultipleCamera;

namespace Alexander_VT19
{
    public static class InGame
    {
        private static GraphicsDevice _graphics;


        private static CameraManager _cameraManager;
        private static List<GameInstance> _gameInstances;

        private static SkyBox _skyBox;

        private static Effect _simpleEffect;
        private static LightingMaterial _material;

        public static void Initialize(int numPlayers, ContentManager content)
        {
        }


        public static void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _graphics = graphicsDevice;

            // Create new cameras
            ChaseCamera chaseCamera = new ChaseCamera(new Vector3(0,100f,30f), Vector3.Zero, Vector3.Zero, _graphics);
            chaseCamera.Move(Vector3.Zero, Vector3.Zero);
            FreeCamera freeCamera = new FreeCamera(_graphics, 0f, 0f, new Vector3(10f));
            _cameraManager = new CameraManager(chaseCamera, freeCamera, CameraType.Chase);

            LoadSkyBox(content);

            _simpleEffect = content.Load<Effect>("Effects/SimpleEffect");
            _material = new LightingMaterial()
            {
                AmbientColor = Color.Red.ToVector3() * .15f,
                LightColor = Color.White.ToVector3() * .85f,
            };
        }

        private static void LoadSkyBox(ContentManager content)
        {
            // Load sky texture
            TextureCube skyTexture = content.Load<TextureCube>("SkyBox/clouds");
            // create new SkyBox
            _skyBox = new SkyBox(content, _graphics, skyTexture);

        }

        /// <summary>
        /// Entry point for starting a new game, creates new players etc
        /// </summary>
        /// <param name="numPlayers"></param>
        public static void StartNewGame(int numPlayers)
        {
            // Check that amount of players is within 1 - 4
            if (numPlayers < 1 || numPlayers > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(numPlayers), "Valid range for number of players is 1 - 4.");
            }

            for (int i = 0; i < numPlayers; i++)
            {
                
            }
        }

        public static void Update(GameTime gameTime)
        {

            //_cameraManager.Update(gameTime);
        }




        public static void Draw(SpriteBatch spriteBatch)
        {

            _graphics.Clear(Color.LightSkyBlue);
             

            _skyBox.Draw(_cameraManager.Camera.View, _cameraManager.Camera.Projection, _cameraManager.Camera.Position);
            // Draw every model in models with the current camera´s view and projection matrix
            foreach (CustomModel model in _models)
            {
                model.Draw(_cameraManager.Camera.View, _cameraManager.Camera.Projection, _cameraManager.Camera.Position);
            }
            PlayerManager.Draw(_cameraManager.Camera.View, _cameraManager.Camera.Projection, _cameraManager.Camera.Position);

        }
        /*
         void Draw(GameTime gameTime)
        {
            _graphics.SetRenderTarget(renderTarget);
            _graphics.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            DrawScene("ShadowMap");

            _graphics.SetRenderTarget(null);
            shadowMap = (Texture2D)renderTarget;

            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            DrawScene("ShadowedScene");
            shadowMap = null;

            base.Draw(gameTime);
        }

        private void DrawScene(string technique)
        {
            effect.CurrentTechnique = effect.Techniques[technique];
            effect.Parameters["CamerasViewProjection"].SetValue(viewMatrix * projectionMatrix);
            effect.Parameters["LightsViewProjection"].SetValue(lightsViewProjectionMatrix);
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["Texture"].SetValue(streetTexture);
            effect.Parameters["LightPos"].SetValue(lightPos);
            effect.Parameters["LightPower"].SetValue(lightPower);
            effect.Parameters["Ambient"].SetValue(ambientPower);
            effect.Parameters["ShadowMap"].SetValue(shadowMap);
            effect.Parameters["LightTexture"].SetValue(carLight);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.SetVertexBuffer(vertexBuffer);
                _graphics.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 16);
            }

            Matrix car1Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-3, 0, -15);
            DrawModel(carModel, carTextures, car1Matrix, technique);

            Matrix car2Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi * 5.0f / 8.0f) * Matrix.CreateTranslation(-28, 0, -1.9f);
            DrawModel(carModel, carTextures, car2Matrix, technique);
        }

        private void DrawModel(Model model, Texture2D[] textures, Matrix wMatrix, string technique)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques[technique];
                    currentEffect.Parameters["xCamerasViewProjection"].SetValue(viewMatrix * projectionMatrix);
                    currentEffect.Parameters["xLightsViewProjection"].SetValue(lightsViewProjectionMatrix);
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(textures[i++]);
                    currentEffect.Parameters["xLightPos"].SetValue(lightPos);
                    currentEffect.Parameters["xLightPower"].SetValue(lightPower);
                    currentEffect.Parameters["xAmbient"].SetValue(ambientPower);
                    currentEffect.Parameters["xShadowMap"].SetValue(shadowMap);
                    currentEffect.Parameters["xCarLightTexture"].SetValue(carLight);
                }
                mesh.Draw();
            }
        }
        */
    }
}
