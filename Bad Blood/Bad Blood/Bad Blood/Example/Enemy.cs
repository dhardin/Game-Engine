//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Media;
//using Engine.Sprites;
//using Engine.Extensions;
//using Microsoft.Xna.Framework.Content;
//using System.Xml;
//using Engine.GameStateManagerment;
//using Microsoft.Xna.Framework.Audio;

//namespace Game
//{
//    enum AttackType
//    {
//        Basic_Range,
//        Heavy_Range,
//        Burst_Range,
//        Spread_Range,
//        Disrupt_Range,
//        Basic_Chase,
//        Advanced_Chase,
//        Fast_Chase,
//        Split_Chase,
//        Heavy_Chase,
//        Shrink_Chase
//    }
//    enum LevelType
//    {
//        Temperate,
//        Sky,
//        Tropical,
//        Arctic,
//        Volcanic,
//        Space,
//        Ocean,
//        Robot
//    }
//    class Enemy : GameObject
//    {
//        enum AI_State
//        {
//            Stationary,
//            Chasing,
//            Caught,
//            Evade_Player,
//            Evade_Projectile,
//            Evade_All,
//            Wander,
//        }
//        #region Fields
//        AnimatedSpriteFromSpriteSheet _sprite;
//        AnimatedSpriteFromSpriteSheet _spriteBlood;
//        public RotatedRectangle rotatedCollisionBox;
//        public RotatedRectangle rotatedEvadeBounds;

//        public List<Projectile> projectiles = new List<Projectile>();

//        string enemyName, levelName, firstFrame;

//        const string PATH = "Enemies\\Level Type\\";

//        const int NUM_FRAMES = 1;

//        float speed = 5;
//        const float chaseDistance = 250.0f;
//        float caughtDistance = 10.0f;
//        float evadeDistance = 5.0f;
//        const float turnSpeed = 0.10f;
//        // this constant is used to avoid hysteresis, which is common in ai programming.
//        const float hysteresis = 15.0f;
//        const int MAX_BURST_PROJECTILES = 3;
//        private int currentBurstProjectiles = 0;
//        private const int MAX_FIRE_RATE_MODIFIER = 4;
//        private int health;
//        bool isActive = false;
//        bool isAlive = false;
//        public bool isHit = false;
//        float orientation;
//        bool isEvading;
//        private int pointValue;
//        PlayerIndex playerIndex;

//        SoundEffect hitSoundEffect;
//        bool hitSoundPlayed = false;

//        Vector2 enemyWanderDirection;
//        public Vector2 EvadeProjectilePos;
//        AI_State currentState;
//        public AttackType attackType;
//        public List<AttackType> chaseTypes = new List<AttackType>();
//        public List<AttackType> rangeTypes = new List<AttackType>();

//        double maxFireCooldown = 2.0;
//        const double MAX_BURST_FIRE_COOLDOWN = 0.3;
//        const double HIT_COOLDOWN = 0.2;

//        public TimeSpan FireCooldown;
//        public TimeSpan BurstCooldown = TimeSpan.FromSeconds(MAX_BURST_FIRE_COOLDOWN);
//        private TimeSpan HitCooldown = TimeSpan.FromSeconds(HIT_COOLDOWN);

//        #endregion

//        #region Properties
//        public float Speed
//        {
//            get { return speed; }

//        }
//        public override Rectangle CollisionBounds
//        {
//            get
//            {
//                if (!attackType.Equals(AttackType.Heavy_Chase))
//                    return new Rectangle((int)(Position.X - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 0.5), (int)(Position.Y - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height), _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width, _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height);
//                else
//                    return new Rectangle((int)(Position.X - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 0.5 * 2), (int)(Position.Y - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height) * 2, _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 2, _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height * 2);
//            }
//        }

//        public bool IsEvading
//        {
//            get { return isEvading; }
//            set { isEvading = value; }
//        }

//        public bool IsAlive
//        {
//            get { return isAlive; }
//            set { isAlive = value; }
//        }
//        public bool IsActive
//        {
//            get { return isActive; }
//            set { isActive = value; }
//        }

//        public int Health
//        {
//            get { return health; }
//            set { health = value; }
//        }

