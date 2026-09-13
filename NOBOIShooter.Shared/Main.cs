using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NOBOIShooter.Screens;


namespace NOBOIShooter
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ScreenManager _screenManager;

        // Main Constructure 
        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = Singleton.Instance.ContentRootDir;
            IsMouseVisible = Singleton.Instance.IsMouseVisible;
        }

        // Initialize (Run on start)
        protected override void Initialize()
        {
            //setting screen size
            _graphics.PreferredBackBufferHeight = Singleton.Instance.ScreenHeight;
            _graphics.PreferredBackBufferWidth = Singleton.Instance.ScreenWidth;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        // Load content (such as assets, picture, music)
        protected override void LoadContent()
        {
            // Create a spriteBatch
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load screen manager
            _screenManager = new ScreenManager(this, _graphics.GraphicsDevice, Content);
            

        }
      
        // Update program logic everytime
        protected override void Update(GameTime gameTime)
        {
           
            // Update screen
            _screenManager.Update(gameTime);

            base.Update(gameTime);
        }

        // Draw Main Method 
        protected override void Draw(GameTime gameTime)
        {
            // Clear programpage with white colour
            GraphicsDevice.Clear(Color.White);

            // Draw program current screen
            _screenManager.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        public void ChangeScreen(ScreenSelect screenSelect)
        {
            _screenManager.ChangeScreen(screenSelect);
        }
    }
}
