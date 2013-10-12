using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.TaskManagement
{
    public class DelegateTask : Task
    {
        #region Fields
        Predicate<int> updateAction = null;
        Action initializationAction = null;
        #endregion

        public DelegateTask(TaskType type, Predicate<int> update, Action init)
            : base(type)
        {
            updateAction = update;
            initializationAction = init;
        }

        public DelegateTask(TaskType type, Predicate<int> update)
            : this(type, update, null)
        {
        }

        public override void Update(int milliseconds)
        {
            base.Update(milliseconds);

            if (updateAction != null)
                if (!updateAction(milliseconds))
                    Kill();
        }

        public override void Initialize()
        {
            base.Initialize();

            if (initializationAction != null)
                initializationAction();
        }

    }

    //public class DelegateTask<T> : Task
    //{
    //    #region Fields
    //    Action<T, int> updateAction = null;
    //    Action<T> initializationAction = null;
    //    T context = default(T);
    //    #endregion

    //    public DelegateTask(TaskType type, T arg0, Predicate<T, int> update, Action<T> init)
    //        : base(type)
    //    {
    //        updateAction = update;
    //        initializationAction = init;
    //        context = arg0;
    //    }

    //    public DelegateTask(TaskType type, T arg0, Predicate<T, int> update)
    //        : this(type, arg0, update, null)
    //    {
    //    }

    //    public override void Update(int milliseconds)
    //    {
    //        base.Update(milliseconds);

    //        if (updateAction != null)
    //            updateAction(context, milliseconds);
    //    }

    //    public override void Initialize()
    //    {
    //        base.Initialize();

    //        if (initializationAction != null)
    //            initializationAction(context);
    //    }

    //}
}
