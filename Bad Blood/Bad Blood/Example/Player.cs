using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Engine.Sprites;
using Engine.Audio;
using Engine.Extensions;
using Engine.TaskManagement;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Engine.GameStateManagerment;
using System.IO;
using Engine.ParticleSystem;
using Engine.ParticleEmitter;
using Squared.Tiled;
using C3.XNA;

namespace Game
{
    public class Player : GameObject
    {
            #region Fields
        #region StateMachine
  
        #endregion
        private const float SPEED = 3.0f;
       
            private const float RotationRadiansPerSecond = 15.0f;
            private const float FullSpeed = 10.0f;
            private const float VelocityMaximum = 10.0f;
            private const float DragPerSecond = 0.9f;
            private Vector2 mousePosition;
            private const float SIZE_MOD = 0.4f;
           
            private int health;
            public const int MAX_HEALTH = 10;
            private const float MAX_FIRE_RATE_TIME = 0.1f;

            private int shieldCount;
            public const int MAX_SHIELD_COUNT = 10;
            private const float MAX_SHIELD_TIME = 15.0f;

            private int nukeCount;
            public const int MAX_NUKE_COUNT = 2;
            private const float MAX_NUKE_COOLDOWN = 2;

            
            private int score;
            private const int MAX_SCORE = int.MaxValue;

            public PlayerIndex playerIndex;
          
            public List<Projectile> projectiles = new List<Projectile>();
            //public List<Weapon> weapons = new List<Weapon>();
            public RotatedRectangle rotatedCollisionBox;

            public TimeSpan CollideWaitTime = TimeSpan.FromSeconds(2.0);
            private TimeSpan FlickerTime = TimeSpan.FromSeconds(0.01);
            private TimeSpan FireCooldownTime = TimeSpan.FromSeconds(MAX_FIRE_RATE_TIME);

            public TimeSpan previousGrappleTime { get; private set; }
            public TimeSpan previousMeleeTime { get; private set; }
            private TimeSpan previousFireTime;
            private TimeSpan previousNukeTime;
            private TimeSpan ShieldTime = TimeSpan.FromSeconds(15.0f);
            private TimeSpan NukeTime = TimeSpan.FromSeconds(MAX_NUKE_COOLDOWN);
            private TimeSpan GrappleTime = TimeSpan.FromSeconds(0.5f);
            private TimeSpan MeleeTime = TimeSpan.FromSeconds(0.2f);

            public bool invincibilityFlicker = false;
            public bool HasCollided = false;
            private bool nukeFired = false;
            private bool shieldUsed = false;

            public override Vector2 GunBarrelPosition { get; set; }
            //public delegate void ChangedEventHandler(object sender, EventArgs e);




            #endregion

            #region Properties

            public TopDownGame _game;

            public bool SoundEffects
            {
                get;
                set;
            }

            public float Speed
            {
                get { return SPEED; }
            }

            public int Score
            {
                get { return score; }

                set
                {
                    if (value > MAX_SCORE)
                        score = MAX_SCORE;
                    else
                        score = value;
                }
            }

            public int Health
            {
                get { return health; }
            }

            public int MaxHealth
            {
                get { return MAX_HEALTH; }
            }

            public int NukeCount
            {
                get { return nukeCount; }
                set { nukeCount = value; }
            }

            public int Max_Nuke_Count
            {
                get { return MAX_NUKE_COUNT; }
            }

            public bool NukeFired
            {
                get { return nukeFired; }
                set { nukeFired = value; }
            }

            public int ShieldCount
            {
                get { return shieldCount; }
                set { shieldCount = value; }
            }

            public bool ShieldUsed
            {
                get { return shieldUsed; }
                set { shieldUsed = value; }
            }

            public int Max_Sheild_Count
            {
                get { return MAX_SHIELD_COUNT; }
            }
            #endregion

