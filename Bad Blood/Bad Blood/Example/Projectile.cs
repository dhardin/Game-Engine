using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Engine.Extensions;
using C3.XNA;
using Squared.Tiled;

namespace Game
{
        public enum ProjectileType
        {
            Standard,
            Rapid,
            Spread,
            Heavy,
            Orb
        }
        public class Projectile : GameObject
        {
            #region Fields
            // Image representing the Projectile
            //public Texture2D Texture { get; set; }

            public static Texture2D Texture;

            public Vector2 firePosition;

            // The amount of damage the projectile can inflict to an enemy
            public int Damage;

            const float SIZE_MOD = 0.4f;

            private ProjectileType projectileType;

            // Determines how fast the projectile moves
            public float projectileMoveSpeed;

            private const float MAX_PROJECTILE_SPEED_MULTIPLYER = 4f;

            #endregion

            #region Properties



            public ProjectileType ProjectileType
            {
                get { return projectileType; }
            }

            #endregion
            public Projectile(Vector2 position, float angle, ProjectileType projectile, int currentLayer)
            {

               
                Layer = currentLayer;
                Rotation = angle;
                Height = (int)(Texture.Height * SIZE_MOD);
                Width = (int)(Texture.Width * SIZE_MOD);
                Orgin = new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f);
                Position = position - Orgin * SIZE_MOD; //this step is needed so our projectile appears to fire out of the gun's barrel and not off to the side.
                Rect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

                this.rotatedRect = new RotatedRectangle(Rect, Rotation);
                Active = true;
                
                 projectileType = projectile;
              
                firePosition.X = (float)Math.Cos(Rotation);
                firePosition.Y = (float)Math.Sin(Rotation);
                Damage = 5;
                projectileMoveSpeed = 8.0f * MAX_PROJECTILE_SPEED_MULTIPLYER;

              
            }

