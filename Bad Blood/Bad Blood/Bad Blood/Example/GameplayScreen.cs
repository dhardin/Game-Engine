//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Engine.GameStateManagerment;
//using Microsoft.Xna.Framework;
//using Engine.TaskManagement;
//using Engine.Sprites;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Graphics;
//using System.IO;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Media;
//using Engine.Primitives;

//namespace Game
//{
//    class GameplayScreen : GameScreen
//    {
        
//        GameLevel _level = new GameLevel();
//        List<string> levels = new List<string>();

//        const int MAX_PLAYERS = 2;
//        Player[] players = new Player[MAX_PLAYERS];
//        PlayerIndex[] playerIndex = new PlayerIndex[MAX_PLAYERS];
//        bool playersControl = true;

//        List<Enemy> enemies = new List<Enemy>();

//        Random rand = new Random();

//        private string currentScoreP1;
//        private string currentScoreP2;
//        private string previousScoreP1;
//        private string previousScoreP2;

//        SpriteFont scoreFont;
//        AnchoredText levelWin, levelLose, scoreDisplay, scoreDisplay2;
//        private int MAX_SCORE_DIGITS;

//        private const double MAX_TRANSITION_TIME = 5;
//        TimeSpan levelTransitionTimer = TimeSpan.FromSeconds(MAX_TRANSITION_TIME);

//        #region Textures
//        Texture2D _lives;
//        SpriteFont _font;
//        Texture2D _winOverlay;
//        Texture2D _loseOverlay;
//        Texture2D _timeOutOverlay;
//        Texture2D _pausedOverlay;
//        Texture2D _optionsOverlay;
//        Texture2D _playerHealth;
//        Texture2D _powerUpSheild;
//        Texture2D _powerUpNuke;
//        Texture2D status;

//        #endregion

//        Settings _settings;
//        private bool optionsOverlay;
//        private int pauseSelect;
//        private int optionsSelect;
//        private string sound, music, fullScreen;
//        private bool _fullscreen;
//        private bool win = false;
//        private bool lose = false;

//        PrimitiveBatch primitiveBatch;// = new PrimitiveBatch(ScreenManager.GraphicsDevice;);
//        Color tempColor = Color.LawnGreen;
//        Color tempColor2 = Color.AliceBlue;
//        private string _levelFile;

//        ParallaxingBackground parallaxBackground1;
//        ParallaxingBackground parallaxBackground2;

//        public GameplayScreen(string levelFile)
//        {
//            highScores = new HighScore();
//            List<string> scores = new List<string>();

//            levels.Add("Temperate");
//            levels.Add("Space");
//            levels.Add("Tropical");
//            levels.Add("Sky");
//            levels.Add("Volcanic");
//            levels.Add("Arctic");

//            if (!highScores.HighScores.Count().Equals(0))
//            {
                

//                for (int i = 0; i < highScores.HighScores.Count() && i < 3; i++)
//                {
//                    scores.Add(highScores.HighScores[i]);
//                }
//            }

//            string prevScore1, prevScore2;
//            if (!scores[0].Equals(""))
//            {
//                 prevScore1 = scores[0];
//                 prevScore2 = scores[1];
//            }
//            else
//            {
//                 prevScore1 = scores[1];
//                 prevScore2 = scores[2];
//            }
//            _levelFile = levelFile;
//            if (prevScore1.Contains("Player 1: "))
//            {
//                previousScoreP1 = prevScore1.Substring(10);
//                previousScoreP2 = prevScore2.Substring(10); ;
//            }
//            else
//            {
//                previousScoreP1 = prevScore2.Substring(10);
//                previousScoreP2 = prevScore1.Substring(10);
//            }

//            MAX_SCORE_DIGITS = GetNumDigits(int.MaxValue);
//        }