            public Player(PlayerIndex pIndex, Vector2 pos)
            {
                playerIndex = pIndex;
                Position = pos;
                pState = new Process();
                rotatedCollisionBox = new RotatedRectangle(Rect, Rotation);
            }
            public Player(PlayerIndex pIndex, Vector2 pos, float rotation, int layer)
            {
                playerIndex = pIndex;
                pState = new Process();
                Position = pos;
                Layer = layer;
                Rotation = rotation;
                Height = (int)(Texture.Height * SIZE_MOD);
                Width = (int)(Texture.Width * SIZE_MOD);
                Orgin = new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f);
                Rect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

                this.rotatedRect = new RotatedRectangle(Rect, Rotation);
                GunBarrelPosition = Orgin* SIZE_MOD;
            }
           

           
            public override void Draw(SpriteBatch batch, Vector2 offset, Vector2 viewportPosition, float opacity, int width, int height)
            {

                //batch.Draw(this.Texture, this.Position, Color.White);
                batch.Draw(Texture, this.Position + Orgin * SIZE_MOD - viewportPosition + Offset, null, Color.White, this.Rotation, this.Orgin, SIZE_MOD, SpriteEffects.None, 0);

                Primitives2D.DrawLine(batch, rotatedRect.UpperLeftCorner() - viewportPosition + Offset, rotatedRect.UpperRightCorner() - viewportPosition + Offset, Color.LightPink);
                Primitives2D.DrawLine(batch, rotatedRect.UpperRightCorner() - viewportPosition + Offset, rotatedRect.LowerRightCorner() - viewportPosition + Offset, Color.LightPink);
                Primitives2D.DrawLine(batch, rotatedRect.LowerRightCorner() - viewportPosition + Offset, rotatedRect.LowerLeftCorner() - viewportPosition + Offset, Color.LightPink);
                Primitives2D.DrawLine(batch, rotatedRect.LowerLeftCorner() - viewportPosition + Offset, rotatedRect.UpperLeftCorner() - viewportPosition + Offset, Color.LightPink);

                Primitives2D.DrawLine(batch, new Vector2(Rect.Left, Rect.Top) - viewportPosition + Offset, new Vector2(Rect.Right, Rect.Top) - viewportPosition + Offset, Color.LightBlue);
                Primitives2D.DrawLine(batch, new Vector2(Rect.Right, Rect.Top) - viewportPosition + Offset, new Vector2(Rect.Right, Rect.Bottom) - viewportPosition + Offset, Color.LightBlue);
                Primitives2D.DrawLine(batch, new Vector2(Rect.Right, Rect.Bottom) - viewportPosition + Offset, new Vector2(Rect.Left, Rect.Bottom) - viewportPosition + Offset, Color.LightBlue);
                Primitives2D.DrawLine(batch, new Vector2(Rect.Left, Rect.Bottom) - viewportPosition + Offset, new Vector2(Rect.Left, Rect.Top) - viewportPosition + Offset, Color.LightBlue);


                //Primitives2D.DrawCircle(batch, new Vector2(rotatedRect.X, rotatedRect.Y), 2f, 100, Color.White);
                //Primitives2D.DrawCircle(batch, Position + Orgin * SIZE_MOD, 2f, 100, Color.Red);

                foreach (Rectangle rect in tileCollisionChecks)
                {
                    Primitives2D.DrawLine(batch, new Vector2(rect.Left, rect.Top) - viewportPosition + Offset, new Vector2(rect.Right, rect.Top) - viewportPosition + Offset, Color.Red);
                    Primitives2D.DrawLine(batch, new Vector2(rect.Right, rect.Top) - viewportPosition + Offset, new Vector2(rect.Right, rect.Bottom) - viewportPosition + Offset, Color.Red);
                    Primitives2D.DrawLine(batch, new Vector2(rect.Right, rect.Bottom) - viewportPosition + Offset, new Vector2(rect.Left, rect.Bottom) - viewportPosition + Offset, Color.Red);
                    Primitives2D.DrawLine(batch, new Vector2(rect.Left, rect.Bottom) - viewportPosition + Offset, new Vector2(rect.Left, rect.Top) - viewportPosition + Offset, Color.Red);
                }
                //draw mouse crosshair
                Primitives2D.DrawLine(batch, this.Position + GunBarrelPosition + new Vector2(Width * 0.5f * (float)Math.Cos(Rotation), Width * 0.5f * (float)Math.Sin(Rotation)) - viewportPosition + Offset, mousePosition, Color.Red, 0.6f);
              
            }

