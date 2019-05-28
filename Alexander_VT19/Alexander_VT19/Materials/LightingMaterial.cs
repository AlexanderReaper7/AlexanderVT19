using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public class LightingMaterial : Material
    {
        public bool EnableTexture { get; set; }
        public Texture2D Texture { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Vector3 AmbientColor { get; set; }
        public Vector3 LightDirection { get; set; }
        public Vector3 LightColor { get; set; }
        public Vector3 SpecularColor { get; set; }
        public int SpecularPower { get; set; }

        public LightingMaterial()
        {
            EnableTexture = false;
            DiffuseColor = Vector3.One;
            AmbientColor = new Vector3(.2f, .2f, .2f);
            LightDirection = new Vector3(1, 1, 1);
            LightColor = new Vector3(.9f, .9f, .9f);
            SpecularColor = Vector3.One;
            SpecularPower = 32;
            Texture = new Texture2D(Game1.Graphics.GraphicsDevice,1,1);
            Texture.SetData(new Color[] { Color.White });
        }

        public override void SetEffectParameters(Effect effect)
        {
            effect.Parameters["TextureEnabled"]?.SetValue(EnableTexture);
            effect.Parameters["BasicTexture"]?.SetValue(Texture);
            effect.Parameters["DiffuseColor"]?.SetValue(DiffuseColor);
            effect.Parameters["AmbientColor"]?.SetValue(AmbientColor);
            effect.Parameters["LightDirection"]?.SetValue(LightDirection);
            effect.Parameters["LightColor"]?.SetValue(LightColor);
            effect.Parameters["SpecularColor"]?.SetValue(SpecularColor);
            effect.Parameters["SpecularPower"]?.SetValue(SpecularPower);
        }
    }
}