//        public int PointValue
//        {
//            get { return pointValue; }
//        }

//        public PlayerIndex KilledByPlayer
//        {
//            get { return playerIndex; }
//        }
//        #endregion

//        public Enemy(AttackType attack_Type, LevelType levelType, Vector2 pos, float health_factor)
//        {
//            chaseTypes.Add(AttackType.Advanced_Chase);
//            chaseTypes.Add(AttackType.Basic_Chase);
//            chaseTypes.Add(AttackType.Fast_Chase);
//            chaseTypes.Add(AttackType.Heavy_Chase);
//            chaseTypes.Add(AttackType.Shrink_Chase);
//            chaseTypes.Add(AttackType.Split_Chase);

//            rangeTypes.Add(AttackType.Basic_Range);
//            rangeTypes.Add(AttackType.Burst_Range);
//            rangeTypes.Add(AttackType.Disrupt_Range);
//            rangeTypes.Add(AttackType.Heavy_Range);
//            rangeTypes.Add(AttackType.Spread_Range);
//            Vector2 temp;
            
//            attackType = attack_Type;
//            Position = pos;

            
//            orientation = 0.0f;

//            switch (attack_Type)
//            {
//                case (AttackType.Basic_Range):
//                    enemyName = "Basic_Range";
//                    currentState = AI_State.Stationary;
//                    firstFrame = "Ranged1";
//                    maxFireCooldown = 2.0;
//                    health = (int)Math.Floor((5 + (1 * health_factor)));
//                    pointValue = 5;
//                    break;
//                case (AttackType.Burst_Range):
//                    enemyName = "Burst_Range";
//                    currentState = AI_State.Stationary;
//                    firstFrame = "Ranged2";
//                    maxFireCooldown = 1.0;
//                    health = (int)Math.Floor((5 + (1 * health_factor)));
//                    pointValue = 10;
//                    break;
//                case (AttackType.Spread_Range):
//                    enemyName = "Spread_Range";
//                    currentState = AI_State.Stationary;
//                    firstFrame = "Ranged1a";
//                    maxFireCooldown = 2.5;
//                    health = (int)Math.Floor((5 + (1 * health_factor)));
//                    pointValue = 10;
//                    break;
//                case (AttackType.Disrupt_Range):
//                    enemyName = "Disrupt_Range";
//                    currentState = AI_State.Stationary;
//                    firstFrame = "Ranged2";
//                    maxFireCooldown = 2.3;
//                    pointValue = 15;
//                    health = (int)Math.Floor((5 + (1 * health_factor)));
//                    break;
//                case (AttackType.Heavy_Range):
//                    enemyName = "Heavy_Range";
//                    firstFrame = "Ranged3";
//                    currentState = AI_State.Stationary;
//                    maxFireCooldown = 4.0;
//                    pointValue = 20;
//                    health = (int)Math.Floor((60 + (1 * health_factor)));
//                    break;
//                case (AttackType.Basic_Chase):
//                    enemyName = "Basic_Chase";
//                    currentState = AI_State.Chasing;
//                    firstFrame = "Chase1";
//                    pointValue = 5;
//                    health = (int)Math.Floor((1 + (1 * health_factor)));
//                    break;
//                case (AttackType.Advanced_Chase):
//                    enemyName = "Advanced_Chase";
//                    currentState = AI_State.Chasing;
//                    firstFrame = "Chase2";
//                    pointValue = 5;
//                    health = (int)Math.Floor((1 + (1 * health_factor)));
//                    break;
//                case (AttackType.Fast_Chase):
//                    enemyName = "Fast_Chase";
//                    currentState = AI_State.Chasing;
//                    firstFrame = "Chase3";
//                    pointValue = 10;
//                    health = (int)Math.Floor((1 + (1 * health_factor)));
//                    speed *= 2;
//                    break;
//                case (AttackType.Heavy_Chase):
//                    enemyName = "Heavy_Chase";
//                    currentState = AI_State.Chasing;
//                    firstFrame = "Chase3";
//                    pointValue = 15;
//                    health = (int)Math.Floor((80 + (1 * health_factor)));
//                    speed *= 0.5f;
//                    break;
//                case (AttackType.Shrink_Chase):
//                    enemyName = "Shrink_Chase";
//                    currentState = AI_State.Chasing;
//                    firstFrame = "Chase3";
//                    pointValue = 15;
//                    health = (int)Math.Floor((5 + (1 * health_factor)));
//                    break;
//                case (AttackType.Split_Chase):
//                    enemyName = "Split_Chase";
//                    currentState = AI_State.Chasing;
//                    firstFrame = "Chase3";
//                    pointValue = 20;
//                    health = (int)Math.Floor((5 + (1 * health_factor)));
//                    break;
//                default:
//                    throw new ArgumentException("Enemy type does not exist");               
//            };

