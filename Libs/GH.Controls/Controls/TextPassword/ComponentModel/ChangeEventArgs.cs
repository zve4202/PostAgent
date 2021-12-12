using System;
using System.Collections.Generic;

namespace GH.Controls.ComponentModel
{
    public class ChangeEventArgs<T> : EventArgs, IEquatable<ChangeEventArgs<T>>
    {
        public static bool operator ==(ChangeEventArgs<T> left, ChangeEventArgs<T> right)
        {
            return object.Equals((object)left, (object)right);
        }

        public static bool operator !=(ChangeEventArgs<T> left, ChangeEventArgs<T> right)
        {
            return !object.Equals((object)left, (object)right);
        }

        public static ChangeEventArgs<T> DoIf<TSender>(
          TSender sender,
          EventHandler<ChangeEventArgs<T>> @event,
          Action<ChangeEventArgs<T>> callEventRaisingMethod,
          T oldValue,
          T newValue)
        {
            if (callEventRaisingMethod == null)
                throw new ArgumentNullException(nameof(callEventRaisingMethod));
            return Event.DoIf<TSender, ChangeEventArgs<T>>(sender, @event, (Func<ChangeEventArgs<T>>)(() =>
           {
               ChangeEventArgs<T> changeEventArgs = new ChangeEventArgs<T>(oldValue, newValue);
               callEventRaisingMethod(changeEventArgs);
               return changeEventArgs;
           }));
        }

        public ChangeEventArgs(T oldValue, T newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public T NewValue { get; protected set; }

        public T OldValue { get; protected set; }

        public override bool Equals(object other)
        {
            return this.Equals(other as ChangeEventArgs<T>);
        }

        public bool Equals(ChangeEventArgs<T> other)
        {
            if ((object)other == null)
                return false;
            if ((object)this == (object)other)
                return true;
            return EqualityComparer<T>.Default.Equals(this.NewValue, other.NewValue) && EqualityComparer<T>.Default.Equals(this.OldValue, other.OldValue);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(this.NewValue) * 397 ^ EqualityComparer<T>.Default.GetHashCode(this.OldValue);
        }
    }
}
