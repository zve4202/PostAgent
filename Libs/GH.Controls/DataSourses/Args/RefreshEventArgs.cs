using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using GH.Forms.ComponentModel;

namespace GH.Controls.DataSourses.Args
{
    public class RefreshArgs : EventArgs
    {
        public RefreshArgs(ListChangedType changedType, Control control, ListChangedEventArgs listChanged)
        {
            ChangedType = changedType;
            Control = control;
            ListChanged = listChanged;
        }

        public ListChangedType ChangedType { get; protected set; }
        public Control Control { get; protected set; }
        public ListChangedEventArgs ListChanged { get; private set; }
    }
}