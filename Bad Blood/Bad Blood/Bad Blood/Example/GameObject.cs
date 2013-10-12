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
    public enum ProcessState
    {
        Dead,
        Alive,
        Idle,
        Moving,
        Shooting,
        Meleeing,
        Grappling,
        Injured

    }

    public enum Command
    {
        None,
        Move,
        Melee,
        Shoot,
        Grapple,
        Attacked,
        Killed,
        Disengage
    }

    public class Process
    {
        //public event ChangedEventHandler Changed;

        //// Invoke the Changed event; called whenever list changes
        //protected virtual void OnChanged(EventArgs e)
        //{
        //    if (Changed != null)
        //        Changed(this, e);
        //}

        public override string ToString()
        {
            switch (CurrentState)
            {
                case ProcessState.Dead:
                    return "Dead";
                    break;
                case ProcessState.Alive:
                    return "Alive";
                    break;
                case ProcessState.Idle:
                    return "Idle";
                    break;
                case ProcessState.Moving:
                    return "Moving";
                    break;
                case ProcessState.Shooting:
                    return "Shooting";
                    break;
                case ProcessState.Meleeing:
                    return "Meleeing";
                    break;
                case ProcessState.Grappling:
                    return "Grappling";
                    break;
                case ProcessState.Injured:
                    return "Injured";
                    break;
                default:
                    return "Undefined";
                    break;
            };
        }
        class StateTransition
        {
            readonly ProcessState CurrentState;
            readonly Command Command;

            public StateTransition(ProcessState currentState, Command command)
            {
                CurrentState = currentState;
                Command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                StateTransition other = obj as StateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }
        }

        Dictionary<StateTransition, ProcessState> transitions;
        public ProcessState CurrentState { get; private set; }

        public Process()
        {
            CurrentState = ProcessState.Alive;
            transitions = new Dictionary<StateTransition, ProcessState>
            {
                { new StateTransition(ProcessState.Alive, Command.Move), ProcessState.Moving },
                { new StateTransition(ProcessState.Alive, Command.None), ProcessState.Idle },
                { new StateTransition(ProcessState.Alive, Command.Attacked), ProcessState.Injured },
                { new StateTransition(ProcessState.Alive, Command.Grapple), ProcessState.Grappling },
                { new StateTransition(ProcessState.Injured, Command.Killed), ProcessState.Dead },
                { new StateTransition(ProcessState.Moving, Command.Shoot), ProcessState.Shooting },
                { new StateTransition(ProcessState.Moving, Command.Melee), ProcessState.Meleeing },
                { new StateTransition(ProcessState.Moving, Command.Grapple), ProcessState.Grappling },
                { new StateTransition(ProcessState.Moving, Command.None), ProcessState.Idle},
                { new StateTransition(ProcessState.Moving, Command.Move), ProcessState.Moving},
                { new StateTransition(ProcessState.Idle, Command.Shoot), ProcessState.Shooting },
                { new StateTransition(ProcessState.Idle, Command.Melee), ProcessState.Meleeing },
                { new StateTransition(ProcessState.Idle, Command.Grapple), ProcessState.Grappling },
                { new StateTransition(ProcessState.Idle, Command.Move), ProcessState.Moving},
                 { new StateTransition(ProcessState.Idle, Command.None), ProcessState.Idle},
                 { new StateTransition(ProcessState.Shooting, Command.None), ProcessState.Idle},
                 { new StateTransition(ProcessState.Shooting, Command.Move), ProcessState.Moving},
                 { new StateTransition(ProcessState.Shooting, Command.Melee), ProcessState.Meleeing},
                  { new StateTransition(ProcessState.Shooting, Command.Shoot), ProcessState.Shooting},
                 { new StateTransition(ProcessState.Shooting, Command.Grapple), ProcessState.Grappling},
                { new StateTransition(ProcessState.Meleeing, Command.Move), ProcessState.Meleeing},
                { new StateTransition(ProcessState.Meleeing, Command.Grapple), ProcessState.Grappling},
                { new StateTransition(ProcessState.Meleeing, Command.None), ProcessState.Meleeing},
                { new StateTransition(ProcessState.Meleeing, Command.Disengage), ProcessState.Idle},
                { new StateTransition(ProcessState.Meleeing, Command.Melee), ProcessState.Meleeing},
               // { new StateTransition(ProcessState.Meleeing, Command.Shoot), ProcessState.Shooting},
                 // { new StateTransition(ProcessState.Grappling, Command.Move), ProcessState.Moving},
                //{ new StateTransition(ProcessState.Grappling, Command.Grapple), ProcessState.Grappling},
                { new StateTransition(ProcessState.Grappling, Command.None), ProcessState.Grappling},
                 { new StateTransition(ProcessState.Grappling, Command.Disengage), ProcessState.Idle}
                //{ new StateTransition(ProcessState.Grappling, Command.Shoot), ProcessState.Shooting},

            };
        }

        public ProcessState GetNext(Command command)
        {
            StateTransition transition = new StateTransition(CurrentState, command);
            ProcessState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
            return nextState;
        }

        public ProcessState MoveNext(Command command)
        {
            CurrentState = GetNext(command);
            return CurrentState;
        }
    }
    public class GameObject
    {
        public Process pState { get; set; }
        public float Rotation { get; set; }
        public Vector2 Orgin;
        public int Layer { get; set; }
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
        public bool Active { get; set; }
        public int SizeMod { get; set; }
      
        public int PreviousLevel { get; private set; }

        public virtual Vector2 GunBarrelPosition { get; set; }

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
        

        public virtual bool CollidesWith(Rectangle rectangle, ref Vector2 newPosition)
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
        public virtual void Initialize(Vector2 position, float rotation, int layer)
        {

          
          
         
        }
        public virtual void Rotate() { }
        public virtual void HandleInput(GameTime gameTime,Viewport v, ref Map map)
        { }
        public virtual void Update(GameTime gameTime)
        {
            //this.Rotate();
            rotatedRect.ChangePosition(this.Position);
           rotatedRect.Rotation = this.Rotation;
           Rect.Location = new Point((int)(this.Position.X), (int)(this.Position.Y));
        }

        public virtual void Update(GameTime gameTime, ref Map map)
        {
        }
        public virtual bool willCollideLevel(ref Map map, GameObject obj, Vector2 velocity, bool rotatedCollision)
        {
            tileCollisionChecks.Clear();


            if (rotatedCollision)
            {
                //to prevent the bullet through paper effect, we'll have to subtract our velocity from our initial position and use that for our starting point and loop until we hit our initial point.
                //maybe just do fractions of velocity (i.e., * 0.5
                float i = 0f;
                for (; i < 1; i += 0.1f)
                {
                    Vector2 tempVelocity = velocity * i;
                    int lowestPoint = (int)(Math.Min((int)Math.Min(obj.rotatedRect.UpperLeftCorner().Y - tempVelocity.Y, obj.rotatedRect.UpperRightCorner().Y - tempVelocity.Y), (int)Math.Min(obj.rotatedRect.LowerLeftCorner().Y - tempVelocity.Y, obj.rotatedRect.LowerRightCorner().Y - tempVelocity.Y)));
                    int highestPoint = (int)(Math.Max((int)Math.Max(obj.rotatedRect.UpperLeftCorner().Y - tempVelocity.Y, obj.rotatedRect.UpperRightCorner().Y - tempVelocity.Y), (int)Math.Max(obj.rotatedRect.LowerLeftCorner().Y - tempVelocity.Y, obj.rotatedRect.LowerRightCorner().Y - tempVelocity.Y)));
                    int leftMostPoint = (int)(Math.Min((int)Math.Min(obj.rotatedRect.UpperLeftCorner().X - tempVelocity.X, obj.rotatedRect.UpperRightCorner().X - tempVelocity.X), (int)Math.Min(obj.rotatedRect.LowerLeftCorner().X - tempVelocity.X, obj.rotatedRect.LowerRightCorner().X - tempVelocity.X)));
                    int rightMostPoint = (int)(Math.Max((int)Math.Max(obj.rotatedRect.UpperLeftCorner().X - tempVelocity.X, obj.rotatedRect.UpperRightCorner().X - tempVelocity.X), (int)Math.Max(obj.rotatedRect.LowerLeftCorner().X - tempVelocity.X, obj.rotatedRect.LowerRightCorner().X - tempVelocity.X)));
                    //Vector2 objAfterMove = obj.Position + rotatedRect.Origin + velocity;

                    for (int y = lowestPoint; y < highestPoint + map.TileHeight; y += map.TileHeight)
                    {

                        for (int x = leftMostPoint; x < rightMostPoint + map.TileWidth; x += map.TileWidth)
                        {
                            int tileXindex = (int)x / map.TileWidth;
                            int tileYindex = (int)y / map.TileHeight;
                            if (tileXindex > 0 && tileYindex > 0 && tileXindex < map.Width && tileYindex < map.Height)
                            {
                                Rectangle currentTile = new Rectangle(x - x % map.TileWidth, y - y % map.TileHeight, map.TileWidth, map.TileHeight);
                                //tileCollisionChecks.Add(currentTile);

                                Tileset.TilePropertyList tileProperties = new Tileset.TilePropertyList();

                                tileCollisionChecks.Add(currentTile);
                                //get tile starting id and then add the tile id for that tile
                                int tile = map.Layers["meta " + obj.Layer].GetTile(tileXindex, tileYindex);
                                tileProperties = map.Tilesets["meta"].GetTileProperties(tile);

                                if (tile > 0)
                                {
                                    RotatedRectangle tempRect = obj.rotatedRect;
                                    tempRect.ChangePosition(obj.Position - tempVelocity);
                                    if (tileProperties.ContainsKey("Collision") && tempRect.Intersects(currentTile))
                                    {
                                        return true;
                                    }
                                    else if (tileProperties.ContainsKey("TransDown"))
                                    {
                                        PreviousLevel = obj.Layer;
                                        obj.Layer--;
                                       
                                    }
                                    else if (tileProperties.ContainsKey("TransUp"))
                                    {
                                        PreviousLevel = obj.Layer;
                                        obj.Layer++;
                                        
                                    }
                                   
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Vector2 objAfterMove = obj.Position + velocity; //magic number seems to work perfect...why?
                Rectangle objAfterMoveRect = new Rectangle((int)objAfterMove.X, (int)objAfterMove.Y, obj.Width, obj.Height);

                //now that we have our objects location, we'll have to check all of the tiles that the object is "standing" on and see if they
                //contain the collision meta value or not
                for (int y = (int)objAfterMove.Y; y <= (objAfterMove.Y + obj.Height); y += map.TileHeight)
                {

                    for (int x = (int)objAfterMove.X; x <= (objAfterMove.X + obj.Width * 2); x += map.TileWidth)
                    {
                        //we'll have to check every tile index that is contained within our objects area

                        int tileXindex = (int)x / map.TileWidth;
                        int tileYindex = (int)y / map.TileHeight;


                        //lets first build a rectangle out of the tile so we can check for intersections with our objects rectangle
                        Rectangle currentTile = new Rectangle(x - x % map.TileWidth, y - y % map.TileHeight, map.TileWidth, map.TileHeight);

                        tileCollisionChecks.Add(currentTile);


                        Tileset.TilePropertyList tileProperties = new Tileset.TilePropertyList();


                        //get tile starting id and then add the tile id for that tile
                        int tile = map.Layers["meta " + obj.Layer].GetTile(tileXindex, tileYindex);
                        tileProperties = map.Tilesets["meta"].GetTileProperties(tile);

                        if (tile > 0)
                        {
                            if (tileProperties.ContainsKey("Collision") && objAfterMoveRect.Intersects(currentTile))
                            {
                                return true;
                            }
                        
                            else if (tileProperties.ContainsKey("TransDown"))
                            {
                                PreviousLevel = obj.Layer;
                                obj.Layer--;
                         
                            }
                            else if (tileProperties.ContainsKey("TransUp"))
                            {
                                PreviousLevel = obj.Layer;
                                obj.Layer++;
                              
                            }
                           
                        }
                    }
                }
            }
            return false;
        }
      




        public virtual void Draw(SpriteBatch batch, Vector2 offset, Vector2 viewportPosition, float opacity, int width, int height)
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

        
        public virtual Vector2 rotate_vector(Vector2 orgin, float angle, Vector2 p)
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
        public virtual void Draw(SpriteBatch batch) { }

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