            public override bool CollidesWith(Rectangle rectangle, ref Vector2 newPosition)
            {
                Vector2 collisionDepth = RectangleExtensions.GetIntersectionDepth(CollisionBounds, rectangle);

                if (collisionDepth != Vector2.Zero)
                {
                    if (Math.Abs(collisionDepth.Y) < Math.Abs(collisionDepth.X))
                        newPosition.Y += collisionDepth.Y;
                    else
                        newPosition.X += collisionDepth.X;
                }

                return collisionDepth != Vector2.Zero;

            }
            #region Update and Draw
            public override void Update(GameTime gameTime, ref Map map, Vector2 viewportPosition)
            {
                
                if (Active)
                {
                    //we need to figure out how to fire towards the direction we are currently facing
                    //the current bug is directing our projectiles at about 90 degrees off the 
                    //desired course
                    tileCollisionChecks.Clear();

                    if ((Position.X + projectileMoveSpeed * firePosition.X < 0) ||
                        (Position.X + projectileMoveSpeed * firePosition.X > map.Width * map.TileWidth) ||
                        (Position.Y + projectileMoveSpeed * firePosition.Y < 0) ||
                        (Position.Y + projectileMoveSpeed * firePosition.Y > map.Height * map.TileWidth))
                        Active = false;
                    else if (willCollideLevel(ref map, this, new Vector2(projectileMoveSpeed * firePosition.X, projectileMoveSpeed * firePosition.Y), true))
                        Active = false;

                    if (Active)
                    {
                        this.Position.X += (projectileMoveSpeed * firePosition.X);
                        this.Position.Y += (projectileMoveSpeed * firePosition.Y);

                        this.rotatedRect.ChangePosition(this.Position);
                        this.Rect.Location = new Point((int)(this.Position.X), (int)(this.Position.Y));
                    }
                   
                }
            }
            public override void Draw(SpriteBatch batch, Vector2 offset, Vector2 viewportPosition, float opacity, int width, int height)
            {

                //batch.Draw(this.Texture, this.Position, Color.White);
               // batch.Draw(Texture, this.Position, null, Color.White, this.Rotation, this.Orgin, 1f, SpriteEffects.None, 0);


                if (Active)
                {
                    //batch.Draw(Texture, this.Position, null, Color.White, this.Rotation, this.Orgin, SIZE_MOD, SpriteEffects.None, 0);

                    batch.Draw(Texture, Position + Orgin * SIZE_MOD - viewportPosition + Offset, null, Color.White, this.Rotation, Orgin, SIZE_MOD, SpriteEffects.None, 0);


                    //int lowestPoint = (int)(Math.Min((int)Math.Min(rotatedRect.UpperLeftCorner().Y, rotatedRect.UpperRightCorner().Y), (int)Math.Min(rotatedRect.LowerLeftCorner().Y, rotatedRect.LowerRightCorner().Y)));
                    //int highestPoint = (int)(Math.Max((int)Math.Max(rotatedRect.UpperLeftCorner().Y, rotatedRect.UpperRightCorner().Y), (int)Math.Max(rotatedRect.LowerLeftCorner().Y, rotatedRect.LowerRightCorner().Y)));
                    //int leftMostPoint = (int)(Math.Min((int)Math.Min(rotatedRect.UpperLeftCorner().X, rotatedRect.UpperRightCorner().X), (int)Math.Min(rotatedRect.LowerLeftCorner().X, rotatedRect.LowerRightCorner().X)));
                    //int rightMostPoint = (int)(Math.Max((int)Math.Max(rotatedRect.UpperLeftCorner().X, rotatedRect.UpperRightCorner().X), (int)Math.Max(rotatedRect.LowerLeftCorner().X, rotatedRect.LowerRightCorner().X)));

                    //Primitives2D.DrawCircle(batch, new Vector2(leftMostPoint, lowestPoint), 2f, 100, Color.Red);
                    //Primitives2D.DrawCircle(batch, new Vector2(leftMostPoint, highestPoint), 2f, 100, Color.Green);
                    //Primitives2D.DrawCircle(batch, new Vector2(rightMostPoint, lowestPoint), 2f, 100, Color.Blue);
                    //Primitives2D.DrawCircle(batch, new Vector2(rightMostPoint, highestPoint), 2f, 100, Color.White);

                    //Primitives2D.DrawLine(batch, rotatedRect.UpperLeftCorner(), rotatedRect.UpperRightCorner(), Color.LightPink);
                    //Primitives2D.DrawLine(batch, rotatedRect.UpperRightCorner(), rotatedRect.LowerRightCorner(), Color.LightPink);
                    //Primitives2D.DrawLine(batch, rotatedRect.LowerRightCorner(), rotatedRect.LowerLeftCorner(), Color.LightPink);
                    //Primitives2D.DrawLine(batch, rotatedRect.LowerLeftCorner(), rotatedRect.UpperLeftCorner(), Color.LightPink);

                    //Primitives2D.DrawLine(batch, new Vector2(Rect.Left, Rect.Top), new Vector2(Rect.Right, Rect.Top), Color.LightBlue);
                    //Primitives2D.DrawLine(batch, new Vector2(Rect.Right, Rect.Top), new Vector2(Rect.Right, Rect.Bottom), Color.LightBlue);
                    //Primitives2D.DrawLine(batch, new Vector2(Rect.Right, Rect.Bottom), new Vector2(Rect.Left, Rect.Bottom), Color.LightBlue);
                    //Primitives2D.DrawLine(batch, new Vector2(Rect.Left, Rect.Bottom), new Vector2(Rect.Left, Rect.Top), Color.LightBlue);


                    for (int i = 0; i < tileCollisionChecks.Count; i++)
                    {


                        Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Top) - viewportPosition + Offset, new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Top) - viewportPosition + Offset, Color.Red);
                        Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Top) - viewportPosition + Offset, new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Bottom) - viewportPosition + Offset, Color.Red);
                        Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Bottom) - viewportPosition + Offset, new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Bottom) - viewportPosition + Offset, Color.Red);
                        Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Bottom) - viewportPosition + Offset, new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Top) - viewportPosition + Offset, Color.Red);
                    }

                }
            }

            public override void Draw(SpriteBatch batch)
            {
                if (Active)
                {

                    batch.Draw(Texture, Position, null, Color.White, Rotation /*+ (float)(Math.PI * 0.5f)*/,
                    new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);


                    Primitives2D.DrawLine(batch, rotatedRect.UpperLeftCorner(), rotatedRect.UpperRightCorner(), Color.LightPink);
                    Primitives2D.DrawLine(batch, rotatedRect.UpperRightCorner(), rotatedRect.LowerRightCorner(), Color.LightPink);
                    Primitives2D.DrawLine(batch, rotatedRect.LowerRightCorner(), rotatedRect.LowerLeftCorner(), Color.LightPink);
                    Primitives2D.DrawLine(batch, rotatedRect.LowerLeftCorner(), rotatedRect.UpperLeftCorner(), Color.LightPink);

                    for (int i = 0; i < tileCollisionChecks.Count; i++)
                    {


                        Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Top), new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Top), Color.Red);
                        Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Top), new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Bottom), Color.Red);
                        Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Bottom), new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Bottom), Color.Red);
                        Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Bottom), new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Top), Color.Red);
                    }
                }
            }
            #endregion
        }

    }