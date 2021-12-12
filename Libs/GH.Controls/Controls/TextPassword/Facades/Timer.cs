using System;
using System.ComponentModel;
using System.Timers;

namespace GH.Controls.Facades
{
    public class Timer : ITimer, IDisposable
    {
        private readonly System.Timers.Timer native;

        public static explicit operator System.Timers.Timer(Timer timer)
        {
            return timer.native;
        }

        public Timer(bool autoReset, double interval, ISynchronizeInvoke synchronizingObject)
        {
            this.native = new System.Timers.Timer()
            {
                AutoReset = autoReset,
                Interval = interval,
                SynchronizingObject = synchronizingObject
            };
        }

        ~Timer()
        {
            this.Dispose(false);
        }

        public event ElapsedEventHandler Elapsed
        {
            add
            {
                this.native.Elapsed += value;
            }
            remove
            {
                this.native.Elapsed -= value;
            }
        }

        public bool AutoReset
        {
            get
            {
                return this.native.AutoReset;
            }
        }

        public double Interval
        {
            get
            {
                return this.native.Interval;
            }
            set
            {
                this.native.Interval = value;
            }
        }

        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                return this.native.SynchronizingObject;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            this.native.Dispose();
        }

        public void Start()
        {
            this.native.Start();
        }

        public void Stop()
        {
            this.native.Stop();
        }
    }
}
