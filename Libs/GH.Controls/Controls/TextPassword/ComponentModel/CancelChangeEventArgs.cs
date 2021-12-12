using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GH.Controls.ComponentModel
{
    public class CancelChangeEventArgs<T> : CancelEventArgs
    {
        protected readonly GH.Controls.ComponentModel.ChangeEventArgs<T> ChangeEventArgs;

        public static bool operator ==(CancelChangeEventArgs<T> left, CancelChangeEventArgs<T> right)
        {
            return object.Equals((object)left, (object)right);
        }

        public static bool operator !=(CancelChangeEventArgs<T> left, CancelChangeEventArgs<T> right)
        {
            return !object.Equals((object)left, (object)right);
        }

        public static CancelChangeEventArgs<T> DoIf<TSender>(
          TSender sender,
          EventHandler<CancelChangeEventArgs<T>> @event,
          Action<CancelChangeEventArgs<T>> callEventRaisingMethod,
          T oldValue,
          T newValue)
        {
            if (callEventRaisingMethod == null)
                throw new ArgumentNullException(nameof(callEventRaisingMethod));
            return Event.DoIf<TSender, CancelChangeEventArgs<T>>(sender, @event, (Func<CancelChangeEventArgs<T>>)(() =>
           {
               CancelChangeEventArgs<T> cancelChangeEventArgs = new CancelChangeEventArgs<T>(oldValue, newValue);
               callEventRaisingMethod(cancelChangeEventArgs);
               return cancelChangeEventArgs;
           }));
        }

        public CancelChangeEventArgs(T oldValue, T newValue)
        {
            this.ChangeEventArgs = new GH.Controls.ComponentModel.ChangeEventArgs<T>(oldValue, newValue);
        }

        public CancelChangeEventArgs(T oldValue, T newValue, bool cancel)
          : base(cancel)
        {
            this.ChangeEventArgs = new GH.Controls.ComponentModel.ChangeEventArgs<T>(oldValue, newValue);
        }

        public T NewValue
        {
            get
            {
                return this.ChangeEventArgs.NewValue;
            }
        }

        public T OldValue
        {
            get
            {
                return this.ChangeEventArgs.OldValue;
            }
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as CancelChangeEventArgs<T>);
        }

        public bool Equals(CancelChangeEventArgs<T> other)
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
