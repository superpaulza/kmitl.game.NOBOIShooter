using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace NOBOIShooter.GameObjects
{
    public class BallTexture
    {
        private List<Ball> BallTextures;

        class Ball
        {
            private const int BALL_TILES_WIDTH = 40;
            public Texture2D Texture;
            public Color Colour;
            public float Scale;

            public Ball (Texture2D texture)
            {
                Texture = texture;
                Scale = (float) BALL_TILES_WIDTH / texture.Width;
                Colour = Color.White;
            }

            public Ball(Texture2D texture, Color color)
            {
                Texture = texture;
                Scale = (float)BALL_TILES_WIDTH / texture.Width;
                Colour = color;
            }
        }

        public BallTexture ()
        {
            BallTextures = new List<Ball>
            {
                null
            };
        }

        public void Add(Texture2D texture)
        {
            BallTextures.Add(new Ball(texture));
        }

        public void Add(Texture2D texture, Color color)
        {
            BallTextures.Add(new Ball(texture, color));
        }

        public Texture2D GetTexture(int index)
        {
            if (index < 1)
            {
                //Debug.WriteLine("Why you get negative number Ball");
                return null;
            }

            return BallTextures[index].Texture;
        }

        public float GetScale(int index)
        {
            if (index < 0)
            {
                //Debug.WriteLine("Why you get negative number Ball");
                return 1f;
            }

            return BallTextures[index].Scale;
        }

        public Color GetColor(int index)
        {
            if (index < 0)
            {
                //Debug.WriteLine("Why you get negative number Ball");
                return Color.White;
            }

            return BallTextures[index].Colour;
        }

        public void RemoveAt(int i)
        {
            BallTextures.RemoveAt(i);
        }

        public void Clear()
        {
            BallTextures.Clear();
        }
    }
}
