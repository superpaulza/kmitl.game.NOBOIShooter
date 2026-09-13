using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NOBOIShooter.GameObjects
{
    public class BallGridManager
    {
        // Game Lock Setting
        // Private const  
        private const int NUMBER_OF_COLUMNS = 15;
        private const int NUMBER_OF_ROWS = 16;
        private const int BALL_TILES_WIDTH = 40;
        private const int BALL_TILES_HEIGHT = 40;
        private const int TILES_ROWS_HEIGHT = 34;
        private const int BALL_RADIAN = 20;
        private const int START_FLOOR = 4;
        private const int TOTAL_BALL_COLOR = 4;
        private const int GRID_MOVE_DOWN = 20;
        private const int ANIMATION_DROP_HEIGHT = 50;
        private const int SCOLLING_SPEED = 5;
        private const int SCORE_DROP_BALL = 100;

        // Global variable
        public int TotalColor { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public float TileWidth { get; private set; }
        public float TileHeight { get; private set; }
        public float RowHeight { get; private set; }
        public float Radius { get; private set; }
        public bool FirstShift { get; private set; }
        public bool GamePause { get; private set; }
        public bool GameClear { get; private set; }
        public bool GameWin { get; private set; }
        public bool GameEnd { get; private set; }
        public int GameScore { get; private set; }
        public List<Point> RemoveCluster { get; set; }
        public List<List<Point>> FloatingCluster { get; set; }

        private List<BallFadeOut> AnimationFadeManager;
        private List<BallDrop> AnimationDropManager;

        private BallTexture _ballTexture;
        public Vector2 Position;
        public double lastTimeScoling = 0;

        public int[,] BallTiles;
        private bool[,] _removed;
        private bool[,] _processed;
        private int _removeClusterEffect = 0;

        private SoundEffectInstance _bubblePop;

        private Random _random = new Random();

        private int[,,] _neighborsOffsets = new int[,,] { { { 1, 0 }, {0, 1}, {-1, 1}, {-1, 0}, {-1, -1}, {0, -1}}, // Even row tiles
						                                    {{1, 0}, {1, 1}, {0, 1}, {-1, 0}, {0, -1}, {1, -1}}};  // Odd row tiles
        public BallGridManager(BallTexture texture, ContentManager _content)
        {
            _bubblePop = _content.Load<SoundEffect>("BGM/BubblePop").CreateInstance();

            _ballTexture = texture;

            Columns = NUMBER_OF_COLUMNS;
            Rows = NUMBER_OF_ROWS;
            
            TileWidth = BALL_TILES_WIDTH;
            TileHeight = BALL_TILES_HEIGHT;
            //_ballScale = TileWidth / _ballTexture.Width;
            
            RowHeight = TILES_ROWS_HEIGHT;
            Radius = BALL_RADIAN;

            TotalColor = TOTAL_BALL_COLOR;
            BallTiles = new int[Columns, Rows];
            _processed = new bool[Columns, Rows];
            _removed = new bool[Columns, Rows];
            Width = (int)(Columns * TileWidth + (float)TileWidth / 2);
            Height = (int)(RowHeight * (Rows));
            GameScore = 0;

            FirstShift = false;
            AnimationFadeManager = new List<BallFadeOut>();
            AnimationDropManager = new List<BallDrop>();

            Position = new Vector2((Singleton.Instance.ScreenWidth - Width) / 2f, GRID_MOVE_DOWN);
            for (int floor = 0; floor < START_FLOOR; floor++)
                Scrolling();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {   
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    int ball = BallTiles[col,row];
                    if (ball > 0) // Check it not null
                    {
                        Vector2 drawPosition = Position + new Vector2(col * TileWidth, row * RowHeight);
                        if ((FirstShift && row % 2 == 0) || (!FirstShift && row % 2 != 0))
                            drawPosition.X += (float) TileWidth / 2;
                        spriteBatch.Draw(_ballTexture.GetTexture(ball), drawPosition, null, _ballTexture.GetColor(ball), 0f, Vector2.Zero, _ballTexture.GetScale(ball), SpriteEffects.None, 0f);

                    }
                }
            }

            foreach (var fadeBall in AnimationFadeManager)
                if(fadeBall.Visible) fadeBall.Draw(spriteBatch); 

            foreach (var fadeDrop in AnimationDropManager)
                if(fadeDrop.Visible) fadeDrop.Draw(spriteBatch); 
        }

        public void Update(GameTime gameTime)
        {
            lastTimeScoling += gameTime.ElapsedGameTime.TotalSeconds;
            if (lastTimeScoling > SCOLLING_SPEED)
            {
                lastTimeScoling = 0;
                Scrolling();
            }

            GamePause = (GameWin || GameEnd);

            for (int i = 0; i < AnimationFadeManager.Count; i++)
                if (!AnimationFadeManager[i].Visible) AnimationFadeManager.RemoveAt(i);
                else AnimationFadeManager[i].Update(gameTime);

            for (int i = 0; i < AnimationDropManager.Count; i++)
                if (!AnimationDropManager[i].Visible) AnimationDropManager.RemoveAt(i);
                else AnimationDropManager[i].Update(gameTime);

        }

        public Color GetColor(int i)
        {
            // Int value to color of ball
            if (i == 1) return Color.FromNonPremultiplied(252, 132, 29, 255); // orange; // Color.FromNonPremultiplied(225, 78, 175, 255); //pink 
            else if (i == 2) return Color.FromNonPremultiplied(124, 90, 188, 255); // purple
            else if (i == 3) return Color.FromNonPremultiplied(249, 219, 99, 255); // yellow // Color.FromNonPremultiplied(50, 85, 166, 255); // blue
            else if (i == 4) return Color.FromNonPremultiplied(76, 176, 99, 255); // greenn
            else if (i == 5) return Color.FromNonPremultiplied(210, 71, 51, 255); // red
            // else if (i == 6) return Color.FromNonPremultiplied(252, 132, 29, 255); // orange;
            // else if (i == 5) return Color.FromNonPremultiplied(249, 219, 99, 255); // yellow
            return new Color(Color.White,.1f);
        }

        // Random method
        public int RandomBuble()
        {
            return _random.Next(1, TOTAL_BALL_COLOR + 1);
        }

        public Point GetGridPosition(float x, float y)
        {
            int gridy = (int)Math.Round((y - Position.Y) / RowHeight);
            float xoffset = Position.X;
            if ((gridy) % 2 == 0 && FirstShift || (gridy % 2 != 0 && !FirstShift))
            {
                xoffset += TileWidth / 2;
            }
            int gridx = (int)Math.Round((x - xoffset) / TileWidth);

            if (gridx < 0) gridx = 0;
            if (gridx >= Columns) gridx = Columns - 1;
            if (gridy < 0) gridy = 0;
            if (gridy >= Rows) gridy = Rows - 1;

            return new Point(gridx, gridy);
        }

        public Vector2 GetTileCoordinate(int column, int row)
        {
            var tilex = Position.X + column * TileWidth;
            if ((row % 2 == 0 && FirstShift) || (row % 2 != 0 && !FirstShift))
            {
                tilex += TileWidth / 2;
            }
            var tiley = Position.Y + row * RowHeight;
            return new Vector2(tilex, tiley);
        }

        public int NextColorBubble()
        {
            var existingcolors = DetectedTileType();
            var randomColor = 0;
            if (existingcolors.Count > 0)
            {
                randomColor = existingcolors[RamdomRange(0, existingcolors.Count)];
            }
            return randomColor;
        }

        private int RamdomRange(int min, int max)
        {
            return _random.Next(min, max);
        }

        public List<int> DetectedTileType()
        {

            List<int> found = new List<int>();
            List<bool> detect = new List<bool>();
            for (var tile = 0; tile <= TotalColor; tile++)
                detect.Add(false);

            for (var col = 0; col < Columns; col++)
            {
                for (var row = 0; row < Rows; row++)
                {
                    var tile = BallTiles[col,row];
                    if (tile > 0)
                    {
                        if (!detect[tile])
                        {
                            detect[tile] = true;
                            found.Add(tile);
                        }
                    }
                }
            }
            return found;
        }

        public List<List<Point>> FindBubbleCluster()
        {

            ResetProcessed();

            List<List<Point>> bubbleCluster = new List<List<Point>>();

            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    if (!_processed[col, row])
                    {
                        List<Point> cluster = FindCluster(col, row, false, false, true);
                        if (cluster.Count < 1)
                        {
                            continue;
                        }

                        bool isbubble = true;

                        for (int c = 0; c < cluster.Count; c++)
                        {
                            if (cluster[c].Y < 1)
                            {
                                isbubble = false;
                                break;
                            }
                        }

                        if (isbubble)
                        {
                            bubbleCluster.Add(cluster);
                        }
                    }
                }
            }
            return bubbleCluster;
        }

        public List<Point> FindCluster(int tx, int ty, bool matchtype, bool reset, bool skipremoved)
        {
            // Reset check process
            if (reset)
                ResetProcessed();

            int targettile = BallTiles[tx, ty];
            Stack<Point> topProcess = new Stack<Point>();

            // Store the root ball to process
            topProcess.Push(new Point(tx, ty));
            _processed[tx, ty] = true;

            List<Point> foundcluster = new List<Point>();
            
            //Loop for check every connected ball 
            while (topProcess.Count > 0)
            {
                Point currentTile = topProcess.Pop();
                
                // Skip null ball 
                if (BallTiles[currentTile.X, currentTile.Y] < 1)
                    continue;

                // Skip removed ball
                if (skipremoved && _removed[currentTile.X, currentTile.Y])
                    continue;

                // Check the same type
                if (!matchtype || (BallTiles[currentTile.X, currentTile.Y] == targettile))
                {
                    // Add target same ball
                    foundcluster.Add(currentTile);
                    List<Point> neighbors = GetNeighbors(currentTile);

                    // Check every next connect ball
                    for (int n = 0; n < neighbors.Count; n++)
                    {
                        // Check is already process 
                        if (!_processed[neighbors[n].X, neighbors[n].Y])
                        {
                            // Add next leaf to check
                            topProcess.Push(neighbors[n]);
                            // Pin the processed point
                            _processed[neighbors[n].X, neighbors[n].Y] = true;
                        }
                    }
                }
            }
            return foundcluster;
        }

        private void ResetProcessed()
        {
            // Set false to all in processed data
            for (int col = 0; col < Columns; col++)
                for (int row = 0; row < Rows; row++)
                    _processed[col, row] = false;
        }

        // Get the neighbors of the specified tile
        private List<Point> GetNeighbors(Point finder)
        {
            int tilerow = ((FirstShift && finder.Y % 2 == 0) || (!FirstShift && finder.Y % 2 != 0)) ? 1 : 0; // Row (Odd, Even)

            List<Point> neighbors = new List<Point>();
            Point tile = new Point();

            // Get the neighbors
            for (int i = 0; i < _neighborsOffsets.GetLength(1); i++)
            {
                // Neighbor coordinate
                tile.X = finder.X + _neighborsOffsets[tilerow, i, 0];
                tile.Y = finder.Y + _neighborsOffsets[tilerow, i, 1];

                // Make sure the tile is valid
                if (tile.X >= 0 && tile.X < Columns && tile.Y >= 0 && tile.Y < Rows)
                {
                    neighbors.Add(tile);
                }
            }

            return neighbors;
        }

        private void ResetRemoved()
        {
            // Set false to all in removed data
            for (int col = 0; col < Columns; col++)
                for (int row = 0; row < Rows; row++)
                    _removed[col, row] = false;
        }

        private void Scrolling()
        {
            if (GamePause) return;
            // Move every ball down 
            for (int col = 0; col < Columns; col++)
                for (int row = 0; row < Rows - 1; row++)
                    BallTiles[col, Rows - 1 - row] = BallTiles[col, Rows - 1 - row - 1];
            
            // Random ball in new line
            for (int col = 0; col < Columns; col++)
                BallTiles[col, 0] = RandomBuble();

            // Swap shift floor
            FirstShift = !FirstShift;

            GameoverLineCheck();
        }

        public void StateRemovecluster()
        {
            // Remove phace 1 count score
            if (_removeClusterEffect == 0)
            {
                _bubblePop.Play();
                ResetRemoved();
                // Mark to remove
                for (int i = 0; i < RemoveCluster.Count; i++)
                    _removed[RemoveCluster[i].X, RemoveCluster[i].Y] = true;

                // Add score 
                GameScore += RemoveCluster.Count * 100;

                // Find float cluster
                FloatingCluster = FindBubbleCluster();

                if (FloatingCluster.Count > 0)
                {
                    for (int group = 0; group < FloatingCluster.Count; group++)
                    {
                        for (int ball = 0; ball < FloatingCluster[group].Count; ball++)
                        {
                            // Counting Point
                            GameScore += SCORE_DROP_BALL;
                        }
                    }
                }
                // Go to next phace
                _removeClusterEffect = 1;
            }

            if (_removeClusterEffect == 1)
            {
                // Remove the same of hitting ball
                for (int ball = 0; ball < RemoveCluster.Count; ball++)
                {
                    Point ar = RemoveCluster[ball];
                    if (BallTiles[ar.X, ar.Y] > 0)
                    {
                        // Set to null in grid
                        AnimationFadeManager.Add(new BallFadeOut(_ballTexture.GetTexture(BallTiles[ar.X, ar.Y]), _ballTexture.GetColor(BallTiles[ar.X, ar.Y]), this, true, BallTiles[ar.X, ar.Y], GetTileCoordinate(ar.X, ar.Y)));
                        BallTiles[ar.X, ar.Y] = 0;
                    }
                }

                // Remove every floating ball
                for (int x = 0; x < FloatingCluster.Count; x++)
                {
                    for (int y = 0; y < FloatingCluster[x].Count; y++)
                    {
                        Point ball = FloatingCluster[x][y];
                        if (BallTiles[ball.X, ball.Y] >= 0)
                        {
                            // Set to null in grid
                            Vector2 positionDrop = GetTileCoordinate(ball.X, ball.Y);
                            AnimationDropManager.Add(new BallDrop(this,_ballTexture.GetTexture(BallTiles[ball.X, ball.Y]), _ballTexture.GetColor(BallTiles[ball.X, ball.Y]), _ballTexture.GetScale(BallTiles[ball.X, ball.Y]), positionDrop, positionDrop + new Vector2(0, ANIMATION_DROP_HEIGHT)));
                            BallTiles[ball.X, ball.Y] = 0;
                        }
                    }
                }
                _removeClusterEffect = 0;
                GameWinCheck();
            }
        }

        public bool GameWinCheck()
        {
            if (GameWin) return true;

            // Check the First Line
            for (int x = 0; x < Columns; x++)
            {
                if (BallTiles[x, 0] > 0)
                {
                    return false;
                }
            }

            GameScore += (int) Math.Pow(10, TotalColor);
            GameWin = true;
            return true;
        }

        public bool GameoverLineCheck()
        {
            if (GameEnd) return true;

            // Check the deadline
            for (int x = 0; x < Columns; x++)
            {
                if (BallTiles[x, Rows - 1] > 0)
                {
                    //pickBubble();
                    GameEnd = true;
                    //setgamestate(state.gameover);
                    return true;
                }
            }
            return false;
        }

        public void ClearGame ()
        {
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                    BallTiles[col, row] = 0;
            GameWin = false;
            GamePause = false;
            GameEnd = false;
            FirstShift = false;
            GameScore = 0;

            for (int floor = 0; floor < START_FLOOR; floor++)
                Scrolling();

        }
    }
}
