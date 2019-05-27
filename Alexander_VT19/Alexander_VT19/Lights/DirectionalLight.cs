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
        Vector3 _direction;
        //Color
        Vector4 _color;
        //Intensity
        float _intensity;

        #region Get Functions
        //Get Direction
        public Vector3 GetDirection() { return _direction; }
        //Get Color
        public Vector4 GetColor() { return _color; }
        //Get Intensity
        public float GetIntensity() { return _intensity; }
        #endregion

        #region Set Functions
        //Set Direction
        public void SetDirection(Vector3 dir) { dir.Normalize(); this._direction = dir; }
        //Set Color
        public void SetColor(Vector4 color) { this._color = color; }
        //Set Color
        public void SetColor(Color color) { this._color = color.ToVector4(); }
        //Set Intensity
        public void SetIntensity(float intensity) { this._intensity = intensity; }
        #endregion

        //Constructor
        public DirectionalLight(Vector3 direction, Vector4 color, float intensity)
        {
            SetDirection(direction);
            SetColor(color);
            SetIntensity(intensity);
        }

        //Constructor
        public DirectionalLight(Vector3 direction, Color color, float intensity)
        {
            SetDirection(direction);
            SetColor(color);
            SetIntensity(intensity);
        }
    }
}
