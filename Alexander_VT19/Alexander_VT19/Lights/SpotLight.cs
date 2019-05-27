using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19.Lights
{
    /// <summary>
    /// A Spot Light with shadows
    /// </summary>
    class SpotLight
    {
        //Position
        Vector3 _position;
        //Direction
        Vector3 _direction;
        //Color
        Vector4 _color;
        //Intensity
        float _intensity;
        //NearPlane
        float _nearPlane;
        //FarPlane
        float _farPlane;
        //FOV
        float _fov;
        //Is this Light with Shadows?
        bool _isWithShadows;
        //Shadow Map Resoloution
        int _shadowMapResoloution;
        //DepthBias for the Shadowing... (1.0f / 2000.0f)
        float _depthBias;
        //World(for geometry in LightMapping phase...)
        Matrix _world;
        //View
        Matrix _view;
        //Projection
        Matrix _projection;
        //Shadow Map
        RenderTarget2D _shadowMap;
        //Attenuation Texture
        Texture2D _attenuationTexture;
        #region Get Functions
        //Get Position
        public Vector3 GetPosition() { return _position; }
        //Get Direction
        public Vector3 GetDirection() { return _direction; }
        //Get Color
        public Vector4 GetColor() { return _color; }
        //Get Intensity
        public float GetIntensity() { return _intensity; }
        //Get NearPlane
        public float GetNearPlane() { return _nearPlane; }
        //Get FarPlane
        public float GetFarPlane() { return _farPlane; }
        //Get FOV
        public float GetFov() { return _fov; }
        //Get IsWithShadows
        public bool GetIsWithShadows() { return _isWithShadows; }
        //Get ShadowMapResoloution
        public int GetShadowMapResoloution()
        {
            if (_shadowMapResoloution < 2048)
                return _shadowMapResoloution;
            else
                return 2048;
        }
        //Get DepthBias
        public float GetDepthBias() { return _depthBias; }
        //Get World
        public Matrix GetWorld() { return _world; }
        //Get View
        public Matrix GetView() { return _view; }
        //Get Projection
        public Matrix GetProjection() { return _projection; }
        //Get ShadowMap
        public RenderTarget2D GetShadowMap() { return _shadowMap; }
        //Get Attenuation Texture
        public Texture2D GetAttenuationTexture() { return _attenuationTexture; }
        #endregion
        #region Set Functions
        //Set Position
        public void SetPosition(Vector3 position) { this._position = position; }
        //Set Direction
        public void SetDirection(Vector3 direction)
        {
            direction.Normalize();
            this._direction = direction;
        }
        //Set Color
        public void SetColor(Vector4 color) { this._color = color; }
        //Set Color
        public void SetColor(Color color) { this._color = color.ToVector4(); }
        //Set Intensity
        public void SetIntensity(float intensity) { this._intensity = intensity; }
        //Set isWithShadows
        public void SetIsWithShadows(bool iswith) { this._isWithShadows = iswith; }
        //Set DepthBias
        public void SetDepthBias(float bias) { this._depthBias = bias; }
        //Set Attenuation Texture
        public void SetAttenuationTexture(Texture2D attenuationTexture)
        {
            this._attenuationTexture = attenuationTexture;
        }
        #endregion
        //Constructor
        public SpotLight(GraphicsDevice graphicsDevice, Vector3 position,
       Vector3 direction, Vector4 color, float intensity,
       bool isWithShadows, int shadowMapResoloution,
       Texture2D attenuationTexture)
        {
            //Position
            SetPosition(position);
            //Direction
            SetDirection(direction);
            //Color
            SetColor(color);
            //Intensity
            SetIntensity(intensity);
            //NearPlane
            _nearPlane = 1.0f;
            //FarPlane
            _farPlane = 100.0f;
            //FOV
            _fov = MathHelper.PiOver2;
            //Set whether Is With Shadows
            SetIsWithShadows(isWithShadows);
            //Shadow Map Resoloution
            _shadowMapResoloution = shadowMapResoloution;
            //Depth Bias
            _depthBias = 1.0f / 2000.0f;
            //Projection
            _projection = Matrix.CreatePerspectiveFieldOfView(_fov, 1.0f, _nearPlane,
            _farPlane);
            //Shadow Map
            _shadowMap = new RenderTarget2D(graphicsDevice, GetShadowMapResoloution(),
            GetShadowMapResoloution(), false,
            SurfaceFormat.Single,
            DepthFormat.Depth24Stencil8);
            //Attenuation Texture
            _attenuationTexture = attenuationTexture;
            //Create View and World
            Update();
        }
        //Calculate the Cosine of the LightAngle
        public float LightAngleCos()
        {
            //float ConeAngle = 2 * atanf(Radius / Height);
            float coneAngle = _fov;
            return (float)Math.Cos((double)coneAngle);
        }
        //Update
        public void Update()
        {
            //Target
            Vector3 target = (_position + _direction);
            if (target == Vector3.Zero) target = -Vector3.Up;
            //Up
            Vector3 up = Vector3.Cross(_direction, Vector3.Up);
            if (up == Vector3.Zero) up = Vector3.Right;
            else up = Vector3.Up;
            //ReMake View
            _view = Matrix.CreateLookAt(_position, target, up);
            //Make Scaling Factor
            float radial = (float)Math.Tan((double)_fov / 2.0) * 2 * _farPlane;
            //Make Scaling Matrix
            Matrix scaling = Matrix.CreateScale(radial, radial, _farPlane);
            //Make Translation Matrix
            Matrix translation = Matrix.CreateTranslation(_position.X, _position.Y, _position.Z);
            //Make Inverse View
            Matrix inverseView = Matrix.Invert(_view);
            //Make Semi-Product
            Matrix semiProduct = scaling * inverseView;
            //Decompose Semi-Product
            Vector3 s; Vector3 p; Quaternion q;
            semiProduct.Decompose(out s, out q, out p);
            //Make Rotation
            Matrix rotation = Matrix.CreateFromQuaternion(q);
            //Make World
            _world = scaling * rotation * translation;
        }
    }
}