            public override void Rotate()
            {

                MouseState mouse = Mouse.GetState();
                mousePosition = new Vector2(mouse.X, mouse.Y);

                Vector2 direction = mousePosition - this.Position;
                direction.Normalize();

                Rotation = (float)Math.Atan2(
                              (double)direction.Y,
                              (double)direction.X);
                float tempDeg = (float)(Rotation * 180 / Math.PI);
                int x = 0;

            }
            public override void HandleInput(GameTime gameTime, Viewport viewport, ref Map map, Vector2 viewportPosition)
            {
                GamePadState gamePadState = GamePad.GetState(playerIndex);
                KeyboardState keyboardState = Keyboard.GetState();//input.CurrentKeyboardStates[0];
                MouseState mouseState = Mouse.GetState();
                Keys[] pressedKeys = keyboardState.GetPressedKeys();
                Vector2 newPosition = Position;
                int inputNum = 0;



                switch (pState.CurrentState)
                {
                    case ProcessState.Grappling://Player can't do anything else if they're grappling unless they want to disengage by pressing the escape key
                        pState.MoveNext(keyboardState.IsKeyDown(Keys.Escape) ? Command.Disengage : Command.None);
                        break;
                    default:
                    
                        Rotate();
                        //reset rotation angle if it is greater than 2PI radians or less than 0 radians
                        Rotation = MathHelper.WrapAngle(Rotation);
                        // Get Thumbstick Controls
                        newPosition.X += gamePadState.ThumbSticks.Left.X * Speed;
                        newPosition.Y -= gamePadState.ThumbSticks.Left.Y * Speed;

                        // calculate the current forward vector (make it negative so we turn in the correct direction)
                        Vector2 forward = new Vector2((float)Math.Sin(Rotation),
                            (float)Math.Cos(Rotation));
                        Vector2 right = new Vector2(-1 * forward.Y, forward.X);//vector that is at a right angle (+PI/2) to the vector that you're moving in

                        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // calculate the new forward vector with the left stick
                        if (gamePadState.ThumbSticks.Right.LengthSquared() > 0f)
                        {
                            // change the direction 
                            Vector2 wantedForward = Vector2.Normalize(gamePadState.ThumbSticks.Right);
                            //find the difference between our current vector and wanted vector using the dot product
                            float angleDiff = (float)Math.Acos(
                                Vector2.Dot(wantedForward, forward));

                            //check the angle between the right angle from the current vector and wanted.  Adjust rotation direction accordingly
                            float facing;
                            if (Vector2.Dot(wantedForward, right) > 0f)
                            {
                                //rotate towards right angle first
                                facing = -1.0f;
                            }
                            else
                            {
                                //rotate towards the wanted angle since it is closer
                                facing = 1.0f;
                            }

                            //if we have an acceptable change in direction that is not too small
                            if (angleDiff > (Math.PI / 20))
                            {
                                Rotation += facing * Math.Min(angleDiff, elapsed *
                                    RotationRadiansPerSecond);
                            }

                            // add velocity
                            Velocity += gamePadState.ThumbSticks.Left * (elapsed * FullSpeed);
                            if (Velocity.Length() > VelocityMaximum)
                            {
                                Velocity = Vector2.Normalize(Velocity) *
                                    VelocityMaximum;
                            }
                        }
                        //currentGamePadState.ThumbSticks.Left = Vector2.Zero;

                        // apply drag to the velocity
                        Velocity -= Velocity * (elapsed * DragPerSecond);
                        if (Velocity.LengthSquared() <= 0f)
                        {
                            Velocity = Vector2.Zero;
                        }

                        // Use the Keyboard / Dpad
                        Vector2 mod = new Vector2(0, 0);
                        bool moved = false;
                        if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left) ||
                        gamePadState.DPad.Left == ButtonState.Pressed)
                        {
                            newPosition.X -= Speed;
                            mod.X = -1;
                            moved = true;
                        }
                        if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right) ||
                        gamePadState.DPad.Right == ButtonState.Pressed)
                        {
                            newPosition.X += Speed;
                            mod.X = 1;
                            moved = true;
                        }
                        if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up) ||
                        gamePadState.DPad.Up == ButtonState.Pressed)
                        {
                            newPosition.Y -= Speed;
                            mod.Y = -1;
                            moved = true;
                        }
                        if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down) ||
                        gamePadState.DPad.Down == ButtonState.Pressed)
                        {
                            newPosition.Y += Speed;
                            mod.Y = 1;
                            moved = true;
                        }

                        // Make sure that the player does not go out of bounds
                        newPosition.X = MathHelper.Clamp(newPosition.X, Width, map.Width * map.TileWidth - 2*Width);
                        newPosition.Y = MathHelper.Clamp(newPosition.Y, Height , map.Height * map.TileHeight -  2* Height);

                        if (moved)
                        {
                            pState.MoveNext(Command.Move);

                            if (willCollideLevel(ref map, this, new Vector2(Speed * mod.X, 0), false) == false)
                            {

                                this.Position.X = newPosition.X;

                            }


                            if (willCollideLevel(ref map, this, new Vector2(0, Speed * mod.Y), false) == false)
                            {
                                this.Position.Y = newPosition.Y;

                            }
                        }
                        else
                        {
                            pState.MoveNext(Command.None);
                        }


                    switch(pState.CurrentState)
                    {
                        case ProcessState.Meleeing:
                            break;
                        default:
                            // Fire in indicated right thumbstick direction
                            if ((gamePadState.Triggers.Right >= 0.5f || (mouseState.LeftButton == ButtonState.Pressed) || keyboardState.IsKeyDown(Keys.Space)) && (gameTime.TotalGameTime - previousFireTime > FireCooldownTime))
                            {
                                pState.MoveNext(Command.Shoot);
                                // Reset our current time
                                previousFireTime = gameTime.TotalGameTime;
                   
                    
                                //AddProjectile(Position, (float)Rotation);

                                var pos = new Vector2(Position.X - 15, Position.Y - 5);//PARTICLES
                               // smoke.AddParticles(pos, Vector2.Zero);//PARTICLES
                                pos = new Vector2(Position.X + 15, Position.Y - 5);//PARTICLES
                                //smoke.AddParticles(pos, Vector2.Zero);//PARTICLES

                                
                            }
                            if (mouseState.RightButton == ButtonState.Pressed)
                            {
                                pState.MoveNext(Command.Melee);
                                previousMeleeTime = gameTime.TotalGameTime;
                            }
                            if (mouseState.MiddleButton == ButtonState.Pressed)
                            {
                                pState.MoveNext(Command.Grapple);
                                previousGrappleTime = gameTime.TotalGameTime;
                            }
                            break;
                    };

                    break;
                };
                this.Update(gameTime, ref map, viewportPosition);

            }
            public override void Update(GameTime gameTime, ref Map map, Vector2 viewportPosition)
            {
                switch (this.pState.CurrentState)
                {
                    case ProcessState.Dead:
                        break;
                    case ProcessState.Idle:
                        break;
                    case ProcessState.Moving:
                        break;
                    case ProcessState.Shooting:
                        break;
                    case ProcessState.Meleeing:
                        pState.MoveNext((gameTime.TotalGameTime - previousMeleeTime > MeleeTime) ? Command.Disengage : Command.None);
                        break;
                    case ProcessState.Grappling:
                        pState.MoveNext((gameTime.TotalGameTime - previousGrappleTime > GrappleTime) ? Command.Disengage : Command.None);
                        break;
                    case ProcessState.Injured:
                        break;
                    default:
                        break;  
                };
               
                //this.Rotate();
                this.rotatedRect.ChangePosition(this.Position);
                this.rotatedRect.Rotation = this.Rotation;
                this.Rect.Location = new Point((int)(this.Position.X), (int)(this.Position.Y));
            }
            /// <summary>
            /// Add projectile fired by the player to the game
            /// </summary>
            /// <param name="position"></param>
            /// <param name="fireDirection"></param>
            

            private WeaponType currentWeapon()
            {

                return WeaponType.Auto;
            }            
        }
    }

        
