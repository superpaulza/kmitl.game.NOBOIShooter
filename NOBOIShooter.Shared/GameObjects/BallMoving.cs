using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NOBOIShooter.GameObjects
{
    class BallMoving 
    {
        public float Speed { get; set; }
        public float Angle { get; set; }
        public float DropSpeed { get; set; }
        public int TileType { get; set; }
        public bool Visible { get; set; }

        private BallGridManager _bord;
        private BallTexture _ballTexture;

        private Vector2 Position, _posible, _velocity;

        private Point _gridPosible;

        public BallMoving(BallTexture texture, BallGridManager bord)
        {
            _bord = bord;
            _ballTexture = texture;
            TileType = 0;
            Speed = 1000;
            Visible = false;
        }

        public void SetAnimation(int type, Vector2 position, float nangle)
        {
            Angle = nangle;
            Position = position;
            TileType = type;
            Visible = true;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Visible)
            {
                //spriteBatch.Draw(_texture, Posible, null, Color.Orange, 0f, Vector2.Zero, scaleball, SpriteEffects.None, 0f);
                spriteBatch.Draw(_ballTexture.GetTexture(TileType), Position, null, _ballTexture.GetColor(TileType), 0f, Vector2.Zero, _ballTexture.GetScale(TileType), SpriteEffects.None, 0f);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Visible)
            {
                Calculate(gameTime);
            }
        }

        private void Calculate(GameTime gameTime)
        {
            _velocity.X = (float)Math.Cos(Angle) * Speed;
            _velocity.Y = (float)Math.Sin(Angle) * Speed;

            Position += _velocity * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

            _gridPosible = _bord.GetGridPosition(Position.X, Position.Y);

            if (_bord.BallTiles[_gridPosible.X, _gridPosible.Y] < 1)
                _posible = _bord.GetTileCoordinate(_gridPosible.X, _gridPosible.Y);

            if (Position.X <= _bord.Position.X)
            {
                Angle = -Angle;
                Angle += MathHelper.ToRadians(180);
            }

            if (Position.X + _bord.TileWidth >= _bord.Position.X + _bord.Width)
            {
                Angle = -Angle;
                Angle += MathHelper.ToRadians(180);
            }

            if (Position.Y <= _bord.Position.Y)
            {
                Position.Y = _bord.Position.Y;
                //y = _bord.y;
                SnapBubble((int)Position.X, (int)Position.Y);
                return;
            }

            for (var i = 0; i < _bord.Columns; i++)
            {
                for (var j = 0; j < _bord.Rows; j++)
                {
                    var tile = _bord.BallTiles[i, j];
                    if (tile < 1)
                    {
                        continue;
                    }

                    Vector2 coord = _bord.GetTileCoordinate(i, j);
                    int posX = (int)(coord.X), posY = (int)(coord.Y);

                    if (CircleInterSection(Position.X, Position.Y, _bord.Radius, posX, posY, _bord.Radius))
                    {
                        SnapBubble((int)Position.X, (int)Position.Y);
                        return;
                    }
                }
            }
        }

        public void SnapBubble(int atx, int aty)
        {
            Point gridpos = _bord.GetGridPosition(atx, aty);
            int posX = gridpos.X;
            int posY = gridpos.Y;

            if (posX < 0) posX = 0;
            if (posY < 0) posY = 0;
            if (posX >= _bord.Columns) posX = _bord.Columns - 1;
            if (posY >= _bord.Rows) posY = _bord.Rows - 1;

            bool isSnap = false;
            Point replace = new Point(0, 0);

            if (_bord.BallTiles[posX, posY] > 1)
            {
                int newx = _gridPosible.X, newy = _gridPosible.Y;

                if (_bord.BallTiles[newx, newy] < 1)
                {
                    _bord.BallTiles[newx, newy] = TileType;
                    replace.X = newx;
                    replace.Y = newy;
                    // Debug.WriteLine("Grid found");
                    isSnap = true;
                }

                Point get = _bord.GetGridPosition(_posible.X, _posible.Y);

                if (_bord.BallTiles[get.X, get.Y] < 1)
                {
                    _bord.BallTiles[get.X, get.Y] = TileType;
                    replace = get;
                    //  Debug.WriteLine("Position found");
                    isSnap = true;
                }
            }
            else
            {
                _bord.BallTiles[posX, posY] = TileType;
                replace.X = posX;
                replace.Y = posY;
                isSnap = true;
            }

            if (isSnap)
            {
                Visible = false;
                if (_bord.GameoverLineCheck())
                {
                    //Debug.WriteLine("Game end");
                    return;
                }
               
                List<Point> RemoveCluster = _bord.FindCluster(replace.X, replace.Y, true, true, false);
                //Debug.WriteLine("Touch : " + replace.X + " + " + replace.Y + " & Friend : " + RemoveCluster.Count);

                if (RemoveCluster.Count >= 3)
                {
                    //Debug.WriteLine("Take : " + RemoveCluster.Count);
                    _bord.RemoveCluster = RemoveCluster;
                    _bord.StateRemovecluster();
                    _bord.GameWinCheck();
                    return;
                }
            }
        }

        public bool CircleInterSection(float x1, float y1, float r1, float x2, float y2, float r2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;
            var len = Math.Sqrt(dx * dx + dy * dy);
            if (len < r1 + r2)
            {
                return true;
            }
            return false;
        }
    }
}
