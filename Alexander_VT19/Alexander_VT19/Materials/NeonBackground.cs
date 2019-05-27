using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    class NeonBackground : ExtendedMaterial
    {
        public Vector2 Resolution { get; set; }
        public float Time { get; set; }

        protected NeonBackground(Effect effect) : base(effect)
        {
        }

        public static NeonBackground CreateNew(ContentManager content)
        {
            Effect effect = content.Load<Effect>(@"Effects/NeonBackground");
            return new NeonBackground(effect);
        }

        protected override void SetEffectParameters(Effect effect)
        {
            try
            {
                effect.Parameters["resolution"].SetValue(Resolution);
                effect.Parameters["time"].SetValue(Time);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void UpdateEffectParameters(GameTime gameTime)
        {
            Time = (float)gameTime.TotalGameTime.TotalMilliseconds / 1000f;
            SetEffectParameters(Effect);
        }
    }
}
