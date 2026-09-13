using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NOBOIShooter.GameObjects
{
        public class MotorParticular
    {
            private Random random;

            public Vector2 IssuerPosition { get; set; }
            public List<Particular> particular;
            private List<Texture2D> texture;
            public Boolean randoms = false;

            public MotorParticular(List<Texture2D> texture, Vector2 position)
            {
                IssuerPosition = position;
                this.texture = texture;
                this.particular = new List<Particular>();
                random = new Random();
            }

            public MotorParticular()
            {
                this.particular = new List<Particular>();
                this.texture = new List<Texture2D>();
                random = new Random();
            }

            private Particular GenerateRandomParticles(float maxSize, Int32 max_Life_Time, Color color)
            {
                Texture2D texture = this.texture[random.Next(this.texture.Count)];
                Vector2 posicion = IssuerPosition;
                Vector2 velocidad = new Vector2(1f * (float)(random.NextDouble() * 2 - 1),
                1f * (float)(random.NextDouble() * 2 - 1));
                float angle = 0;
                float angular_velocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
                float size = (float)random.NextDouble() * maxSize;
                Int16 time_of_life = Convert.ToInt16(random.Next(max_Life_Time));
                return new Particular(texture, posicion, velocidad, angle, angular_velocity, color, size, time_of_life);
            }

            public void addParticle(Texture2D texture, Vector2 position, Vector2 speed, float angle, float angular_velocity,
            Color color, float size, Int16 time_of_life)
            {
                particular.Add(new Particular(texture, position, speed, angle, angular_velocity, color, size, time_of_life));
            }

            public void startParticles(Int32 quantityTextures, float maxSize, Int32 max_Life_Time, Color color)
            {
                for (int i = 0; i < quantityTextures; i++)
                {
                    particular.Add(GenerateRandomParticles(maxSize, max_Life_Time, color));
                }
            }

            public void addTexture(Texture2D texture)
            {
                this.texture.Add(texture);
            }

            public void Update()
            {
                for (Int32 particular = 0; particular < this.particular.Count; particular++)
                {
                    this.particular[particular].Update();
                    if (this.particular[particular].Time_of_life <= 0)
                    {
                        this.particular.RemoveAt(particular);
                        particular--;
                    }
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                for (Int32 index = 0; index < particular.Count; index++)
                {
                    particular[index].Draw(spriteBatch);
                }
            }
        }
}
