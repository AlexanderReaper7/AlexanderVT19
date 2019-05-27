using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19.Lights
{
    /// <summary>
    /// A Point Light
    /// </summary>
    class PointLight
    {
        //Position
        Vector3 _position;
        //Radius
        float _radius;
        //Color
        Vector4 _color;
        //Intensity
        float _intensity;
        //ShadowMap
        RenderTargetCube _shadowMap;
        //Is this Light with Shadows?
        bool _isWithShadows;
        //Shadow Map Resoloution
        int _shadowMapResoloution;
        #region Get Functions
        //Get Position
        public Vector3 GetPosition() { return _position; }
        //Get Radius
        public float GetRadius() { return _radius; }
        //Get Color
        public Vector4 GetColor() { return _color; }
        //Get Intensity
        public float GetIntensity() { return _intensity; }
        //Get IsWithShadows
        public bool GetIsWithShadows() { return _isWithShadows; }
        //Get ShadowMapResoloution
        public int GetShadowMapResoloution()
        {
            if (_shadowMapResoloution < 2048) return _shadowMapResoloution;
            else return 2048;
        }
        //Get DepthBias
        public float GetDepthBias() { return (1.0f / (20 * _radius)); }
        //Get ShadowMap
        public RenderTargetCube GetShadowMap() { return _shadowMap; }
        #endregion
        #region Set Functions
        //Set Position
        public void SetPosition(Vector3 position) { this._position = position; }
        //Set Radius
        public void SetRadius(float radius) { this._radius = radius; }
        //Set Color
        public void SetColor(Color color) { this._color = color.ToVector4(); }
        //Set Color
        public void SetColor(Vector4 color) { this._color = color; }
        //Set Intensity
        public void SetIntensity(float intensity) { this._intensity = intensity; }
        //Set isWithShadows
        public void SetIsWithShadows(bool shadows) { this._isWithShadows = shadows; }
        #endregion
        //Constructor
        public PointLight(GraphicsDevice graphicsDevice, Vector3 position, float radius,
       Vector4 color, float intensity, bool isWithShadows,
       int shadowMapResoloution)
        {
            //Set Position
            SetPosition(position);
            //Set Radius
            SetRadius(radius);
            //Set Color
            SetColor(color);
            //Set Intensity
            SetIntensity(intensity);
            //Set isWithShadows
            this._isWithShadows = isWithShadows;
            //Set shadowMapResoloution
            this._shadowMapResoloution = shadowMapResoloution;
            //Make ShadowMap
            _shadowMap = new RenderTargetCube(graphicsDevice, GetShadowMapResoloution(),
           false, SurfaceFormat.Single,
           DepthFormat.Depth24Stencil8);
        }
        //Create World Matrix for Deferred Rendering Geometry
        public Matrix World()
        {
            //Make Scaling Matrix
            Matrix scale = Matrix.CreateScale(_radius / 100.0f);
            //Make Translation Matrix
            Matrix translation = Matrix.CreateTranslation(_position);
            //Return World Transform
            return (scale * translation);
        }
    }
}
