using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Alexander_VT19.Lights
{
    class DirectionalLight
    {
        //Direction
        Vector3 direction;
        //Color
        Vector4 color;
        //Intensity
        float intensity;

        #region Get Functions
        //Get Direction
        public Vector3 getDirection() { return direction; }
        //Get Color
        public Vector4 getColor() { return color; }
        //Get Intensity
        public float getIntensity() { return intensity; }
        #endregion

        #region Set Functions
        //Set Direction
        public void setDirection(Vector3 dir) { dir.Normalize(); this.direction = dir; }
        //Set Color
        public void setColor(Vector4 color) { this.color = color; }
        //Set Color
        public void setColor(Color color) { this.color = color.ToVector4(); }
        //Set Intensity
        public void setIntensity(float intensity) { this.intensity = intensity; }
        #endregion

        //Constructor
        public DirectionalLight(Vector3 Direction, Vector4 Color, float Intensity)
        {
            setDirection(Direction);
            setColor(Color);
            setIntensity(Intensity);
        }

        //Constructor
        public DirectionalLight(Vector3 Direction, Color Color, float Intensity)
        {
            setDirection(Direction);
            setColor(Color);
            setIntensity(Intensity);
        }
    }
}
