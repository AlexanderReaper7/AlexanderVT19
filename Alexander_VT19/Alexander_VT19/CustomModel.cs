using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public class CustomModel
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Model Model { get; private set; }
        public Material Material { get; set; }

        private Matrix[] modelTransforms;
        private GraphicsDevice graphicsDevice;


        public CustomModel(Model model, Vector3 position, Vector3 rotation, Vector3 scale,
            GraphicsDevice graphicsDevice)
        {
            Model = model;
            // Create a new array of matrices with capacity of total bones in the model
            modelTransforms = new Matrix[Model.Bones.Count];
            // Copy the models bone matrices to the modelTransforms matrix array
            Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Position = position;
            Rotation = rotation;
            Scale = scale;

            this.graphicsDevice = graphicsDevice;
            Material = new Material();
            GenerateTags();
        }

        public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition)
        {
            // Create new matrix with current scale, rotation and translation
            Matrix baseWorld = Matrix.CreateScale(Scale)
                * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                * Matrix.CreateTranslation(Position);

            // For every mesh in model 
            foreach (ModelMesh mesh in Model.Meshes)
            {
                // Create a local matrix by combining the parent bone´s matrix and baseworld matrix
                Matrix localWorldMatrix = modelTransforms[mesh.ParentBone.Index] * baseWorld;

                // For every mesh part in mesh
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    // Create an effect with the material proprieties
                    Effect effect = part.Effect;

                    if (effect is BasicEffect)
                    {
                        // Set matrices
                        ((BasicEffect)effect).World = localWorldMatrix;
                        ((BasicEffect)effect).View = view;
                        ((BasicEffect)effect).Projection = projection;
                        // enable lighting
                        ((BasicEffect)effect).EnableDefaultLighting();
                    }
                    else
                    {
                        SetEffectParameter(effect, "World", localWorldMatrix);
                        SetEffectParameter(effect, "View", view);
                        SetEffectParameter(effect, "Projection", projection);
                        SetEffectParameter(effect, "CameraPosition", cameraPosition);
                        Material.SetEffectParameters(effect);
                    }
                }

                // finally draw entire mesh
                mesh.Draw();
            }
        }

        protected void SetEffectParameter(Effect effect, string param, object val)
        {
            if(effect.Parameters[param] == null) return;

            if (val is Vector3) effect.Parameters[param].SetValue((Vector3)val);
            else if (val is bool) effect.Parameters[param].SetValue((bool)val);
            else if (val is Matrix) effect.Parameters[param].SetValue((Matrix)val);
            else if (val is Texture2D) effect.Parameters[param].SetValue((Texture2D)val);
        }

        public void SetModelEffect(Effect effect, bool copyEffect)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Effect toSet = effect;

                    // Copy the effect if necessary
                    if (copyEffect) toSet = effect.Clone();

                    MeshTag tag = (MeshTag)part.Tag;

                    // if this ModelMeshPart has a texture, set it to the effect
                    if (tag.Texture != null)
                    {
                        SetEffectParameter(toSet, "BasicTexture", tag.Texture);
                        SetEffectParameter(toSet, "TextureEnabled", true);
                    }
                    else
                    {
                        SetEffectParameter(toSet, "TextureEnabled", false);
                    }

                    // Set our remaining parameters to the effect
                    SetEffectParameter(toSet, "diffuseColor", tag.Color);
                    SetEffectParameter(toSet, "SpecularPower", tag.SpecularPower);

                    part.Effect = toSet;
                }
            }
        }

        private void GenerateTags()
        {
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    if (part.Effect is BasicEffect)
                    {
                        BasicEffect effect = (BasicEffect) part.Effect;
                        MeshTag tag = new MeshTag(effect.DiffuseColor, effect.Texture, effect.SpecularPower);
                        part.Tag = tag;
                    }
        }

        // Store references to all of the model´s current effects
        public void CacheEffects()
        {
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    ((MeshTag)part.Tag).CachedEffect = part.Effect;
        }

        // Restore the effects referenced by the model´s cache
        public void RestoreEffects()
        {
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    if (((MeshTag)part.Tag).CachedEffect != null)
                        part.Effect = ((MeshTag) part.Tag).CachedEffect;
        }
    }

    public class MeshTag
    {
        public Vector3 Color;
        public Texture2D Texture;
        public float SpecularPower;
        public Effect CachedEffect = null;

        public MeshTag(Vector3 color, Texture2D texture, float specularPower)
        {
            Color = color;
            Texture = texture;
            SpecularPower = specularPower;
        }
    }
}