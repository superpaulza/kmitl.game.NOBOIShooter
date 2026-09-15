using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace NOBOIShooter.GameObjects
{
    class Player
    {
        // Create Variable
        private const int SHOOTER_WIDTH = 100;
        private const int SHOOTER_RADIAN = 50;
        private const int SHOOTER_MOVE_UP = 150;
        private int _aimerLength = 200;
        private int _aimerThick = 3;

        private float _shooterAngle, _shooterScale;

        private BallTexture _ballTexture;
        private BallGridManager _gameBord;
        private BallMoving _ballShoot;
        private BallDrop _ballEffect;

        private Texture2D _shooterTexture, _pencilDot;
        private Vector2 _position, _shooterCenterPosition, _shooterBallPosition,
                        _nextBallPosintion, _currentBallPosition, _aimerMovePosition;

        private int _currentBall = 0;
        private int _nextBall = 0;

        private Color _aimerColor;

        private bool _isShooting, _isSwapping;

        private MouseState MousePrevious, MouseCurrent;

        private Random _random = new Random();

        SoundEffectInstance _gunSound;

        public Player(BallGridManager ballGrid, Texture2D shooterTexture, BallTexture ballTexture, Texture2D pencil, ContentManager _content)
        {
            _shooterTexture = shooterTexture;
            _ballTexture = ballTexture;
            _gameBord = ballGrid;
            _pencilDot = pencil;

            _gunSound = _content.Load<SoundEffect>("BGM/GunSoundEffect").CreateInstance();

            _position = new Vector2((Singleton.Instance.ScreenWidth - SHOOTER_WIDTH) / 2f, Singleton.Instance.ScreenHeight - SHOOTER_MOVE_UP);
            _shooterCenterPosition = _position + new Vector2(SHOOTER_RADIAN, SHOOTER_RADIAN);
            _shooterBallPosition = _shooterCenterPosition + new Vector2(-20, -10);
            _nextBallPosintion = _shooterCenterPosition + new Vector2(-20, 50);

            _shooterScale = (float) SHOOTER_WIDTH / _shooterTexture.Width;

            _ballShoot = new BallMoving(_ballTexture, _gameBord);
            _currentBall = _gameBord.RandomBuble();
            _nextBall = _gameBord.RandomBuble();
            _isShooting = false;
            _isSwapping = false;

            _currentBallPosition = new Vector2(1.3f * _gameBord.TileWidth, _shooterTexture.Height / 2 + _gameBord.TileHeight);
            _aimerMovePosition = new Vector2(0, .2f* _aimerThick);
            _aimerColor = new Color(Color.Green, .2f);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Singleton.Instance.IsEnableAimer)
            {
                // Draw Aimer
                spriteBatch.Draw(_pencilDot, new Rectangle((int)_shooterCenterPosition.X, (int)_shooterCenterPosition.Y, _aimerLength, _aimerThick), null,
                    _aimerColor, _shooterAngle + MathHelper.ToRadians(-180f), _aimerMovePosition, SpriteEffects.None, 0);
            }

            // Draw Swaping effect
            if (_isSwapping && _ballEffect.Visible)
                _ballEffect.Draw(spriteBatch);

            // Draw Flying ball
            if (_ballShoot != null)
                _ballShoot.Draw(spriteBatch, gameTime);

            // Hide ball went swap
            if (!_isSwapping)
                spriteBatch.Draw(_ballTexture.GetTexture(_nextBall), _nextBallPosintion, null, 
                    _ballTexture.GetColor(_nextBall), 0f, Vector2.Zero, _ballTexture.GetScale(_nextBall), SpriteEffects.None, 0f);

            // Shooter Draw
            spriteBatch.Draw(_shooterTexture, _shooterCenterPosition, null,
                Color.White, _shooterAngle + MathHelper.ToRadians(-90f), new Vector2(_shooterTexture.Width / 2, _shooterTexture.Height / 2),
                _shooterScale, SpriteEffects.None, 0f);

            spriteBatch.Draw(_ballTexture.GetTexture(_currentBall), _shooterCenterPosition, null,
                _ballTexture.GetColor(_currentBall), _shooterAngle + MathHelper.ToRadians(-90f), _currentBallPosition, _ballTexture.GetScale(_currentBall), SpriteEffects.None, 0f);
        }

        public void Update(GameTime gameTime)
        {
            _gunSound.Volume = Singleton.Instance.SFXVolume;

            // Load pointer state (mouse on desktop, touch on mobile)
            if (_isSwapping)
            {
                _isSwapping = _ballEffect.Visible;
                _ballEffect.Update(gameTime);

            }
            Vector2 pointer = Controls.InputHelper.Position;

            // find angle of shooter — follows mouse cursor on desktop,
            // follows finger (drag to aim) on touch.
            _shooterAngle = (float)Math.Atan2((_position.Y + SHOOTER_RADIAN) - pointer.Y, (_position.X + SHOOTER_RADIAN) - pointer.X);

            // Tap/click release to shoot (drag-to-aim, release-to-fire on touch).
            // Ignores taps on the top-right UI strip (back button) and taps below the shooter.
            bool pointerReleased = Controls.InputHelper.IsReleased();
            bool inUiStrip = pointer.X > 1100 && pointer.Y < 100;
            if (!_gameBord.GamePause && !_isShooting && pointerReleased
                && pointer.Y < _position.Y + SHOOTER_RADIAN && !inUiStrip)
            {
                // Touch QoL: tapping the next-ball preview swaps instead of shooting
                // (mobile has no right-click).
                Rectangle nextBallRect = new Rectangle((int)_nextBallPosintion.X - 10, (int)_nextBallPosintion.Y - 10,
                    (int)_gameBord.TileWidth + 20, (int)_gameBord.TileHeight + 20);
                if (nextBallRect.Contains((int)pointer.X, (int)pointer.Y))
                {
                    DoSwap();
                }
                else
                {
                    try { _gunSound.Play(); } catch { }
                    _ballShoot.SetAnimation(_currentBall, _shooterCenterPosition - new Vector2(_gameBord.Radius, _gameBord.Radius), (float)(_shooterAngle + MathHelper.ToRadians(180f)));
                    _currentBall = _nextBall;
                    _nextBall = _gameBord.NextColorBubble();
                    _isShooting = true;
                }
            }
            if (_isShooting)
            {
                _ballShoot.Update(gameTime);
                _isShooting = _ballShoot.Visible;
            }

            //swap ball (desktop right-click; touch uses next-ball tap above)
            if (!_isShooting && Controls.InputHelper.IsRightClicked())
            {
                DoSwap();
            }

        }

        private void DoSwap()
        {
            // Guard textures during early frames
            if (_ballTexture.GetTexture(_currentBall) == null) return;
            int tempSwap = _currentBall;
            _ballEffect = new BallDrop(_gameBord, _ballTexture.GetTexture(_currentBall),
                _ballTexture.GetColor(_currentBall), _ballTexture.GetScale(_currentBall), _shooterBallPosition, _nextBallPosintion);
            _isSwapping = true;
            _currentBall = _nextBall;
            _nextBall = tempSwap;
        }

    }
}