//        public override void Activate(bool instancePreserved)
//        {
//            /// <summary>
//            /// Load player
//            /// </summary>
//            for (int i = 0; i < MAX_PLAYERS; i++)
//            {
//                Vector2 pos;
//                pos.X = rand.Next(25, ScreenManager.GraphicsDevice.Viewport.Width - 25);
//                pos.Y = ScreenManager.GraphicsDevice.Viewport.Height;
                
//                switch (i)
//                {
//                    case 0:
//                        players[i] = new Player(PlayerIndex.One, pos);
//                        break;
//                    case 1:
//                        players[i] = new Player(PlayerIndex.Two, pos);
//                        break;
//                    case 2:
//                        players[i] = new Player(PlayerIndex.Three, pos);
//                        break;
//                    case 3:
//                        players[i] = new Player(PlayerIndex.Four, pos);
//                        break;
//                    default:
//                        throw new ArgumentException("Not a valid number for player index");
//                }

//                players[i]._game = ScreenManager.Game;
//                players[i].LoadContent(ScreenManager.Game.Content);
//            }

//            _level.players = players;
            
//            /// <symmary>
//            /// Load level
//            /// </summary>
//            _level._game = ScreenManager.Game;
//            _level.LoadContent(ScreenManager.Game.Content, _levelFile);
//            enemies = _level.enemies;


//            ///<summary>
//            /// Parallax Layers
//            ///</summary>
//            parallaxBackground1 = new ParallaxingBackground();
//            parallaxBackground2 = new ParallaxingBackground();

//            parallaxBackground1.Initialize(ScreenManager.Game.Content, @"Levels\Parallax\Parallax1", 2048, 4);
//            parallaxBackground2.Initialize(ScreenManager.Game.Content, @"Levels\Parallax\Parallax2", 2048, 3);
            

//            /// <summary>
//            /// Textures
//            /// </summary>
//            _font = ScreenManager.Game.Content.Load<SpriteFont>(@"HUD\Font");
//            _winOverlay = ScreenManager.Game.Content.Load<Texture2D>(@"HUD\Win");
//            _loseOverlay = ScreenManager.Game.Content.Load<Texture2D>(@"HUD\Lose");
//            _timeOutOverlay = ScreenManager.Game.Content.Load<Texture2D>(@"HUD\TimeOut");
//            _pausedOverlay = ScreenManager.Game.Content.Load<Texture2D>(@"HUD\PauseMenu");
//            _optionsOverlay = ScreenManager.Game.Content.Load<Texture2D>(@"HUD\OptionsMenu");

//            _playerHealth = ScreenManager.Game.Content.Load<Texture2D>(@"Player\Health\Health_Bar");

//            _powerUpNuke = ScreenManager.Game.Content.Load<Texture2D>(@"PowerUps\nuke");
//            _powerUpSheild = ScreenManager.Game.Content.Load<Texture2D>(@"PowerUps\shield");

//            pauseSelect = 1;
//            optionsSelect = 1;

//            _settings = new Settings();
//            _settings.Load();
//            _fullscreen = _settings.FullScreen;
//            music = _settings.Music ? "x" : " ";
//            sound = _settings.Sound ? "x" : " ";
//            fullScreen = _settings.FullScreen ? "x" : " ";

//            scoreFont = ScreenManager.Game.Content.Load<SpriteFont>("SpriteFont1");

//            currentScoreP1 = previousScoreP1;
//            currentScoreP2 = previousScoreP2;
//            int tempScore = players[0].Score;
//            int tempScore2 = players[1].Score;
//            Int32.TryParse(previousScoreP1, out tempScore);
//            Int32.TryParse(previousScoreP2, out tempScore2);
//            players[0].Score = tempScore;
//            players[1].Score = tempScore2;
//            //currentScore = previousScore;
//            currentScoreP1 = currentScoreP1.PadLeft(MAX_SCORE_DIGITS - currentScoreP1.Length, '0');
//            currentScoreP2 = currentScoreP2.PadLeft(MAX_SCORE_DIGITS - currentScoreP2.Length, '0');
//            Vector2 position1 = new Vector2(ScreenManager.Instance.GraphicsDevice.Viewport.Width * 0.5f, ScreenManager.Instance.GraphicsDevice.Viewport.Height * 0.5f);
//            Vector2 topRightPosition = new Vector2(ScreenManager.Instance.GraphicsDevice.Viewport.Width, 0);
//            Vector2 position2 = new Vector2(ScreenManager.Instance.GraphicsDevice.Viewport.Width * 0.5f, ScreenManager.Instance.GraphicsDevice.Viewport.Height * 0.5f);
//            Vector2 topLeftPosition = new Vector2(0, 0);
          
