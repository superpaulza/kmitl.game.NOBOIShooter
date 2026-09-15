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

            // NOTE: Shared compiles as plain net8.0, so ANDROID/IOS #defines are NOT
            // visible here — use runtime checks so the same binary adapts per head.
            if (System.OperatingSystem.IsAndroid() || System.OperatingSystem.IsIOS())
            {
                // Mobile: use native fullscreen resolution, landscape only.
                // Virtual 1280x720 design resolution is letterbox-scaled at draw/input time.
                _graphics.IsFullScreen = true;
                _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            }
            else
            {
                // Desktop: fixed design-resolution window (Windows build behaviour unchanged).
                _graphics.PreferredBackBufferHeight = Singleton.Instance.ScreenHeight;
                _graphics.PreferredBackBufferWidth = Singleton.Instance.ScreenWidth;
                _graphics.IsFullScreen = false;
            }
            try { Window.AllowUserResizing = true; } catch { }
        }

        // Initialize (Run on start)
        protected override void Initialize()
        {
            if (!(System.OperatingSystem.IsAndroid() || System.OperatingSystem.IsIOS()))
            {
                //setting screen size (desktop only — mobile uses native resolution)
                _graphics.PreferredBackBufferHeight = Singleton.Instance.ScreenHeight;
                _graphics.PreferredBackBufferWidth = Singleton.Instance.ScreenWidth;
                _graphics.ApplyChanges();
            }

            UpdateViewportScale();
            try { Window.ClientSizeChanged += (s, e) => UpdateViewportScale(); } catch { }

            base.Initialize();
        }

        private void UpdateViewportScale()
        {
            try
            {
                int w = GraphicsDevice?.Viewport.Width ?? Singleton.Instance.ScreenWidth;
                int h = GraphicsDevice?.Viewport.Height ?? Singleton.Instance.ScreenHeight;
                // GraphicsDevice may not exist yet during ctor — fall back to window size.
                if ((w <= 0 || h <= 0) && Window != null)
                {
                    // Window.ClientBounds is in device pixels on mobile.
                    w = Window.ClientBounds.Width;
                    h = Window.ClientBounds.Height;
                }
                Singleton.Instance.UpdateViewport(w, h);
            }
            catch { /* keep identity scale on failure */ }
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
            // Refresh letterbox scale if the window/surface changed (rotation, split-screen, foldables).
            if (GraphicsDevice != null &&
                (GraphicsDevice.Viewport.Width != Singleton.Instance.ViewportWidth ||
                 GraphicsDevice.Viewport.Height != Singleton.Instance.ViewportHeight))
            {
                UpdateViewportScale();
            }

            // Update screen
            _screenManager.Update(gameTime);

            base.Update(gameTime);
        }

        // Draw Main Method
        protected override void Draw(GameTime gameTime)
        {
            // Clear letterbox bars with black; screens paint their own background
            // inside the 1280x720 virtual area via the scale matrix.
            GraphicsDevice.Clear(Color.Black);

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