//            switch (levelType)
//            {
//                case (LevelType.Arctic):
//                    levelName = "Arctic";
//                    break;
//                case (LevelType.Ocean):
//                    levelName = "Ocean";
//                    break;
//                case (LevelType.Robot):
//                    levelName = "Robot";
//                    break;
//                case (LevelType.Sky):
//                    levelName = "Sky";
//                    break;
//                case (LevelType.Space):
//                    levelName = "Space";
//                    break;
//                case (LevelType.Temperate):
//                    levelName = "Temperate";
//                    break;
//                case (LevelType.Tropical):
//                    levelName = "Tropical";
//                    break;
//                case (LevelType.Volcanic):
//                    levelName = "Volcanic";
//                    break;
//                default:
//                    throw new ArgumentException("Level type does not exist");
//            };

//            FireCooldown = TimeSpan.FromSeconds(maxFireCooldown);

//        }

//        public void LoadContent(ContentManager _content)
//        {
//            string temp = PATH + levelName;
//            //Modify the xml file to point to the right texture files.
//            //This will occur on every new isntance of enemy since the XML file is only used initially to create
//            //the spritesheet, which is then used continuously.
//            //LinkToXML(temp);
            
//            var spriteSheet = _content.Load<SpriteSheet>( temp + "\\Enemy");
//            var spriteBloodSheet = _content.Load<SpriteSheet>("Blood\\Blood");

//            _sprite = new AnimatedSpriteFromSpriteSheet(spriteSheet, firstFrame, 2, NUM_FRAMES);
//            _spriteBlood = new AnimatedSpriteFromSpriteSheet(spriteBloodSheet, "B2", 8, 8);
//            _spriteBlood.IsLooping = false;

//            rotatedCollisionBox = new RotatedRectangle(CollisionBounds, orientation);

//            Rectangle tempRect = new Rectangle((int)CollisionBounds.X - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 4, (int)CollisionBounds.Y - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height * 4, (int)_sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 4, (int)_sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height * 4);
//            rotatedEvadeBounds = new RotatedRectangle(tempRect, orientation);

//            hitSoundEffect = _content.Load<SoundEffect>(@"Sounds\hit");

//            isAlive = true;
//            isActive = true;
//        }

//        public void Update(GameTime gameTime, Vector2 targetVector)
//        {
//            if (isAlive && isActive)
//            {
//                // First we have to use the current state to decide what the thresholds are
//                // for changing state
//                float enemyChaseThreshold = chaseDistance;
//                float enemyCaughtThreshold = caughtDistance;
//                float enemyEvadeThreshold = evadeDistance;
//                // if the enemy is idle, he prefers to stay idle. we do this by making the
//                // chase distance smaller, so the enemy will be less likely to begin chasing
//                // the target.
//                if (currentState == AI_State.Wander)
//                {
//                    enemyChaseThreshold -= hysteresis * 0.5f;
//                }
//                // similarly, if the enemy is active, he prefers to stay active. we
//                // accomplish this by increasing the range of values that will cause the
//                // enemy to go into the active state.
//                else if (currentState == AI_State.Chasing)
//                {
//                    enemyChaseThreshold += hysteresis * 0.5f;
//                    enemyCaughtThreshold -= hysteresis * 0.5f;
//                }
//                // the same logic is applied to the finished state.
//                else if (currentState == AI_State.Caught)
//                {
//                    enemyCaughtThreshold += hysteresis * 0.5f;
//                }
//                else if (currentState == AI_State.Evade_Projectile)
//                {
//                    enemyEvadeThreshold += hysteresis * 0.5f;
//                }

