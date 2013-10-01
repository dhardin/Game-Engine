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
            private const float SPEED = 3.0f;
            private const float RotationRadiansPerSecond = 15.0f;
            private const float FullSpeed = 10.0f;
            private const float VelocityMaximum = 10.0f;
            private const float DragPerSecond = 0.9f;
            private float Rotation;
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
            private TimeSpan previousFireTime;
            private TimeSpan previousNukeTime;
            private TimeSpan ShieldTime = TimeSpan.FromSeconds(15.0f);
            private TimeSpan NukeTime = TimeSpan.FromSeconds(MAX_NUKE_COOLDOWN);

            public bool invincibilityFlicker = false;
            public bool HasCollided = false;
            private bool collideReset = true;
            private bool hasFired = false;
            private bool nukeFired = false;
            private bool shieldUsed = false;
            private bool isDead = false;
       

            SoundEffect powerUpRapidFireSoundEffect;
            SoundEffect powerUpShieldSoundEffect;
            SoundEffect powerUpSonicSoundEffect;
            SoundEffect powerUpHeavySoundEffect;
            SoundEffect hitSoundEffect;
            SoundEffect standardFireSoundEffect;
            SoundEffect nukeSoundEffect;

            ParticleSystem smoke;

            ContentManager c;


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
                
            }
            public Player(PlayerIndex pIndex)
            {
                playerIndex = pIndex;
              

            }
           

            public void LoadContent(ContentManager content)
            {
               

                rotatedCollisionBox = new RotatedRectangle(CollisionBounds, Rotation);
            }
            public void Draw(SpriteBatch batch, Vector2 offset, Vector2 viewportPosition, float opacity, int width, int height)
            {

                //batch.Draw(this.Texture, this.Position, Color.White);
                batch.Draw(Texture, this.Position, null, Color.White, this.Rotation, this.Orgin, SIZE_MOD, SpriteEffects.None, 0);

                Primitives2D.DrawLine(batch, rotatedRect.UpperLeftCorner(), rotatedRect.UpperRightCorner(), Color.LightPink);
                Primitives2D.DrawLine(batch, rotatedRect.UpperRightCorner(), rotatedRect.LowerRightCorner(), Color.LightPink);
                Primitives2D.DrawLine(batch, rotatedRect.LowerRightCorner(), rotatedRect.LowerLeftCorner(), Color.LightPink);
                Primitives2D.DrawLine(batch, rotatedRect.LowerLeftCorner(), rotatedRect.UpperLeftCorner(), Color.LightPink);

                Primitives2D.DrawLine(batch, new Vector2(Rect.Left, Rect.Top), new Vector2(Rect.Right, Rect.Top), Color.LightBlue);
                Primitives2D.DrawLine(batch, new Vector2(Rect.Right, Rect.Top), new Vector2(Rect.Right, Rect.Bottom), Color.LightBlue);
                Primitives2D.DrawLine(batch, new Vector2(Rect.Right, Rect.Bottom), new Vector2(Rect.Left, Rect.Bottom), Color.LightBlue);
                Primitives2D.DrawLine(batch, new Vector2(Rect.Left, Rect.Bottom), new Vector2(Rect.Left, Rect.Top), Color.LightBlue);

                for (int i = 0; i < projectiles.Count; i++)
                {
                    //Vector2 displacement = new Vector2(
                    //    (float)(this.Position.X + (this.Texture.Width - this.Width) * Math.Cos(Rotation)), (float)(this.Position.Y + (this.Texture.Width - this.Width) * Math.Sin(Rotation)));
                    projectiles[i].Draw(batch);

                }

                
                //draw mouse crosshair
                Primitives2D.DrawLine(batch, this.Position, mousePosition, Color.Red, 0.6f);
              
            }

            public void Rotate()
            {

                MouseState mouse = Mouse.GetState();
                mousePosition = new Vector2(mouse.X, mouse.Y);

                Vector2 direction = mousePosition - this.Position;
                direction.Normalize();

                this.Rotation = (float)Math.Atan2(
                              (double)direction.Y,
                              (double)direction.X);


            }
            public void HandleInput(GameTime gameTime, Viewport viewport, ref Map map/*, InputState input*/)
            {
                GamePadState gamePadState = GamePad.GetState(playerIndex);
                KeyboardState keyboardState = Keyboard.GetState();//input.CurrentKeyboardStates[0];
                MouseState mouseState = Mouse.GetState();
                Keys[] pressedKeys = keyboardState.GetPressedKeys();
                Vector2 newPosition = Position;
                int inputNum = 0;

               

                  

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
                    Vector2 mod = new Vector2(1, 1);
                    if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left) ||
                    gamePadState.DPad.Left == ButtonState.Pressed)
                    {
                        newPosition.X -= Speed;
                        mod.X = -1;
                    }
                    if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right) ||
                    gamePadState.DPad.Right == ButtonState.Pressed)
                    {
                        newPosition.X += Speed;
                        mod.X = 1;
                    }
                    if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up) ||
                    gamePadState.DPad.Up == ButtonState.Pressed)
                    {
                        newPosition.Y -= Speed;
                        mod.Y = -1;
                    }
                    if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down) ||
                    gamePadState.DPad.Down == ButtonState.Pressed)
                    {
                        newPosition.Y += Speed;
                        mod.Y = 1;
                    }

                    // Make sure that the player does not go out of bounds
                    newPosition.X = MathHelper.Clamp(newPosition.X, this.Width * 0.5f/*_sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width*/ * 0.5f, /*ScreenManager.Instance.GraphicsDevice.Viewport.Width*/viewport.Width - this.Width * 0.5f);//(_sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 0.5f));
                    newPosition.Y = MathHelper.Clamp(newPosition.Y, this.Height /* _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height*/, /*ScreenManager.Instance.GraphicsDevice.Viewport.Height*/viewport.Height);

                    if (willCollideLevel(ref map, this, new Vector2(Speed * mod.X, 0), false) == false)
                    {
                       
                        this.Position.X = newPosition.X;
                        
                    }
                    
                    if (willCollideLevel(ref map, this, new Vector2(0, Speed * mod.Y), false) == false)
                    {
                       this.Position.Y = newPosition.Y;
                        
                    }

                    
                    

                    this.Update(gameTime);

                    
                    // Fire in indicated right thumbstick direction
                    if ((gamePadState.Triggers.Right >= 0.5f || (mouseState.LeftButton == ButtonState.Pressed) || keyboardState.IsKeyDown(Keys.Space)) && (gameTime.TotalGameTime - previousFireTime > FireCooldownTime))
                    {
                        // Reset our current time
                        previousFireTime = gameTime.TotalGameTime;
                   
                    
                        AddProjectile(Position, (float)Rotation);

                        var pos = new Vector2(Position.X - 15, Position.Y - 5);//PARTICLES
                       // smoke.AddParticles(pos, Vector2.Zero);//PARTICLES
                        pos = new Vector2(Position.X + 15, Position.Y - 5);//PARTICLES
                        //smoke.AddParticles(pos, Vector2.Zero);//PARTICLES

                        hasFired = true;
                    }
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        if (!projectiles[i].Active)
                            projectiles.RemoveAt(i);
                    }
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        projectiles[i].Update(ref map);   
                    }
                    
                  
                

            }

            /// <summary>
            /// Add projectile fired by the player to the game
            /// </summary>
            /// <param name="position"></param>
            /// <param name="fireDirection"></param>
            private void AddProjectile(Vector2 position, float fireDirection)
            {
                Projectile projectile;
             
                        projectile = new Projectile(position, fireDirection, ProjectileType.Standard, this.Layer);

                        
                projectiles.Add(projectile);
                
            }

            private WeaponType currentWeapon()
            {

                return WeaponType.Auto;
            }

            public void UpdateFireRate()
            {

                //for (int i = 0; i < powerUps.Count; i++)
                //{
                //    if (powerUps[i].Type.Equals(PowerUpType.Fast) || powerUps[i].Type.Equals(PowerUpType.Heavy) || powerUps[i].Type.Equals(PowerUpType.Sonic) || powerUps[i].Type.Equals(PowerUpType.None))
                //        FireCooldownTime = TimeSpan.FromSeconds(powerUps[i].RateOfFire);
                //}
            }

            
        }
    }

        