//            scoreDisplay2 = new AnchoredText(scoreFont, "Score: " + currentScoreP2, topRightPosition, TextAnchor.TopRight);
//            scoreDisplay = new AnchoredText(scoreFont, "Score: " + currentScoreP1, topLeftPosition, TextAnchor.TopLeft);

//            primitiveBatch = new PrimitiveBatch(ScreenManager.GraphicsDevice);

//            base.Activate(instancePreserved);
//        }
//        public string GetNextLevel()
//        {
//            for (int i = 0; i < levels.Count - 1; i++)
//            {
//                if (levels[i].Equals(_levelFile))
//                    return levels[i + 1];//return the next level
//            }

//            return "";
//        }
//        public override void HandleInput(GameTime gameTime, InputState input)
//        {
//            GamePadState gamePadState;
//            for (int i = 0; i < MAX_PLAYERS; i++)
//            {
//                gamePadState = GamePad.GetState(players[i].playerIndex);
//                Keys[] pressedKeys = input.CurrentKeyboardStates[0].GetPressedKeys();

//                InputAction menuUp = new InputAction(
//                    new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
//                    new Keys[] { Keys.Up },
//                    true);
//                InputAction menuDown = new InputAction(
//                    new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
//                    new Keys[] { Keys.Down },
//                    true);
//                InputAction Continue = new InputAction(
//                    new Buttons[] { Buttons.A },
//                    new Keys[] { Keys.Space, Keys.Enter },
//                    true);
//                InputAction pause = new InputAction(
//                    new Buttons[] { Buttons.Start },
//                    new Keys[] { Keys.P, Keys.Escape },
//                    true);

//                PlayerIndex temp;

//                /// <summary>
//                /// Pause
//                /// </summary>
//                if (pause.Evaluate(input, players[i].playerIndex, out temp))
//                {
//                    _level.Paused = _level.Paused ? false : true;
//                    if (_level.Paused == false)
//                    {
//                        optionsOverlay = false;
//                        _settings = new Settings();
//                        _settings.Load();
//                        //_level._soundEffects = _settings.Sound;
//                        //_level._music = _settings.Music;
//                    }
//                    else
//                        pauseSelect = 1;
//                }

