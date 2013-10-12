using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.TaskManagement
{
    public class WaitTask : Task
    {
        #region Fields
        int timeWaiting = 0;
        #endregion

        #region Properties
        public int WaitTime { get; private set; }
        #endregion

        #region Constructor
        public WaitTask(int waitTime)
            : base(TaskType.Wait)
        {
            timeWaiting = 0; 
            WaitTime = waitTime;
        }
        #endregion

        #region Methods
        public override void Update(int milliseconds)
        {
            base.Update(milliseconds);

            if (!IsPaused)
            {
                timeWaiting += milliseconds;
                if (timeWaiting >= WaitTime)
                {
                    Kill();
                }
            }
        }
        #endregion
    }
}
