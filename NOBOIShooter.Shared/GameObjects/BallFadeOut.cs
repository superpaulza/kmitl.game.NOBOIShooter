using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NOBOIShooter.GameObjects
{
    class BallFadeOut
    {
        public bool FadeOut{ get; set; }
        public float FadeSpeed { get; set; }
        public float FadeNow { get; set; }
        public int TileType { get; set; }
        public bool Visible { get; set; }

        private const float FADE_DEFUALT_SPEED = 0.005f;

        private BallGridManager _bord;
        private Texture2D _texture;
        private Vector2 _position;
        private float _scaleball;
        private Color _color;

        private double _timer = 0;

        public BallFadeOut(Texture2D texture,Color color, BallGridManager bord , bool fadeOut, int ballType, Vector2 position)
        {
            _bord = bord;
            _texture = texture;
            _scaleball = (float)_bord.TileWidth / _texture.Width;
            FadeSpeed = FADE_DEFUALT_SPEED;
            FadeOut = fadeOut;
            FadeNow = fadeOut ? 1f : 0f;
            TileType = ballType;
            _position = position;
            _color = color;
            Visible = true;
        }
  
        public void Draw(SpriteBatch spriteBatch)
        {
            //if (FadeNow > 0 && FadeNow < 1)
            if(Visible)
            {
                spriteBatch.Draw(_texture, _position, null, new Color( _color, FadeNow), 0f, Vector2.Zero, _scaleball, SpriteEffects.None, 0f);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Visible)
            {
                _timer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_timer > 10)
                {
                    if (FadeOut)
                        FadeNow -= FadeSpeed * (float)_timer;
                    else
                        FadeNow += FadeSpeed * (float)_timer;
                    _timer = 0;
                }

                if (( FadeNow < 0) || (FadeNow > 1))
                    Visible = false;

            }
        }

    }
}