//                if (_level.Paused)
//                {
//                    if (menuDown.Evaluate(input, players[i].playerIndex, out temp))
//                    {
//                        optionsSelect = (int)MathHelper.Clamp(++optionsSelect, 1, 4);
//                        pauseSelect = (int)MathHelper.Clamp(++pauseSelect, 1, 4);
//                    }
//                    else if (menuUp.Evaluate(input, players[i].playerIndex, out temp))
//                    {
//                        optionsSelect = (int)MathHelper.Clamp(--optionsSelect, 1, 4);
//                        pauseSelect = (int)MathHelper.Clamp(--pauseSelect, 1, 4);
//                    }
//                    else if (Continue.Evaluate(input, players[i].playerIndex, out temp))
//                    {
//                        if (optionsOverlay == false)
//                        {
//                            switch (pauseSelect)
//                            {
//                                case 1:
//                                    _settings = new Settings();
//                                    _level.Paused = false;
//                                    _settings.Load();
//                                    //_level._soundEffects = _settings.Sound;
//                                    //_level._music = _settings.Music;
//                                    break;
//                                case 2:
//                                    optionsOverlay = true;
//                                    optionsSelect = 1;
//                                    _settings = new Settings();
//                                    _settings.Load();
//                                    _fullscreen = _settings.FullScreen;
//                                    break;
//                                case 3:
//                                    ScreenManager.RemoveScreen(this);
//                                    MediaPlayer.Stop();
//                                    break;
//                                case 4:
//                                    ScreenManager.Game.Exit();
//                                    break;
//                            }
//                        }
//                        else
//                        {
//                            switch (optionsSelect)
//                            {
//                                case 1:
//                                    _settings.Sound = _settings.Sound ? false : true;
//                                    sound = _settings.Sound == true ? "x" : " ";
//                                    break;
//                                case 2:
//                                    _settings.Music = _settings.Music ? false : true;
//                                    music = _settings.Music == true ? "x" : " ";
//                                    break;
//                                case 3:
//                                    _settings.FullScreen = _settings.FullScreen ? false : true;
//                                    fullScreen = _settings.FullScreen == true ? "x" : " ";
//                                    break;
//                                case 4:
//                                    _settings.Save();
//                                    if (_settings.FullScreen != _fullscreen)
//                                    {
//                                        ScreenManager.AddScreen(new OptionsConfirmation(), null);
//                                    }
//                                    optionsOverlay = false;
//                                    break;
//                            }
//                        }
//                    }
//                }
//                else if (playersControl)
//                {
//                    players[i].HandleInput(gameTime, input);
//                }
//            }

//            base.HandleInput(gameTime, input);
//        }

//        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
//        {
//            if (!_level.Paused)
//            {
//                Vector2 closestPlayer;
//                for (int i = 0; i < MAX_PLAYERS; i++)
//                    players[i].Update(gameTime);

//                currentScoreP1 = players[0].Score + "";
//                currentScoreP2 = players[1].Score + "";
//                currentScoreP1 = currentScoreP1.PadLeft(MAX_SCORE_DIGITS - currentScoreP1.Length, '0');
//                currentScoreP2 = currentScoreP2.PadLeft(MAX_SCORE_DIGITS - currentScoreP2.Length, '0');
//                scoreDisplay.Text = "Score: " + currentScoreP1;
//                scoreDisplay2.Text = "Score: " + currentScoreP2;

//                status = null;

//                /// <summary>
//                /// Overlays
//                /// </summary>
//                if (players[0].isDead && players[1].isDead)
//                {
//                    status = _loseOverlay;
//                    lose = true;

//                    highScores.Erase();
//                    highScores.AddScore(players[0].Score + "", players[0]);
//                    highScores.AddScore(players[1].Score + "", players[1]);

//                }
//                else if (_level.ExitReached)
//                {
//                    status = _winOverlay;
//                    win = true;

//                    highScores.Erase();
//                    highScores.AddScore(players[0].Score + "", players[0]);
//                    highScores.AddScore(players[1].Score + "", players[1]);

//                }
//                if (optionsOverlay)
//                    status = _optionsOverlay;
//                else if (_level.Paused)
//                    status = _pausedOverlay;

//                //SCALING DIFF
//                _level.score = (players[0].Score + players[1].Score) / 2;
//                _level.Update(gameTime);
//                for (int i = 0; i < enemies.Count; i++)
//                {
//                    closestPlayer = players[0].Position;
//                    for (int j = 1; j < MAX_PLAYERS; j++)
//                    {
//                        if (!players[0].isDead && !players[1].isDead)
//                        {

//                            if (!enemies[i].ClosestTarget(closestPlayer, players[j].Position).Equals(closestPlayer))
//                            {
//                                closestPlayer = players[j].Position;
//                            }

//                        }
//                        else if (players[0].isDead && !players[1].isDead)
//                        {
//                            closestPlayer = players[1].Position;
//                        }
//                        else
//                        {
//                            closestPlayer = players[0].Position;
//                        }

