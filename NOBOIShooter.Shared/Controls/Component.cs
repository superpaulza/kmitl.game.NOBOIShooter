using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NOBOIShooter
{
    //Control && UI_Component Must be inherit (extend) from this class
    public abstract class Component
    {
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void Update(GameTime gameTime);
    }
}
