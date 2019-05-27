using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public class SimpleMaterial : Material
    {
        public Vector3 DiffuseColor { get; set; }
        public Vector3 AmbientColor { get; set; }
        public Texture2D Texture { get; set; }
        public bool TextureEnabled { get; set; }

        public SimpleMaterial()
        {
            DiffuseColor = Vector3.One;
            AmbientColor = Vector3.Zero;
            Texture = null;
            TextureEnabled = false;
        }

        public override void SetEffectParameters(Effect effect)
        {
            effect.Parameters["DiffuseColor"]?.SetValue(DiffuseColor);
            effect.Parameters["AmbientColor"]?.SetValue(DiffuseColor);
            if (Texture != null) effect.Parameters["BasicTexture"]?.SetValue(DiffuseColor);
            effect.Parameters["TextureEnabled"]?.SetValue(TextureEnabled);
        }
    }
}