//                // Second, now that we know what the thresholds are, we compare the enemy's 
//                // distance from the target against the thresholds to decide what the enemy's
//                // current state is.
//                // Stationary,
//                //Chasing,
//                //Caught,
//                //Evade_Player,
//                //Evade_Projectile,
//                //Evade_All,
//                //Wander,
//                float distanceFromTarget = Vector2.Distance(Position, targetVector);
//                if (!currentState.Equals(AI_State.Evade_Projectile) && !isEvading)
//                {
//                    //if (distanceFromTarget > enemyChaseThreshold && chaseTypes.Contains(attackType))
//                    //{
//                    //    // if the enemy is far away from the target, it should
//                    //    // idle.
//                    //    currentState = AI_State.Wander;
//                    //}
//                     if (distanceFromTarget > enemyCaughtThreshold && chaseTypes.Contains(attackType))
//                    {
//                        currentState = AI_State.Chasing;
//                    }
//                    else if (distanceFromTarget <= enemyCaughtThreshold && chaseTypes.Contains(attackType))
//                    {
//                        currentState = AI_State.Caught;
//                    }
//                    // the cat is too close; the mouse should run:
//                    //else if (distanceFromTarget < enemyEvadeThreshold - hysteresis && attackType.Equals(AttackType.Advanced_Chase))
//                    //{
//                    //    currentState = AI_State.Evade_Projectile;
//                    //}
//                    else
//                    {
//                        currentState = AI_State.Stationary;
//                    }
//                }
//                // Third, once we know what state we're in, act on that state.
//                float currentEnemySpeed;
//                if (currentState == AI_State.Evade_Projectile)
//                {
//                    // If the mouse is "active," it is trying to evade the cat. The evasion
//                    // behavior is accomplished by using the TurnToFace function to turn
//                    // towards a point on a straight line facing away from the cat. In other
//                    // words, if the cat is point A, and the mouse is point B, the "seek
//                    // point" is C.
//                    //     C
//                    //   B
//                    // A

//                    Vector2 seekPosition = 2 * Position - EvadeProjectilePos;

//                    // Use the TurnToFace function, which we introduced in the AI Series 1:
//                    // Aiming sample, to turn the mouse towards the seekPosition. Now when
//                    // the mouse moves forward, it'll be trying to move in a straight line
//                    // away from the cat.
//                    orientation = TurnToFace(Position, seekPosition,
//                        orientation, turnSpeed);
//                    currentEnemySpeed = speed * 2;
//                }
//                else if (currentState == AI_State.Chasing)
//                {
//                    // the enemy wants to chase the target, so it will just use the TurnToFace
//                    // function to turn towards the cat's position. Then, when the enemy
//                    // moves forward, he will chase the target.
//                    orientation = TurnToFace(Position, targetVector, orientation,
//                        turnSpeed);
//                    currentEnemySpeed = .5f * speed;
//                }
//                else if (currentState == AI_State.Wander)
//                {
//                    //wander randomly
//                    Wander(Position, ref enemyWanderDirection, ref orientation,
//                        turnSpeed);
//                    currentEnemySpeed = .25f * speed;
//                }
//                else if (currentState == AI_State.Caught)
//                {
//                    // if the enemy catches the target, it should stop. otherwise it will run right by, then spin around and
//                    // try to catch it all over again. The end result is that it will kind
//                    // of "run laps" around the target, which looks funny, but is not what
//                    // we're after.
//                    currentEnemySpeed = 0.0f;
//                }
               
//                else
//                {
//                    orientation = TurnToFace(Position, targetVector, orientation,
//                        turnSpeed);
//                    currentEnemySpeed = 0.0f;
//                }