//                    }
//                    enemies[i].Update(gameTime, closestPlayer);
//                }


//                parallaxBackground1.Update();
//                parallaxBackground2.Update();
//            }
//            if (win || lose)
//            {
//                levelTransitionTimer -= gameTime.ElapsedGameTime;

//                if (levelTransitionTimer.TotalSeconds <= 0)
//                {
//                    if (players[0].Position.Y < -100 && players[0].Position.Y < -100)
//                    {
//                        if (win && !GetNextLevel().Equals(""))
//                        {
//                            ScreenManager.AddScreen(new GameplayScreen(GetNextLevel()), ControllingPlayer);
//                            ExitScreen();
//                        }
//                        else
//                        {
//                            ScreenManager.RemoveScreen(this);
//                            MediaPlayer.Stop();
//                        }
//                    }
//                    else
//                    {
//                        playersControl = false;
//                        Vector2 newPos = new Vector2(players[0].Position.X, players[0].Position.Y - players[0].Speed);
//                        players[0].Position = newPos;
//                        newPos = new Vector2(players[1].Position.X, players[1].Position.Y - players[1].Speed);
//                        players[1].Position = newPos;
//                        status = null;
//                    }
//                }
//            }
//            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
//        }

//        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
//        {
//            DrawContext ctx = new DrawContext();
//            ctx.Batch = ScreenManager.SpriteBatch;
//            ctx.Blank = ScreenManager.BlankTexture;
//            ctx.Device = ScreenManager.GraphicsDevice;
//            ctx.Time = gameTime;

//            ctx.Device.Clear(Color.DeepSkyBlue);

//            Rectangle titleSafeArea = ctx.Device.Viewport.TitleSafeArea;
//            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f, titleSafeArea.Y + titleSafeArea.Height / 2.0f);
           

//            //Color timeColor = (_level.TimeRemaining > TimeSpan.FromSeconds(20) || (int)_level.TimeRemaining.Seconds % 2 == 0) ?
//            //                        Color.Yellow : Color.Red;

//            /// <summary>
//            /// HUD
//            /// </summary>
//            ctx.Batch.Begin();

//                _level.Draw(ctx);
//                parallaxBackground1.Draw(ctx.Batch);
//                parallaxBackground2.Draw(ctx.Batch);
//                scoreDisplay.Draw(ctx);
//                scoreDisplay2.Draw(ctx);
               

//                if (status != null)
//                {
//                    Vector2 statusSize = new Vector2(status.Width, status.Height);
//                    ctx.Batch.Draw(status, center - statusSize / 2, Color.White);
//                    if (status == _timeOutOverlay)
//                    {
//                        ctx.Batch.DrawString(_font, "Time Out", new Vector2(center.X + status.Width / 2 - 30, center.Y - status.Height / 2 - 20),
//                            Color.Red, 0.5f, new Vector2(40, 5), 1.0f, SpriteEffects.None, 0.0f);
//                    }
//                    else if (status == _pausedOverlay)
//                    {
//                        ctx.Batch.DrawString(_font, ">           <", new Vector2(center.X - 85, center.Y - 40 + 40 * pauseSelect), Color.Yellow);
//                    }
//                    else if (status == _optionsOverlay)
//                    {
//                        ctx.Batch.DrawString(_font, sound, new Vector2(center.X + 87, center.Y - 40 + 40 * 1), Color.LimeGreen);
//                        ctx.Batch.DrawString(_font, music, new Vector2(center.X + 88, center.Y - 40 + 40 * 2), Color.Blue);
//                        ctx.Batch.DrawString(_font, fullScreen, new Vector2(center.X + 89, center.Y - 40 + 40 * 3), Color.Magenta);
//                        switch (optionsSelect)
//                        {
//                            case 1:
//                                ctx.Batch.DrawString(_font, ">                  <", new Vector2(center.X - 125, center.Y - 40 + 40 * 1), Color.Yellow);
//                                break;
//                            case 2:
//                                ctx.Batch.DrawString(_font, ">                     <", new Vector2(center.X - 165, center.Y - 40 + 40 * 2), Color.Yellow);
//                                break;
//                            case 3:
//                                ctx.Batch.DrawString(_font, ">               <", new Vector2(center.X - 85, center.Y - 40 + 40 * 3), Color.Yellow);
//                                break;
//                            case 4:
//                                ctx.Batch.DrawString(_font, ">     <", new Vector2(center.X - 35, center.Y - 40 + 40 * 4), Color.Yellow);
//                                break;

