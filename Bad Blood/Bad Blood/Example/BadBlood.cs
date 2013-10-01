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
using Game;


namespace Game {
    public class TopDownGame : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Map map;
        Vector2 viewportPosition;
        Rectangle viewportRect;
        SpriteFont gameFont;
        bool collision;
        //Vector2 orgin;
        //float rotation;
        List<Polygon> collisionTiles = new List<Polygon>();
        Player player;
        //Polygon playerPoly = new Polygon();
        // At the top of your class:
        Texture2D pixel;
        //int currentLevel = 1;

        public TopDownGame ()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if WINDOWS || XBOX
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            IsFixedTimeStep = true;
#endif
        }

        protected override void LoadContent () {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load content

            map = Map.Load(Path.Combine(Content.RootDirectory, "map.tmx"), Content);
            for (int i = -1; i < 2; i++)
                map.Layers["meta " + i].Opacity = 0f;
            map.Layers["Props 0"].Opacity = 5f;
            
            Projectile.Texture = Content.Load<Texture2D>(@"Projectile\laser");
            player = new Player(PlayerIndex.One);
            player.LoadContent(Content);
            Player.Texture = Content.Load<Texture2D>(@"Player\Player");
            player.Initialize(new Vector2(map.ObjectGroups["objects"].Objects["waypoint1"].X, map.ObjectGroups["objects"].Objects["waypoint1"].Y), 0f, 1);

            gameFont = Content.Load<SpriteFont>("GameFont");
            // Somewhere in your LoadContent() method:
            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White }); // so that we can draw whatever color we want on top of it
            viewportRect = new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        protected override void UnloadContent () {
        }

        protected override void Update (GameTime gameTime) {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            float scrollx = 0, scrolly = 0;


            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
            {
                scrollx = -1;

            }
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
            {
                scrollx = 1;
            }
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
            {
                scrolly = 1;
            }
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
            {
                scrolly = -1;
            }
            //if (keyState.IsKeyDown(Keys.Space))
            //{

            //    player.Position = new Vector2(map.ObjectGroups["objects"].Objects["waypoint1"].X, map.ObjectGroups["objects"].Objects["waypoint1"].Y);
            //}
            //if (mouseState.LeftButton == ButtonState.Pressed)
            //{
            //    player.Attack(Content);
            //}

            player.HandleInput(gameTime, GraphicsDevice.Viewport, ref map);
           

            base.Update(gameTime);
        }

        //void fixCollision(GameObject obj, Vector2 objVelocity)
        //{
        //    // A this point we've already determined that the boxes intersect...
        //    objVelocity *= -1;
           
        //    for (int i = 0; i< collisionTiles.Count(); i++)
        //    {
        //        Vector2 polygonATranslation = new Vector2();

        //        PolygonCollisionResult r = PolygonCollision(playerPoly, collisionTiles.ElementAt(i), objVelocity);

        //        if (r.WillIntersect)
        //        {
        //            // Move the polygon by its velocity, then move
        //            // the polygons appart using the Minimum Translation Vector
        //            polygonATranslation = objVelocity + r.MinimumTranslationVector;
        //        }
        //        else
        //        {
        //            // Just move the polygon by its velocity
        //            polygonATranslation = objVelocity;
        //        }

        //        playerPoly.Offset(polygonATranslation);

        //        obj.X = (int)playerPoly.Points[0].X;
        //        obj.Y = (int)playerPoly.Points[0].Y;

        //        if (IsColliding(obj) == false)
        //            break;//no more tiles to collide with so break out of collision fix

        //    }
            
        //}

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            spriteBatch.Begin();
            map.Draw(spriteBatch, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), viewportPosition, player);
            spriteBatch.DrawString(gameFont, "Projectiles: " + player.projectiles.Count, new Vector2(GraphicsDevice.Viewport.Width * 0.8f, 10f), Color.White);
            //DrawBorder(player.Rect, 1, Color.LightGreen);
            //spriteBatch.Draw(player.Texture, player.Position, Color.White); 
            //player.Draw(spriteBatch, player.Position, viewportPosition, 100f, player.Rect.Width, player.Rect.Height);
            spriteBatch.End();
        }

        /// <summary>
        /// Will draw a border (hollow rectangle) of the given 'thicknessOfBorder' (in pixels)
        /// of the specified color.
        ///
        /// By Sean Colombo, from http://bluelinegamestudios.com/blog
        /// </summary>
        /// <param name="rectangleToDraw"></param>
        /// <param name="thicknessOfBorder"></param>
        private void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }
        //public bool IsColliding(GameObject obj)
        //{
        //    if (obj.X < 0)
        //        obj.X = 0;
        //    if (obj.Y < 0)
        //        obj.Y = 0;
        //    collision = false;
        //    for (int y = obj.Y; y <= (obj.Y + obj.Height); y += map.TileHeight)
        //    {
        //        //
        //        //TO-DO: Roated Polygons
        //        //
        //        for (int x = obj.X; x <= (obj.X + obj.Width); x += map.TileWidth)
        //        {
        //            int tileXindex = (int)x / map.TileWidth;
        //            int tileYindex = (int)y / map.TileHeight;

        //            if (map.Layers["meta"].GetTile(tileXindex, tileYindex) > 0)
        //            {
        //                //first lets clear the tile list since this is a new collision
        //                if (collision == false)
        //                    collisionTiles.Clear();
        //                collisionTiles.Add(new Polygon());
        //                collisionTiles.Last().Points.Add(new Vector2(tileXindex * map.TileWidth, tileYindex * map.TileHeight));
        //                collisionTiles.Last().Points.Add(new Vector2(tileXindex * map.TileWidth + map.TileWidth, tileYindex * map.TileHeight));
        //                collisionTiles.Last().Points.Add(new Vector2(tileXindex * map.TileHeight + map.TileWidth, tileYindex * map.TileHeight + map.TileHeight));
        //                collisionTiles.Last().Points.Add(new Vector2(tileXindex * map.TileHeight, tileYindex * map.TileHeight + map.TileHeight));
        //                collisionTiles.Last().BuildEdges();
        //                collision = true;
        //            }
        //        }
        //    }
        //    if (collision)
        //        return true;
        //    else
        //        return false;
        //}
       
        // Calculate the projection of a polygon on an axis
