using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.TaskManagement
{
    public class TaskManager : Task
    {
        #region Fields
        List<Task> processes = new List<Task>();
        #endregion

        #region Properties
        public bool StayAlive { get; set; }
        #endregion

        #region Constructor
        public TaskManager()
            : base(TaskType.Control)
        {
            // Most process managers will kill themselves
            // Only the global one should stay alive.
            StayAlive = false;
        }
        #endregion

        #region Methods
        public override void Update(int deltaMilliseconds)
        {
            base.Update(deltaMilliseconds);

            if (!IsPaused)
            {
                for(int i = 0; i < processes.Count(); ++i)
                {
                    Task p = processes[i];

                    // Check for dead processes and move to the next children
                    if (p.IsDead)
                    {
                        Detach(p);

                        if (p.Next != null)
                            Attach(p.Next);

                        // Fix up the index so we don't skip a process.
                        --i;
                    }
                    else if (p.IsActive && !p.IsPaused)
                    {
                        p.Update(deltaMilliseconds);
                    }
                }
            }

            // If this is a transient process manager and 
            // it's empty, kill it.
            if (!StayAlive && !HasProcesses())
                Kill();
        }

        void ClearProcesses()
        {
            while (processes.Count() > 0)
            {
                Detach(processes.First());
            }
        }

        bool IsProcessActive(TaskType type)
        {
            // Check for living processes.  If they are dead, make sure no children 
            // are attached as they will be brought to life on next cycle.
#if WINDOWS_PHONE
            return processes.Where(p => p.Type == type && (!p.IsDead || p.Next != null)).Count() > 0;
#else
            return processes.Exists(p => p.Type == type && (!p.IsDead || p.Next != null));
#endif
            }

        bool HasProcesses()
        {
            return processes.Count() > 0;
        }

        public void Attach(Task p)
        {
            processes.Add(p);
            p.IsAttached = true;
        }
        void Detach(Task p)
        {
            processes.Remove(p);
            p.IsAttached = false;
        }
        #endregion

        #region Helpers
        #endregion
    }
}
