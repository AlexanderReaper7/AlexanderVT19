using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alexander_VT19
{
    public class Player
    {
        private CustomModel customModel;
        private PlayerIndex _controllerIndex;

        public Vector2 Input => GamePad.GetState(_controllerIndex).ThumbSticks.Right;

        public Player(PlayerIndex controllerIndex, CustomModel model, GraphicsDevice graphicsDevice)
        {
            _controllerIndex = controllerIndex;
            customModel = model;
        }



        public void Update()
        {
            // Get controller input
            GamePadState gamePad = GamePad.GetState(_controllerIndex);
            
            Vector3 rotation = new Vector3(gamePad.ThumbSticks.Right, gamePad.ThumbSticks.Left.X);

            // Convert to radians
            rotation *= MathHelper.Pi;
            customModel.Rotation += rotation;
        }

    }
}
