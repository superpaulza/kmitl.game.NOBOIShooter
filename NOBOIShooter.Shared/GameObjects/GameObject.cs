using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NOBOIShooter
{
    //Every game object must inherit from this class
    public class GameObject
    {
        protected Texture2D _texture;

        public float Rotation;
        public string Name;
        public Vector2 Position, Scale;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int) Position.X, (int) Position.Y, _texture.Width, _texture.Height);
            }
        }

        //Constructor
        public GameObject(Texture2D texture)
        {
            _texture = texture;
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Rotation = 0f;
        }

        //Virtual method (parent class) is only accept "override" from child class 
        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void Reset()
        {
        }
    }
}
