using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using GH.Entity;
using GH.Forms.ComponentModel;

namespace GH.DataSourses.Args
{
    public class RefreshArgs : EventArgs
    {
        public RefreshArgs(ListChangedType changedType, Control control, object current)
        {
            ChangedType = changedType;
            Control = control;
            Current = (AbstractEntity)current;
        }

        public ListChangedType ChangedType { get; protected set; }
        public Control Control { get; protected set; }
        public AbstractEntity Current { get; private set; }
    }
}