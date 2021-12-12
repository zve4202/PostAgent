using System;
using System.Timers;

namespace GH.Forms.Facades
{
    public interface ITimer : IDisposable
    {
        event ElapsedEventHandler Elapsed;

        double Interval { get; set; }

        void Start();

        void Stop();
    }
}
