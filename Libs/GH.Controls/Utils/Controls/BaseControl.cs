using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GH.Controls.Utils.Controls
{
    [GHToolboxItem(GHToolboxItemKind.Free)]
    [ToolboxBitmap(typeof(ToolboxIconsRootNS), "ToolTipController")]
    [Designer("DevExpress.Utils.Design.ToolTipControllerDesigner, DevExpress.Design.v17.2")]
    [ProvideProperty("ToolTip", typeof(Control))]
    [ProvideProperty("Title", typeof(Control))]
    [ProvideProperty("ToolTipIconType", typeof(Control))]
    [ProvideProperty("SuperTip", typeof(Control))]
    [ProvideProperty("ToolTipAnchor", typeof(Control))]
    [ProvideProperty("AllowHtmlText", typeof(Control))]
    [Description("Manages tooltips for a specific control or controls.")]
    [ToolboxTabName("GH Components")]
    public class ToolTipController : Component, IExtenderProvider
    {
        private static readonly object beforeShow = new object();
        private static readonly object calcSize = new object();
        private static readonly object customDraw = new object();
        private static readonly object getActiveObjectInfo = new object();
        private static readonly object hyperlinkClick = new object();
        private bool closeOnClick = true;
        internal const int DefaultReshowDelay = 100;
        internal const int DefaultInitialDelay = 500;
        internal const int DefaultAutoPopDelay = 5000;
        private Hashtable controlClientOwners;
        private ToolTipControlInfo activeObjectInfo;
        private ToolTipControllerBaseWindow toolWindow;
        private ToolTipViewInfoBase viewInfo;
        private int initialDelay;
        private int autoPopDelay;
        private int reshowDelay;
        private bool active;
        private Timer initialTimer;
        private Timer autoPopTimer;
        private ClosingDelayTimer closingDelayTimer;
        private Control activeControl;
        private object activeObject;
        private Hashtable toolTips;
        private ToolTipControllerShowEventArgs showArgs;
        private ToolTipControllerShowEventArgs currentShowArgs;
        private Point oldCursorPosition;
        private bool showShadow;
        private bool allowHtmlText;
        private int prevMaxWidth;
        private ToolTipType toolTipType;
        private ToolTipType currentToolTipType;
        [ThreadStatic]
        private static ToolTipController defaultController;
        [ThreadStatic]
        private static ToolTipController activeController;
        private Timer hoverTimer;
        private bool isSuperTipMaxWidthSaved;

        public ToolTipController(IContainer container)
          : this()
        {
            container.Add((IComponent)this);
        }

        public ToolTipController()
        {
            this.superTooltip = new SuperToolTip();
            this.toolTipType = ToolTipType.Default;
            this.currentToolTipType = ToolTipType.SuperTip;
            this.toolTips = new Hashtable();
            this.toolWindow = (ToolTipControllerBaseWindow)null;
            this.activeObjectInfo = (ToolTipControlInfo)null;
            this.activeControl = (Control)null;
            this.activeObject = (object)null;
            this.active = true;
            this.showArgs = new ToolTipControllerShowEventArgs();
            this.showShadow = true;
            this.allowHtmlText = false;
            this.initialTimer = new Timer();
            this.autoPopTimer = new Timer();
            this.closingDelayTimer = new ClosingDelayTimer();
            this.AutoPopTimer.Tick += new EventHandler(this.OnAutoPopTimerTick);
            this.InitialTimer.Tick += new EventHandler(this.OnInitialTimerTick);
            this.ClosingDelayTimer.Interval = this.ClosingDelayInterval;
            this.controlClientOwners = new Hashtable();
            this.reshowDelay = 100;
            this.InitialDelay = 500;
            this.AutoPopDelay = 5000;
            this.currentShowArgs = (ToolTipControllerShowEventArgs)null;
            this.oldCursorPosition = Point.Empty;
            this.prevMaxWidth = this.superTooltip.MaxWidth;
        }

        [DXCategory("ToolTip")]
        [DefaultValue(ToolTipAnchor.Default)]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerToolTipAnchor")]
        public ToolTipAnchor ToolTipAnchor { get; set; }

        protected virtual int ClosingDelayInterval
        {
            get
            {
                return 600;
            }
        }

        protected bool GetUseSuperTips(ToolTipControllerShowEventArgs eShow)
        {
            return this.GetToolTipType(eShow) == ToolTipType.SuperTip;
        }

        protected virtual ToolTipControllerBaseWindow CreateToolWindow()
        {
            return this.GetUseSuperTips(this.CurrentShowArgs) ? (ToolTipControllerBaseWindow)new SuperToolTipWindow(this.SuperTooltip, this.ShowShadow) : (ToolTipControllerBaseWindow)ToolTipControllerStyledWindowBase.Create(this);
        }

        [DevExpressUtilsLocalizedDescription("ToolTipControllerDefaultController")]
        public static ToolTipController DefaultController
        {
            get
            {
                if (ToolTipController.defaultController == null)
                    ToolTipController.defaultController = (ToolTipController)new ToolTipControllerDefault();
                return ToolTipController.defaultController;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.autoPopTimer.Tick -= new EventHandler(this.OnAutoPopTimerTick);
                this.initialTimer.Tick -= new EventHandler(this.OnInitialTimerTick);
                this.initialTimer.Dispose();
                this.autoPopTimer.Dispose();
                if (this.hoverTimer != null)
                {
                    this.hoverTimer.Tick -= new EventHandler(this.OnHoverTimerTick);
                    this.hoverTimer.Dispose();
                }
                this.closingDelayTimer.Dispose();
                this.DestroyToolWindow();
                if (ToolTipController.activeController == this)
                    ToolTipController.activeController = (ToolTipController)null;
            }
            base.Dispose(disposing);
        }

        protected SuperToolTip SuperTooltip
        {
            get
            {
                return this.superTooltip;
            }
        }

        protected internal ToolTipViewInfoBase ViewInfo
        {
            get
            {
                if (this.viewInfo == null)
                    this.viewInfo = this.CreateViewInfo();
                return this.viewInfo;
            }
        }

        protected ToolTipType GetToolTipType()
        {
            if (this.CurrentShowArgs != null)
                return this.GetToolTipType(this.CurrentShowArgs);
            return this.ToolTipType != ToolTipType.Default ? this.ToolTipType : ToolTipType.Standard;
        }

        protected ToolTipType GetToolTipType(ToolTipControllerShowEventArgs e)
        {
            if (e == null)
                return ToolTipType.Standard;
            if (e.ToolTipType != ToolTipType.Default)
                return e.ToolTipType;
            if (this.ToolTipType != ToolTipType.Default)
                return this.ToolTipType;
            return e.SuperTip != null ? ToolTipType.SuperTip : ToolTipType.Standard;
        }

        protected ToolTipType CurrentToolTipType
        {
            get
            {
                return this.currentToolTipType;
            }
            set
            {
                if (this.CurrentToolTipType == value)
                    return;
                this.currentToolTipType = value;
                this.DestroyToolWindow();
            }
        }

        protected virtual ToolTipViewInfoBase CreateViewInfo()
        {
            return this.CurrentToolTipType == ToolTipType.SuperTip ? (ToolTipViewInfoBase)new ToolTipViewInfoSuperTip(this.SuperTooltip) : (ToolTipViewInfoBase)new ToolTipViewInfo();
        }

        protected virtual void DestroyToolWindow()
        {
            if (this.toolWindow != null)
            {
                this.toolWindow.Paint -= new PaintEventHandler(this.OnToolWindowPaint);
                this.toolWindow.MouseLeave -= new EventHandler(this.OnToolWindowMouseLeave);
                this.toolWindow.Dispose();
                this.toolWindow = (ToolTipControllerBaseWindow)null;
            }
            this.viewInfo = (ToolTipViewInfoBase)null;
        }

        [Browsable(false)]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerActiveObjectInfo")]
        public virtual ToolTipControlInfo ActiveObjectInfo
        {
            get
            {
                return this.activeObjectInfo;
            }
        }

        private bool IsTheSameObjectInfo(ToolTipControlInfo value)
        {
            return (value == null || value.ForcedShow != DefaultBoolean.True) && (this.ActiveObjectInfo == value || this.ActiveObjectInfo != null && value != null && this.ActiveObjectInfo.Object == value.Object);
        }

        protected virtual void SetActiveObjectInfo(ToolTipControlInfo value)
        {
            if (this.IsTheSameObjectInfo(value))
                return;
            this.activeObjectInfo = value;
            this.SetActiveObject(this.ActiveObjectInfo != null ? this.ActiveObjectInfo.Object : (object)null, value != null && value.ForcedShow == DefaultBoolean.True);
        }

        bool IExtenderProvider.CanExtend(object target)
        {
            return target is Control && !(target is IToolTipControlClient);
        }

        private ToolTipController.ControlInfo GetControlInfo(Control control)
        {
            return (ToolTipController.ControlInfo)this.toolTips[(object)control];
        }

        private void SetControlInfo(Control control, ToolTipController.ControlInfo info)
        {
            if (info == null && this.GetControlInfo(control) != null)
            {
                control.MouseEnter -= new EventHandler(this.OnControlMouseEnter);
                control.MouseLeave -= new EventHandler(this.OnControlMouseLeave);
                control.MouseDown -= new MouseEventHandler(this.OnControlMouseDown);
                control.Disposed -= new EventHandler(this.OnControlDisposed);
            }
            if (info != null && this.GetControlInfo(control) == null)
            {
                control.Disposed += new EventHandler(this.OnControlDisposed);
                control.MouseEnter += new EventHandler(this.OnControlMouseEnter);
                control.MouseLeave += new EventHandler(this.OnControlMouseLeave);
                control.MouseDown += new MouseEventHandler(this.OnControlMouseDown);
            }
            if (info == null)
                this.toolTips.Remove((object)control);
            else
                this.toolTips[(object)control] = (object)info;
        }

        protected internal bool IsVistaStyleTooltip
        {
            get
            {
                return ToolTipController.ToolTipControllerHelper.ShouldUseVistaStyleTooltip(this.ToolTipStyle);
            }
        }

        [Category("Tooltip")]
        [DefaultValue("")]
        [Localizable(true)]
        public string GetToolTip(Control control)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control);
            return controlInfo != null ? ToolTipControlInfo.NormalizeSimpleText(controlInfo.Text) : string.Empty;
        }

        internal bool ShouldSerializeSuperTip(Control control)
        {
            SuperToolTip superTip = this.GetSuperTip(control);
            return superTip != null && !superTip.IsEmpty;
        }

        internal bool ShouldSerializeToolTipAnchor(Control control)
        {
            return this.GetToolTipAnchor(control) != ToolTipAnchor.Default;
        }

        [Localizable(true)]
        [Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, DevExpress.XtraEditors.v17.2.Design", typeof(UITypeEditor))]
        [Category("Tooltip")]
        public SuperToolTip GetSuperTip(Control control)
        {
            return this.GetControlInfo(control)?.SuperTip;
        }

        public void SetSuperTip(Control control, SuperToolTip value)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control) ?? new ToolTipController.ControlInfo(string.Empty);
            controlInfo.SuperTip = value;
            this.SetControlInfo(control, controlInfo.IsEmpty ? (ToolTipController.ControlInfo)null : controlInfo);
        }

        public ToolTipAnchor GetToolTipAnchor(Control control)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control);
            return controlInfo != null ? controlInfo.ToolTipAnchor : ToolTipAnchor.Default;
        }

        public void SetToolTipAnchor(Control control, ToolTipAnchor value)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control);
            if (controlInfo == null)
                return;
            controlInfo.ToolTipAnchor = value;
        }

        [Category("Tooltip")]
        [DefaultValue("")]
        [Localizable(true)]
        public string GetTitle(Control control)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control);
            return controlInfo != null ? ToolTipControlInfo.NormalizeSimpleText(controlInfo.Title) : string.Empty;
        }

        [Localizable(true)]
        [Category("Tooltip")]
        [DefaultValue(ToolTipIconType.None)]
        public ToolTipIconType GetToolTipIconType(Control control)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control);
            return controlInfo != null ? controlInfo.IconType : ToolTipIconType.None;
        }

        public void SetToolTip(Control control, string value)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control);
            if (value == null)
                value = string.Empty;
            if (controlInfo == null)
                controlInfo = new ToolTipController.ControlInfo(value);
            controlInfo.Text = value;
            this.SetControlInfo(control, controlInfo.IsEmpty ? (ToolTipController.ControlInfo)null : controlInfo);
        }

        public void SetTitle(Control control, string value)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control);
            if (value == null)
                value = string.Empty;
            if (controlInfo == null)
                controlInfo = new ToolTipController.ControlInfo(string.Empty, value, ToolTipIconType.None);
            controlInfo.Title = value;
            this.SetControlInfo(control, controlInfo.IsEmpty ? (ToolTipController.ControlInfo)null : controlInfo);
        }

        public void SetToolTipIconType(Control control, ToolTipIconType value)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control) ?? new ToolTipController.ControlInfo(string.Empty, value);
            controlInfo.IconType = value;
            this.SetControlInfo(control, controlInfo.IsEmpty ? (ToolTipController.ControlInfo)null : controlInfo);
        }

        [Localizable(true)]
        [Category("Tooltip")]
        [DefaultValue(DefaultBoolean.Default)]
        public DefaultBoolean GetAllowHtmlText(Control control)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control);
            return controlInfo != null ? controlInfo.AllowHtmlText : DefaultBoolean.Default;
        }

        public void SetAllowHtmlText(Control control, DefaultBoolean value)
        {
            ToolTipController.ControlInfo controlInfo = this.GetControlInfo(control) ?? new ToolTipController.ControlInfo(string.Empty, string.Empty, ToolTipIconType.None, value);
            controlInfo.AllowHtmlText = value;
            this.SetControlInfo(control, controlInfo.IsEmpty ? (ToolTipController.ControlInfo)null : controlInfo);
        }

        internal bool ShouldSerializeAllowHtmlText(Control control)
        {
            return this.GetAllowHtmlText(control) != DefaultBoolean.Default;
        }

        protected Hashtable ControlClientOwners
        {
            get
            {
                return this.controlClientOwners;
            }
        }

        public void AddClientControl(Control control, IToolTipControlClient owner)
        {
            this.AddClientControlCore(control);
            if (this.ControlClientOwners.ContainsKey((object)control))
                this.ControlClientOwners[(object)control] = (object)owner;
            else
                this.ControlClientOwners.Add((object)control, (object)owner);
        }

        protected virtual void AddClientControlCore(Control control)
        {
            control.MouseEnter += new EventHandler(this.OnControlMouseEnter);
            control.MouseLeave += new EventHandler(this.OnControlMouseLeave);
            control.MouseMove += new MouseEventHandler(this.OnControlMouseMove);
            control.MouseWheel += new MouseEventHandler(this.OnControlMouseWheel);
            control.LocationChanged += new EventHandler(this.OnControlLocationChanged);
            control.MouseDown += new MouseEventHandler(this.OnControlMouseDown);
        }

        private void OnControlLocationChanged(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            if (control != this.ActiveControl || this.ActiveControl == null)
                return;
            Point client = control.PointToClient(Control.MousePosition);
            MouseEventArgs e1 = new MouseEventArgs(MouseButtons.None, 0, client.X, client.Y, 0);
            this.OnControlMouseMove(sender, e1);
        }

        public void AddClientControl(Control control)
        {
            if (!(control is IToolTipControlClient))
                return;
            this.AddClientControlCore(control);
        }

        public virtual void RemoveClientControl(Control control)
        {
            control.MouseEnter -= new EventHandler(this.OnControlMouseEnter);
            control.MouseLeave -= new EventHandler(this.OnControlMouseLeave);
            control.MouseMove -= new MouseEventHandler(this.OnControlMouseMove);
            control.MouseWheel -= new MouseEventHandler(this.OnControlMouseWheel);
            control.LocationChanged -= new EventHandler(this.OnControlLocationChanged);
            control.MouseDown -= new MouseEventHandler(this.OnControlMouseDown);
            if (this.ControlClientOwners.ContainsKey((object)control))
                this.ControlClientOwners.Remove((object)control);
            if (this.ActiveControlClient != null)
                return;
            this.DestroyToolWindow();
        }

        private void UnsubscribeFormEvents()
        {
            Form targetForm = this.GetTargetForm();
            if (targetForm == null)
                return;
            targetForm.Deactivate -= new EventHandler(this.OnFormDeactivate);
        }

        private Form GetTargetForm()
        {
            if (this.ActiveControl == null)
                return (Form)null;
            Form form = this.ActiveControl.FindForm();
            if (form == null)
                return (Form)null;
            if (form.IsMdiChild && form.MdiParent != null)
                form = form.MdiParent;
            return form;
        }

        protected virtual void SubscribeFormEvents()
        {
            Form targetForm = this.GetTargetForm();
            if (targetForm == null)
                return;
            targetForm.Deactivate -= new EventHandler(this.OnFormDeactivate);
            targetForm.Deactivate += new EventHandler(this.OnFormDeactivate);
        }

        protected virtual void OnFormDeactivate(object sender, EventArgs e)
        {
            this.HideHint();
        }

        protected void OnControlMouseEnter(object sender, EventArgs e)
        {
            if (!(sender is Control))
                return;
            this.ActiveControl = sender as Control;
        }

        protected bool IsMouseInTooltipForm()
        {
            ToolTipControllerBaseWindow toolWindow = this.toolWindow;
            return toolWindow != null && toolWindow.Visible && toolWindow.Bounds.Contains(Control.MousePosition);
        }

        protected void OnControlMouseLeave(object sender, EventArgs e)
        {
            if (this.toolWindow != null && this.ToolWindow.Visible && this.ActiveControl != null)
            {
                if (this.ToolWindow.HasHyperlink && this.CloseOnClick != DefaultBoolean.False)
                {
                    this.ClosingDelayTimer.Start((MethodInvoker)(() =>
                    {
                        if (this.IsMouseInTooltipForm())
                            return;
                        this.ResetActiveControl();
                    }));
                    return;
                }
                if (this.ActiveControl.ClientRectangle.Contains(this.ActiveControl.PointToClient(Control.MousePosition)) && this.ToolWindow.Bounds.Contains(Control.MousePosition) || (this.CloseOnClick == DefaultBoolean.True || this.HoverTimer.Enabled))
                    return;
            }
            this.ResetActiveControl();
        }

        protected virtual bool IsHyperLinkAccessible
        {
            get
            {
                return this.toolWindow != null && this.toolWindow.Visible && this.toolWindow.HasHyperlink;
            }
        }

        protected internal virtual void ResetActiveControl()
        {
            if (this.toolWindow != null)
            {
                this.toolWindow.Dispose();
                this.toolWindow = (ToolTipControllerBaseWindow)null;
            }
            this.ActiveControl = (Control)null;
            if (this.ViewInfo.ShowArgs != null)
            {
                this.ViewInfo.ShowArgs.SelectedControl = (Control)null;
                this.ViewInfo.ShowArgs.SelectedObject = (object)null;
            }
            if (this.SuperTooltip == null)
                return;
            this.SuperTooltip.LookAndFeel = (UserLookAndFeel)null;
        }

        protected Form FindTopLevelForm(Control control)
        {
            Control control1 = control;
            while (control1.Parent != null)
                control1 = control1.Parent;
            return !(control1 is Form form) || !form.TopLevel ? (Form)null : form;
        }

        protected void OnControlMouseWheel(object sender, MouseEventArgs e)
        {
            this.OnControlMouseMove(sender, e);
        }

        protected void OnControlMouseMove(object sender, MouseEventArgs e)
        {
            this.ProcessMouseMove(sender, e.Location);
        }

        public virtual void ProcessNCMouseMove(object sender, Point p)
        {
            this.ProcessMouseMove(sender, p);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DXHelpExclude(true)]
        public virtual void ProccessNCMouseMove(object sender, Point p)
        {
            this.ProcessNCMouseMove(sender, p);
        }

        [DevExpressUtilsLocalizedDescription("ToolTipControllerKeepWhileHovered")]
        [DefaultValue(false)]
        [DXCategory("Behavior")]
        public bool KeepWhileHovered { get; set; }

        protected Timer HoverTimer
        {
            get
            {
                if (this.hoverTimer == null)
                    this.hoverTimer = this.CreateHoverTimer();
                return this.hoverTimer;
            }
        }

        protected Point LastScreenPoint { get; set; }

        protected virtual Timer CreateHoverTimer()
        {
            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += new EventHandler(this.OnHoverTimerTick);
            return timer;
        }

        protected bool IsCursorInToolWindow(Point pt)
        {
            return this.toolWindow != null && this.ToolWindow.Visible && this.ToolWindow.Bounds.Contains(pt);
        }

        protected bool ForceProcessMouseMove { get; set; }

        private void OnHoverTimerTick(object sender, EventArgs e)
        {
            if (this.LastScreenPoint == Control.MousePosition)
            {
                if (this.IsCursorInToolWindow(Control.MousePosition))
                    return;
                if (this.ActiveControl != null && this.IsMouseInActiveControl())
                {
                    this.ForceProcessMouseMove = true;
                    this.ProcessMouseMove((object)this.ActiveControl, this.ActiveControl.PointToClient(this.LastScreenPoint));
                    this.ForceProcessMouseMove = false;
                    return;
                }
                this.SetActiveObjectInfo((ToolTipControlInfo)null);
                this.HideHint();
                this.HoverTimer.Stop();
            }
            this.LastScreenPoint = Control.MousePosition;
        }

        private void ProcessMouseMove(object sender, Point p)
        {
            if (this.KeepWhileHovered && this.HoverTimer.Enabled && !this.ForceProcessMouseMove)
                return;
            if (sender is Control control)
            {
                Form topLevelForm = this.FindTopLevelForm(control);
                Form activeForm = Form.ActiveForm;
                ISupportToolTipsForm supportToolTipsForm1 = activeForm as ISupportToolTipsForm;
                ISupportToolTipsForm supportToolTipsForm2 = topLevelForm as ISupportToolTipsForm;
                ISupportToolTipsForm supportToolTipsForm3 = control as ISupportToolTipsForm;
                bool flag = false;
                if (topLevelForm == activeForm)
                    flag = true;
                if (!flag && supportToolTipsForm3 != null)
                {
                    flag = supportToolTipsForm3.ShowToolTipsWhenInactive;
                }
                else
                {
                    if (!flag && supportToolTipsForm2 != null)
                        flag = supportToolTipsForm2.ShowToolTipsWhenInactive;
                    if (!flag && supportToolTipsForm1 != null)
                        flag = supportToolTipsForm1.ShowToolTipsFor(topLevelForm);
                }
                if (!flag)
                    return;
                this.ActiveControl = control;
            }
            ToolTipControlInfo info = this.ActiveObjectInfo;
            IToolTipControlClient tipControlClient = sender as IToolTipControlClient;
            if (this.ControlClientOwners.ContainsKey(sender))
                tipControlClient = this.ControlClientOwners[sender] as IToolTipControlClient;
            if (tipControlClient != null && tipControlClient.ShowToolTips)
            {
                info = tipControlClient.GetObjectInfo(new Point(p.X, p.Y));
                info?.Normalize();
            }
            ToolTipControllerGetActiveObjectInfoEventArgs e = new ToolTipControllerGetActiveObjectInfoEventArgs(this.ActiveControl, this.ActiveObject, info, new Point(p.X, p.Y));
            this.OnGetActiveObjectInfo(e);
            if (info != null)
                info.ForcedShow = this.GetForcedShow(info);
            bool isTheSameActiveObjectInfo = this.IsTheSameObjectInfo(e.Info);
            this.SetActiveObjectInfo(e.Info);
            if (this.ShouldHideHintByMouseMove(e.Info, isTheSameActiveObjectInfo))
                this.HideHint();
            if (!this.KeepWhileHovered || this.HoverTimer.Enabled)
                return;
            this.LastScreenPoint = Control.MousePosition;
            this.HoverTimer.Start();
        }

        protected virtual DefaultBoolean GetForcedShow(ToolTipControlInfo value)
        {
            if (value == null)
                return DefaultBoolean.False;
            if (value.ForcedShow != DefaultBoolean.Default)
                return value.ForcedShow;
            if (this.ActiveObjectInfo == null || this.ActiveObjectInfo.SuperTip == null || value.SuperTip == null)
                return DefaultBoolean.False;
            if (this.ActiveObjectInfo.SuperTip.Items.Count != value.SuperTip.Items.Count)
                return DefaultBoolean.True;
            for (int index = 0; index < value.SuperTip.Items.Count; ++index)
            {
                if (value.SuperTip.Items[index] is ToolTipItem && this.ActiveObjectInfo.SuperTip.Items[index] is ToolTipItem && !(((ToolTipItem)value.SuperTip.Items[index]).Text == ((ToolTipItem)this.ActiveObjectInfo.SuperTip.Items[index]).Text))
                    return DefaultBoolean.True;
            }
            return DefaultBoolean.False;
        }

        protected virtual bool ShouldHideHintByMouseMove(
          ToolTipControlInfo info,
          bool isTheSameActiveObjectInfo)
        {
            return isTheSameActiveObjectInfo && info != null && (info.HideHintOnMouseMove && this.IsHintVisible) && Control.MousePosition != this.ShowHintCursorPosition;
        }

        private void OnControlDisposed(object sender, EventArgs e)
        {
            this.SetControlInfo((Control)sender, (ToolTipController.ControlInfo)null);
        }

        protected void OnControlMouseDown(object sender, MouseEventArgs e)
        {
            this.HideHint();
        }

        protected virtual void OnInitialTimerTick(object sender, EventArgs e)
        {
            this.ShowHint();
        }

        protected virtual void OnAutoPopTimerTick(object sender, EventArgs e)
        {
            if (this.CloseOnClick == DefaultBoolean.True)
                this.AutoPopTimer.Stop();
            else if (this.ActiveObject != null)
                this.HideToolWindow();
            else
                this.HideHint();
        }

        protected virtual void OnToolWindowPaint(object sender, PaintEventArgs e)
        {
            using (GraphicsCache cache = new GraphicsCache(e))
            {
                if (this.GetUseSuperTips(this.CurrentShowArgs))
                    this.DrawSuperToolWindow(cache);
                else
                    this.DrawToolWindow(cache);
            }
        }

        protected virtual void DrawSuperToolWindow(GraphicsCache cache)
        {
        }

        protected virtual void DrawToolWindow(GraphicsCache cache)
        {
            if (!(this.ViewInfo is ToolTipViewInfo viewInfo) || this.CurrentShowArgs == null)
                return;
            this.ToolWindow.DrawBackground(cache, viewInfo.PaintAppearance);
            ToolTipControllerCustomDrawEventArgs e = new ToolTipControllerCustomDrawEventArgs(cache, this.CurrentShowArgs, viewInfo.ContentBounds);
            this.OnCustomDraw(e);
            if (e.Handled)
                return;
            ToolTipControllerImage tipControllerImage = new ToolTipControllerImage(this.CurrentShowArgs);
            if (tipControllerImage.HasImage)
                tipControllerImage.Draw(cache, viewInfo.ImageBounds.X, viewInfo.ImageBounds.Y);
            if (viewInfo.AllowHtmlText)
            {
                this.DrawHtmlString(cache, viewInfo.HtmlTextInfo);
                this.DrawHtmlString(cache, viewInfo.HtmlTitleInfo);
            }
            else
            {
                this.DrawString(cache, this.CurrentShowArgs.ToolTip, viewInfo.PaintAppearance, viewInfo.TextBounds);
                this.DrawString(cache, this.CurrentShowArgs.Title, viewInfo.PaintAppearanceTitle, viewInfo.TitleBounds);
            }
        }

        protected virtual void DrawHtmlString(GraphicsCache cache, StringInfo info)
        {
            StringPainter.Default.DrawString(cache, info);
        }

        protected virtual void DrawString(
          GraphicsCache cache,
          string text,
          AppearanceObject appearance,
          Rectangle bounds)
        {
            if (text == string.Empty)
                return;
            cache.DrawString(text, appearance.Font, appearance.GetForeBrush(cache), bounds, this.Style.TextOptions.GetStringFormat(TextOptions.DefaultOptionsMultiLine));
        }

        protected virtual string CurrentToolTipText
        {
            get
            {
                if (this.ActiveControl == null)
                    return string.Empty;
                return this.ActiveControlClient != null && this.ActiveObjectInfo != null ? this.ActiveObjectInfo.Text : this.GetToolTip(this.ActiveControl);
            }
        }

        protected virtual SuperToolTip CurrentSuperTip
        {
            get
            {
                return this.ActiveControl != null ? (this.ActiveControlClient != null && this.ActiveObjectInfo != null ? this.ActiveObjectInfo.SuperTip : this.GetSuperTip(this.ActiveControl)) : (this.ActiveObjectInfo != null ? this.ActiveObjectInfo.SuperTip : (SuperToolTip)null);
            }
        }

        protected virtual ToolTipAnchor CurrentToolTipAnchor
        {
            get
            {
                if (this.ActiveControl != null)
                {
                    if (this.ActiveControlClient != null && this.ActiveObjectInfo != null && this.ActiveObjectInfo.ToolTipAnchor != ToolTipAnchor.Default)
                        return this.ActiveObjectInfo.ToolTipAnchor;
                    ToolTipAnchor toolTipAnchor = this.GetToolTipAnchor(this.ActiveControl);
                    if (toolTipAnchor != ToolTipAnchor.Default)
                        return toolTipAnchor;
                }
                return this.ToolTipAnchor != ToolTipAnchor.Default ? this.ToolTipAnchor : ToolTipAnchor.Cursor;
            }
        }

        protected virtual string CurrentToolTipTitle
        {
            get
            {
                if (this.ActiveControl == null)
                    return string.Empty;
                return this.ActiveControlClient != null && this.ActiveObjectInfo != null ? this.ActiveObjectInfo.Title : this.GetTitle(this.ActiveControl);
            }
        }

        protected virtual ToolTipIconType CurrentToolTipIconType
        {
            get
            {
                if (this.ActiveControl == null)
                    return ToolTipIconType.None;
                return this.ActiveControlClient != null && this.ActiveObjectInfo != null ? this.ActiveObjectInfo.IconType : this.GetToolTipIconType(this.ActiveControl);
            }
        }

        protected virtual DefaultBoolean CurrentAllowHtmlText
        {
            get
            {
                if (this.ActiveControl == null)
                    return DefaultBoolean.Default;
                return this.ActiveControlClient != null && this.ActiveObjectInfo != null ? this.ActiveObjectInfo.AllowHtmlText : this.GetAllowHtmlText(this.ActiveControl);
            }
        }

        protected Control ActiveControl
        {
            get
            {
                return this.activeControl;
            }
            set
            {
                if (this.ActiveControl == value)
                    return;
                this.activeObject = (object)null;
                if (this.ActiveControl != null)
                    this.ActiveControl.HandleDestroyed -= new EventHandler(this.OnActiveControl_HandleDestroyed);
                Control activeControl = this.ActiveControl;
                this.UnsubscribeFormEvents();
                this.activeControl = value;
                this.activeObjectInfo = (ToolTipControlInfo)null;
                if (this.ActiveControl != null)
                    this.ActiveControl.HandleDestroyed += new EventHandler(this.OnActiveControl_HandleDestroyed);
                this.ActiveObjectChanged(activeControl, this.ActiveObject);
                this.SubscribeFormEvents();
            }
        }

        protected void OnActiveControl_HandleDestroyed(object sender, EventArgs e)
        {
            this.ActiveControl = (Control)null;
        }

        [Browsable(false)]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerActiveObject")]
        public object ActiveObject
        {
            get
            {
                return this.activeObject;
            }
        }

        protected void SetActiveObject(object value)
        {
            this.SetActiveObject(value, false);
        }

        protected void SetActiveObject(object value, bool force)
        {
            if (!force && (this.ActiveObject == value || object.Equals(this.ActiveObject, value)))
                return;
            object activeObject = this.ActiveObject;
            this.activeObject = value;
            this.ActiveObjectChanged(this.ActiveControl, activeObject);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IToolTipControlClient ActiveControlClient
        {
            get
            {
                if (this.ActiveControl != null)
                {
                    if (this.ActiveControl is IToolTipControlClient)
                        return this.ActiveControl as IToolTipControlClient;
                    if (this.ControlClientOwners.ContainsKey((object)this.ActiveControl))
                        return this.ControlClientOwners[(object)this.ActiveControl] as IToolTipControlClient;
                }
                return (IToolTipControlClient)null;
            }
        }

        private void ForceHideWithoutTimer()
        {
            this.HideHintCore();
            this.ClosingDelayTimer.Stop();
        }

        private bool IsToolTipVisible
        {
            get
            {
                return this.toolWindow != null && !this.toolWindow.IsDisposed && this.toolWindow.Visible;
            }
        }

        protected virtual void ActiveObjectChanged(Control prevControl, object prevObject)
        {
            if (!this.Active)
                return;
            if (this.ActiveControl != null || this.ActiveObject != null)
            {
                this.InitialTimer.Interval = prevObject == null && prevControl == null || !this.IsToolTipVisible ? this.InitialDelay : this.ReshowDelay;
                if (this.ActiveControlClient != null || this.ActiveObject != null)
                {
                    this.HideHint();
                    if (this.ActiveObject != null)
                    {
                        if (this.ActiveObjectInfo != null && this.ActiveObjectInfo.ImmediateToolTip)
                        {
                            this.ForceHideWithoutTimer();
                            this.ShowHint();
                        }
                        else
                        {
                            if (this.ActiveObjectInfo != null && this.ActiveObjectInfo.Interval > 0)
                                this.InitialTimer.Interval = this.ActiveObjectInfo.Interval;
                            this.ForceHideWithoutTimer();
                            this.InitialTimer.Start();
                        }
                    }
                    else
                    {
                        this.HideHint();
                        if (this.GetControlInfo(this.ActiveControl) == null)
                            return;
                        this.InitialTimer.Start();
                    }
                }
                else
                    this.InitialTimer.Start();
            }
            else
                this.HideHint();
        }

        protected virtual Timer InitialTimer
        {
            get
            {
                return this.initialTimer;
            }
        }

        protected virtual Timer AutoPopTimer
        {
            get
            {
                return this.autoPopTimer;
            }
        }

        protected virtual ClosingDelayTimer ClosingDelayTimer
        {
            get
            {
                return this.closingDelayTimer;
            }
        }

        protected internal virtual ToolTipControllerBaseWindow ToolWindow
        {
            get
            {
                if (this.toolWindow == null || this.toolWindow.IsDisposed)
                {
                    this.toolWindow = this.CreateToolWindow();
                    this.toolWindow.Paint += new PaintEventHandler(this.OnToolWindowPaint);
                    this.toolWindow.MouseLeave += new EventHandler(this.OnToolWindowMouseLeave);
                }
                return this.toolWindow;
            }
        }

        protected virtual bool IsMouseInActiveControl()
        {
            Point client = this.ActiveControl.PointToClient(Control.MousePosition);
            return client.X > 0 && client.Y > 0 && client.X < this.ActiveControl.Width && client.Y < this.ActiveControl.Height;
        }

        protected virtual void OnToolWindowMouseLeave(object sender, EventArgs e)
        {
            if (this.CloseOnClick == DefaultBoolean.True || this.CloseOnClick == DefaultBoolean.Default && this.ToolWindow.HasHyperlink || (this.ActiveControl == null || this.IsMouseInActiveControl()))
                return;
            this.HideHint();
            this.ActiveControl = (Control)null;
        }

        protected virtual void UpdateSuperTip(ToolTipControllerShowEventArgs eShow)
        {
            if (eShow.SuperTip == null || eShow.SuperTip.IsEmpty)
            {
                SuperToolTipSetupArgs info = new SuperToolTipSetupArgs();
                info.OwnerAllowHtmlText = this.AllowHtmlText;
                info.Title.Text = eShow.Title;
                info.Contents.Text = eShow.ToolTip;
                info.Contents.Images = eShow.ImageList;
                info.Contents.ImageIndex = eShow.ImageIndex;
                ToolTipControllerImage tipControllerImage = new ToolTipControllerImage(eShow);
                if (eShow.ToolTipImage != null)
                    info.Contents.Image = eShow.ToolTipImage;
                info.Contents.Icon = tipControllerImage.GetIcon();
                this.RestoreSuperTipMaxWidth();
                this.SuperTooltip.Setup(info);
                this.SuperTooltip.Appearance.Assign(this.Appearance);
            }
            else
            {
                this.SaveSuperTipMaxWidth();
                this.SuperTooltip.Assign(eShow.SuperTip);
                AppearanceHelper.Combine(this.SuperTooltip.Appearance, new AppearanceObject[1]
                {
          this.Appearance
                });
            }
            this.SuperTooltip.SetController(this);
            this.SuperTooltip.RightToLeft = eShow.RightToLeft;
            this.SuperTooltip.LookAndFeel = this.ViewInfo.GetLookAndFeel(eShow);
            this.SuperTooltip.AdjustSize();
        }

        protected void SaveSuperTipMaxWidth()
        {
            if (this.isSuperTipMaxWidthSaved)
                return;
            this.isSuperTipMaxWidthSaved = true;
            this.prevMaxWidth = this.SuperTooltip.MaxWidth;
        }

        protected void RestoreSuperTipMaxWidth()
        {
            if (!this.isSuperTipMaxWidthSaved)
                return;
            this.isSuperTipMaxWidthSaved = false;
            this.SuperTooltip.FixedTooltipWidth = false;
            this.SuperTooltip.MaxWidth = this.prevMaxWidth;
        }

        protected Point ShowHintCursorPosition { get; set; }

        protected virtual void ShowSuperTipCore()
        {
            if (this.SuperTooltip.Items.Count == 0)
                return;
            this.ToolWindow.Bounds = this.ViewInfo.Bounds;
            if (this.ToolWindow.Visible)
            {
                this.ToolWindow.Invalidate();
                this.ToolWindow.Update();
            }
            this.ToolWindow.Visible = true;
            this.ShowHintCursorPosition = Control.MousePosition;
        }

        private Color ConstrainBackColor(Color c)
        {
            return c.A < byte.MaxValue ? Color.FromArgb((int)byte.MaxValue, c) : c;
        }

        protected virtual void ShowRegularTipCore(ToolTipLocation oldLocation, bool needUpdateRegion)
        {
            if (!(this.ViewInfo is ToolTipViewInfo viewInfo))
                return;
            this.ToolWindow.Font = viewInfo.PaintAppearance.Font;
            this.ToolWindow.BackColor = this.ConstrainBackColor(viewInfo.PaintAppearance.BackColor);
            this.ToolWindow.ForeColor = viewInfo.PaintAppearance.ForeColor;
            this.ToolWindow.Location = this.ViewInfo.Bounds.Location;
            if (this.ToolWindow.Size != this.ViewInfo.Bounds.Size || needUpdateRegion || oldLocation != this.CurrentShowArgs.ToolTipLocation)
            {
                this.ToolWindow.Size = this.ViewInfo.Bounds.Size;
                this.SetToolWindowRegion(this.CurrentShowArgs);
            }
            if (this.ToolWindow.Visible)
            {
                this.ToolWindow.Invalidate();
            }
            else
            {
                this.ToolWindow.Visible = true;
                this.ShowHintCursorPosition = Control.MousePosition;
            }
        }

        protected virtual void ShowHintCore(ToolTipControllerShowEventArgs eShow)
        {
            this.ResetAutoPopupDelay();
            this.InitialTimer.Stop();
            this.ClosingDelayTimer.Stop();
            if (this.DesignMode)
                return;
            if (eShow.Show)
            {
                this.CurrentToolTipType = this.GetToolTipType(eShow);
                if (ToolTipController.activeController != null && ToolTipController.activeController != this)
                    ToolTipController.activeController.HideHint();
                ToolTipController.activeController = this;
                bool needUpdateRegion = this.CurrentShowArgs == null || (this.CurrentShowArgs.ShowBeak != eShow.ShowBeak || this.CurrentShowArgs.Rounded != eShow.Rounded);
                ToolTipLocation oldLocation = this.CurrentShowArgs != null ? this.CurrentShowArgs.GetToolTipLocation() : ToolTipLocation.BottomRight;
                this.currentShowArgs = eShow;
                this.ToolWindow.CloseOnClick = this.CloseOnClick;
                this.ToolWindow.Controller = this;
                if (this.GetUseSuperTips(eShow))
                    this.ShowSuperTipCore();
                else
                    this.ShowRegularTipCore(oldLocation, needUpdateRegion);
                if (!eShow.AutoHide)
                    return;
                this.AutoPopTimer.Interval = this.AutoPopDelay;
                this.AutoPopTimer.Start();
            }
            else
                this.HideHint();
        }

        protected virtual bool IsShowArgValid(ToolTipControllerShowEventArgs eShow)
        {
            return this.GetToolTipType(eShow) != ToolTipType.Standard || !this.IsHyperLinkAccessible || eShow.ToolTipImage != null || !string.IsNullOrEmpty(eShow.ToolTip);
        }

        public void ShowHint(ToolTipControllerShowEventArgs eShow, Point cursorPosition)
        {
            if (!this.IsShowArgValid(eShow))
            {
                this.InitialTimer.Stop();
            }
            else
            {
                Point oldCursorPosition = this.oldCursorPosition;
                this.OnBeforeShow(eShow);
                ToolTipControllerCalcSizeEventArgs e = new ToolTipControllerCalcSizeEventArgs(this.ActiveControl, this.ActiveObject, eShow);
                this.CurrentToolTipType = this.GetToolTipType(eShow);
                if (eShow.Show)
                {
                    e.Position = cursorPosition;
                    if (this.GetUseSuperTips(eShow))
                    {
                        ToolTipViewInfoSuperTip viewInfo = (ToolTipViewInfoSuperTip)this.ViewInfo;
                        this.UpdateSuperTip(eShow);
                        ToolTipLocation toolTipLocation = eShow.ToolTipLocation;
                        viewInfo.Calculate(eShow, Point.Empty);
                        eShow.ToolTipLocation = toolTipLocation;
                        e.Size = viewInfo.SuperTip.ContentBounds.Size;
                        Size size = e.Size;
                        this.OnCalcSize(e);
                        bool flag = e.ShowInfo != null && size != e.Size;
                        if (flag)
                        {
                            e.ShowInfo.CustomSize = e.Size;
                            this.UpdateSuperTipSize(eShow.SuperTip, e.Size);
                        }
                        e.ShowInfo.UseCustomSize = flag;
                        this.UpdateSuperTip(eShow);
                    }
                    bool? applyCursorOffset = new bool?();
                    if (this.ActiveObjectInfo != null)
                        applyCursorOffset = new bool?(this.ActiveObjectInfo.ApplyCursorOffset);
                    this.ViewInfo.Calculate(eShow, e.Size, e.Position, this.AllowHtmlText, applyCursorOffset);
                    if (!this.GetUseSuperTips(eShow))
                    {
                        ToolTipViewInfo viewInfo = this.ViewInfo as ToolTipViewInfo;
                        e.Size = viewInfo.ContentBounds.Size;
                        e.Position = cursorPosition;
                        this.oldCursorPosition = cursorPosition;
                        this.OnCalcSize(e);
                        this.ViewInfo.Calculate(eShow, e.Size, e.Position, this.AllowHtmlText, applyCursorOffset);
                    }
                }
                if (eShow.Show && this.CurrentShowArgs != null && (this.CurrentShowArgs.Equals((object)eShow) && oldCursorPosition.Equals((object)cursorPosition)))
                    return;
                this.ShowHintCore(eShow);
            }
        }

        protected virtual void UpdateSuperTipSize(SuperToolTip superTip, Size size)
        {
            if (superTip == null || superTip.IsEmpty || (size.IsEmpty || superTip.MaxWidth == size.Width))
                return;
            superTip.MaxWidth = size.Width;
        }

        public void ShowHint(ToolTipControllerShowEventArgs eShow)
        {
            this.ShowHint(eShow, Cursor.Position);
        }

        public void ShowHint(
          string toolTip,
          string title,
          ToolTipLocation toolTipLocation,
          Point cursorPosition)
        {
            ToolTipControllerShowEventArgs eShow = this.CloneCurrentShowArgs();
            eShow.ToolTip = toolTip;
            eShow.Title = title;
            eShow.ToolTipLocation = toolTipLocation;
            eShow.Rounded = this.Rounded;
            eShow.ToolTipStyle = this.ToolTipStyle;
            eShow.ShowBeak = this.ShowBeak;
            eShow.RoundRadius = this.RoundRadius;
            this.ShowHint(eShow, cursorPosition);
        }

        private string GetCurrentTitle()
        {
            return this.CurrentShowArgs == null ? string.Empty : this.CurrentShowArgs.Title;
        }

        private ToolTipLocation GetCurrentToolTipLocation()
        {
            return this.CurrentShowArgs == null ? this.ToolTipLocation : this.CurrentShowArgs.GetToolTipLocation();
        }

        private Point GetCurrentCursorPosition()
        {
            return this.CurrentShowArgs == null ? Cursor.Position : this.oldCursorPosition;
        }

        public void ShowHint(string toolTip, ToolTipLocation toolTipLocation, Point cursorPosition)
        {
            this.ShowHint(toolTip, this.GetCurrentTitle(), toolTipLocation, cursorPosition);
        }

        public void ShowHint(string toolTip, Point cursorPosition)
        {
            this.ShowHint(toolTip, this.GetCurrentToolTipLocation(), cursorPosition);
        }

        public void ShowHint(string toolTip, string title, Point cursorPosition)
        {
            this.ShowHint(toolTip, title, this.GetCurrentToolTipLocation(), cursorPosition);
        }

        public void ShowHint(string toolTip, ToolTipLocation toolTipLocation)
        {
            this.ShowHint(toolTip, toolTipLocation, this.GetCurrentCursorPosition());
        }

        public void ShowHint(string toolTip, string title, ToolTipLocation toolTipLocation)
        {
            this.ShowHint(toolTip, title, toolTipLocation, this.GetCurrentCursorPosition());
        }

        public void ShowHint(string toolTip)
        {
            this.ShowHint(toolTip, this.GetCurrentCursorPosition());
        }

        public void ShowHint(string toolTip, string title)
        {
            this.ShowHint(toolTip, title, this.GetCurrentCursorPosition());
        }

        public void ShowHint(
          string toolTip,
          string title,
          Control control,
          ToolTipLocation toolTipLocation)
        {
            ToolTipControllerShowEventArgs eShow = this.CloneCurrentShowArgs();
            eShow.ToolTip = toolTip;
            eShow.Title = title;
            eShow.ToolTipLocation = toolTipLocation;
            eShow.Rounded = this.Rounded;
            eShow.ToolTipStyle = this.ToolTipStyle;
            eShow.ShowBeak = this.ShowBeak;
            eShow.RoundRadius = this.RoundRadius;
            this.ShowHint(eShow, control);
        }

        public void ShowHint(string toolTip, Control control, ToolTipLocation toolTipLocation)
        {
            this.ShowHint(toolTip, this.GetCurrentTitle(), control, toolTipLocation);
        }

        public void ShowHint(ToolTipControllerShowEventArgs eShow, Control control)
        {
            this.ShowHint(eShow, this.ViewInfo.GetCursorPosition(eShow, control));
        }

        public void ShowHint(ToolTipControlInfo info)
        {
            this.SetActiveObjectInfo(info);
        }

        protected internal bool CanShowToolTip(ToolTipControllerShowEventArgs eShow)
        {
            bool flag1 = !string.IsNullOrEmpty(eShow.Title) || !string.IsNullOrEmpty(eShow.ToolTip);
            if (this.ToolTipType == ToolTipType.Standard || eShow.ToolTipType == ToolTipType.Standard)
                return flag1;
            bool flag2 = eShow.SuperTip != null && !eShow.SuperTip.IsEmpty || this.GetUseSuperTips(eShow) && flag1;
            if (this.ToolTipType == ToolTipType.SuperTip || eShow.ToolTipType == ToolTipType.SuperTip)
                return flag2;
            return flag2 || flag1;
        }

        protected void ShowHint()
        {
            Point cursorPosition = Cursor.Position;
            ToolTipControllerShowEventArgs showArgs = this.CreateShowArgs();
            showArgs.RightToLeft = this.ActiveControl != null && WindowsFormsSettings.GetIsRightToLeft(this.ActiveControl);
            showArgs.ObjectBounds = this.ActiveControl != null ? this.ActiveControl.RectangleToScreen(this.ActiveControl.ClientRectangle) : Rectangle.Empty;
            if (this.ActiveObjectInfo != null)
                showArgs.ObjectBounds = this.ActiveObjectInfo.ObjectBounds;
            showArgs.ToolTip = this.CurrentToolTipText;
            showArgs.Title = this.CurrentToolTipTitle;
            showArgs.IconType = this.CurrentToolTipIconType;
            showArgs.SuperTip = this.CurrentSuperTip;
            showArgs.ToolTipAnchor = this.CurrentToolTipAnchor;
            if (showArgs.SuperTip != null)
                showArgs.SuperTip.OwnerAllowHtmlText = this.AllowHtmlText;
            showArgs.AllowHtmlText = this.CurrentAllowHtmlText;
            showArgs.ToolTipType = this.ActiveObjectInfo != null ? this.ActiveObjectInfo.ToolTipType : ToolTipType.Default;
            if (this.ActiveObjectInfo != null)
                showArgs.ToolTipIndent = this.ActiveObjectInfo.ToolTipIndent;
            if (this.ActiveObjectInfo != null)
            {
                showArgs.ToolTipImage = this.activeObjectInfo.ToolTipImage;
                if (this.ActiveObjectInfo.ToolTipLocation != ToolTipLocation.Default)
                    showArgs.ToolTipLocation = this.activeObjectInfo.ToolTipLocation;
                if (this.ActiveObjectInfo.ToolTipPosition.X > -10000)
                    cursorPosition = this.ActiveObjectInfo.ToolTipPosition;
            }
            if (this.ActiveObjectInfo != null)
            {
                if (showArgs.ToolTip == string.Empty)
                    showArgs.ToolTip = this.ActiveObjectInfo.Text;
                if (showArgs.Title == string.Empty)
                    showArgs.Title = this.ActiveObjectInfo.Title;
                showArgs.Show = this.CanShowToolTip(showArgs);
                showArgs.SelectedControl = this.ActiveControl;
                showArgs.SelectedObject = this.ActiveObjectInfo.Object;
                showArgs.IconType = this.ActiveObjectInfo.IconType;
            }
            else
            {
                showArgs.SelectedControl = this.ActiveControl;
                showArgs.SelectedObject = this.ActiveObject;
            }
            this.ShowHint(showArgs, cursorPosition);
        }

        public void HideHint()
        {
            if (this.IsHyperLinkAccessible)
                this.ClosingDelayTimer.Start((MethodInvoker)(() =>
                {
                    if (this.IsMouseInTooltipForm())
                        return;
                    this.HideHintCore();
                }));
            else
                this.HideHintCore();
        }

        public void HideHintCore()
        {
            this.currentShowArgs = (ToolTipControllerShowEventArgs)null;
            this.oldCursorPosition = Point.Empty;
            this.HideToolWindow();
            this.AutoPopTimer.Stop();
            this.InitialTimer.Stop();
            this.ClosingDelayTimer.Stop();
            this.HoverTimer.Stop();
        }

        private bool IsHintVisible
        {
            get
            {
                return this.toolWindow != null && this.ToolWindow.Visible;
            }
        }

        protected virtual void HideToolWindow()
        {
            if (this.toolWindow == null || this.toolWindow.IsDisposed)
                return;
            int num = this.ToolWindow.Visible ? 1 : 0;
            this.ToolWindow.Visible = false;
        }

        [DXCategory("Events")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerGetActiveObjectInfo")]
        public event ToolTipControllerGetActiveObjectInfoEventHandler GetActiveObjectInfo
        {
            add
            {
                this.Events.AddHandler(ToolTipController.getActiveObjectInfo, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(ToolTipController.getActiveObjectInfo, (Delegate)value);
            }
        }

        [DevExpressUtilsLocalizedDescription("ToolTipControllerBeforeShow")]
        [DXCategory("Events")]
        public event ToolTipControllerBeforeShowEventHandler BeforeShow
        {
            add
            {
                this.Events.AddHandler(ToolTipController.beforeShow, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(ToolTipController.beforeShow, (Delegate)value);
            }
        }

        [DevExpressUtilsLocalizedDescription("ToolTipControllerCalcSize")]
        [DXCategory("Events")]
        public event ToolTipControllerCalcSizeEventHandler CalcSize
        {
            add
            {
                this.Events.AddHandler(ToolTipController.calcSize, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(ToolTipController.calcSize, (Delegate)value);
            }
        }

        [DXCategory("Events")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerCustomDraw")]
        public event ToolTipControllerCustomDrawEventHandler CustomDraw
        {
            add
            {
                this.Events.AddHandler(ToolTipController.customDraw, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(ToolTipController.customDraw, (Delegate)value);
            }
        }

        [DevExpressUtilsLocalizedDescription("ToolTipControllerHyperlinkClick")]
        [DXCategory("Action")]
        public event HyperlinkClickEventHandler HyperlinkClick
        {
            add
            {
                this.Events.AddHandler(ToolTipController.hyperlinkClick, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(ToolTipController.hyperlinkClick, (Delegate)value);
            }
        }

        protected virtual void OnBeforeShow(ToolTipControllerShowEventArgs e)
        {
            ToolTipControllerBeforeShowEventHandler showEventHandler = (ToolTipControllerBeforeShowEventHandler)this.Events[ToolTipController.beforeShow];
            if (showEventHandler != null)
                showEventHandler((object)this, e);
            if (!(this.ActiveControlClient is IToolTipControlClientEx))
                return;
            ((IToolTipControlClientEx)this.ActiveControlClient).OnBeforeShow(e);
        }

        protected virtual void OnCalcSize(ToolTipControllerCalcSizeEventArgs e)
        {
            ToolTipControllerCalcSizeEventHandler sizeEventHandler = (ToolTipControllerCalcSizeEventHandler)this.Events[ToolTipController.calcSize];
            if (sizeEventHandler == null)
                return;
            sizeEventHandler((object)this, e);
        }

        protected virtual void OnCustomDraw(ToolTipControllerCustomDrawEventArgs e)
        {
            ToolTipControllerCustomDrawEventHandler drawEventHandler = (ToolTipControllerCustomDrawEventHandler)this.Events[ToolTipController.customDraw];
            if (drawEventHandler == null)
                return;
            drawEventHandler((object)this, e);
        }

        protected virtual void OnGetActiveObjectInfo(ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            ToolTipControllerGetActiveObjectInfoEventHandler infoEventHandler = (ToolTipControllerGetActiveObjectInfoEventHandler)this.Events[ToolTipController.getActiveObjectInfo];
            if (infoEventHandler == null)
                return;
            infoEventHandler((object)this, e);
        }

        protected virtual void OnHyperlinkClick(HyperlinkClickEventArgs e)
        {
            HyperlinkClickEventHandler clickEventHandler = (HyperlinkClickEventHandler)this.Events[ToolTipController.hyperlinkClick];
            if (clickEventHandler == null)
                return;
            clickEventHandler((object)this, e);
        }

        [DevExpressUtilsLocalizedDescription("ToolTipControllerToolTipType")]
        [DXCategory("ToolTip")]
        [DefaultValue(ToolTipType.Default)]
        public ToolTipType ToolTipType
        {
            get
            {
                return this.toolTipType;
            }
            set
            {
                if (this.ToolTipType == value)
                    return;
                this.toolTipType = value;
                this.HideHint();
                this.DestroyToolWindow();
                this.CurrentToolTipType = this.GetToolTipType();
            }
        }

        [DXCategory("Behavior")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerActive")]
        [DefaultValue(true)]
        public bool Active
        {
            get
            {
                return this.active;
            }
            set
            {
                if (value == this.Active)
                    return;
                this.active = value;
                if (value)
                    return;
                this.HideHint();
            }
        }

        [DevExpressUtilsLocalizedDescription("ToolTipControllerInitialDelay")]
        [DXCategory("Appearance")]
        [DefaultValue(500)]
        public int InitialDelay
        {
            get
            {
                return this.initialDelay;
            }
            set
            {
                if (value < 1)
                    value = 1;
                if (this.InitialDelay == value)
                    return;
                this.initialDelay = value;
                this.InitialTimer.Interval = this.InitialDelay;
            }
        }

        [DXCategory("Behavior")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerReshowDelay")]
        [DefaultValue(100)]
        public int ReshowDelay
        {
            get
            {
                return this.reshowDelay;
            }
            set
            {
                if (value < 1)
                    value = 1;
                if (this.ReshowDelay == value)
                    return;
                this.reshowDelay = value;
            }
        }

        [DXCategory("Behavior")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerAutoPopDelay")]
        [DefaultValue(5000)]
        public int AutoPopDelay
        {
            get
            {
                return this.autoPopDelay;
            }
            set
            {
                if (value < 1)
                    value = 1;
                if (this.AutoPopDelay == value)
                    return;
                this.autoPopDelay = value;
                this.AutoPopTimer.Interval = this.AutoPopDelay;
            }
        }

        public void ResetAutoPopupDelay()
        {
            if (!this.AutoPopTimer.Enabled)
                return;
            this.AutoPopTimer.Enabled = false;
            this.AutoPopTimer.Enabled = true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseWindowStyle
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppearanceObject Style
        {
            get
            {
                return this.Appearance;
            }
            set
            {
                if (value == null)
                    return;
                this.Appearance.Assign(value);
            }
        }

        private void ResetAppearance()
        {
            this.Appearance.Reset();
        }

        private bool ShouldSerializeAppearance()
        {
            return this.Appearance.ShouldSerialize();
        }

        [DXCategory("Appearance")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerAppearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AppearanceObject Appearance
        {
            get
            {
                return this.showArgs.Appearance;
            }
        }

        private void ResetAppearanceTitle()
        {
            this.AppearanceTitle.Reset();
        }

        private bool ShouldSerializeAppearanceTitle()
        {
            return this.AppearanceTitle.ShouldSerialize();
        }

        [DXCategory("Appearance")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerAppearanceTitle")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AppearanceObject AppearanceTitle
        {
            get
            {
                return this.showArgs.AppearanceTitle;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool IsDefaultToolTipLocation
        {
            get
            {
                return this.showArgs.ToolTipLocation == ToolTipLocation.Default;
            }
        }

        [DXCategory("ToolTip")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerToolTipLocation")]
        [DefaultValue(ToolTipLocation.BottomRight)]
        public ToolTipLocation ToolTipLocation
        {
            get
            {
                return this.showArgs.GetToolTipLocation();
            }
            set
            {
                this.showArgs.ToolTipLocation = value;
            }
        }

        [DXCategory("Appearance")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerShowBeak")]
        [DefaultValue(false)]
        public bool ShowBeak
        {
            get
            {
                return this.showArgs.ShowBeak;
            }
            set
            {
                this.showArgs.ShowBeak = value;
            }
        }

        [DevExpressUtilsLocalizedDescription("ToolTipControllerRounded")]
        [DXCategory("Appearance")]
        [DefaultValue(false)]
        public bool Rounded
        {
            get
            {
                return this.showArgs.Rounded;
            }
            set
            {
                this.showArgs.Rounded = value;
            }
        }

        [DXCategory("ToolTip")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerToolTipStyle")]
        [DefaultValue(ToolTipStyle.Default)]
        public ToolTipStyle ToolTipStyle
        {
            get
            {
                return this.showArgs.ToolTipStyle;
            }
            set
            {
                this.showArgs.ToolTipStyle = value;
            }
        }

        [DXCategory("Appearance")]
        [DefaultValue(7)]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerRoundRadius")]
        public int RoundRadius
        {
            get
            {
                return this.showArgs.RoundRadius;
            }
            set
            {
                this.showArgs.RoundRadius = value;
            }
        }

        [DefaultValue(ToolTipIconType.None)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This property is obsolete. Each BaseControl descendant has the BaseControl.ToolTipIconType property which can be used to specify the tooltip icon type for this editor. Thus it's possible to provide different icon types for different controls.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerIconType")]
        public ToolTipIconType IconType
        {
            get
            {
                return this.showArgs.IconType;
            }
            set
            {
            }
        }

        [DefaultValue(ToolTipIconSize.Small)]
        [DXCategory("Appearance")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerIconSize")]
        public ToolTipIconSize IconSize
        {
            get
            {
                return this.showArgs.IconSize;
            }
            set
            {
                this.showArgs.IconSize = value;
            }
        }

        [DXCategory("Appearance")]
        [TypeConverter(typeof(ImageCollectionImagesConverter))]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerImageList")]
        [DefaultValue(null)]
        public virtual object ImageList
        {
            get
            {
                return this.showArgs.ImageList;
            }
            set
            {
                this.showArgs.ImageList = value;
            }
        }

        [DevExpressUtilsLocalizedDescription("ToolTipControllerImageIndex")]
        [Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor))]
        [DevExpress.Utils.ImageList("ImageList")]
        [DXCategory("Appearance")]
        [DefaultValue(-1)]
        public int ImageIndex
        {
            get
            {
                return this.showArgs.ImageIndex;
            }
            set
            {
                this.showArgs.ImageIndex = value;
            }
        }

        [DXCategory("Appearance")]
        [DefaultValue(true)]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerShowShadow")]
        public bool ShowShadow
        {
            get
            {
                return this.showShadow;
            }
            set
            {
                if (this.showShadow == value)
                    return;
                this.showShadow = value;
                this.DestroyToolWindow();
            }
        }

        [DefaultValue(false)]
        [DXCategory("Appearance")]
        [DevExpressUtilsLocalizedDescription("ToolTipControllerAllowHtmlText")]
        public bool AllowHtmlText
        {
            get
            {
                return this.allowHtmlText;
            }
            set
            {
                if (this.AllowHtmlText == value)
                    return;
                this.allowHtmlText = value;
                this.DestroyToolWindow();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DXCategory("ToolTip")]
        public int SuperTipMaxWidth
        {
            set
            {
                if (this.SuperTooltip == null || value <= 0)
                    return;
                this.SuperTooltip.MaxWidth = value;
            }
        }

        protected ToolTipControllerShowEventArgs CurrentShowArgs
        {
            get
            {
                return this.currentShowArgs;
            }
        }

        protected ToolTipControllerShowEventArgs CloneCurrentShowArgs()
        {
            return this.CurrentShowArgs != null ? (ToolTipControllerShowEventArgs)((ICloneable)this.CurrentShowArgs).Clone() : this.CreateShowArgs();
        }

        public ToolTipControllerShowEventArgs CreateShowArgs()
        {
            return (ToolTipControllerShowEventArgs)((ICloneable)this.showArgs).Clone();
        }

        protected virtual void SetToolWindowRegion(ToolTipControllerShowEventArgs eShow)
        {
            if (!(this.ViewInfo is ToolTipViewInfo viewInfo))
                return;
            if (eShow.Rounded || eShow.ShowBeak)
            {
                ToolTipGraphicsPath toolTipGraphicsPath = new ToolTipGraphicsPath(viewInfo.ContentBounds, eShow.GetToolTipLocation(), eShow.Rounded ? eShow.RoundRadius : 0, eShow.ShowBeak ? viewInfo.OutOffHeight : 0, eShow.ShowBeak ? viewInfo.OutOffWidth : 0);
                this.ToolWindow.Path = toolTipGraphicsPath.Path;
                this.ToolWindow.Region = BitmapToRegion.ConvertPathToRegion(toolTipGraphicsPath.Path);
            }
            else
            {
                if (this.ToolWindow.Region != null)
                    this.ToolWindow.Region = (Region)null;
                this.ToolWindow.Path = (GraphicsPath)null;
            }
        }

        [DevExpressUtilsLocalizedDescription("ToolTipControllerCloseOnClick")]
        [DefaultValue(DefaultBoolean.Default)]
        [DXCategory("Behavior")]
        public virtual DefaultBoolean CloseOnClick
        {
            get
            {
                return this.closeOnClick;
            }
            set
            {
                this.closeOnClick = value;
            }
        }

        internal void OnHyperlinkClick(StringBlock block)
        {
            this.OnHyperlinkClick(new HyperlinkClickEventArgs()
            {
                Text = block.Text,
                Link = block.Link
            });
        }

        protected class ControlInfo
        {
            private DefaultBoolean allowHtmlText;
            private string text;
            private string title;
            private ToolTipIconType iconType;
            private SuperToolTip superTip;
            private static ToolTipController.ControlInfo empty;

            public ControlInfo(string text)
              : this(text, ToolTipIconType.None)
            {
            }

            public ControlInfo(string text, ToolTipIconType iconType)
              : this(text, "", iconType)
            {
            }

            public ControlInfo(string text, string title, ToolTipIconType iconType)
              : this(text, title, iconType, DefaultBoolean.Default)
            {
            }

            public ControlInfo(
              string text,
              string title,
              ToolTipIconType iconType,
              DefaultBoolean allowHtmlText)
            {
                if (text == null)
                    text = string.Empty;
                if (title == null)
                    title = string.Empty;
                this.text = text;
                this.title = title;
                this.iconType = iconType;
                this.allowHtmlText = allowHtmlText;
            }

            public SuperToolTip SuperTip
            {
                get
                {
                    return this.superTip;
                }
                set
                {
                    if (value != null && value.IsEmpty)
                        value = (SuperToolTip)null;
                    this.superTip = value;
                }
            }

            public string Text
            {
                get
                {
                    return this.text;
                }
                set
                {
                    if (value == null)
                        value = string.Empty;
                    this.text = value;
                }
            }

            public string Title
            {
                get
                {
                    return this.title;
                }
                set
                {
                    if (value == null)
                        value = string.Empty;
                    this.title = value;
                }
            }

            public ToolTipIconType IconType
            {
                get
                {
                    return this.iconType;
                }
                set
                {
                    this.iconType = value;
                }
            }

            public DefaultBoolean AllowHtmlText
            {
                get
                {
                    return this.allowHtmlText;
                }
                set
                {
                    this.allowHtmlText = value;
                }
            }

            public bool IsEmpty
            {
                get
                {
                    if (this.Text.Length != 0 || this.Title.Length != 0 || this.IconType != ToolTipIconType.None)
                        return false;
                    return this.SuperTip == null || this.SuperTip.IsEmpty;
                }
            }

            [DefaultValue(ToolTipAnchor.Default)]
            public ToolTipAnchor ToolTipAnchor { get; set; }

            public static ToolTipController.ControlInfo Empty
            {
                get
                {
                    if (ToolTipController.ControlInfo.empty == null)
                        ToolTipController.ControlInfo.empty = new ToolTipController.ControlInfo(string.Empty);
                    return ToolTipController.ControlInfo.empty;
                }
            }
        }

        internal class ToolTipControllerHelper
        {
            public static bool ShouldUseVistaStyleTooltip(ToolTipStyle style)
            {
                return style != ToolTipStyle.WindowsXP && NativeVista.IsVistaOrLater && !ToolTipController.ToolTipControllerHelper.IsXPVisualStyle;
            }

            internal static bool IsXPVisualStyle
            {
                get
                {
                    return NativeVista.GetCurrentThemeName(new StringBuilder(260, 260), 260, IntPtr.Zero, 0, IntPtr.Zero, 0) < 0;
                }
            }
        }
    }


    [SmartTagAction(typeof(ControlActions), "UndockFromParentContainer", "Undock from parent container", SmartTagActionType.CloseAfterExecute)]
    [SmartTagFilter(typeof(ControlFilter))]
    [SmartTagAction(typeof(ControlActions), "DockInParentContainer", "Dock in parent container", SmartTagActionType.CloseAfterExecute)]
    [ToolboxItem(false)]
    [SmartTagSupport(typeof(BaseControlBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto)]
    public abstract class BaseControl : ControlBase, IGHFocusController, IToolTipControlClient, ISupportStyleController, IGHResizableControl, ISupportToolTipsForm
    {
        private SizeF scaleFactor = new SizeF(1f, 1f);
        private Size prevSizeableMinSize = Size.Empty;
        private Size prevSizeableMaxSize = Size.Empty;
        private static readonly object sizeableChanged = new object();
        internal const int CS_VREDRAW = 1;
        internal const int CS_HREDRAW = 2;
        private BorderStyles borderStyle;
        private bool _focusOnMouseDown;
        protected bool fShowToolTips;
        private IStyleController fStyleController;
        private string _toolTip;
        private string _toolTipTitle;
        private ToolTipIconType _toolTipIconType;
        private ToolTipController toolTipController;
        protected bool fDisposing;
        private BaseAccessible dxAccessible;
        protected internal bool shouldRaiseSizeableChanged;
        private int lockFireChanged;
        private bool autoSizeInLayoutControl;

        static BaseControl()
        {
            DXAssemblyResolver.Init();
        }

        public BaseControl()
        {
            this.SetStyle(ControlStyles.UserMouse, true);
            ToolTipController.DefaultController.AddClientControl((Control)this);
            this._toolTipTitle = this._toolTip = "";
            this._toolTipIconType = ToolTipIconType.None;
            this.allowHtmlText = DefaultBoolean.Default;
            this.fShowToolTips = true;
            this.borderStyle = BorderStyles.Default;
            this.fStyleController = (IStyleController)null;
            this.lookAndFeel = (UserLookAndFeel)null;
            this._focusOnMouseDown = true;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF ScaleFactor
        {
            get
            {
                return this.scaleFactor;
            }
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            this.scaleFactor = factor;
            base.ScaleControl(factor, specified);
        }

        internal void UpdateScaleFactor(SizeF factor)
        {
            this.scaleFactor = factor;
        }

        protected internal Size ScaleSize(Size size)
        {
            return RectangleHelper.ScaleSize(size, this.ScaleFactor);
        }

        protected internal int ScaleHorizontal(int width)
        {
            return RectangleHelper.ScaleHorizontal(width, this.ScaleFactor.Width);
        }

        protected internal int ScaleVertical(int height)
        {
            return RectangleHelper.ScaleVertical(height, this.ScaleFactor.Height);
        }

        protected internal int DeScaleHorizontal(int width)
        {
            return RectangleHelper.DeScaleHorizontal(width, this.ScaleFactor.Width);
        }

        protected internal int DeScaleVertical(int height)
        {
            return RectangleHelper.DeScaleVertical(height, this.ScaleFactor.Height);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public BaseAccessible GetAccessible()
        {
            return this.DXAccessible;
        }

        protected internal virtual BaseAccessible DXAccessible
        {
            get
            {
                if (this.dxAccessible == null)
                    this.dxAccessible = this.CreateAccessibleInstance();
                return this.dxAccessible;
            }
        }

        protected virtual bool IsDisposing
        {
            get
            {
                return this.fDisposing;
            }
        }

        protected virtual BaseAccessible CreateAccessibleInstance()
        {
            return (BaseAccessible)null;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return this.DXAccessible == null ? base.CreateAccessibilityInstance() : this.DXAccessible.Accessible;
        }

        bool ISupportLookAndFeel.IgnoreChildren
        {
            get
            {
                return false;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ClassStyle |= 3;
                return createParams;
            }
        }

        public virtual Size CalcBestSize()
        {
            Size defaultSize = this.DefaultSize;
            if (this.ViewInfo == null)
                return defaultSize;
            if (!this.ViewInfo.IsReady)
            {
                this.ViewInfo.UpdateAppearances();
                this.ViewInfo.UpdatePaintAppearance();
            }
            GraphicsInfo graphicsInfo = new GraphicsInfo();
            graphicsInfo.AddGraphics((Graphics)null);
            try
            {
                return this.ViewInfo.CalcBestFit(graphicsInfo.Graphics);
            }
            finally
            {
                graphicsInfo.ReleaseGraphics();
            }
        }

        protected void SetBackColor(Color color)
        {
            this.BackColor = color;
        }

        protected internal bool ShowKeyboardCuesCore
        {
            get
            {
                return this.ShowKeyboardCues;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(75, 14);
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.fDisposing = true;
            if (disposing)
            {
                this.StyleController = (IStyleController)null;
                if (this.lookAndFeel != null)
                {
                    this.lookAndFeel.StyleChanged -= new EventHandler(this.OnLookAndFeelChanged);
                    this.lookAndFeel.Dispose();
                }
                this.ToolTipController = (ToolTipController)null;
                ToolTipController.DefaultController.RemoveClientControl((Control)this);
                XtraAnimator.RemoveObject((ISupportXtraAnimation)this);
            }
            base.Dispose(disposing);
        }

        [DefaultValue(null)]
        [DevExpressXtraEditorsLocalizedDescription("BaseControlToolTipController")]
        [DXCategory("Appearance")]
        public ToolTipController ToolTipController
        {
            get
            {
                return this.toolTipController;
            }
            set
            {
                if (this.ToolTipController == value)
                    return;
                if (this.ToolTipController != null)
                    this.ToolTipController.RemoveClientControl((Control)this);
                this.toolTipController = value;
                if (this.ToolTipController != null)
                {
                    ToolTipController.DefaultController.RemoveClientControl((Control)this);
                    this.ToolTipController.AddClientControl((Control)this);
                }
                else
                    ToolTipController.DefaultController.AddClientControl((Control)this);
            }
        }

        [DevExpressXtraEditorsLocalizedDescription("BaseControlShowToolTips")]
        [DefaultValue(true)]
        [DXCategory("ToolTip")]
        public virtual bool ShowToolTips
        {
            get
            {
                return this.fShowToolTips;
            }
            set
            {
                this.fShowToolTips = value;
            }
        }

        [DXCategory("ToolTip")]
        [DevExpressXtraEditorsLocalizedDescription("BaseControlToolTip")]
        [Localizable(true)]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        public virtual string ToolTip
        {
            get
            {
                return this._toolTip;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (this.ToolTip == value)
                    return;
                this._toolTip = value;
            }
        }

        [DevExpressXtraEditorsLocalizedDescription("BaseControlToolTipTitle")]
        [Localizable(true)]
        [DefaultValue("")]
        [DXCategory("ToolTip")]
        public virtual string ToolTipTitle
        {
            get
            {
                return this._toolTipTitle;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (this.ToolTipTitle == value)
                    return;
                this._toolTipTitle = value;
            }
        }

        [DevExpressXtraEditorsLocalizedDescription("BaseControlAllowHtmlTextInToolTip")]
        [DefaultValue(DefaultBoolean.Default)]
        [Localizable(true)]
        [DXCategory("ToolTip")]
        public virtual DefaultBoolean AllowHtmlTextInToolTip
        {
            get
            {
                return this.allowHtmlText;
            }
            set
            {
                if (this.AllowHtmlTextInToolTip == value)
                    return;
                this.allowHtmlText = value;
            }
        }

        internal bool ShouldSerializeSuperTip()
        {
            return this.SuperTip != null && !this.SuperTip.IsEmpty;
        }

        [DXCategory("ToolTip")]
        [Localizable(true)]
        [Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, DevExpress.XtraEditors.v17.2.Design", typeof(UITypeEditor))]
        [DevExpressXtraEditorsLocalizedDescription("BaseControlSuperTip")]
        public virtual SuperToolTip SuperTip
        {
            get
            {
                return this.superTip;
            }
            set
            {
                this.superTip = value;
            }
        }

        public void ResetSuperTip()
        {
            this.SuperTip = (SuperToolTip)null;
        }

        [DefaultValue(ToolTipAnchor.Default)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual ToolTipAnchor ToolTipAnchor { get; set; }

        [DefaultValue(ToolTipIconType.None)]
        [DXCategory("ToolTip")]
        [Localizable(true)]
        [DevExpressXtraEditorsLocalizedDescription("BaseControlToolTipIconType")]
        public virtual ToolTipIconType ToolTipIconType
        {
            get
            {
                return this._toolTipIconType;
            }
            set
            {
                this._toolTipIconType = value;
            }
        }

        ToolTipControlInfo IToolTipControlClient.GetObjectInfo(
          Point point)
        {
            return this.GetToolTipInfo(point);
        }

        bool IToolTipControlClient.ShowToolTips
        {
            get
            {
                return this.ShowToolTipsCore;
            }
        }

        protected ToolTipAnchor GetToolTipAnchor()
        {
            if (this.ToolTipAnchor != ToolTipAnchor.Default)
                return this.ToolTipAnchor;
            if (this.ToolTipController != null && this.ToolTipController.ToolTipAnchor != ToolTipAnchor.Default)
                return this.ToolTipController.ToolTipAnchor;
            return ToolTipController.DefaultController.ToolTipAnchor != ToolTipAnchor.Object ? ToolTipAnchor.Cursor : ToolTipAnchor.Object;
        }

        protected virtual ToolTipControlInfo GetToolTipInfo(Point point)
        {
            if (this.IsDesignMode || !this.ShowToolTips || this.ToolTip.Length <= 0 && !this.ShouldSerializeSuperTip())
                return (ToolTipControlInfo)null;
            ToolTipControlInfo toolTipControlInfo = new ToolTipControlInfo((object)this, this.ToolTip, this.ToolTipTitle, this.ToolTipIconType);
            toolTipControlInfo.AllowHtmlText = this.AllowHtmlTextInToolTip;
            toolTipControlInfo.SuperTip = this.SuperTip;
            toolTipControlInfo.ToolTipAnchor = this.GetToolTipAnchor();
            if (toolTipControlInfo.ToolTipAnchor == ToolTipAnchor.Object && this.IsHandleCreated)
                toolTipControlInfo.ObjectBounds = this.RectangleToScreen(this.ClientRectangle);
            return toolTipControlInfo;
        }

        protected virtual bool ShowToolTipsCore
        {
            get
            {
                return this.ShowToolTips;
            }
        }

        protected internal bool FocusOnMouseDown
        {
            get
            {
                return ((IGHFocusController)this).FocusOnMouseDown;
            }
            set
            {
                ((IGHFocusController)this).FocusOnMouseDown = value;
            }
        }

        bool IGHFocusController.FocusOnMouseDown
        {
            get
            {
                return this._focusOnMouseDown;
            }
            set
            {
                this._focusOnMouseDown = value;
            }
        }

        [Browsable(false)]
        public virtual bool IsDesignMode
        {
            get
            {
                return this.DesignMode;
            }
        }

        private bool ShouldSerializeLookAndFeel()
        {
            return this.StyleController == null && this.LookAndFeel != null && this.LookAndFeel.ShouldSerialize();
        }

        [DevExpressXtraEditorsLocalizedDescription("BaseControlLookAndFeel")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [DXCategory("Appearance")]
        public virtual UserLookAndFeel LookAndFeel
        {
            get
            {
                if (this.StyleController != null && this.StyleController.LookAndFeel != null)
                    return this.StyleController.LookAndFeel;
                if (this.lookAndFeel == null)
                    this.CreateLookAndFeel();
                return this.lookAndFeel;
            }
        }

        protected virtual void OnLookAndFeelChanged(object sender, EventArgs e)
        {
            if (this.ViewInfo != null)
            {
                this.ViewInfo.ResetAppearanceDefault();
                this.ViewInfo.UpdatePaintersCore();
            }
            this.CheckRightToLeft();
            this.OnPropertiesChanged(true);
        }

        protected override void OnRightToLeftChanged()
        {
            base.OnRightToLeftChanged();
            this.LayoutChanged();
        }

        protected virtual void CreateLookAndFeel()
        {
            this.lookAndFeel = (UserLookAndFeel)new ControlUserLookAndFeel((Control)this);
            this.lookAndFeel.StyleChanged += new EventHandler(this.OnLookAndFeelChanged);
        }

        public override void Refresh()
        {
            this.LayoutChanged();
            base.Refresh();
        }

        protected internal virtual void RemoveXtraAnimator()
        {
            XtraAnimator.RemoveObject((ISupportXtraAnimation)this);
        }

        protected internal virtual void LayoutChangedCore(bool invalidateVisual)
        {
            if (this.IsDisposing)
                return;
            this.RemoveXtraAnimator();
            if (this.IsLayoutLocked || !this.IsHandleCreated)
            {
                this.ViewInfo.IsReady = false;
            }
            else
            {
                this.ViewInfo.CalcViewInfo((Graphics)null, Control.MouseButtons, this.PointToClient(Control.MousePosition), this.ClientRectangle);
                if (!invalidateVisual)
                    return;
                this.Invalidate();
            }
        }

        protected internal virtual void LayoutChanged()
        {
            this.LayoutChangedCore(true);
        }

        [DevExpressXtraEditorsLocalizedDescription("BaseControlStyleController")]
        [DXCategory("Appearance")]
        [DefaultValue(null)]
        public virtual IStyleController StyleController
        {
            get
            {
                return this.fStyleController;
            }
            set
            {
                if (this.StyleController == value)
                    return;
                if (this.StyleController != null)
                {
                    this.StyleController.PropertiesChanged -= new EventHandler(this.OnStyleController_PropertiesChanged);
                    this.StyleController.Disposed -= new EventHandler(this.OnStyleController_Disposed);
                }
                this.fStyleController = value;
                if (this.StyleController != null)
                {
                    this.StyleController.PropertiesChanged += new EventHandler(this.OnStyleController_PropertiesChanged);
                    this.StyleController.Disposed += new EventHandler(this.OnStyleController_Disposed);
                }
                this.OnStyleControllerChanged();
            }
        }

        protected BorderStyles BaseBorderStyle
        {
            get
            {
                return this.borderStyle;
            }
        }

        private bool ShouldSerializeBorderStyle()
        {
            return this.StyleController == null && this.BorderStyle != BorderStyles.Default;
        }

        [DXCategory("Appearance")]
        [DevExpressXtraEditorsLocalizedDescription("BaseControlBorderStyle")]
        public virtual BorderStyles BorderStyle
        {
            get
            {
                return this.StyleController != null ? this.StyleController.BorderStyle : this.borderStyle;
            }
            set
            {
                if (this.BorderStyle == value)
                    return;
                this.borderStyle = value;
                this.OnPropertiesChanged(true);
            }
        }

        internal void OnPropertiesChanged(bool shrpc)
        {
            this.shouldRaiseSizeableChanged = shrpc;
            this.OnPropertiesChanged();
        }

        protected internal virtual void OnPropertiesChanged()
        {
            if (this.IsDisposing)
                return;
            if (this.shouldRaiseSizeableChanged)
            {
                this.ViewInfo.IsReady = false;
                this.RaiseSizeableChanged();
                this.shouldRaiseSizeableChanged = false;
            }
            this.FireChanged();
            this.LayoutChanged();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public BaseControlViewInfo GetViewInfo()
        {
            return this.ViewInfo;
        }

        protected internal abstract BaseControlPainter Painter { get; }

        protected internal abstract BaseControlViewInfo ViewInfo { get; }

        protected virtual void OnStyleControllerChanged()
        {
            this.OnPropertiesChanged(true);
        }

        protected Color GetColor(Color color, Color defaultColor)
        {
            return this.GetColor(color, defaultColor, true);
        }

        protected Color GetColor(Color color, Color defaultColor, bool useColor)
        {
            return !(color == Color.Empty) && useColor ? color : defaultColor;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.ViewInfo.Bounds.Size == this.ClientSize)
                return;
            this.LayoutChanged();
        }

        protected virtual void UpdateViewInfo(Graphics g)
        {
            if (this.ViewInfo.IsReady || !this.IsHandleCreated || this.IsDisposed)
                return;
            this.OnBeforeUpdateViewInfo();
            this.ViewInfo.CalcViewInfo(g, Control.MouseButtons, this.PointToClient(Control.MousePosition), this.ClientRectangle);
            this.OnAfterUpdateViewInfo();
        }

        protected virtual void OnBeforeUpdateViewInfo()
        {
        }

        protected virtual void OnAfterUpdateViewInfo()
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ControlState.CheckPaintError((Control)this);
            this.UpdateViewInfo(e.Graphics);
            GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(e));
            try
            {
                ControlGraphicsInfoArgs info = new ControlGraphicsInfoArgs(this.ViewInfo, cache, this.ViewInfo.Bounds);
                if (this.Parent is ISupportGlassRegions parent)
                    info.IsDrawOnGlass = parent.IsOnGlass(this.Parent.RectangleToClient(this.RectangleToScreen(this.ViewInfo.Bounds)));
                this.Painter.Draw(info);
                this.RaisePaintEvent((object)this, e);
            }
            finally
            {
                cache.Dispose();
            }
        }

        protected void UpdateViewInfoState()
        {
            this.UpdateViewInfoState(true);
        }

        protected virtual void UpdateViewInfoState(bool useValidMouse, Point actualMousePoint)
        {
            if (!this.IsHandleCreated || !this.ViewInfo.UpdateObjectState(useValidMouse ? Control.MouseButtons : MouseButtons.None, useValidMouse ? actualMousePoint : new Point(-10000, -10000)))
                return;
            this.Invalidate();
        }

        protected virtual void UpdateViewInfoState(bool useValidMouse)
        {
            if (!this.IsHandleCreated || !this.ViewInfo.UpdateObjectState(useValidMouse ? Control.MouseButtons : MouseButtons.None, useValidMouse ? this.PointToClient(Control.MousePosition) : new Point(-10000, -10000)))
                return;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.UpdateViewInfoState(false);
            base.OnMouseLeave(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.UpdateViewInfoState();
            base.OnMouseEnter(e);
        }

        protected virtual bool IsLayoutLocked
        {
            get
            {
                return false;
            }
        }

        [Browsable(false)]
        public virtual bool IsLoading
        {
            get
            {
                return false;
            }
        }

        internal void LockFireChanged()
        {
            ++this.lockFireChanged;
        }

        internal void UnlockFireChanged()
        {
            --this.lockFireChanged;
        }

        protected internal virtual void FireChanged()
        {
            if (this.Site == null || this.IsLoading || this.lockFireChanged != 0 || (this.Site.GetService(typeof(IDesignerHost)) is IDesignerHost service && service.Loading || !(this.Site.GetService(typeof(IComponentChangeService)) is IComponentChangeService service)))
                return;
            service.OnComponentChanged((object)this, (MemberDescriptor)null, (object)null, (object)null);
        }

        protected virtual void OnStyleController_PropertiesChanged(object sender, EventArgs e)
        {
            this.OnStyleControllerChanged();
        }

        protected virtual void OnStyleController_Disposed(object sender, EventArgs e)
        {
            this.StyleController = (IStyleController)null;
        }

        Size IGHResizableControl.MinSize
        {
            get
            {
                return this.SizeableMinSize;
            }
        }

        Size IGHResizableControl.MaxSize
        {
            get
            {
                return this.SizeableMaxSize;
            }
        }

        event EventHandler IGHResizableControl.Changed
        {
            add
            {
                this.Events.AddHandler(BaseControl.sizeableChanged, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(BaseControl.sizeableChanged, (Delegate)value);
            }
        }

        bool IGHResizableControl.IsCaptionVisible
        {
            get
            {
                return this.SizeableIsCaptionVisible;
            }
        }

        [DXCategory("Properties")]
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool AutoSizeInLayoutControl
        {
            get
            {
                return this.autoSizeInLayoutControl;
            }
            set
            {
                this.autoSizeInLayoutControl = value;
            }
        }

        protected virtual Size CalcSizeableMinSize()
        {
            return !this.AutoSizeInLayoutControl ? new Size(50, this.CalcBestSize().Height) : this.CalcBestSize();
        }

        protected virtual Size CalcSizeableMaxSize()
        {
            return !this.AutoSizeInLayoutControl ? new Size(0, this.CalcBestSize().Height) : this.CalcBestSize();
        }

        protected Size SizeableMinSize
        {
            get
            {
                this.prevSizeableMinSize = this.CalcSizeableMinSize();
                return this.prevSizeableMinSize;
            }
        }

        protected Size SizeableMaxSize
        {
            get
            {
                this.prevSizeableMaxSize = this.CalcSizeableMaxSize();
                return this.prevSizeableMaxSize;
            }
        }

        protected virtual void RaiseSizeableChanged()
        {
            EventHandler eventHandler = (EventHandler)this.Events[BaseControl.sizeableChanged];
            if (eventHandler == null)
                return;
            Size prevSizeableMinSize = this.prevSizeableMinSize;
            Size prevSizeableMaxSize = this.prevSizeableMaxSize;
            if (prevSizeableMinSize == this.SizeableMinSize && prevSizeableMaxSize == this.SizeableMaxSize)
                return;
            eventHandler((object)this, EventArgs.Empty);
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            this.RaiseSizeableChanged();
        }

        protected virtual bool SizeableIsCaptionVisible
        {
            get
            {
                return false;
            }
        }

        Control ISupportXtraAnimation.OwnerControl
        {
            get
            {
                return (Control)this;
            }
        }

        bool ISupportXtraAnimation.CanAnimate
        {
            get
            {
                return this.CanAnimateCore;
            }
        }

        protected virtual bool CanAnimateCore
        {
            get
            {
                return this.ViewInfo.AllowAnimation && XtraAnimator.Current.CanAnimate(this.LookAndFeel);
            }
        }

        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg);
            CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, (object)this);
        }

        bool ISupportToolTipsForm.ShowToolTipsFor(Form form)
        {
            return true;
        }

        bool ISupportToolTipsForm.ShowToolTipsWhenInactive
        {
            get
            {
                return true;
            }
        }
    }

}
