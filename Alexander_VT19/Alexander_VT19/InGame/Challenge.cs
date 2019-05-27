using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexander_VT19;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public class Challenge
    {
        public static Vector3 Position { get; set; } = Vector3.Zero;

        public Model Model
        {
            set { _customModel.Model = value; }
        }

        private CustomModel _customModel;
        private readonly float _margin;

        /// <summary>
        /// Creates a new random Challenge with the specified margin using a pre-existing challenge
        /// </summary>
        /// <param name="challenge"></param>
        /// <param name="margin">How accurate the player needs to be in % (0 to 1) to get correct</param>
        public Challenge(Challenge challenge, float margin) : this(challenge._customModel, margin)
        {
        }

        /// <summary>
        /// Creates a new random Challenge with the specified margin
        /// </summary>
        /// <param name="customModel"></param>
        /// <param name="margin">How accurate the player needs to be in % (0 to 1) to get correct</param>
        public Challenge(CustomModel customModel, float margin)
        {
            // Create a random rotation Vector3
            Random r = new Random();
            Vector3 v = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
            v *= MathHelper.TwoPi;

            // Apply new values
            _customModel = customModel;
            _customModel.Rotation = v;
            _customModel.Position = Position;
            _customModel.Material = new SimpleMaterial();
            // Calculate and apply new color
            ((SimpleMaterial) _customModel.Material).DiffuseColor = ColorHelper.CalculateColorFromRotation(_customModel.Rotation).ToVector3();

            _margin = MathHelper.Clamp(margin, 0, 1);
        }

        #region Old checking (numerical)
        //public bool CheckCorrectRotation(Player player)
        //{
        //    Vector3 minRotation = new Vector3(_customModel.Rotation.X - Margin, _customModel.Rotation.Y - Margin, _customModel.Rotation.Z - Margin);
        //    Vector3 maxRotation = new Vector3(_customModel.Rotation.X + Margin, _customModel.Rotation.Y + Margin, _customModel.Rotation.Z + Margin);

        //    if (player.customModel.Rotation.X < maxRotation.X && player.customModel.Rotation.X > minRotation.X)
        //        if (player.customModel.Rotation.Y < maxRotation.Y && player.customModel.Rotation.Y > minRotation.Y)
        //            if (player.customModel.Rotation.Z < maxRotation.Z && player.customModel.Rotation.Z > minRotation.Z)
        //                return true;

        //    return false;
        //}

        //public bool CheckCorrectColor(Player player)
        //{
        //    Color playerColor = CalculateColorFromRotation(player.customModel.Rotation); //TODO: refactor
        //    Color maxColor = CalculateColorFromRotation(_customModel.Rotation);
        //    Color minColor = maxColor;

        //    maxColor.R += (byte)Margin;
        //    maxColor.G += (byte)Margin;
        //    maxColor.B += (byte)Margin;
        //    minColor.R -= (byte)Margin;
        //    minColor.G -= (byte)Margin;
        //    minColor.B -= (byte)Margin;

        //    if (playerColor.R < maxColor.R && playerColor.R > minColor.R)
        //        if (playerColor.G < maxColor.G && playerColor.G > minColor.G)
        //            if (playerColor.B < maxColor.B && playerColor.B > minColor.B)
        //                return true;

        //    return false;
        //}
        #endregion

        public bool CheckCorrectColor(Player player)
        {
            // Calculate colors from rotation
            Color playerColor = ColorHelper.CalculateColorFromRotation(player.CustomModel.Rotation); //TODO: refactor
            Color color = ColorHelper.CalculateColorFromRotation(_customModel.Rotation);

            if (playerColor.R / (float)color.R < 1 + _margin && playerColor.R / (float)color.R > 1 - _margin)
                if (playerColor.G / (float)color.G < 1 + _margin && playerColor.G / (float)color.G > 1 - _margin)
                    if (playerColor.B / (float)color.B < 1 + _margin && playerColor.B / (float)color.B > 1 - _margin)
                        return true; // If all values are within the margins, return true

            return false; // Else return false
        }



        public void Draw(Camera camera)
        {
            _customModel.Draw(camera.View, camera.Projection, camera.Position);
        }
    }
}