//                //if ranged enemy, update fire cooldown and fire if cooldown timer has run out
//                if (rangeTypes.Contains(attackType))
//                {
//                    if (attackType.Equals(AttackType.Basic_Range) || attackType.Equals(AttackType.Heavy_Range) || attackType.Equals(AttackType.Spread_Range) || attackType.Equals(AttackType.Disrupt_Range))
//                        {
//                        if (FireCooldown.TotalSeconds > 0)
//                        {
//                            FireCooldown -= gameTime.ElapsedGameTime;
//                            if (FireCooldown.TotalSeconds <= 0)
//                            {
//                                AddProjectile(Position, orientation);
//                                FireCooldown = TimeSpan.FromSeconds(maxFireCooldown * MAX_FIRE_RATE_MODIFIER);
//                            }
//                        }
//                    }
//                    else if (attackType.Equals(AttackType.Spread_Range))
//                    {
//                        if (FireCooldown.TotalSeconds > 0)
//                        {
//                            FireCooldown -= gameTime.ElapsedGameTime;
//                            if (FireCooldown.TotalSeconds <= 0)
//                            {
//                                AddProjectile(Position, orientation);
//                                AddProjectile(Position, (float)(orientation - Math.PI / 4));
//                                AddProjectile(Position, (float)(orientation + Math.PI / 4));
//                                FireCooldown = TimeSpan.FromSeconds(maxFireCooldown * MAX_FIRE_RATE_MODIFIER);
//                            }
//                        }
//                    }
//                    else if (attackType.Equals(AttackType.Burst_Range))
//                    {
//                        if (BurstCooldown.TotalSeconds > 0 && currentBurstProjectiles < MAX_BURST_PROJECTILES)
//                        {
                            
//                            BurstCooldown -= gameTime.ElapsedGameTime;
//                            if (BurstCooldown.TotalSeconds <= 0)
//                            {
//                                currentBurstProjectiles++;
//                                AddProjectile(Position, orientation);
//                                BurstCooldown = TimeSpan.FromSeconds(MAX_BURST_FIRE_COOLDOWN);
//                            }
//                        }
//                        else if (currentBurstProjectiles.Equals(MAX_BURST_PROJECTILES))
//                        {
//                            FireCooldown -= gameTime.ElapsedGameTime;
//                            if (FireCooldown.TotalSeconds <= 0)
//                            {
//                                FireCooldown = TimeSpan.FromSeconds(maxFireCooldown * MAX_FIRE_RATE_MODIFIER);
//                                currentBurstProjectiles = 0;
//                            }
//                        }
//                    }
//                }


//                // we construct a heading vector based on the enemy's orientation, and then make the enemy move along
//                // that heading.
//                Vector2 heading = new Vector2(
//                    (float)Math.Cos(orientation), (float)Math.Sin(orientation));
//                Position += heading * currentEnemySpeed;

//                rotatedCollisionBox.ChangePosition(new Vector2(Position.X - rotatedCollisionBox.Width / 2, Position.Y - rotatedCollisionBox.Height / 2));
//                rotatedEvadeBounds.ChangePosition(new Vector2(Position.X - rotatedEvadeBounds.Width / 2, Position.Y - rotatedEvadeBounds.Height / 2));
//                rotatedCollisionBox.Rotation = orientation;
//                rotatedEvadeBounds.Rotation = orientation;
                
//                if (HitCooldown.TotalSeconds > 0)
//                {
//                    HitCooldown -= gameTime.ElapsedGameTime;
//                }
//                else
//                {
//                    HitCooldown = TimeSpan.FromSeconds(HIT_COOLDOWN);
//                    isHit = false;
//                    hitSoundPlayed = false;
//                }

//                if (isHit && !hitSoundPlayed)
//                {
//                    hitSoundEffect.Play();
//                }

                

//                // Update Animation
//                _sprite.Update(gameTime);
//            }

//            else if (isActive)
//            {
//                _spriteBlood.Update(gameTime);
//                if (_spriteBlood.IsDone)
//                    isActive = false;

//            }

//            if (health <= 0)
//                isAlive = false;

//            for (int i = 0; i < projectiles.Count; i++)
//                projectiles[i].Update();
//        }

//        public void ChangeEvadeState(bool evade)
//        {
//            if (!evade)
//                currentState = AI_State.Chasing;
//            else
//                currentState = AI_State.Evade_Projectile;
//        }