// and returns it as a [min, max] interval
public void ProjectPolygon(Vector2 axis, Polygon polygon, 
                           ref float min, ref float max) {
    // To project a point on an axis use the dot product
    float dotProduct = Vector2.Dot(axis,polygon.Points[0]);
    min = dotProduct;
    max = dotProduct;
    for (int i = 0; i < polygon.Points.Count; i++) {
        dotProduct = Vector2.Dot(polygon.Points[i],axis);
        if (dotProduct < min)
        {
            min = dotProduct;
        } else {
            if (dotProduct> max) {
                max = dotProduct;
            }
        }
    }

    
}

        // Structure that stores the results of the PolygonCollision function
public struct PolygonCollisionResult
{
    // Are the polygons going to intersect forward in time?
    public bool WillIntersect;
    // Are the polygons currently intersecting?
    public bool Intersect;
    // The translation to apply to the first polygon to push the polygons apart.
    public Vector2 MinimumTranslationVector;
}


// Calculate the distance between [minA, maxA] and [minB, maxB]
// The distance will be negative if the intervals overlap
public float IntervalDistance(float minA, float maxA, float minB, float maxB) {
    if (minA < minB) {
        return minB - maxA;
    } else {
        return minA - maxB;
    }
}

// Check if polygon A is going to collide with polygon B.
// The last parameter is the *relative* velocity 
// of the polygons (i.e. velocityA - velocityB)
public PolygonCollisionResult PolygonCollision(Polygon polygonA, 
                              Polygon polygonB, Vector2 velocity) {
    PolygonCollisionResult result = new PolygonCollisionResult();
    result.Intersect = true;
    result.WillIntersect = true;

    int edgeCountA = polygonA.Edges.Count;
    int edgeCountB = polygonB.Edges.Count;
    float minIntervalDistance = float.PositiveInfinity;
    Vector2 translationAxis = new Vector2();
    Vector2 edge;

    // Loop through all the edges of both polygons
    for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++) {
        if (edgeIndex < edgeCountA) {
            edge = polygonA.Edges[edgeIndex];
        } else {
            edge = polygonB.Edges[edgeIndex - edgeCountA];
        }

        // ===== 1. Find if the polygons are currently intersecting =====

        // Find the axis perpendicular to the current edge
        Vector2 axis = new Vector2(-edge.Y, edge.X);
        axis.Normalize();

        // Find the projection of the polygon on the current axis
        float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
        ProjectPolygon(axis, polygonA, ref minA, ref maxA);
        ProjectPolygon(axis, polygonB, ref minB, ref maxB);

        // Check if the polygon projections are currentlty intersecting
        if (IntervalDistance(minA, maxA, minB, maxB) > 0)
            result.Intersect = false;

        // ===== 2. Now find if the polygons *will* intersect =====

        // Project the velocity on the current axis
        float velocityProjection = Vector2.Dot(axis, velocity);

        // Get the projection of polygon A during the movement
        if (velocityProjection < 0) {
            minA += velocityProjection;
        } else {
            maxA += velocityProjection;
        }

        // Do the same test as above for the new projection
        float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
        if (intervalDistance > 0) result.WillIntersect = false;

        // If the polygons are not intersecting and won't intersect, exit the loop
        if (!result.Intersect && !result.WillIntersect) break;

        // Check if the current interval distance is the minimum one. If so store
        // the interval distance and the current distance.
        // This will be used to calculate the minimum translation vector
        intervalDistance = Math.Abs(intervalDistance);
        if (intervalDistance < minIntervalDistance) {
            minIntervalDistance = intervalDistance;
            translationAxis = axis;

            Vector2 d = polygonA.Center - polygonB.Center;
            if (Vector2.Dot(d,translationAxis) < 0)
                translationAxis = -translationAxis;
        }
    }

    // The minimum translation vector
    // can be used to push the polygons appart.
    if (result.WillIntersect)
        result.MinimumTranslationVector = 
               translationAxis * minIntervalDistance;
    
    return result;
}
    }
}


