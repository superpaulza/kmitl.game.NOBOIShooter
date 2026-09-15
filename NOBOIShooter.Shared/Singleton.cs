using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace NOBOIShooter
{
    //apply for singleton design pattern
    class Singleton
    {
        //Store default value or parameter here!
        
        public int Score = 0;
        public bool IsMouseVisible = true, Shooting = false;
        public string ContentRootDir = "Content";
        public bool IsEnableAimer = false;
        public bool IsBGMEnable = true;
        public bool IsSFXEnable = true;
        public float BGMVolume = 1.0f;
        public float SFXVolume = 1.0f;

        public readonly int ScreenHeight = 720;
        public readonly int ScreenWidth = 1280;

        // Virtual resolution is the design resolution (Desktop 1280x720).
        // On mobile the real backbuffer differs, so we letterbox-scale.
        public Matrix ScaleMatrix = Matrix.Identity;
        public float ViewScale = 1f;
        public float ViewOffsetX = 0f;
        public float ViewOffsetY = 0f;
        public int ViewportWidth = 1280;
        public int ViewportHeight = 720;

        public void UpdateViewport(int realWidth, int realHeight)
        {
            if (realWidth <= 0 || realHeight <= 0) return;
            ViewportWidth = realWidth;
            ViewportHeight = realHeight;
            ViewScale = System.Math.Min((float)realWidth / ScreenWidth, (float)realHeight / ScreenHeight);
            if (ViewScale <= 0f) ViewScale = 1f;
            ViewOffsetX = (realWidth - ScreenWidth * ViewScale) / 2f;
            ViewOffsetY = (realHeight - ScreenHeight * ViewScale) / 2f;
            ScaleMatrix = Matrix.CreateTranslation(-ViewOffsetX, -ViewOffsetY, 0f)
                * Matrix.CreateScale(1f / ViewScale, 1f / ViewScale, 1f);
            // NOTE: ScaleMatrix maps device pixels -> virtual pixels.
            // For SpriteBatch (virtual -> device) use GetRenderScaleMatrix().
        }

        public Matrix GetRenderScaleMatrix()
        {
            return Matrix.CreateScale(ViewScale, ViewScale, 1f)
                * Matrix.CreateTranslation(ViewOffsetX, ViewOffsetY, 0f);
        }

        public Vector2 ToVirtual(Vector2 devicePixels)
        {
            if (ViewScale <= 0f) return devicePixels;
            return new Vector2((devicePixels.X - ViewOffsetX) / ViewScale, (devicePixels.Y - ViewOffsetY) / ViewScale);
        }

        //Base of singleton
        private static Singleton s_instance;

        //Constructor
        private Singleton()
        {
        }

        public static Singleton Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new Singleton();
                }
                return s_instance;
            }
        }
    }
}
