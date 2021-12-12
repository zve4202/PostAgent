using System;

namespace GH.Forms.ComponentModel
{
    public static class Event
    {
        public static TEventArgs DoIf<TSender, TEventArgs>(
          TSender sender,
          EventHandler<TEventArgs> @event,
          Func<TEventArgs> constructEventArgsAndCallEventRaisingMethod)
          where TEventArgs : EventArgs
        {
            if ((object)sender == null)
                throw new ArgumentNullException(nameof(sender));
            if (constructEventArgsAndCallEventRaisingMethod == null)
                throw new ArgumentNullException(nameof(constructEventArgsAndCallEventRaisingMethod));
            return !(sender.GetType() == typeof(TSender)) ? constructEventArgsAndCallEventRaisingMethod() : Event.DoIf<TEventArgs>(@event, constructEventArgsAndCallEventRaisingMethod);
        }

        private static TEventArgs DoIf<TEventArgs>(
          EventHandler<TEventArgs> @event,
          Func<TEventArgs> constructEventArgsAndCallEventRaisingMethod)
          where TEventArgs : EventArgs
        {
            return @event != null ? constructEventArgsAndCallEventRaisingMethod() : default(TEventArgs);
        }
    }
}
