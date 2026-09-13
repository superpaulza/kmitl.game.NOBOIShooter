using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NOBOIShooter.GameObjects
{
    public class Particular : GameObject
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }
        public float Angle { get; set; }
        public float Angular_velocity { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        public Int16 Time_of_life { get; set; }

        public Particular(Texture2D texture, Vector2 position, Vector2 speed, float angle, float angular_velocity, Color color, float size, Int16 time_of_life)
            : base(texture)
        {
            Texture = texture;
            Position = position;
            Speed = speed;
            Angle = angle;
            Angular_velocity = angular_velocity;
            Color = color;
            Size = size;
            Time_of_life = time_of_life;
        }

        public void Update()
        {
            Time_of_life--;
            Position += Speed;
            Angle += Angular_velocity;
        }

        override
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rectangle_Font = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            spriteBatch.Draw(Texture, Position, rectangle_Font, Color, Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }
}