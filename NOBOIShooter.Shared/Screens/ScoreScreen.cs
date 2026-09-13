using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NOBOIShooter.Controls;
using System;
using NOBOIShooter.Data;

namespace NOBOIShooter.Screens
{
    class ScoreScreen : AScreen
    {
        // Set Confix value
        private const int LIMIT_SCORE_PER_PAGE = 7;

        // Create private value
        private Texture2D _backIcon;
        private Texture2D _background;
        private Texture2D _leftIcon;
        private Texture2D _rightIcon;
        private Texture2D _pen;
        private Button _backButton;
        private Button _leftButton;
        private Button _rightButton;
        
        private SpriteFont _font;
        private ScoreData _scoreBord;

        private Rectangle _areaBackGround;

        // Action value
        private int _scorePage = 0;
        private bool _hasNextPage = true;
        private string _titleText = "Score List";
        private string _scoretext;
        private Vector2 _titlePosion;

        public ScoreScreen(Main game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            // Create and Load Context
            _backIcon = _content.Load<Texture2D>("Controls/BackButtonBlack");
            _leftIcon = _content.Load<Texture2D>("Icons/ArrowLeft");
            _rightIcon = _content.Load<Texture2D>("Icons/ArrowRight");
            _background = _content.Load<Texture2D>("Backgrouds/background");
            _font = _content.Load<SpriteFont>("Fonts/Font");
            _scoreBord = new ScoreData();
            
            _pen = new Texture2D(graphicsDevice, 1, 1);
            _pen.SetData(new[] { Color.White });

            // Setting value
            int width = Singleton.Instance.ScreenWidth / 5;
            int height = Singleton.Instance.ScreenHeight / 15;
            _areaBackGround = new Rectangle(width, height, width * 3 , height * 10);

            _titlePosion = new Vector2((float)(Singleton.Instance.ScreenWidth - _font.MeasureString(_titleText).X) / 2, 100f);


            // Create Button
            _backButton = new Button(_backIcon, content)
            {
                Position = new Vector2(1200, 20),
            };

            _backButton.Click += BackButtonOnClick;

            _leftButton = new Button(_leftIcon, content)
            {
                Position = new Vector2(550, 550),
            };

            _leftButton.Click += LeftButtonOnClick;

            _rightButton = new Button(_rightIcon, content)
            {
                Position = new Vector2(700, 550),
            };

            _rightButton.Click += RightButtonOnClick;

        }

        

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Start drawing
            spriteBatch.Begin();

            // Draw backgrounds
            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(_pen, _areaBackGround, new Color(Color.Black,.5f));

            // Draw text title
            spriteBatch.DrawString(_font, _titleText, _titlePosion, Color.Orange);

            // Write score 
            for (int i = 0; i < LIMIT_SCORE_PER_PAGE; i++)
            {
                int index = _scorePage * LIMIT_SCORE_PER_PAGE + i;
                if(index < _scoreBord.ScoresTables.Count)
                {
                    _scoretext = (index + 1) +  ". Score : "+ _scoreBord.ScoresTables[index].ScoreGet.ToString() + " | " + _scoreBord.ScoresTables[index].ScoreDate.ToString("g");
                    spriteBatch.DrawString(_font, _scoretext, new Vector2((float)(Singleton.Instance.ScreenWidth - _font.MeasureString(_scoretext).X) / 2, 150f + i* 50), (i % 2 == 0) ? Color.White : Color.LightGray);
                }

            }

            // Show button
            _backButton.Draw(gameTime, spriteBatch);
            if(_scorePage > 0)
                _leftButton.Draw(gameTime, spriteBatch);
            if(_hasNextPage)
                _rightButton.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime)
        {
            
            // Update button
            _backButton.Update(gameTime);
            if (_scorePage > 0)
                _leftButton.Update(gameTime);
            if (_hasNextPage)
                _rightButton.Update(gameTime);
        }

        private void CheckHaveNextPage()
        {
            _hasNextPage = _scorePage < _scoreBord.ScoresTables.Count / LIMIT_SCORE_PER_PAGE;
        }

        private void BackButtonOnClick(object sender, EventArgs e)
        {
            _game.ChangeScreen(ScreenSelect.Menu);
        }

        private void LeftButtonOnClick(object sender, EventArgs e)
        {
            if (_scorePage > 0)
                _scorePage--;
            CheckHaveNextPage();
        }

        private void RightButtonOnClick(object sender, EventArgs e)
        {
            if (_hasNextPage)
                _scorePage++;
            CheckHaveNextPage();
        }

    }
}