//        /// <summary>
//        /// Wander contains functionality that is shared between both the mouse and the
//        /// tank, and does just what its name implies: makes them wander around the
//        /// screen. The specifics of the function are described in more detail in the
//        /// accompanying doc.
//        /// </summary>
//        /// <param name="position">the position of the character that is wandering
//        /// </param>
//        /// <param name="wanderDirection">the direction that the character is currently
//        /// wandering. this parameter is passed by reference because it is an input and
//        /// output parameter: Wander accepts it as input, and will update it as well.
//        /// </param>
//        /// <param name="orientation">the character's orientation. this parameter is
//        /// also passed by reference and is an input/output parameter.</param>
//        /// <param name="turnSpeed">the character's maximum turning speed.</param>
//        private void Wander(Vector2 position, ref Vector2 wanderDirection,
//            ref float orientation, float turnSpeed)
//        {
//            // The wander effect is accomplished by having the character aim in a random
//            // direction. Every frame, this random direction is slightly modified.
//            // Finally, to keep the characters on the center of the screen, we have them
//            // turn to face the screen center. The further they are from the screen
//            // center, the more they will aim back towards it.

//            // the first step of the wander behavior is to use the random number
//            // generator to offset the current wanderDirection by some random amount.
//            // .25 is a bit of a magic number, but it controls how erratic the wander
//            // behavior is. Larger numbers will make the characters "wobble" more,
//            // smaller numbers will make them more stable. we want just enough
//            // wobbliness to be interesting without looking odd.
//            Random random;

//            // Initialize our random number generator
//            random = new Random();

//            wanderDirection.X +=
//                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());
//            wanderDirection.Y +=
//                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());

//            // we'll renormalize the wander direction, ...
//            if (wanderDirection != Vector2.Zero)
//            {
//                wanderDirection.Normalize();
//            }
//            // ... and then turn to face in the wander direction. We don't turn at the
//            // maximum turning speed, but at 15% of it. Again, this is a bit of a magic
//            // number: it works well for this sample, but feel free to tweak it.
//            orientation = TurnToFace(position, position + wanderDirection, orientation,
//                .15f * turnSpeed);

//            // next, we'll turn the characters back towards the center of the screen, to
//            // prevent them from getting stuck on the edges of the screen.
            
//            Vector2 screenCenter = Vector2.Zero;
//            screenCenter.X = ScreenManager.Instance.GraphicsDevice.Viewport.Width *0.5f;
//            screenCenter.Y = ScreenManager.Instance.GraphicsDevice.Viewport.Height * 0.5f;

//            // Here we are creating a curve that we can apply to the turnSpeed. This
//            // curve will make it so that if we are close to the center of the screen,
//            // we won't turn very much. However, the further we are from the screen
//            // center, the more we turn. At most, we will turn at 30% of our maximum
//            // turn speed. This too is a "magic number" which works well for the sample.
//            // Feel free to play around with this one as well: smaller values will make
//            // the characters explore further away from the center, but they may get
//            // stuck on the walls. Larger numbers will hold the characters to center of
//            // the screen. If the number is too large, the characters may end up
//            // "orbiting" the center.
//            float distanceFromScreenCenter = Vector2.Distance(screenCenter, position);
//            float MaxDistanceFromScreenCenter =
//                Math.Min(screenCenter.Y, screenCenter.X);

//            float normalizedDistance =
//                distanceFromScreenCenter / MaxDistanceFromScreenCenter;

//            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance *
//                turnSpeed;

//            // once we've calculated how much we want to turn towards the center, we can
//            // use the TurnToFace function to actually do the work.
//            orientation = TurnToFace(position, screenCenter, orientation,
//                turnToCenterSpeed);
//        }

//        /// <summary>
//        /// Calculates the angle that an object should face, given its position, its
//        /// target's position, its current angle, and its maximum turning speed.
//        /// </summary>
//        private static float TurnToFace(Vector2 position, Vector2 faceThis,
//            float currentAngle, float turnSpeed)
//        {
//            // consider this diagram:
//            //         B 
//            //        /|
//            //      /  |
//            //    /    | y
//            //  / o    |
//            // A--------
//            //     x
//            // 
//            // where A is the position of the object, B is the position of the target,
//            // and "o" is the angle that the object should be facing in order to 
//            // point at the target. we need to know what o is. using trig, we know that
//            //      tan(theta)       = opposite / adjacent
//            //      tan(o)           = y / x
//            // if we take the arctan of both sides of this equation...
//            //      arctan( tan(o) ) = arctan( y / x )
//            //      o                = arctan( y / x )
//            // so, we can use x and y to find o, our "desiredAngle."
//            // x and y are just the differences in position between the two objects.
//            float x = faceThis.X - position.X;
//            float y = faceThis.Y - position.Y;

