using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using NOBOIShooter.Controls;
using NOBOIShooter.GameObjects;
using System;
using NOBOIShooter.Data;

namespace NOBOIShooter.Screens
{
    //Game screen
    public class GameScreen : AScreen
    {
        // Create all value
        private const int DEAD_LINE_THICK = 5;
        private BallTexture _ballTexture;
        private Texture2D _bubbleImg;
        private Texture2D _shooterImg;
        private Texture2D _backBlackIcon;
        private Texture2D _backWhiteIcon;
        private Texture2D _restartIcon;
        private Texture2D _background;
        private Texture2D _pen;

        private SpriteFont _textFront;
        private SoundEffect _gameSfxBg;
        private SoundEffect _gameSfxEnd;
        private SoundEffect _gameSfxWin;
        private SoundEffectInstance _sfxBgInstance;
        private SoundEffectInstance _sfxEndInstance;
        private SoundEffectInstance _sfxWinInstance;

        private BallGridManager _bord;
        private Player _player;

        private DynamicButton _backButton;
        private DynamicButton _restartButton;

        private ScoreData _scoreTable;
        private bool _gameScoreSave = false;

        private Rectangle _fullScreen, _gameScreen, _deadline;
        private Vector2 _textPosition;
        private Color _gameColorBackground;

