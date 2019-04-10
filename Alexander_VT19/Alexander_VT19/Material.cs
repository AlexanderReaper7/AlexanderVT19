using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public class Material
    {
        public virtual void SetEffectParameters(Effect effect)
        {

        }
    }

    public class LightingMaterial : Material
    {
        public Vector3 AmbientColor { get; set; }
        public Vector3 LightDirection { get; set; }
        public Vector3 LightColor { get; set; }
        public Vector3 SpecularColor { get; set; }

        public LightingMaterial()
        {
            AmbientColor = new Vector3(.1f,.1f,.1f);
            LightDirection = Vector3.One;
            LightColor = new Vector3(.9f,.9f,.9f);
            SpecularColor = Vector3.One;
        }

        public override void SetEffectParameters(Effect effect)
        {
            effect.Parameters["AmbientColor"]?.SetValue(AmbientColor);
            effect.Parameters["LightDirection"]?.SetValue(LightDirection);
            effect.Parameters["LightColor"]?.SetValue(LightColor);
            effect.Parameters["SpecularColor"]?.SetValue(SpecularColor);
        }
    }
}
