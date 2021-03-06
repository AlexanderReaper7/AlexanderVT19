﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    public static class SplitScreenHelper
    {
        private static int _screenWidth;
        private static int _screenHeight;
        private static Rectangle[] _rectangles;
        private static SpriteBatch _spriteBatch;

        public static void DrawSplitScreen(GraphicsDevice graphicsDevice, params RenderTarget2D[] renderTargets)
        {
            // Initialize
            _screenWidth = graphicsDevice.Viewport.Width;
            _screenHeight = graphicsDevice.Viewport.Height;
            int n = renderTargets.Length;
            if (_rectangles == null || _rectangles.Length != n) _rectangles = GenerateRectangleArray(n); 
            if (_spriteBatch == null) _spriteBatch = new SpriteBatch(graphicsDevice);

            // Draw
            for (int i = 0; i < n; i++)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(renderTargets[i], _rectangles[i], Color.White);
                _spriteBatch.End();
            }
        }

        private static Rectangle[] GenerateRectangleArray(int numRect)
        {
            Rectangle[] output = new Rectangle[numRect];

            switch (numRect)
            {
                case 1: // fullscreen
                    output[0] = new Rectangle(0,0,_screenWidth, _screenHeight);
                    return output;

                case 2: // side by side split screen
                    output[0] = new Rectangle(0,0, _screenWidth / 2, _screenHeight);
                    output[1] = new Rectangle(_screenWidth / 2,0, _screenWidth / 2, _screenHeight);
                    return output;

                case 3: // two top one bottom, all the same sizes TODO
                    output[0] = new Rectangle();
                    output[1] = new Rectangle();
                    output[2] = new Rectangle();
                    return output;

                case 4: // quarters in each corner of window TODO
                    output[0] = new Rectangle();
                    output[0] = new Rectangle();
                    output[0] = new Rectangle();
                    output[0] = new Rectangle();
                    return output;

                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
