using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.TaskManagement
{
    public enum TaskType
    {
        None,
        Wait,
        Sprite,
        Control,
        Screen,
        Music,
        SoundFX,
        GameSpecific,
    }

    public class Task
    {
        #region Properties
        public bool IsDead { get; private set; }
        public bool IsPaused { get; set; }
        public TaskType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsInitialized { get; private set; }
        public Task Next { get; set; }
        public bool IsAttached { get; set; }
        #endregion

        #region Construction
        public Task(TaskType type)
        {
            IsDead = false;
            IsPaused = false;
            IsInitialized = false;
            Type = type;
            IsActive = true;
            Next = null;
            IsAttached = false;
        }
        #endregion

        #region Methods
        public void Kill()
        {
            IsDead = true;
        }

        public void Append(Task process)
        {
            Task p = this;
            while (p.Next != null)
                p = p.Next;

            p.Next = process;
        }

        public void Insert(Task p)
        {
            Task next = Next;
            Next = p;
            p.Append(next);
        }

        public virtual void Update(int milliseconds)
        {
            if (!IsInitialized)
            {
                Initialize();
                IsInitialized = true;
            }
        }

        public virtual void Initialize()
        {
            // This space intentionally left blank
        }

        #endregion

    }
}
