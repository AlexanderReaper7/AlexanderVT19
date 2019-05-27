using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19.Materials
{
    public class VoronoiDistances : ExtendedMaterial
    {
        public float Gametime { get; set; }
        public Vector2 Resolution { get; set; }
        public Vector3 BorderColor { get; set; }
        public Texture2D RGBANoise { get; set; }

        protected VoronoiDistances(Effect effect) : base(effect)
        {
        }

        public static VoronoiDistances CreateNew(ContentManager content)
        {

            Effect effect = content.Load<Effect>(@"Voronoi");

            VoronoiDistances voronoi = new VoronoiDistances(effect)
                { RGBANoise = content.Load<Texture2D>(@"RGBANoise") };

            return voronoi;
        }

        protected override void SetEffectParameters(Effect effect)
        {
            try
            {
                effect.Parameters["borderColor"].SetValue(BorderColor);
                effect.Parameters["gameTime"].SetValue(Gametime);
                effect.Parameters["resolution"].SetValue(Resolution);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override void UpdateEffectParameters()
        {

        }

        public void UpdateEffectParameters(GameTime gameTime)
        {
            Gametime = (float)gameTime.TotalGameTime.TotalMilliseconds / 1000f;
            SetEffectParameters(Effect);
        }
    }
}