//                        }
//                    }
//                }

//            //draw each player's health bar.  Currently only works w/ 2 players
           
//            //player 1 health gui
//                for (int i = 0; i < players[0].Health; i++)
//                {
//                    Vector2 startPos = new Vector2((float)titleSafeArea.Left + _playerHealth.Width, (float)titleSafeArea.Top + _playerHealth.Height);
                    
//                    ctx.Batch.Draw(_playerHealth, startPos + new Vector2(i * _playerHealth.Width * 0.5f + 20, 0.0f), Color.Gold);
//                }

//            //player 2 health gui
//                for (int i = 0; i < players[1].Health; i++)
//                {
//                    Vector2 startPos = new Vector2((float)titleSafeArea.Right - (players[1].MaxHealth * _playerHealth.Width) -_powerUpNuke.Width * 3, (float)titleSafeArea.Top + _playerHealth.Height);

//                    ctx.Batch.Draw(_playerHealth, startPos + new Vector2(i * _playerHealth.Width * 0.5f + 20, 0.0f), Color.Silver);
//                }

//            //draw each player's acquired power-ups.  Currently only works w/ 2 players

//            //player 1 nuke gui
//                for (int i = 0; i < players[0].NukeCount; i++)
//                {
//                    Vector2 startPos = new Vector2((float)titleSafeArea.Left + _playerHealth.Width * players[0].MaxHealth * 0.5f + 20.0f, (float)titleSafeArea.Top + _playerHealth.Height);

//                    ctx.Batch.Draw(_powerUpNuke, startPos + new Vector2(i * _powerUpNuke.Width + _powerUpNuke.Width, 0.0f), Color.Gold);
//                }

//            //player 2 nuke gui
//                for (int i = 0; i < players[1].NukeCount; i++)
//                {
//                    Vector2 startPos = new Vector2((float)titleSafeArea.Right - (_powerUpNuke.Width * 6) + 20.0f, (float)titleSafeArea.Top + _playerHealth.Height);

//                    ctx.Batch.Draw(_powerUpNuke, startPos + new Vector2(i * _powerUpNuke.Width + _powerUpNuke.Width, 0.0f), Color.Silver);
//                }


//            //player 1 shield gui
//                for (int i = 0; i < players[0].ShieldCount; i++)
//                {
                    
//                    Vector2 startPos = new Vector2((float)titleSafeArea.Left + _playerHealth.Width * players[0].MaxHealth * 0.5f + 20.0f, (float)titleSafeArea.Top + _playerHealth.Height + _powerUpNuke.Height);

//                    ctx.Batch.Draw(_powerUpSheild, startPos + new Vector2(i * _powerUpSheild.Width + _powerUpSheild.Width, 0.0f), Color.Gold);
//                }

//            //player 2 shield gui
//                for (int i = 0; i < players[1].NukeCount; i++)
//                {
//                    Vector2 startPos = new Vector2((float)titleSafeArea.Right - (_powerUpSheild.Width * 6) + 20.0f, (float)titleSafeArea.Top + _playerHealth.Height + _powerUpSheild.Height);

//                    ctx.Batch.Draw(_powerUpSheild, startPos + new Vector2(i * _powerUpSheild.Width + _powerUpSheild.Width, 0.0f), Color.Silver);
//                }
//            ctx.Batch.End();

