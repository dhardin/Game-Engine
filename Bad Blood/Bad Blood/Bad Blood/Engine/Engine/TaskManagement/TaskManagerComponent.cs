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


namespace Engine.TaskManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TaskManagerComponent : Microsoft.Xna.Framework.GameComponent
    {
        static TaskManager instance;
        public static TaskManager Instance { get { return instance; } }
        public TaskManagerComponent(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
            if (instance != null)
                throw new InvalidOperationException("Only one process manager component should be created.");

            instance = new TaskManager();
            instance.StayAlive = true;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            instance.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            instance.Update(gameTime.ElapsedGameTime.Milliseconds);

            base.Update(gameTime);
        }

    }
}
