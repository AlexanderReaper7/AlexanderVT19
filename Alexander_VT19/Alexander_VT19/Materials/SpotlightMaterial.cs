using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public class SpotlightMaterial : Material
    {
        public Vector3 AmbientLightColor { get; set; }
        public Vector3 LightPosition { get; set; }
        public Vector3 LightColor { get; set; }
        public Vector3 LightDirection { get; set; }
        public float ConeAngle { get; set; }
        public float LightFalloff { get; set; }

        /// <summary>
        /// Creates a new spotlightmaterial with default values
        /// </summary>
        public SpotlightMaterial()
        {
            AmbientLightColor = new Vector3(.15f);
            LightPosition = new Vector3(0, 300, 0);
            LightColor = new Vector3(.85f);
            LightDirection = Vector3.Down;
            ConeAngle = 30f;
            LightFalloff = 20;
        }

        public override void SetEffectParameters(Effect effect)
        {
            effect.Parameters["AmbientLightColor"]?.SetValue(AmbientLightColor);
            effect.Parameters["LightPosition"]?.SetValue(LightPosition);
            effect.Parameters["LightColor"]?.SetValue(LightColor);
            effect.Parameters["LightDirection"]?.SetValue(LightDirection);
            effect.Parameters["ConeAngle"]?.SetValue(MathHelper.ToRadians(ConeAngle / 2f));
            effect.Parameters["LightFalloff"]?.SetValue(LightFalloff);
        }
    }
}
