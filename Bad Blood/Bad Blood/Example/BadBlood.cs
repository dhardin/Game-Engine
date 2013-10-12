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
    struct WorldObjectHolder
    {
        public GameObject gameObject;
        public int toLevel, fromLevel;
        public ObjectType objectType;
    }
    public class MultiMap<V>
    {
        
        // 1
        Dictionary<ObjectType, List<V>> _dictionary =
        new Dictionary<ObjectType, List<V>>();

        // 2
        public void Add(ObjectType key, V value)
        {
            List<V> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                // 2A.
                list.Add(value);
            }
            else
            {
                // 2B.
                list = new List<V>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }

        // 3
        public IEnumerable<ObjectType> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }

        // 4
        public List<V> this[ObjectType key]
        {
            get
            {
                List<V> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                    list = new List<V>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }
    }

    public enum ObjectType
    {
        Projectile,
        Enemy,
        Player
    }
    public class TopDownGame : Microsoft.Xna.Framework.Game {
        const int MAX_LAYERS = 4;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Map map;
        Vector2 viewportPosition;
        Rectangle viewportRect;
        SpriteFont gameFont;
        bool collision;
        List<Polygon> collisionTiles = new List<Polygon>();
        List<WorldObjectHolder> WorldObjectManager = new List<WorldObjectHolder>();
        Vector2 offset = new Vector2(0, 0);
      
        Player player;
        Texture2D pixel;
        
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

        public List<MultiMap<GameObject>> WorldObjects= new List<MultiMap<GameObject>>();
       

        protected override void LoadContent () {
            spriteBatch = new SpriteBatch(GraphicsDevice);
         
            //Load content
            map = Map.Load(Path.Combine(Content.RootDirectory, "map.tmx"), Content);
            
            //make meta data transparent
            
            
            Projectile.Texture = Content.Load<Texture2D>(@"Projectile\laser");
           
            
           
            Player.Texture = Content.Load<Texture2D>(@"Player\Player");
            
            
            //now we'll build our list of world objects
            //this list corresponds 1 to 1 with the number of max layers
            for (int i = 0; i < MAX_LAYERS; i++)
                WorldObjects.Add(new MultiMap<GameObject>());

            WorldObjects.ElementAt(2).Add(ObjectType.Player, new Player(PlayerIndex.One, new Vector2(map.ObjectGroups["objects"].Objects["waypoint1"].X, map.ObjectGroups["objects"].Objects["waypoint1"].Y), 0f, 2));
            
            gameFont = Content.Load<SpriteFont>("GameFont");
            // Somewhere in your LoadContent() method:
            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White }); // so that we can draw whatever color we want on top of it
            viewportRect = new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        protected override void UnloadContent () {
        }

        protected override void Update (GameTime gameTime) {
            
            
            for (int i = 0; i < WorldObjects.Count; i++)
            {
                foreach (ObjectType objectType in WorldObjects.ElementAt(i).Keys)
                {
                    foreach (GameObject gameObject in WorldObjects.ElementAt(i)[objectType])
                    {
                        WorldObjectHolder worldObjectHolder = new WorldObjectHolder();
                       
                        switch (objectType)
                        {
                                
                            case ObjectType.Player:
                                gameObject.HandleInput(gameTime, GraphicsDevice.Viewport, ref map, viewportPosition);
                                if (gameObject.Layer != i)
                                {
                                    worldObjectHolder.gameObject = gameObject;
                                    worldObjectHolder.toLevel = gameObject.Layer;
                                    worldObjectHolder.fromLevel = gameObject.PreviousLevel;
                                    worldObjectHolder.objectType = ObjectType.Player;
                                    WorldObjectManager.Add(worldObjectHolder);
                                }
                                if (gameObject.pState.CurrentState.Equals(ProcessState.Shooting))
                                {
                                    //SCREEN SHAKE
                                    Random rand = new Random();
                                    offset.X = rand.Next(-10, 10);
                                    offset.Y = rand.Next(-10, 10);

                                    //in order to place our projectile initially, we want to think of our object as a unit cirle.
                                    //the projectile placement is relative to the player's position, so we'll add the position.
                                    //after, we need to "center" the projectile in the center of the player (i.e., orgin) so we'll add that relative to the
                                    //player's position since the position is always the top left on our character.
                                    //now that we have our projectile centered, we need to position it just at the edge of the gameobject's texture circumference.
                                    //This can be thought of as adding the vector of the direction the gameobject is facing multiplied by the radius (width * 0.5)
                                    //of the gameobject. 
                                    //
                                    //Later on, we'll have to take into consideration different weapon type's physical length so the projectile is placed correctly.
                                    float xPos = gameObject.Position.X + gameObject.GunBarrelPosition.X + (float)((gameObject.Width) * Math.Cos(gameObject.Rotation));
                                    float yPos = gameObject.Position.Y + gameObject.GunBarrelPosition.Y + (float)((gameObject.Width) * Math.Sin(gameObject.Rotation));

                                    worldObjectHolder.gameObject = new Projectile(new Vector2(xPos, yPos), gameObject.Rotation, ProjectileType.Standard, gameObject.Layer);
                                    worldObjectHolder.fromLevel = gameObject.Layer;
                                    worldObjectHolder.toLevel = gameObject.Layer;
                                    worldObjectHolder.objectType = ObjectType.Projectile;
                                    WorldObjectManager.Add(worldObjectHolder);
                                }
                                else
                                {
                                    offset.X = 0; 
                                    offset.Y = 0;
                                }
                                 

                                //now we will need to update our drawing offset for the world. In order to do this, we must do the following...
                                //If player's position is greater than the 1/2 of the viewport width and the player's position is less than the
                                //(map width * tile width - viewport width / 2)
                                //      offset += player's position - viewport width / 2
                                //we'll do the same for the height as well
                                if (gameObject.Position.X >= (viewportRect.Width * 0.5f) && gameObject.Position.X <= (map.Width * map.TileWidth - viewportRect.Width * 0.5f))
                                {
                                    viewportPosition.X = gameObject.Position.X - viewportRect.Width * 0.5f;

                                }
                                //else if (gameObject.Position.X > (viewportRect.Width * 0.5f) && gameObject.Position.X > (map.Width * map.TileWidth))
                                //{
                                //    gameObject.Offset.X = viewportPosition.X;
                                //    //viewportPosition.X = viewportRect.Width - gameObject.Position.X;
                                //}
                                if (gameObject.Position.Y >= (viewportRect.Height * 0.5f) && gameObject.Position.Y <= (map.Height * map.TileHeight - viewportRect.Height * 0.5f))
                                {
                                    viewportPosition.Y = gameObject.Position.Y - viewportRect.Height * 0.5f;
                                }
                                //else if (gameObject.Position.Y > (viewportRect.Height * 0.5f) && gameObject.Position.Y > (map.Height * map.TileHeight))
                                //{
                                //    gameObject.Offset.Y = viewportPosition.Y;
                                //}

                                break;
                            case ObjectType.Projectile:
                                if (gameObject.Layer != i)
                                {
                                    worldObjectHolder.gameObject = gameObject;
                                    worldObjectHolder.toLevel = gameObject.Layer;
                                    worldObjectHolder.fromLevel = gameObject.PreviousLevel;
                                    worldObjectHolder.objectType = ObjectType.Projectile;
                                    WorldObjectManager.Add(worldObjectHolder);
                                }
                                if (!gameObject.Active)
                                {
                                    worldObjectHolder.gameObject = gameObject;//new Projectile(gameObject.Position, gameObject.rotatedRect.Rotation, ProjectileType.Standard, gameObject.Layer);
                                    worldObjectHolder.fromLevel = gameObject.Layer;
                                    worldObjectHolder.toLevel = gameObject.Layer;
                                    worldObjectHolder.objectType = ObjectType.Projectile;
                                    WorldObjectManager.Add(worldObjectHolder);
                                }
                                else
                                    gameObject.Update(gameTime, ref map, viewportPosition);
                                break;
                            default:
                                //gameObject.Update(gameTime);
                                break;
                        };

                    }
                }
            }
            
            foreach (WorldObjectHolder worldObjectHolder in WorldObjectManager)
            {
                    WorldObjects[worldObjectHolder.toLevel].Add(worldObjectHolder.objectType, worldObjectHolder.gameObject);
                    WorldObjects[worldObjectHolder.fromLevel][worldObjectHolder.objectType].Remove(worldObjectHolder.gameObject);
                switch (worldObjectHolder.objectType)
                {
                    case ObjectType.Player:
                       
                        
                        break;
                    case ObjectType.Projectile:
                       if (worldObjectHolder.gameObject.Active)
                            WorldObjects[worldObjectHolder.toLevel].Add(worldObjectHolder.objectType, worldObjectHolder.gameObject);
                        else
                            WorldObjects[worldObjectHolder.toLevel][worldObjectHolder.objectType].Remove(worldObjectHolder.gameObject);
                        break;
                    default:
                        break;
                }
            }

            WorldObjectManager.Clear();

            //after our player updates, let's check their state to make any world changes
            
           

            base.Update(gameTime);
        }

 

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            spriteBatch.Begin();
            map.Draw(spriteBatch, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), viewportPosition + offset, WorldObjects);

            //count the total projectiles in the world
            int numProjectiles = 0;
            for(int i = 0; i < WorldObjects.Count; i++)
                numProjectiles += WorldObjects[i][ObjectType.Projectile].Count;
            spriteBatch.DrawString(gameFont, "Projectiles: " + numProjectiles, new Vector2(GraphicsDevice.Viewport.Width * 0.8f, 10f), Color.White);
            //spriteBatch.DrawString(gameFont, "State: " + player.pState.ToString(), new Vector2(GraphicsDevice.Viewport.Width * 0.8f, 25f), Color.White);
            //if (player.pState.ToString().Equals("Grappling"))
            //    spriteBatch.DrawString(gameFont, "Grapple Time: " + (gameTime.TotalGameTime - player.previousGrappleTime), new Vector2(GraphicsDevice.Viewport.Width * 0.7f, 40f), Color.White);
            //if (player.pState.ToString().Equals("Meleeing"))
            //    spriteBatch.DrawString(gameFont, "MeleeTime Time: " + (gameTime.TotalGameTime - player.previousMeleeTime), new Vector2(GraphicsDevice.Viewport.Width * 0.7f, 55f), Color.White);
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


