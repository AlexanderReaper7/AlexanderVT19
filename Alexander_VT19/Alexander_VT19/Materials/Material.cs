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


    public class ExtendedMaterial
    {
        public Effect Effect;

        protected ExtendedMaterial(Effect effect)
        {
            this.Effect = effect;
        }

        public virtual void UpdateEffectParameters()
        {
            SetEffectParameters(Effect);
        }

        protected virtual void SetEffectParameters(Effect effect)
        {

        }
    }

}
