using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Squared.Tiled;
using PolygonIntersection;
using System.IO;
using System.Collections;
using C3.XNA;
using Engine.Extensions;
using Engine.Sprites;

namespace Game
{
    public class GameObject
    {
       
        public float Rotation;
        public Vector2 Orgin;
        public int Layer;
        public int Height;
        public int Width;
        public Polygon Poly;
        public Rectangle Rect;
        private const float SIZE_MOD = 0.4f;
        private List<Projectile> Projectiles = new List<Projectile>();
        public bool isDead { get; set; }
        public AnimatedSpriteFromSpriteSheet _sprite;
        public AnimatedSpriteFromSpriteSheet _spriteDeath;
        public Vector2 Position;
        public Vector2 Direction { get; set; }
        public static Texture2D Texture { get; set; }
        public Vector2 Velocity;
        public RotatedRectangle rotatedRect;
        public List<Rectangle> tileCollisionChecks = new List<Rectangle>();

        public Rectangle CollisionBounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }


        public GameObject()
        {
           
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
        public void Initialize(Vector2 position, float rotation, int layer)
        {

            
            this.Position = position;
            this.Layer = layer;
            this.Height = (int)(Math.Min(Texture.Height,Texture.Width) * SIZE_MOD);
            this.Width = (int)(Math.Min(Texture.Height, Texture.Width) * SIZE_MOD);
            this.Orgin = new Vector2(Texture.Width * 0.26f, Texture.Height / 2);
            this.Rect = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);  
            
            this.rotatedRect = new RotatedRectangle(Rect, rotation);
         
        }


        public void Update(GameTime gameTime)
        {
            //this.Rotate();
            this.rotatedRect.ChangePosition(this.Position - this.Orgin/2);
            this.rotatedRect.Rotation = this.Rotation;
            this.Rect.Location = new Point((int)(this.Position.X - this.Orgin.X/2), (int)(this.Position.Y - this.Orgin.Y/2));    
        }

        public bool willCollideLevel(ref Map map, GameObject obj, Vector2 velocity, bool rotatedCollision)
        {

            

            if (rotatedCollision)
            {

                int lowestPoint = (int)(Math.Min((int)Math.Min(obj.rotatedRect.UpperLeftCorner().Y, obj.rotatedRect.UpperRightCorner().Y), (int)Math.Min(obj.rotatedRect.LowerLeftCorner().Y, obj.rotatedRect.LowerRightCorner().Y)) + velocity.Y - Orgin.Y / 2);
                int highestPoint = (int)(Math.Max((int)Math.Max(obj.rotatedRect.UpperLeftCorner().Y, obj.rotatedRect.UpperRightCorner().Y), (int)Math.Max(obj.rotatedRect.LowerLeftCorner().Y, obj.rotatedRect.LowerRightCorner().Y)) + velocity.Y - Orgin.Y / 2);
                int leftMostPoint = (int)(Math.Min((int)Math.Min(obj.rotatedRect.UpperLeftCorner().X, obj.rotatedRect.UpperRightCorner().X), (int)Math.Min(obj.rotatedRect.LowerLeftCorner().X, obj.rotatedRect.LowerRightCorner().X)) + velocity.X - Orgin.X / 2);
                int rightMostPoint = (int)(Math.Max((int)Math.Max(obj.rotatedRect.UpperLeftCorner().X, obj.rotatedRect.UpperRightCorner().X), (int)Math.Max(obj.rotatedRect.LowerLeftCorner().X, obj.rotatedRect.LowerRightCorner().X)) + velocity.X - Orgin.X / 2);

                
                for (int y = lowestPoint; y < highestPoint; y += map.TileHeight)
                {

                    for (int x = leftMostPoint; x < rightMostPoint; x+= map.TileWidth)
                    {
                        int tileXindex = (int)x / map.TileWidth;
                        int tileYindex = (int)y / map.TileHeight;
                        tileCollisionChecks.Add(new Rectangle(x - x%map.TileWidth, y - y%map.TileHeight, map.TileWidth, map.TileHeight));

                        Tileset.TilePropertyList tileProperties = new Tileset.TilePropertyList();


                        //get tile starting id and then add the tile id for that tile
                        int tile = map.Layers["meta " + obj.Layer].GetTile(tileXindex, tileYindex);
                        tileProperties = map.Tilesets["meta"].GetTileProperties(tile);

                        if (tile > 0)
                        {
                            if (tileProperties.ContainsKey("Collision"))
                            {
                                return true;
                            }
                            //else if (tileProperties.ContainsKey("TransDown"))
                            //{
                            //    obj.Layer--;
                            //}
                            //else if (tileProperties.ContainsKey("TransUp"))
                            //{
                            //    obj.Layer++;
                            //}
                        }
                    }
                }
            }
            else
            {
                Vector2 objAfterMove = obj.Position + velocity - Orgin / 2;
                for (int y = (int)objAfterMove.Y; y <= (objAfterMove.Y + obj.Height); y += map.TileHeight)
                {
                  
                    for (int x = (int)objAfterMove.X; x <= (objAfterMove.X + obj.Width); x += map.TileWidth)
                    {
                        int tileXindex = (int)x / map.TileWidth;
                        int tileYindex = (int)y / map.TileHeight;


                        Tileset.TilePropertyList tileProperties = new Tileset.TilePropertyList();


                        //get tile starting id and then add the tile id for that tile
                        int tile = map.Layers["meta " + obj.Layer].GetTile(tileXindex, tileYindex);
                        tileProperties = map.Tilesets["meta"].GetTileProperties(tile);

                        if (tile > 0)
                        {
                            if (tileProperties.ContainsKey("Collision"))
                            {
                                return true;
                            }
                            else if (tileProperties.ContainsKey("TransDown"))
                            {
                                obj.Layer--;
                            }
                            else if (tileProperties.ContainsKey("TransUp"))
                            {
                                obj.Layer++;
                            }
                        }
                    }
                }
            }
            return false;
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
            
        }

        
        Vector2 rotate_vector(Vector2 orgin, float angle, Vector2 p)
        {
            float s = (float)Math.Sin((double)angle);
            float c = (float)Math.Cos((double)angle);

            // translate point back to origin:
            p.X -= orgin.X;
            p.Y -= orgin.Y;

            // Counterclockwise
            float xnew = p.X * c - p.Y * s;
            float ynew = p.X * s + p.Y * c;
            // Clockwise
            //float xnew = p.X * c + p.Y * s;
            //float ynew = -p.Y * s + p.Y * c;

            // translate point back:
            p.X = xnew + orgin.X;
            p.Y = ynew + orgin.Y;

            return p;
        }

        private void findOrgin()
        {
            float minX = Poly.Points[0].X;
            float minY = Poly.Points[0].Y;
            float maxX = Poly.Points[0].X;
            float maxY = Poly.Points[0].Y;

            for (int i = 1; i < Poly.Points.Count; i++)
            {
                if (Poly.Points[i].X < minX)
                    minX = Poly.Points[i].X;
                else if (Poly.Points[i].X > maxX)
                    maxX = Poly.Points[i].X;

                if (Poly.Points[i].Y < minY)
                    minY = Poly.Points[i].Y;
                else if (Poly.Points[i].Y > maxY)
                    maxY = Poly.Points[i].Y;
            }

            //since we have our min and max x & y values, we can calculate the orgin
            this.Orgin = new Vector2((maxX - minX) / 2, (maxY - minY) / 2);
        }

        
    }
}