//            #region Draw Bounding Collision Boxes
//            //for (int i = 0; i < MAX_PLAYERS; i++)
//            //{
//            //    if (players[i].NukeFired)
//            //        DrawBoundingBox(players[i].nuke.CollisionBounds, ctx.Offset, tempColor);
//            //}
//            //for (int i = 0; i < MAX_PLAYERS; i++)
//            //{
//            //    DrawBoundingBox(players[i].rotatedCollisionBox, ctx.Offset, tempColor);
//            //    for (int j = 0; j < players[i].projectiles.Count; j++)
//            //    {
//            //        DrawBoundingBox(players[i].projectiles[j].rotatedCollisionBox, ctx.Offset, tempColor);
//            //    }
//            //}
             
//            //for (int i = 0; i < enemies.Count; i++)
//            //{
//            //    if (enemies[i].IsAlive)
//            //    {
//            //        DrawBoundingBox(enemies[i].rotatedCollisionBox, ctx.Offset, tempColor);
//            //        if(enemies[i].attackType.Equals(AttackType.Advanced_Chase))
//            //        DrawBoundingBox(enemies[i].rotatedEvadeBounds, ctx.Offset, tempColor2);
//            //    }
//            //}
//            #endregion

//            base.Draw(gameTime);
//        }
//        public void DrawBoundingBox(RotatedRectangle box, Vector2 offset, Color color)
//        {

//            // tell the primitive batch to start drawing lines
//            primitiveBatch.Begin(PrimitiveType.LineList);

//            // from the top left to the top right
//            primitiveBatch.AddVertex(
//                offset + box.UpperLeftCorner(), color);
//            primitiveBatch.AddVertex(
//                offset + box.UpperRightCorner(), color);

//            // from the top right to the bottom right
//            primitiveBatch.AddVertex(
//                offset + box.UpperRightCorner(), color);
//            primitiveBatch.AddVertex(
//                offset + box.LowerRightCorner(), color);

//            // from the bottom right to the bottom left
//            primitiveBatch.AddVertex(
//                offset + box.LowerRightCorner(), color);
//            primitiveBatch.AddVertex(
//                offset + box.LowerLeftCorner(), color);

//            // from the bottom left to the top left
//            primitiveBatch.AddVertex(
//                offset + box.LowerLeftCorner(), color);
//            primitiveBatch.AddVertex(
//                offset + box.UpperLeftCorner(), color);

//            // and we're done.
//            primitiveBatch.End();
//        }
//        public void DrawBoundingBox(Rectangle box, Vector2 offset, Color color)
//        {

//            // tell the primitive batch to start drawing lines
//            primitiveBatch.Begin(PrimitiveType.LineList);

//            // from the top left to the top right
//            primitiveBatch.AddVertex(
//                offset + new Vector2(box.Left, box.Top), color);
//            primitiveBatch.AddVertex(
//                offset + new Vector2(box.Right, box.Top), color);

//            // from the top right to the bottom right
//            primitiveBatch.AddVertex(
//                offset + new Vector2(box.Right, box.Top), color);
//            primitiveBatch.AddVertex(
//                offset + new Vector2(box.Right, box.Bottom), color);

//            // from the bottom right to the bottom left
//            primitiveBatch.AddVertex(
//                offset + new Vector2(box.Right, box.Bottom), color);
//            primitiveBatch.AddVertex(
//                offset + new Vector2(box.Left, box.Bottom), color);

//            // from the bottom left to the top left
//            primitiveBatch.AddVertex(
//                offset + new Vector2(box.Left, box.Bottom), color);
//            primitiveBatch.AddVertex(
//                offset + new Vector2(box.Left, box.Top), color);

//            // and we're done.
//            primitiveBatch.End();
//        }

//        public int GetNumDigits(int num)
//        {
//            int i = 0;
//            while (num > 1)
//            {
//                num = (int)(num * 0.1);
//                i++;
//            }
//            return i;
//        }
//    }
//}
