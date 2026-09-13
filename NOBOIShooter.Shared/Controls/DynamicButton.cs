using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NOBOIShooter.Controls
{
    public class DynamicButton : Component
    {
        SoundEffectInstance _sound;

        private MouseState _currentMouse;

        private SpriteFont _font;

        private bool _isHovering;

        private MouseState _previousMouse;

        private Rectangle mouseRectangle;

        private Color colour;

        public event EventHandler Click;

        public bool IsVisible { get; set; }

        public bool Clicked { get; private set; }

        public Color PenColour { get; set; }

        public Vector2 Position { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            }
        }

        public string Text { get; set; }

        public Texture2D Texture;

        public DynamicButton(Texture2D texture, SpriteFont font, ContentManager _content)
        {
            Texture = texture;

            _font = font;

            PenColour = Color.Black;

            _sound = _content.Load<SoundEffect>("BGM/ButtonBGM").CreateInstance();

            IsVisible = true;

        }

        public DynamicButton(Texture2D texture, ContentManager _content)
        {
            Texture = texture;

            _sound = _content.Load<SoundEffect>("BGM/ButtonBGM").CreateInstance();

            IsVisible = true;

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                colour = Color.White;

                if (_isHovering)
                    colour = Color.Gray;

                spriteBatch.Draw(Texture, Rectangle, colour);

                if (!string.IsNullOrEmpty(Text))
                {
                    var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                    var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                    spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (IsVisible)
            {
                _sound.Volume = Singleton.Instance.SFXVolume;

                _previousMouse = _currentMouse;
                _currentMouse = Mouse.GetState();

                mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

                _isHovering = false;

                if (mouseRectangle.Intersects(Rectangle))
                {
                    _isHovering = true;

                    if (_currentMouse.LeftButton == ButtonState.Released &&
                        _previousMouse.LeftButton == ButtonState.Pressed)
                    {
                        Click?.Invoke(this, new EventArgs());
                        _sound.Play();
                    }
                }
            }
        }

    }
}