//            // we'll use the Atan2 function. Atan will calculates the arc tangent of 
//            // y / x for us, and has the added benefit that it will use the signs of x
//            // and y to determine what cartesian quadrant to put the result in.
//            // http://msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
//            float desiredAngle = (float)Math.Atan2(y, x);

//            // so now we know where we WANT to be facing, and where we ARE facing...
//            // if we weren't constrained by turnSpeed, this would be easy: we'd just 
//            // return desiredAngle.
//            // instead, we have to calculate how much we WANT to turn, and then make
//            // sure that's not more than turnSpeed.

//            // first, figure out how much we want to turn, using WrapAngle to get our
//            // result from -Pi to Pi ( -180 degrees to 180 degrees )
//            float difference = WrapAngle(desiredAngle - currentAngle);

//            // clamp that between -turnSpeed and turnSpeed.
//            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

//            // so, the closest we can get to our target is currentAngle + difference.
//            // return that, using WrapAngle again.
//            return WrapAngle(currentAngle + difference);
//        }
     
//        /// <summary>
//        /// Returns the angle expressed in radians between -Pi and Pi.
//        /// <param name="radians">the angle to wrap, in radians.</param>
//        /// <returns>the input value expressed in radians from -Pi to Pi.</returns>
//        /// </summary>
//        private static float WrapAngle(float radians)
//        {
//            while (radians < -MathHelper.Pi)
//            {
//                radians += MathHelper.TwoPi;
//            }
//            while (radians > MathHelper.Pi)
//            {
//                radians -= MathHelper.TwoPi;
//            }
//            return radians;
//        }

//        /// <summary>
//        /// Add projectile fired by the player to the game
//        /// </summary>
//        /// <param name="position"></param>
//        /// <param name="fireDirection"></param>
//        private void AddProjectile(Vector2 position, float fireDirection)
//        {
//            Projectile projectile = new Projectile(fireDirection, ProjectileType.Orb);
//            projectile.Initialize(position, ProjectileType.Orb);
//            projectiles.Add(projectile);
//        }

//        public void KilledBy(PlayerIndex pIndex)
//        {
//            playerIndex = pIndex;
//        }

//        /// <summary>
//        /// Returns the closest of the two passed vectors
//        /// </summary>
//        /// <param name="target1"></param>
//        /// <param name="target2"></param>
//        /// <returns></returns>
//        public Vector2 ClosestTarget(Vector2 target1, Vector2 target2)
//        {
//            Vector2 differeceTarget1 = target1 - Position;
//            Vector2 differenceTarget2 = target2 - Position;

//            if (differeceTarget1.Length() < differenceTarget2.Length())
//                return target1;
//            else
//                return target2;
//        }

//        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
//        {
//            _sprite.Update(gameTime);
//        }

//        public override void Draw(DrawContext ctx)
//        {

//            if (isAlive && isActive)
//            {
//                if (!isHit && !attackType.Equals(AttackType.Heavy_Chase))
//                    _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.White, AnchorType.Center, SpriteEffects.None, orientation);
//                else if (!isHit)
//                    _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.White, AnchorType.Center, SpriteEffects.None, orientation, 2.0f);
//                else if (isHit && !attackType.Equals(AttackType.Heavy_Chase))
//                    _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.Red, AnchorType.Center, SpriteEffects.None, orientation);
//                else
//                    _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.Red, AnchorType.Center, SpriteEffects.None, orientation, 2.0f);
//            }
//            else if (!isAlive && isActive)
//                _spriteBlood.Draw(ctx.Batch, Position + ctx.Offset, Color.White, AnchorType.Center, SpriteEffects.None, orientation );


//            for (int i = 0; i < projectiles.Count; i++)
//                projectiles[i].Draw(ctx);
//        }
//    }
//}