        public GameScreen(Main game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            // Load all Context
            _textFront = _content.Load<SpriteFont>("Fonts/Font");
            _backBlackIcon = _content.Load<Texture2D>("Controls/BackButtonBlack");
            _backWhiteIcon = _content.Load<Texture2D>("Controls/BackButtonWhite");
            _restartIcon = _content.Load<Texture2D>("Controls/RestartIcon");
            _background = _content.Load<Texture2D>("Backgrouds/gameBackground");
            _gameSfxBg = _content.Load<SoundEffect>("BGM/GameScreenBGM");
            _gameSfxEnd = _content.Load<SoundEffect>("BGM/GameOverBGM");
            _gameSfxWin = _content.Load<SoundEffect>("BGM/VictoryBGM");
            _bubbleImg = _content.Load<Texture2D>("Item/bubble");
            _shooterImg = _content.Load<Texture2D>("Item/hand-gun");
            _pen = new Texture2D(_graphicsDevice, 1, 1);
            _pen.SetData(new[] { Color.White });
            
            // Create Instance to control sound
            _sfxBgInstance = _gameSfxBg.CreateInstance();
            _sfxWinInstance = _gameSfxWin.CreateInstance();
            _sfxEndInstance = _gameSfxEnd.CreateInstance();
            _sfxBgInstance.Volume = Singleton.Instance.BGMVolume;
            _sfxWinInstance.Volume = Singleton.Instance.SFXVolume;
            _sfxEndInstance.Volume = Singleton.Instance.SFXVolume;
            _sfxBgInstance.Play();

            // Create Button
            _backButton = new DynamicButton(_backBlackIcon, _content)
            {
                Position = new Vector2(1200, 20),
            };

            _backButton.Click += BackButton_Click;

            _restartButton = new DynamicButton(_restartIcon, _content)
            {
                IsVisible = false,
            };

            _restartButton.Click += RestartButton_Click;

            // Adding Image Ball
            _ballTexture = new BallTexture();
            _ballTexture.Add(_bubbleImg);
            _ballTexture.Add(_bubbleImg,Color.Red);
            _ballTexture.Add(_bubbleImg,Color.Green);
            _ballTexture.Add(_bubbleImg,Color.Blue);

            // Create Game Main object
            _bord = new BallGridManager(_ballTexture, content);
            _player = new Player(_bord, _shooterImg, _ballTexture, _pen, content);
            
            // Load Score damanager
            _scoreTable = new ScoreData();
            _scoreTable.LoadSave();

            // Set game ghaphic Value
            _fullScreen = new Rectangle(0, 0, Singleton.Instance.ScreenWidth, Singleton.Instance.ScreenHeight);
            _gameScreen = new Rectangle((int)_bord.Position.X, 0, _bord.Width, Singleton.Instance.ScreenHeight);
            _gameColorBackground = new Color(Color.Black, 0.5f);
            _textPosition = new Vector2(1000, 180);

            int line = (int)(_bord.Position.Y + _bord.Height - 2 * _bord.RowHeight + _bord.TileHeight );
            _deadline = new Rectangle((int)_bord.Position.X, line, _bord.Width, DEAD_LINE_THICK);

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw Backgrounds
            spriteBatch.Draw(_background, _fullScreen, Color.White);
            spriteBatch.Draw(_pen, _gameScreen, _gameColorBackground);
            spriteBatch.Draw(_pen, _deadline, Color.DarkRed);

            // Draw game object
            _player.Draw(spriteBatch, gameTime);
            _bord.Draw(spriteBatch, gameTime);

            if (!_bord.GamePause)
            {
                spriteBatch.DrawString(_textFront, "Score \n\n " + _bord.GameScore, _textPosition, Color.Black);
                //Debug.WriteLine("Score : " + _bord.GameScore);
            }

            // Draw fade out
            if (_bord.GameWin || _bord.GameEnd)
            {

                if (!_gameScoreSave)
                {
                    _scoreTable.Add(new Score(_bord.GameScore, DateTime.Now));
                    _scoreTable.Sort();
                    _scoreTable.SaveGame();
                    _gameScoreSave = true;
                    //Debug.WriteLine("Save Score ?: " + _bord.GameScore);
                    
                    // Change Color of back button went fade screen
                    _backButton.Texture = _backWhiteIcon;
                }

                spriteBatch.Draw(_pen, _fullScreen, new Color(Color.Black, .6f));

                string gameScore = "Your score is " + _bord.GameScore;
                spriteBatch.DrawString(_textFront, gameScore,
                    new Vector2((float)(Singleton.Instance.ScreenWidth - _textFront.MeasureString(gameScore).X) / 2, 260f), Color.White);

                _restartButton.Draw(gameTime, spriteBatch);
                _sfxBgInstance.Pause();
            }

            // Write text Win
            if (_bord.GameWin)
            {
                _sfxWinInstance.Play();
                string gameWon = "You Won \n\n Want to play again?";
                spriteBatch.DrawString(_textFront, gameWon,  new Vector2(FindCenterText(gameWon), 100f), Color.White);
            }

            // Write text Lose
            else if (_bord.GameEnd)
            {
                _sfxEndInstance.Play();
                string gameEnding = "Game Ending \n\n Thank for playing.";
                spriteBatch.DrawString(_textFront, gameEnding, new Vector2(FindCenterText(gameEnding), 100f), Color.White);
            }

            // Draw button
            _backButton.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            _backButton.Update(gameTime);
            _restartButton.Update(gameTime);
            _bord.Update(gameTime);
            _player.Update(gameTime);

            if (_bord.GameWin || _bord.GameEnd)
            {
                _restartButton.IsVisible = true;
                _restartButton.Position = new Vector2(500, 500);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            // Action went out
                _bord.ClearGame();
                _sfxBgInstance.Dispose();
                _sfxEndInstance.Dispose();
                _sfxWinInstance.Dispose();
                _game.ChangeScreen(ScreenSelect.Menu);
        }

        private void RestartButton_Click(object sender, EventArgs e) 
        {
            _bord.ClearGame();
            _sfxBgInstance.Dispose();
            _sfxEndInstance.Dispose();
            _sfxWinInstance.Dispose();
            _game.ChangeScreen(ScreenSelect.Game);
        }

        // Create method wnt use lot of time
        private float FindCenterText(string text)
        {
            return (float)(Singleton.Instance.ScreenWidth - _textFront.MeasureString(text).X) / 2;
        }
    }
}
