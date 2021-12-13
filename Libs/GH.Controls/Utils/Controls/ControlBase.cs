using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace GH.Controls.Utils.Controls
{
    [ToolboxItem(false)]
    public class ControlBase : Control
    {
        private const int WM_CAPTURECHANGED = 533;
        private bool isRightToLeft;
        private static MethodInfo xClear;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 533)
            {
                if (m.LParam == this.Handle)
                    this.OnGotCapture();
                else
                    this.OnLostCapture();
            }
            base.WndProc(ref m);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.CheckRightToLeft();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.CheckRightToLeft();
        }

        protected void CheckRightToLeft()
        {
            bool isRightToLeft = WindowsFormsSettings.GetIsRightToLeft((Control)this);
            if (isRightToLeft == this.isRightToLeft)
                return;
            this.isRightToLeft = isRightToLeft;
            this.OnRightToLeftChanged();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual bool IsRightToLeft
        {
            get
            {
                return this.isRightToLeft;
            }
        }

        protected virtual void OnRightToLeftChanged()
        {
        }

        protected virtual void OnLostCapture()
        {
        }

        protected virtual void OnGotCapture()
        {
        }

        protected virtual bool GetValidationCanceled()
        {
            return ControlBase.GetValidationCanceled((Control)this);
        }

        public static bool GetCanProcessMnemonic(Control control)
        {
            if (control == null)
                return false;
            MethodInfo method = typeof(Control).GetMethod("CanProcessMnemonic", BindingFlags.Instance | BindingFlags.NonPublic);
            return !(method != (MethodInfo)null) || (bool)method.Invoke((object)control, (object[])null);
        }

        public static Control GetValidationCanceledSource(Control control)
        {
            if (control == null || !ControlBase.GetValidationCanceled(control))
                return (Control)null;
            Control control1 = control;
            while (control1.Parent != null && ControlBase.GetValidationCanceled(control1.Parent))
                control1 = control1.Parent;
            return control1;
        }

        public static bool GetValidationCanceled(Control control)
        {
            if (control == null)
                return false;
            PropertyInfo property = typeof(Control).GetProperty("ValidationCancelled", BindingFlags.Instance | BindingFlags.NonPublic);
            return !(property != (PropertyInfo)null) || (bool)property.GetValue((object)control, (object[])null);
        }

        public static void ResetValidationCanceled(Control control)
        {
            if (control == null)
                return;
            PropertyInfo property = typeof(Control).GetProperty("ValidationCancelled", BindingFlags.Instance | BindingFlags.NonPublic);
            if (!(property != (PropertyInfo)null))
                return;
            property.SetValue((object)control, (object)false, (object[])null);
        }

        public static Control GetUnvalidatedControl(Control control)
        {
            if (control == null)
                return (Control)null;
            if (!(control is ContainerControl containerControl))
                containerControl = control.GetContainerControl() as ContainerControl;
            if (containerControl == null)
                containerControl = (ContainerControl)control.FindForm();
            if (containerControl == null)
                return (Control)null;
            FieldInfo field = typeof(ContainerControl).GetField("unvalidatedControl", BindingFlags.Instance | BindingFlags.NonPublic);
            return field != (FieldInfo)null ? field.GetValue((object)containerControl) as Control : (Control)null;
        }

        public static bool IsUnvalidatedControlIsParent(Control control)
        {
            Control unvalidatedControl = ControlBase.GetUnvalidatedControl(control);
            if (unvalidatedControl == null)
                return false;
            for (; control != null; control = control.Parent)
            {
                if (unvalidatedControl == control)
                    return true;
            }
            return false;
        }

        [Browsable(false)]
        [Obsolete("Use ClearPreferredSizeCache")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ClearPrefferedSizeCache(Control control)
        {
            ControlBase.ClearPreferredSizeCache(control);
        }

        public static void ClearPreferredSizeCache(Control control)
        {
            if (ControlBase.xClear == (MethodInfo)null)
            {
                System.Type type = typeof(Control).Assembly.GetType("System.Windows.Forms.Layout.CommonProperties", false);
                if (type != (System.Type)null)
                    ControlBase.xClear = type.GetMethod("xClearPreferredSizeCache", BindingFlags.Static | BindingFlags.NonPublic);
            }
            if (!(ControlBase.xClear != (MethodInfo)null))
                return;
            ControlBase.xClear.Invoke((object)null, new object[1]
            {
        (object) control
            });
        }
    }


}
