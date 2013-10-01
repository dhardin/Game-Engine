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

            // State of the Projectile
            public bool Active;

            // The amount of damage the projectile can inflict to an enemy
            public int Damage;

            private float rotationAngle;

           

            private ProjectileType projectileType;

            // Determines how fast the projectile moves
            public float projectileMoveSpeed;

            private const float MAX_PROJECTILE_SPEED_MULTIPLYER = 4f;

            #endregion

            #region Properties

            // Get the width of the projectile ship
            public int Width
            {
                get { return Texture.Width; }
            }

            // Get the height of the projectile ship
            public int Height
            {
                get { return Texture.Height; }
            }

            public ProjectileType ProjectileType
            {
                get { return projectileType; }
            }

            public Rectangle CollisionBounds
            {
                get
                {
                    return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
                }
            }
            #endregion
            public Projectile(Vector2 position, float angle, ProjectileType projectile, int currentLayer)
            {
                Active = true;
                Position = position;
                Layer = currentLayer;
                 projectileType = projectile;
                rotationAngle = angle;
                
                
                //firePosition is the velocity of the projectile (i.e., the direction it's moving in)
                firePosition.X = (float)Math.Cos(rotationAngle);
                firePosition.Y = (float)Math.Sin(rotationAngle);
                Damage = 5;
                projectileMoveSpeed = 8.0f * MAX_PROJECTILE_SPEED_MULTIPLYER;
                
                //Initialize();
                rotatedRect = new RotatedRectangle(CollisionBounds, rotationAngle);
                //Initialize();

            }

            public bool CollidesWith(Rectangle rectangle, ref Vector2 newPosition)
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
            public void Update(ref Map map)
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

                    this.Position.X += (projectileMoveSpeed * firePosition.X);
                    this.Position.Y += (projectileMoveSpeed * firePosition.Y);

                    rotatedRect.ChangePosition(new Vector2(Position.X - rotatedRect.Width / 2, Position.Y));

                    //first ensure that the projectile is on the map
                   

                    
                }


            }

            public void Draw(SpriteBatch batch)
            {
                if (Active)
                {
                   
                    batch.Draw(Texture, Position, null, Color.White, rotationAngle /*+ (float)(Math.PI * 0.5f)*/,
                    new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);


                    //Primitives2D.DrawLine(batch, rotatedRect.UpperLeftCorner(), rotatedRect.UpperRightCorner(), Color.LightPink);
                    //Primitives2D.DrawLine(batch, rotatedRect.UpperRightCorner(), rotatedRect.LowerRightCorner(), Color.LightPink);
                    //Primitives2D.DrawLine(batch, rotatedRect.LowerRightCorner(), rotatedRect.LowerLeftCorner(), Color.LightPink);
                    //Primitives2D.DrawLine(batch, rotatedRect.LowerLeftCorner(), rotatedRect.UpperLeftCorner(), Color.LightPink);

                    //for (int i = 0; i < tileCollisionChecks.Count; i++)
                    //{


                    //    Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Top), new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Top), Color.Red);
                    //    Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Top), new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Bottom), Color.Red);
                    //    Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Right, tileCollisionChecks[i].Bottom), new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Bottom), Color.Red);
                    //    Primitives2D.DrawLine(batch, new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Bottom), new Vector2(tileCollisionChecks[i].Left, tileCollisionChecks[i].Top), Color.Red);
                    //}
                }
            }
            #endregion
        }

    }