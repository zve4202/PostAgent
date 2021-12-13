using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GH.Controls.Utils
{
    public static class WindowsFormsSettings
    {
        private static WindowsFormsFontBehavior? fontBehavior;
        private static bool rightToLeft;

        static WindowsFormsSettings()
        {
            WindowsFormsSettings.AllowDpiScale = true;
            WindowsFormsSettings.InplaceEditorUpdateMode = InplaceEditorUpdateMode.Immediate;
        }

        public static InplaceEditorUpdateMode InplaceEditorUpdateMode { get; set; }

        
        public static bool AllowDpiScale { get; set; }

        public static WindowsFormsFontBehavior FontBehavior
        {
            get
            {
                return !WindowsFormsSettings.fontBehavior.HasValue ? WindowsFormsFontBehavior.Default : WindowsFormsSettings.fontBehavior.Value;
            }
            set
            {
                if (WindowsFormsSettings.FontBehavior == value)
                    return;
                WindowsFormsSettings.fontBehavior = new WindowsFormsFontBehavior?(value);
                FontBehaviorHelper.Update();
            }
        }

        
        


        public static bool RightToLeft
        {
            get
            {
                return WindowsFormsSettings.rightToLeft;
            }
            set
            {
                if (WindowsFormsSettings.RightToLeft == value)
                    return;
                WindowsFormsSettings.rightToLeft = value;
            }
        }

        
        public static void ForcePaintApiDiagnostics(
          PaintApiDiagnosticsLevel level,
          PaintApiTraceLevelResolver resolver = null)
        {
            PaintApiTrace.SetLevel(level, resolver);
        }

        public static DXDashStyle FocusRectStyle
        {
            get
            {
                return XPaint.FocusRectStyle;
            }
            set
            {
                XPaint.FocusRectStyle = value;
            }
        }

        public static void ForceTextRenderPaint()
        {
            XPaint.ForceTextRenderPaint();
        }

        public static void ForceGDIPlusPaint()
        {
            XPaint.ForceGDIPlusPaint();
        }

        public static void ForceAPIPaint()
        {
            XPaint.ForceAPIPaint();
        }

        public static bool DefaultAllowHtmlDraw
        {
            get
            {
                return XPaint.DefaultAllowHtmlDraw;
            }
            set
            {
                XPaint.DefaultAllowHtmlDraw = value;
            }
        }

        public static void ForceDirectXPaint()
        {
            DirectXProvider.ForceDirectXPaint();
        }

        public static Font DefaultFont
        {
            get
            {
                return AppearanceObject.DefaultFont;
            }
            set
            {
                AppearanceObject.DefaultFont = value;
            }
        }

        public static Font DefaultMenuFont
        {
            get
            {
                return AppearanceObject.DefaultMenuFont;
            }
            set
            {
                AppearanceObject.DefaultMenuFont = value;
            }
        }

        public static Font DefaultPrintFont
        {
            get
            {
                return AppearanceObjectPrint.DefaultPrintFont;
            }
            set
            {
                AppearanceObjectPrint.DefaultPrintFont = value;
            }
        }

        public static bool SmartMouseWheelProcessing
        {
            get
            {
                return MouseWheelHelper.SmartMouseWheelProcessing;
            }
            set
            {
                MouseWheelHelper.SmartMouseWheelProcessing = value;
            }
        }

        public static bool ShowTouchScrollBarOnMouseMove
        {
            get
            {
                return WindowsFormsSettings.showTouchScrollBarOnMouseMove;
            }
            set
            {
                WindowsFormsSettings.showTouchScrollBarOnMouseMove = value;
            }
        }

        public static DefaultBoolean AllowAutoScale
        {
            get
            {
                return WindowsFormsSettings.allowAutoScale;
            }
            set
            {
                WindowsFormsSettings.allowAutoScale = value;
            }
        }

        public static bool GetAllowAutoScale()
        {
            return WindowsFormsSettings.AllowAutoScale != DefaultBoolean.False;
        }

        public static void ApplyDemoSettings()
        {
            WindowsFormsSettings.AllowPixelScrolling = DefaultBoolean.True;
            WindowsFormsSettings.ScrollUIMode = ScrollUIMode.Touch;
        }

        public static PopupMenuStyle PopupMenuStyle
        {
            get
            {
                return WindowsFormsSettings.popupMenuStyle;
            }
            set
            {
                WindowsFormsSettings.popupMenuStyle = value;
            }
        }

        public static bool GetIsRightToLeft(Control control)
        {
            return WindowsFormsSettings.GetRightToLeft(control) == System.Windows.Forms.RightToLeft.Yes;
        }

        public static bool GetIsRightToLeft(System.Windows.Forms.RightToLeft rightToLeft)
        {
            return WindowsFormsSettings.GetRightToLeft(rightToLeft) == System.Windows.Forms.RightToLeft.Yes;
        }

        public static System.Windows.Forms.RightToLeft GetRightToLeft(Control control)
        {
            return WindowsFormsSettings.GetRightToLeft(control == null ? System.Windows.Forms.RightToLeft.No : control.RightToLeft);
        }

        public static bool GetIsRightToLeftLayout(Control control)
        {
            return WindowsFormsSettings.GetIsRightToLeftLayout(control != null && WindowsFormsSettings.ExtractRightToLeftLayout(control));
        }

        private static bool ExtractRightToLeftLayout(Control control)
        {
            return control is Form form && form.RightToLeftLayout;
        }

        public static System.Windows.Forms.RightToLeft GetRightToLeft(System.Windows.Forms.RightToLeft rightToLeft)
        {
            if (WindowsFormsSettings.RightToLeft == DefaultBoolean.False)
                return System.Windows.Forms.RightToLeft.No;
            return WindowsFormsSettings.RightToLeft == DefaultBoolean.True ? System.Windows.Forms.RightToLeft.Yes : rightToLeft;
        }

        public static bool GetIsRightToLeftLayout(bool rightToLeft)
        {
            if (WindowsFormsSettings.RightToLeftLayout == DefaultBoolean.False)
                return false;
            return WindowsFormsSettings.RightToLeftLayout == DefaultBoolean.True || rightToLeft;
        }

        public static void LoadApplicationSettings()
        {
            ((UserLookAndFeelDefault)UserLookAndFeel.Default).LoadSettings((MethodInvoker)null);
        }

        [DefaultValue(TouchUIMode.Default)]
        public static TouchUIMode TouchUIMode
        {
            get
            {
                return WindowsFormsSettings.touchUIMode;
            }
            set
            {
                if (WindowsFormsSettings.TouchUIMode == value)
                    return;
                WindowsFormsSettings.touchUIMode = value;
                WindowsFormsSettings.DefaultLookAndFeel.OnStyleChanged(LookAndFeelChangedEventArgs.TouchUIModeChanged);
            }
        }

        [DefaultValue(2f)]
        public static float TouchScaleFactor
        {
            get
            {
                return WindowsFormsSettings.touchScaleFactor;
            }
            set
            {
                if ((double)WindowsFormsSettings.TouchScaleFactor == (double)value)
                    return;
                WindowsFormsSettings.touchScaleFactor = value;
                WindowsFormsSettings.DefaultLookAndFeel.OnStyleChanged(LookAndFeelChangedEventArgs.TouchScaleFactorChanged);
            }
        }

        [DefaultValue(SnapMode.None)]
        public static SnapMode CustomizationFormSnapMode
        {
            get
            {
                return WindowsFormsSettings.snapMode;
            }
            set
            {
                if (WindowsFormsSettings.snapMode == value)
                    return;
                WindowsFormsSettings.snapMode = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DXHelpExclude(true)]
        public static ExpressionEditorMode GetExpressionEditorMode(
          ExpressionEditorMode mode)
        {
            if (mode != ExpressionEditorMode.Default)
                return mode;
            return WindowsFormsSettings.DefaultSettingsCompatibilityMode <= SettingsCompatibilityMode.v16 ? ExpressionEditorMode.Standard : ExpressionEditorMode.AutoComplete;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DXHelpExclude(true)]
        public static PopupWidthMode GetPopupWidthMode(
          bool allowUpdatePopupWidth,
          PopupWidthMode mode)
        {
            if (mode != PopupWidthMode.Default)
                return mode;
            return WindowsFormsSettings.DefaultSettingsCompatibilityMode <= SettingsCompatibilityMode.v17_1 || !allowUpdatePopupWidth ? PopupWidthMode.ContentWidth : PopupWidthMode.UseEditorWidth;
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Editor("DevExpress.Utils.Design.AppearanceObjectUITypeEditor, DevExpress.Design.v17.2, Version=17.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", typeof(UITypeEditor))]
    public class AppearanceObject : IDisposable, ICloneable, ISerializableLayoutEx, ISerializable
    {
        private string name = string.Empty;
        internal static string optUseTextOptions = "UseTextOptions";
        internal static string optUseForeColor = "UseForeColor";
        internal static string optUseBorderColor = "UseBorderColor";
        internal static string optUseBackColor = "UseBackColor";
        internal static string optUseImage = "UseImage";
        internal static string optUseFont = "UseFont";
        internal static string optHighPriority = "HighPriority";
        private const float fontSizeEpsilon = 1E-05f;
        internal Color foreColor;
        internal Color backColor;
        internal Color backColor2;
        internal Color borderColor;
        private TextureBrush textureBrush;
        internal Image image;
        internal LinearGradientMode gradientMode;
        private Font _font;
        private TextOptions textOptions;
        private AppearanceObject parentAppearance;
        internal AppearanceOptions options;
        protected int lockUpdate;
        private int fontSizeDelta;
        private FontStyle fontStyleDelta;
        private IAppearanceOwner owner;
        private bool isDisposed;
        [ThreadStatic]
        private static AppearanceObject dummy;
        private static AppearanceObject controlAppearance;
        private static AppearanceObject emptyAppearance;
        private Font deltaSourceFont;
        private Lazy<Font> deltaBasedFont;
        private int deltaBasedSize;
        private FontStyle deltaBasedStyle;
        private static Font defaultFont;
        private static Font defaultMenuFont;
        [ThreadStatic]
        private static IDictionary<string, Font> deltaFontsCache;
        private int serializing;
        private int deserializing;
        [ThreadStatic]
        private static Hashtable fontHeights;
        [ThreadStatic]
        private static Hashtable fontSizes;

        [Browsable(false)]
        public event EventHandler Changed;

        [Browsable(false)]
        public event EventHandler PaintChanged;

        [Browsable(false)]
        public event EventHandler SizeChanged;

        internal static AppearanceObject Dummy
        {
            get
            {
                if (AppearanceObject.dummy == null)
                    AppearanceObject.dummy = new AppearanceObject();
                return AppearanceObject.dummy;
            }
        }

        static AppearanceObject()
        {
            GHAssemblyResolverEx.Init();
        }

        public AppearanceObject(IAppearanceOwner owner, AppearanceObject parentAppearance)
          : this(owner, false)
        {
            this.parentAppearance = parentAppearance;
        }

        public AppearanceObject(IAppearanceOwner owner, bool reserved)
          : this((AppearanceObject)null)
        {
            this.owner = owner;
        }

        protected IAppearanceOwner Owner
        {
            get
            {
                return this.owner;
            }
        }

        protected IAppearanceDefaultFontProvider FontProvider
        {
            get
            {
                return this.Owner as IAppearanceDefaultFontProvider;
            }
        }

        public AppearanceObject(AppearanceDefault appearanceDefault)
          : this((AppearanceObject)null)
        {
            this.Assign(appearanceDefault);
        }

        public AppearanceObject(AppearanceObject main, AppearanceObject defaultAppearance)
          : this((AppearanceObject)null)
        {
            AppearanceHelper.Combine(this, main, defaultAppearance);
        }

        public AppearanceObject(AppearanceObject main, AppearanceDefault appearanceDefault)
          : this((AppearanceObject)null)
        {
            AppearanceHelper.Combine(this, main, appearanceDefault);
        }

        public AppearanceObject(string name)
          : this((AppearanceObject)null, name)
        {
        }

        public AppearanceObject(AppearanceObject parentAppearance)
          : this(parentAppearance, string.Empty)
        {
        }

        public AppearanceObject(AppearanceObject parentAppearance, string name)
          : this((IAppearanceOwner)null, parentAppearance, name)
        {
        }

        public AppearanceObject(IAppearanceOwner owner, AppearanceObject parentAppearance, string name)
          : this(owner, parentAppearance, name, true)
        {
        }

        protected AppearanceObject(
          IAppearanceOwner owner,
          AppearanceObject parentAppearance,
          string name,
          bool useReset)
        {
            this.name = name;
            this.parentAppearance = parentAppearance;
            if (useReset)
                this.Reset();
            this.owner = owner;
            ++GUIResources.AppearanceObjectNew;
        }

        public AppearanceObject()
          : this((AppearanceObject)null)
        {
        }

        public virtual void Dispose()
        {
            ++GUIResources.AppearanceObjectDisposed;
            this.DestroyBrush();
            this.DestroyDeltaFonts();
            if (this.options != null)
                this.options.RemoveChangedCoreHandler(new BaseOptionChangedEventHandler(this.OnOptionsChanged));
            this.Changed = (EventHandler)null;
            this.SizeChanged = this.PaintChanged = (EventHandler)null;
            GC.SuppressFinalize((object)this);
            this.isDisposed = true;
        }

        public virtual object Clone()
        {
            AppearanceObject appearanceObject = new AppearanceObject(this.ParentAppearance);
            appearanceObject.Assign(this);
            return (object)appearanceObject;
        }

        public override string ToString()
        {
            return "Appearance";
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return this.isDisposed;
            }
        }

        [DevExpressUtilsLocalizedDescription("AppearanceObjectControlAppearance")]
        public static AppearanceObject ControlAppearance
        {
            get
            {
                if (AppearanceObject.controlAppearance == null)
                {
                    AppearanceObject.controlAppearance = new AppearanceObject();
                    AppearanceObject.controlAppearance.BackColor = SystemColors.Control;
                    AppearanceObject.controlAppearance.ForeColor = SystemColors.ControlText;
                    AppearanceObject.controlAppearance.BorderColor = SystemColors.Control;
                }
                return AppearanceObject.controlAppearance;
            }
        }

        [DevExpressUtilsLocalizedDescription("AppearanceObjectEmptyAppearance")]
        public static AppearanceObject EmptyAppearance
        {
            get
            {
                if (AppearanceObject.emptyAppearance == null)
                    AppearanceObject.emptyAppearance = new AppearanceObject();
                return AppearanceObject.emptyAppearance;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AppearanceObject ParentAppearance
        {
            get
            {
                return this.parentAppearance;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        private void ResetOptions()
        {
            if (this.options == null)
                return;
            this.Options.Reset();
        }

        private bool ShouldSerializeOptions()
        {
            return this.options != null && this.Options.ShouldSerialize();
        }

        [XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
        [DXCategory("Appearance")]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.Options")]
        [DevExpressUtilsLocalizedDescription("AppearanceObjectOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual AppearanceOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    this.options = this.CreateOptions();
                    this.options.AddChangedCoreHandler(new BaseOptionChangedEventHandler(this.OnOptionsChanged));
                    for (int index = 0; index < this.lockUpdate; ++index)
                        this.options.BeginUpdate();
                }
                return this.options;
            }
        }

        private void ResetForeColor()
        {
            this.ForeColor = Color.Empty;
        }

        protected bool ShouldSerializeForeColor()
        {
            return this.ForeColor != Color.Empty;
        }

        [XtraSerializableProperty]
        [DevExpressUtilsLocalizedDescription("AppearanceObjectForeColor")]
        [DXCategory("Appearance")]
        [Localizable(true)]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.ForeColor")]
        public Color ForeColor
        {
            get
            {
                return this.foreColor;
            }
            set
            {
                if (this.ForeColor == value)
                    return;
                this.foreColor = value;
                if (!this.IsLoading)
                {
                    try
                    {
                        this.Options.BeginUpdate();
                        this.Options.UseForeColor = this.foreColor != Color.Empty;
                    }
                    finally
                    {
                        this.Options.CancelUpdate();
                    }
                }
                this.OnPaintChanged();
            }
        }

        public StringFormat GetStringFormat()
        {
            return this.GetTextOptions().GetStringFormat();
        }

        public StringFormat GetStringFormat(TextOptions defaultOptions)
        {
            return this.GetTextOptions().GetStringFormat(defaultOptions);
        }

        [Browsable(false)]
        public HorzAlignment HAlignment
        {
            get
            {
                return this.GetTextOptions().HAlignment;
            }
        }

        protected bool IsLoading
        {
            get
            {
                return this.Owner != null && this.Owner.IsLoading || this.deserializing > 0;
            }
        }

        private void ResetBorderColor()
        {
            this.BorderColor = Color.Empty;
        }

        protected bool ShouldSerializeBorderColor()
        {
            return this.BorderColor != Color.Empty;
        }

        [XtraSerializableProperty]
        [Localizable(true)]
        [DXCategory("Appearance")]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.BorderColor")]
        [DevExpressUtilsLocalizedDescription("AppearanceObjectBorderColor")]
        [RefreshProperties(RefreshProperties.All)]
        public Color BorderColor
        {
            get
            {
                return this.borderColor;
            }
            set
            {
                if (this.BorderColor == value)
                    return;
                this.borderColor = value;
                if (!this.IsLoading)
                {
                    try
                    {
                        this.Options.BeginUpdate();
                        this.Options.UseBorderColor = this.borderColor != Color.Empty;
                    }
                    finally
                    {
                        this.Options.CancelUpdate();
                    }
                }
                this.OnPaintChanged();
            }
        }

        private void ResetBackColor()
        {
            this.BackColor = Color.Empty;
        }

        protected bool ShouldSerializeBackColor()
        {
            return this.BackColor != Color.Empty;
        }

        [DevExpressUtilsLocalizedDescription("AppearanceObjectBackColor")]
        [DXCategory("Appearance")]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.BackColor")]
        [XtraSerializableProperty]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.All)]
        public Color BackColor
        {
            get
            {
                return this.backColor;
            }
            set
            {
                if (this.BackColor == value)
                    return;
                this.backColor = value;
                if (!this.IsLoading)
                {
                    try
                    {
                        this.Options.BeginUpdate();
                        this.Options.UseBackColor = this.backColor != Color.Empty;
                    }
                    finally
                    {
                        this.Options.CancelUpdate();
                    }
                }
                this.OnPaintChanged();
            }
        }

        private void ResetBackColor2()
        {
            this.BackColor2 = Color.Empty;
        }

        private bool ShouldSerializeBackColor2()
        {
            return this.BackColor2 != Color.Empty;
        }

        [Localizable(true)]
        [DevExpressUtilsLocalizedDescription("AppearanceObjectBackColor2")]
        [DXCategory("Appearance")]
        [XtraSerializableProperty]
        [RefreshProperties(RefreshProperties.All)]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.BackColor2")]
        public virtual Color BackColor2
        {
            get
            {
                return this.backColor2;
            }
            set
            {
                if (this.BackColor2 == value)
                    return;
                this.backColor2 = value;
                this.OnPaintChanged();
            }
        }

        [DefaultValue(null)]
        [Localizable(true)]
        [DXCategory("Appearance")]
        [DevExpressUtilsLocalizedDescription("AppearanceObjectImage")]
        [Editor("DevExpress.Utils.Design.DXImageEditor, DevExpress.Design.v17.2", typeof(UITypeEditor))]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.Image")]
        public virtual Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (this.Image == value)
                    return;
                this.image = value;
                this.DestroyBrush();
                if (!this.IsLoading)
                {
                    try
                    {
                        this.Options.BeginUpdate();
                        this.Options.UseImage = this.image != null;
                    }
                    finally
                    {
                        this.Options.CancelUpdate();
                    }
                }
                this.OnImageChanged();
            }
        }

        protected virtual void OnImageChanged()
        {
            this.OnPaintChanged();
        }

        private void ResetTextOptions()
        {
            if (this.textOptions == null)
                return;
            this.TextOptions.Reset();
        }

        private bool ShouldSerializeTextOptions()
        {
            return this.textOptions != null && UniversalTypeConverter.ShouldSerializeObject((object)this.TextOptions);
        }

        [DXCategory("Font")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [DevExpressUtilsLocalizedDescription("AppearanceObjectTextOptions")]
        [Browsable(true)]
        [XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.TextOptions")]
        public virtual TextOptions TextOptions
        {
            get
            {
                if (this.textOptions == null)
                    this.textOptions = this.CreateTextOptions();
                return this.textOptions;
            }
        }

        protected virtual TextOptions CreateTextOptions()
        {
            return new TextOptions(this);
        }

        protected virtual Font InnerDefaultFont
        {
            get
            {
                return this.FontProvider != null && this.FontProvider.DefaultFont != null ? this.FontProvider.DefaultFont : AppearanceObject.DefaultFont;
            }
        }

        private void ResetFont()
        {
            this.Font = (Font)null;
        }

        protected virtual bool ShouldSerializeFont()
        {
            return this._font != null;
        }

        [DXCategory("Font")]
        [Localizable(true)]
        [DevExpressUtilsLocalizedDescription("AppearanceObjectFont")]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.Font")]
        [TypeConverter(typeof(FontTypeConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [XtraSerializableProperty]
        public virtual Font Font
        {
            get
            {
                if (this.serializing == 0 && this.deltaBasedFont != null)
                {
                    this.OnFontSizeDeltaChanged(true);
                    if (this.deltaBasedFont != null)
                        return this.deltaBasedFont.Value;
                }
                return this.GetFontCore();
            }
            set
            {
                value = this.CheckFont(value);
                if (this._font == value)
                    return;
                this._font = value;
                if (!this.IsLoading)
                {
                    this.Options.BeginUpdate();
                    try
                    {
                        this.Options.UseFont = this._font != null && !this._font.Equals((object)this.InnerDefaultFont);
                    }
                    finally
                    {
                        this.Options.CancelUpdate();
                    }
                }
                this.OnFontSizeDeltaChanged(this.deltaSourceFont == null || this.deltaSourceFont.Equals((object)value));
                this.OnSizeChanged();
            }
        }

        internal Font GetFontCore()
        {
            return this._font ?? this.InnerDefaultFont;
        }

        private Font CheckFont(Font font)
        {
            if (font != null)
            {
                if (object.ReferenceEquals((object)this.InnerDefaultFont, (object)font))
                    return (Font)null;
                if (font.Equals((object)this.InnerDefaultFont))
                    return (Font)null;
            }
            return font;
        }

        [DefaultValue(0)]
        [DXCategory("Font")]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.FontSizeDelta")]
        [DevExpressUtilsLocalizedDescription("AppearanceObjectFontSizeDelta")]
        [RefreshProperties(RefreshProperties.All)]
        [XtraSerializableProperty(99)]
        [Localizable(true)]
        public virtual int FontSizeDelta
        {
            get
            {
                return this.fontSizeDelta;
            }
            set
            {
                if (this.FontSizeDelta == value)
                    return;
                this.fontSizeDelta = value;
                this.OnFontSizeDeltaChanged(false);
                if (!this.IsLoading)
                {
                    try
                    {
                        this.Options.BeginUpdate();
                        this.Options.UseFont = !this.Font.Equals((object)this.InnerDefaultFont);
                    }
                    finally
                    {
                        this.Options.CancelUpdate();
                    }
                }
                this.OnSizeChanged();
            }
        }

        [DefaultValue(FontStyle.Regular)]
        [DXCategory("Font")]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.FontStyleDelta")]
        [RefreshProperties(RefreshProperties.All)]
        [XtraSerializableProperty(99)]
        [Localizable(true)]
        [DevExpressUtilsLocalizedDescription("AppearanceObjectFontStyleDelta")]
        public virtual FontStyle FontStyleDelta
        {
            get
            {
                return this.fontStyleDelta;
            }
            set
            {
                if (this.FontStyleDelta == value)
                    return;
                this.fontStyleDelta = value;
                this.OnFontSizeDeltaChanged(false);
                if (!this.IsLoading)
                {
                    try
                    {
                        this.Options.BeginUpdate();
                        this.Options.UseFont = !this.Font.Equals((object)this.InnerDefaultFont);
                    }
                    finally
                    {
                        this.Options.CancelUpdate();
                    }
                }
                this.OnSizeChanged();
            }
        }

        [DXCategory("Appearance")]
        [DevExpressUtilsLocalizedDescription("AppearanceObjectGradientMode")]
        [DefaultValue(LinearGradientMode.Horizontal)]
        [XtraSerializableProperty]
        [DXDisplayName(typeof(ResFinder), "PropertyNamesRes", "DevExpress.Utils.AppearanceObject.GradientMode")]
        [TypeConverter(typeof(LinearGradientModeConverter))]
        [Localizable(true)]
        public virtual LinearGradientMode GradientMode
        {
            get
            {
                return this.gradientMode;
            }
            set
            {
                if (this.GradientMode == value)
                    return;
                this.gradientMode = value;
                this.OnPaintChanged();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal TextureBrush TextureBrush
        {
            get
            {
                return this.textureBrush;
            }
            set
            {
                this.textureBrush = value;
            }
        }

        public void AssignInternal(AppearanceObject val)
        {
            this.BeginUpdate();
            try
            {
                this.Assign(val);
            }
            finally
            {
                this.CancelUpdate();
            }
        }

        public void Combine(AppearanceObject val)
        {
            AppearanceHelper.Apply(this, val);
        }

        private void OnFontSizeDeltaChanged(bool checkOnly)
        {
            if (checkOnly && (this.deltaBasedFont == null || this.deltaBasedSize == this.FontSizeDelta && this.deltaBasedStyle == this.FontStyleDelta && object.ReferenceEquals((object)this.deltaSourceFont, (object)this.GetFontCore()) && (!this.deltaBasedFont.IsValueCreated || this.AreDeltaParametersEquals(this.deltaBasedFont.Value, this.deltaSourceFont))))
                return;
            if (this.FontSizeDelta == 0 && this.FontStyleDelta == FontStyle.Regular)
            {
                this.AssignDeltaFonts((Font)null);
            }
            else
            {
                this.deltaBasedStyle = this.FontStyleDelta;
                this.deltaBasedSize = this.FontSizeDelta;
                this.InitializeDeltaFonts(this.GetFontCore());
            }
        }

        private bool AreDeltaParametersEquals(Font deltaBased, Font deltaSource)
        {
            return deltaBased.Style == (deltaSource.Style | this.FontStyleDelta) && AppearanceObject.AreFontSizeClose(deltaBased.Size, AppearanceObject.GetSize(deltaSource.Size, this.FontSizeDelta));
        }

        private static float GetSize(float source, int sizeDelta)
        {
            if (sizeDelta > 100)
                return source * ((float)(sizeDelta - 100) / 10f);
            return sizeDelta < -100 ? source * ((float)(sizeDelta + 100) / 10f) : Math.Max(source + (float)sizeDelta, 0.0f);
        }

        internal static bool AreFontSizeClose(float deltaBasedSize, float deltaSourceSize)
        {
            return (double)Math.Abs(deltaBasedSize - deltaSourceSize) < 9.99999974737875E-06;
        }

        public virtual void Assign(AppearanceObject source)
        {
            if (source == null)
                return;
            this.BeginUpdate();
            try
            {
                this.DestroyBrush();
                if (source.options != null)
                    this.Options.Assign((BaseOptions)source.Options);
                else if (this.options != null)
                    this.Options.ResetOptions();
                this.foreColor = source.ForeColor;
                this.borderColor = source.BorderColor;
                this.backColor = source.BackColor;
                this.backColor2 = source.BackColor2;
                this.gradientMode = source.GradientMode;
                this.image = source.Image;
                this.AssignDeltaFontsAndSetFont(source);
                if (source.textOptions != null)
                    this.TextOptions.Assign(source.TextOptions);
                else if (this.textOptions != null)
                    this.TextOptions.Reset();
                this.OnFontSizeDeltaChanged(true);
            }
            finally
            {
                this.EndUpdate();
            }
        }

        internal void SetFont(Font font)
        {
            this.SetFont(font, false);
        }

        internal void SetFont(Font font, bool setUseFont)
        {
            this._font = this.CheckFont(font);
            if (!setUseFont || this._font == null)
                return;
            this.Options.UseFont = true;
        }

        public virtual bool IsEqual(AppearanceObject val)
        {
            return (val.options ?? AppearanceOptions.Empty).IsEqual(this.options ?? AppearanceOptions.Empty) && this.foreColor == val.ForeColor && (this.borderColor == val.BorderColor && this.backColor == val.BackColor) && (this.backColor2 == val.BackColor2 && this.gradientMode == val.GradientMode && (this.image == val.Image && this.Font == val.Font)) && (val.textOptions ?? TextOptions.DefaultOptions).IsEqual(this.textOptions ?? TextOptions.DefaultOptions);
        }

        public virtual void BeginUpdate()
        {
            ++this.lockUpdate;
            if (this.options == null)
                return;
            this.Options.BeginUpdate();
        }

        public virtual void CancelUpdate()
        {
            if (this.options != null)
                this.Options.CancelUpdate();
            --this.lockUpdate;
        }

        public virtual void EndUpdate()
        {
            if (this.options != null)
                this.Options.EndUpdate();
            if (--this.lockUpdate != 0)
                return;
            this.OnSizeChanged();
        }

        protected internal void RotateBackColors(int angle)
        {
            if (angle == 0 || this.BackColor2 == Color.Empty)
                return;
            switch (angle)
            {
                case 90:
                case 270:
                    LinearGradientMode linearGradientMode = this.GradientMode;
                    switch (linearGradientMode)
                    {
                        case LinearGradientMode.Horizontal:
                            linearGradientMode = LinearGradientMode.Vertical;
                            break;
                        case LinearGradientMode.Vertical:
                            linearGradientMode = LinearGradientMode.Horizontal;
                            break;
                        case LinearGradientMode.ForwardDiagonal:
                            linearGradientMode = LinearGradientMode.BackwardDiagonal;
                            break;
                        case LinearGradientMode.BackwardDiagonal:
                            linearGradientMode = LinearGradientMode.ForwardDiagonal;
                            break;
                    }
                    this.gradientMode = linearGradientMode;
                    break;
                case 180:
                    Color backColor2 = this.BackColor2;
                    this.backColor2 = this.BackColor;
                    this.backColor = backColor2;
                    break;
            }
        }

        protected virtual void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e)
        {
            this.OnSizeChanged();
        }

        protected internal virtual void OnTextOptionsChanged()
        {
            if (!this.IsLoading)
            {
                if (this.lockUpdate == 0)
                {
                    try
                    {
                        this.Options.BeginUpdate();
                        this.Options.UseTextOptions = !this.TextOptions.IsDefault();
                    }
                    finally
                    {
                        this.Options.CancelUpdate();
                    }
                }
            }
            this.OnSizeChanged();
        }

        protected internal virtual void OnPaintChanged()
        {
            if (this.lockUpdate != 0)
                return;
            if (this.PaintChanged != null)
                this.PaintChanged((object)this, EventArgs.Empty);
            this.OnChanged();
        }

        protected internal virtual void OnSizeChanged()
        {
            if (this.lockUpdate != 0)
                return;
            if (this.SizeChanged != null)
                this.SizeChanged((object)this, EventArgs.Empty);
            this.OnChanged();
        }

        protected internal virtual void OnChanged()
        {
            if (this.lockUpdate != 0 || this.Changed == null)
                return;
            this.Changed((object)this, EventArgs.Empty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public AppearanceObject GetAppearanceByFont()
        {
            return this.GetAppearanceByOption(AppearanceObject.optUseFont);
        }

        public TextOptions GetTextOptions()
        {
            return this.GetAppearanceByOption(AppearanceObject.optUseTextOptions).TextOptions;
        }

        public Color GetForeColor()
        {
            return this.GetAppearanceByOption(AppearanceObject.optUseForeColor).ForeColor;
        }

        public Color GetBorderColor()
        {
            return this.GetAppearanceByOption(AppearanceObject.optUseBorderColor).BorderColor;
        }

        public Color GetBackColor()
        {
            return this.GetAppearanceByOption(AppearanceObject.optUseBackColor).BackColor;
        }

        public Color GetBackColor2()
        {
            return this.GetAppearanceByOption(AppearanceObject.optUseBackColor).BackColor2;
        }

        public LinearGradientMode GetGradientMode()
        {
            return this.GetAppearanceByOption(AppearanceObject.optUseBackColor).GradientMode;
        }

        public Font GetFont()
        {
            return this.GetAppearanceByOption(AppearanceObject.optUseFont).Font;
        }

        public virtual Image GetImage()
        {
            return this.GetAppearanceByOption(AppearanceObject.optUseImage).Image;
        }

        public Brush GetForeBrush(GraphicsCache cache)
        {
            return cache == null ? (Brush)new SolidBrush(this.GetForeColor()) : cache.GetSolidBrush(this.GetForeColor());
        }

        public Pen GetBorderPen(GraphicsCache cache)
        {
            return cache == null ? new Pen(this.GetBorderColor()) : cache.GetPen(this.GetBorderColor());
        }

        public Pen GetBackPen(GraphicsCache cache)
        {
            return cache == null ? new Pen(this.GetBackColor()) : cache.GetPen(this.GetBackColor());
        }

        public Brush GetBorderBrush(GraphicsCache cache)
        {
            return cache == null ? (Brush)new SolidBrush(this.GetBorderColor()) : cache.GetSolidBrush(this.GetBorderColor());
        }

        public Brush GetBackBrush(GraphicsCache cache)
        {
            return cache == null ? (Brush)new SolidBrush(this.GetBackColor()) : cache.GetSolidBrush(this.GetBackColor());
        }

        public Brush GetBackBrush(GraphicsCache cache, Rectangle rect)
        {
            Color backColor = this.GetBackColor();
            Color backColor2 = this.GetBackColor2();
            if (backColor == backColor2 || backColor2 == Color.Empty)
                return this.GetBackBrush(cache);
            return cache == null ? (Brush)new LinearGradientBrush(rect, backColor, backColor2, this.GetGradientMode()) : cache.GetGradientBrush(rect, backColor, backColor2, this.GetGradientMode());
        }

        public Pen GetForePen(GraphicsCache cache)
        {
            return cache == null ? new Pen(this.GetForeColor()) : cache.GetPen(this.GetForeColor());
        }

        public TextureBrush GetTextureBrush()
        {
            AppearanceObject appearanceByOption = this.GetAppearanceByOption(AppearanceObject.optUseImage);
            if (appearanceByOption.textureBrush != null)
                return appearanceByOption.textureBrush;
            if (appearanceByOption.Image == null)
                return (TextureBrush)null;
            appearanceByOption.textureBrush = new TextureBrush(appearanceByOption.Image);
            return appearanceByOption.textureBrush;
        }

        public bool ShouldSerialize()
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties((object)this))
            {
                if (property.SerializationVisibility != DesignerSerializationVisibility.Hidden && property.ShouldSerializeValue((object)this))
                    return true;
            }
            return false;
        }

        public void DrawString(GraphicsCache cache, string text, Rectangle bounds)
        {
            Brush solidBrush = cache.GetSolidBrush(this.GetForeColor());
            this.DrawString(cache, text, bounds, solidBrush);
        }

        public void DrawString(GraphicsCache cache, string text, Rectangle bounds, Brush foreBrush)
        {
            StringFormat stringFormat = this.GetTextOptions().GetStringFormat();
            this.DrawString(cache, text, bounds, foreBrush, stringFormat);
        }

        public void DrawString(
          GraphicsCache cache,
          string text,
          Rectangle bounds,
          StringFormat format)
        {
            this.DrawString(cache, text, bounds, cache.GetSolidBrush(this.GetForeColor()), format);
        }

        public void DrawString(
          GraphicsCache cache,
          string text,
          Rectangle bounds,
          Font font,
          StringFormat format)
        {
            cache.DrawString(text, font, cache.GetSolidBrush(this.GetForeColor()), bounds, format);
        }

        public virtual void DrawString(
          GraphicsCache cache,
          string text,
          Rectangle bounds,
          Brush foreBrush,
          StringFormat format)
        {
            cache.DrawString(text, this.GetFont(), foreBrush, bounds, format);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public void DrawString(
          GraphicsCache cache,
          string text,
          Rectangle bounds,
          Color foreColor,
          TextOptions defaultOptions)
        {
            cache.Paint.DrawString(cache, text, this.Font, foreColor, bounds, this.TextOptions, defaultOptions);
        }

        public void DrawString(
          GraphicsCache cache,
          string s,
          Rectangle bounds,
          Font font,
          Brush foreBrush,
          StringFormat strFormat)
        {
            cache.DrawString(s, font, foreBrush, bounds, strFormat);
        }

        public void DrawString(
          GraphicsCache cache,
          string s,
          Rectangle bounds,
          Font font,
          Color foreColor,
          StringFormat strFormat)
        {
            cache.DrawString(s, font, foreColor, (RectangleF)bounds, strFormat);
        }

        public void DrawString(
          GraphicsCache cache,
          string s,
          Rectangle bounds,
          Color foreColor,
          StringFormat strFormat)
        {
            cache.DrawString(s, this.Font, foreColor, (RectangleF)bounds, strFormat);
        }

        public void DrawVString(
          GraphicsCache cache,
          string text,
          Font font,
          Brush foreBrush,
          Rectangle bounds,
          StringFormat strFormat,
          int angle)
        {
            cache.DrawVString(text, font, foreBrush, bounds, strFormat, angle);
        }

        public void FillRectangle(GraphicsCache cache, Rectangle bounds)
        {
            this.DrawBackground(cache, bounds);
        }

        public void DrawBackground(GraphicsCache cache, Rectangle bounds)
        {
            this.DrawBackground(cache.Graphics, cache, bounds);
        }

        public void FillRectangle(GraphicsCache cache, Rectangle bounds, bool useZeroOffset)
        {
            this.DrawBackground(cache, bounds, useZeroOffset);
        }

        public void DrawBackground(GraphicsCache cache, Rectangle bounds, bool useZeroOffset)
        {
            this.DrawBackground(cache.Graphics, cache, bounds, useZeroOffset);
        }

        public void DrawBackground(Graphics graphics, GraphicsCache cache, Rectangle bounds)
        {
            this.DrawBackground(graphics, cache, bounds, false);
        }

        public virtual void DrawBackground(
          Graphics graphics,
          GraphicsCache cache,
          Rectangle bounds,
          bool useZeroOffset)
        {
            Color backColor = this.GetBackColor();
            bool isEmpty = backColor.IsEmpty;
            if (SystemInformation.HighContrast)
            {
                if (isEmpty)
                    return;
                if (cache == null)
                    graphics.FillRectangle(this.GetBackBrush(cache, bounds), bounds);
                else
                    cache.FillRectangle(this.GetBackBrush(cache, bounds), bounds);
            }
            else
            {
                TextureBrush textureBrush = this.GetTextureBrush();
                if (textureBrush != null && useZeroOffset)
                    textureBrush.TranslateTransform((float)bounds.X, (float)bounds.Y);
                if (cache == null)
                {
                    if (backColor.A != byte.MaxValue || isEmpty)
                    {
                        if (textureBrush != null)
                            graphics.FillRectangle((Brush)textureBrush, bounds);
                        if (!isEmpty)
                            graphics.FillRectangle(this.GetBackBrush(cache, bounds), bounds);
                    }
                    else
                    {
                        graphics.FillRectangle(this.GetBackBrush(cache, bounds), bounds);
                        if (textureBrush != null)
                            graphics.FillRectangle((Brush)textureBrush, bounds);
                    }
                }
                else if (backColor.A != byte.MaxValue || isEmpty)
                {
                    if (textureBrush != null)
                        cache.FillRectangle((Brush)textureBrush, bounds);
                    if (!isEmpty)
                        cache.FillRectangle(this, bounds);
                }
                else
                {
                    cache.FillRectangle(this, bounds);
                    if (textureBrush != null)
                        cache.FillRectangle((Brush)textureBrush, bounds);
                }
                if (textureBrush == null || !useZeroOffset)
                    return;
                textureBrush.ResetTransform();
            }
        }

        public virtual void Assign(AppearanceDefault appearanceDefault)
        {
            if (appearanceDefault == null)
                return;
            this.BeginUpdate();
            try
            {
                this.Reset();
                this.backColor = appearanceDefault.BackColor;
                this.foreColor = appearanceDefault.ForeColor;
                this.borderColor = appearanceDefault.BorderColor;
                this.backColor2 = appearanceDefault.BackColor2;
                this.gradientMode = appearanceDefault.GradientMode;
                this.AssignDeltaFontsAndSetFont(appearanceDefault);
                this.TextOptions.Assign(appearanceDefault);
                this.Options.UseBackColor = this.backColor != Color.Empty;
                this.Options.UseBorderColor = this.borderColor != Color.Empty;
                this.Options.UseForeColor = this.foreColor != Color.Empty;
                this.Options.UseFont = this._font != null;
            }
            finally
            {
                this.EndUpdate();
            }
        }

        public virtual void Reset()
        {
            this.BeginUpdate();
            try
            {
                if (this.textOptions != null)
                    this.TextOptions.Reset();
                if (this.options != null)
                    this.Options.ResetOptions();
                this.backColor = Color.Empty;
                this.backColor2 = Color.Empty;
                this.foreColor = Color.Empty;
                this.borderColor = Color.Empty;
                this.fontSizeDelta = 0;
                this.fontStyleDelta = FontStyle.Regular;
                this.DestroyDeltaFonts();
                this.deltaBasedSize = 0;
                this.deltaBasedStyle = FontStyle.Regular;
                this._font = (Font)null;
                this.gradientMode = LinearGradientMode.Horizontal;
                this.image = (Image)null;
                this.DestroyBrush();
                this.OnFontSizeDeltaChanged(true);
            }
            finally
            {
                this.EndUpdate();
            }
        }

        internal static void ResetDefaultFont()
        {
            AppearanceObject.defaultFont = (Font)null;
        }

        [DevExpressUtilsLocalizedDescription("AppearanceObjectDefaultFont")]
        public static Font DefaultFont
        {
            get
            {
                if (AppearanceObject.defaultFont == null)
                {
                    FontBehaviorHelper.CheckFont();
                    AppearanceObject.defaultFont = AppearanceObject.CreateDefaultFont();
                }
                return AppearanceObject.defaultFont;
            }
            set
            {
                FontBehaviorHelper.CheckFont();
                if (value == AppearanceObject.defaultFont)
                    return;
                if (value == null)
                    value = AppearanceObject.CreateDefaultFont();
                AppearanceObject.defaultFont = value;
                UserLookAndFeel.Default.OnStyleChanged(LookAndFeelChangedEventArgs.DefaultFontChanged);
            }
        }

        [DevExpressUtilsLocalizedDescription("AppearanceObjectDefaultMenuFont")]
        public static Font DefaultMenuFont
        {
            get
            {
                if (AppearanceObject.defaultMenuFont == null)
                    AppearanceObject.defaultMenuFont = SystemInformation.MenuFont;
                return AppearanceObject.defaultMenuFont;
            }
            set
            {
                if (value == null)
                    value = SystemInformation.MenuFont;
                if (value == AppearanceObject.defaultMenuFont)
                    return;
                AppearanceObject.defaultMenuFont = value;
                UserLookAndFeel.Default.OnStyleChanged(LookAndFeelChangedEventArgs.DefaultMenuFontChanged);
            }
        }

        internal static Font CreateDefaultFont()
        {
            try
            {
                return FontBehaviorHelper.GetDefaultFont();
            }
            catch
            {
            }
            return Control.DefaultFont;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public AppearanceObject GetAppearanceByOption(string option)
        {
            return this.GetAppearanceByOption(option, 0, (AppearanceObject)null);
        }

        protected internal AppearanceObject GetAppearanceByOption(
          string option,
          AppearanceObject defaultAppearance)
        {
            return this.GetAppearanceByOption(option, 0, defaultAppearance);
        }

        protected AppearanceObject GetAppearanceByOption(
          string option,
          int level,
          AppearanceObject defaultAppearance)
        {
            if (this.options != null && this.Options.GetOptionValue(option) || level > 10)
                return this;
            if (this.ParentAppearance != null)
                return this.ParentAppearance.GetAppearanceByOption(option, level + 1, defaultAppearance);
            return defaultAppearance != null ? defaultAppearance.GetAppearanceByOption(option, 0, (AppearanceObject)null) : this;
        }

        protected internal bool GetOptionState(string option)
        {
            return this.GetOptionState(option, 0, (AppearanceObject)null);
        }

        protected internal bool GetOptionState(
          string option,
          int level,
          AppearanceObject defaultAppearance)
        {
            if (this.options != null && this.Options.GetOptionValue(option))
                return true;
            if (level > 10)
                return false;
            if (this.ParentAppearance != null)
                return this.ParentAppearance.GetOptionState(option, level + 1, defaultAppearance);
            return defaultAppearance != null && defaultAppearance.GetOptionState(option, 0, (AppearanceObject)null);
        }

        protected internal AppearanceObject GetForeColorAppearance()
        {
            AppearanceObject appearanceObject = this;
            for (int index = 0; index < 10; ++index)
            {
                if (appearanceObject == null)
                    return (AppearanceObject)null;
                if (appearanceObject.options != null && appearanceObject.options.UseForeColor)
                    return appearanceObject;
                appearanceObject = appearanceObject.ParentAppearance;
            }
            return (AppearanceObject)null;
        }

        protected internal AppearanceObject GetBackColorAppearance()
        {
            AppearanceObject appearanceObject = this;
            for (int index = 0; index < 10; ++index)
            {
                if (appearanceObject == null)
                    return (AppearanceObject)null;
                if (appearanceObject.options != null && appearanceObject.options.UseBackColor)
                    return appearanceObject;
                appearanceObject = appearanceObject.ParentAppearance;
            }
            return (AppearanceObject)null;
        }

        protected internal AppearanceObject GetBorderColorAppearance()
        {
            AppearanceObject appearanceObject = this;
            for (int index = 0; index < 10; ++index)
            {
                if (appearanceObject == null)
                    return (AppearanceObject)null;
                if (appearanceObject.options != null && appearanceObject.options.UseBorderColor)
                    return appearanceObject;
                appearanceObject = appearanceObject.ParentAppearance;
            }
            return (AppearanceObject)null;
        }

        protected internal AppearanceObject GetFontAppearance()
        {
            AppearanceObject appearanceObject = this;
            for (int index = 0; index < 10; ++index)
            {
                if (appearanceObject == null)
                    return (AppearanceObject)null;
                if (appearanceObject.options != null && appearanceObject.options.UseFont)
                    return appearanceObject;
                appearanceObject = appearanceObject.ParentAppearance;
            }
            return (AppearanceObject)null;
        }

        protected internal AppearanceObject GetImageAppearance()
        {
            AppearanceObject appearanceObject = this;
            for (int index = 0; index < 10; ++index)
            {
                if (appearanceObject == null)
                    return (AppearanceObject)null;
                if (appearanceObject.options != null && appearanceObject.options.UseImage)
                    return appearanceObject;
                appearanceObject = appearanceObject.ParentAppearance;
            }
            return (AppearanceObject)null;
        }

        protected internal AppearanceObject GetTextOptionsAppearance()
        {
            AppearanceObject appearanceObject = this;
            for (int index = 0; index < 10; ++index)
            {
                if (appearanceObject == null)
                    return (AppearanceObject)null;
                if (appearanceObject.options != null && appearanceObject.options.UseTextOptions)
                    return appearanceObject;
                appearanceObject = appearanceObject.ParentAppearance;
            }
            return (AppearanceObject)null;
        }

        protected virtual AppearanceOptions CreateOptions()
        {
            return new AppearanceOptions();
        }

        protected internal virtual void DestroyBrush()
        {
            Ref.Dispose<TextureBrush>(ref this.textureBrush);
        }

        protected void InitializeDeltaFonts(Font sourceFont)
        {
            this.deltaSourceFont = sourceFont;
            this.deltaBasedFont = sourceFont != null ? new Lazy<Font>(new Func<Font>(this.GetOrCreateDeltaBasedFont)) : (Lazy<Font>)null;
        }

        protected void DestroyDeltaFonts()
        {
            this.AssignDeltaFonts((Font)null);
        }

        protected void AssignDeltaFonts(Font value)
        {
            if (this.deltaBasedFont != null && this.deltaBasedFont.IsValueCreated)
                this.deltaBasedFont = (Lazy<Font>)null;
            this.InitializeDeltaFonts(value);
        }

        protected Font GetOrCreateDeltaBasedFont()
        {
            return AppearanceObject.GetOrCreateDeltaBasedFont(this.deltaSourceFont, this.fontStyleDelta, this.fontSizeDelta, new Func<Font, Font>(this.CreateDeltaBasedFont));
        }

        protected virtual Font CreateDeltaBasedFont(Font font)
        {
            return new Font(font.FontFamily, AppearanceObject.GetSize(font.Size, this.FontSizeDelta), font.Style | this.FontStyleDelta, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
        }

        private static Font GetOrCreateDeltaBasedFont(
          Font font,
          FontStyle styleDelta,
          int sizeDelta,
          Func<Font, Font> createFont)
        {
            string deltaFontKey = AppearanceObject.GetDeltaFontKey(font, styleDelta, sizeDelta);
            if (AppearanceObject.deltaFontsCache == null)
                AppearanceObject.deltaFontsCache = (IDictionary<string, Font>)new Dictionary<string, Font>(8);
            Font font1;
            if (!AppearanceObject.deltaFontsCache.TryGetValue(deltaFontKey, out font1))
            {
                font1 = createFont(font);
                AppearanceObject.deltaFontsCache.Add(deltaFontKey, font1);
            }
            return font1;
        }

        private static string GetDeltaFontKey(Font font, FontStyle styleDelta, int sizeDelta)
        {
            NumberFormatInfo invariantInfo = NumberFormatInfo.InvariantInfo;
            return font.FontFamily.Name + " " + AppearanceObject.GetSize(font.Size, sizeDelta).ToString((IFormatProvider)invariantInfo) + "," + ((int)(font.Style | styleDelta)).ToString((IFormatProvider)invariantInfo) + "," + ((int)font.Unit).ToString((IFormatProvider)invariantInfo) + (font.GdiCharSet != (byte)1 ? "," + font.GdiCharSet.ToString((IFormatProvider)invariantInfo) : string.Empty) + (font.GdiVerticalFont ? "v" : string.Empty);
        }

        protected internal void AssignDeltaFontsAndSetFont(AppearanceObject source)
        {
            this.AssignDeltaFonts(source);
            this.SetFont(source._font);
        }

        protected internal void AssignDeltaFontsAndSetFont(AppearanceDefault source)
        {
            this.AssignDeltaFonts(source);
            this.SetFont(source.Font ?? this._font);
        }

        protected void AssignDeltaFonts(AppearanceObject source)
        {
            this.fontSizeDelta = source.FontSizeDelta;
            this.fontStyleDelta = source.FontStyleDelta;
            this.AssignDeltaFonts(source.deltaSourceFont);
            this.deltaBasedSize = source.deltaBasedSize;
            this.deltaBasedStyle = source.deltaBasedStyle;
        }

        protected void AssignDeltaFonts(AppearanceDefault source)
        {
            this.fontSizeDelta = source.FontSizeDelta;
            this.fontStyleDelta = source.FontStyleDelta;
            this.AssignDeltaFonts(this.FontSizeDelta != 0 || this.FontStyleDelta != FontStyle.Regular ? source.Font ?? this.GetFontCore() : (Font)null);
        }

        public int GetFontHeight(GraphicsCache cache)
        {
            return cache.Paint.GetFontHeight(this.GetFont());
        }

        public Size CalcTextSizeInt(GraphicsCache cache, string s, int width)
        {
            return this.CalcTextSizeInt(cache, this.GetTextOptions().GetStringFormat(), s, width);
        }

        public Size CalcTextSizeInt(GraphicsCache cache, StringFormat sf, string s, int width)
        {
            return cache.Paint.CalcTextSizeInt(cache.Graphics, s, this.Font, sf, width);
        }

        public Size CalcDefaultTextSize(GraphicsCache cache)
        {
            return cache.Paint.CalcDefaultTextSize(cache.Graphics, this.GetFont());
        }

        public SizeF CalcTextSize(GraphicsCache cache, string s, int width)
        {
            return this.CalcTextSize(cache, this.GetTextOptions().GetStringFormat(), s, width);
        }

        public SizeF CalcTextSize(GraphicsCache cache, StringFormat sf, string s, int width)
        {
            return cache.Paint.CalcTextSize(cache.Graphics, s, this.Font, sf, width);
        }

        public SizeF CalcTextSize(
          GraphicsCache cache,
          StringFormat sf,
          string s,
          int width,
          int height)
        {
            return cache.Paint.CalcTextSize(cache.Graphics, s, this.Font, sf, width, height);
        }

        public SizeF CalcTextSize(
          GraphicsCache cache,
          StringFormat sf,
          string s,
          int width,
          int height,
          out bool isCropped)
        {
            return cache.Paint.CalcTextSize(cache.Graphics, s, this.Font, sf, width, height, out isCropped);
        }

        bool ISerializableLayoutEx.AllowProperty(
          OptionsLayoutBase options,
          string propertyName,
          int id)
        {
            return true;
        }

        void ISerializableLayoutEx.ResetProperties(
          OptionsLayoutBase options)
        {
            this.BeginUpdate();
            try
            {
                this.Reset();
            }
            catch
            {
            }
            this.CancelUpdate();
        }

        void IXtraSerializable.OnStartSerializing()
        {
            ++this.serializing;
        }

        void IXtraSerializable.OnEndSerializing()
        {
            --this.serializing;
        }

        void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e)
        {
            ++this.deserializing;
        }

        void IXtraSerializable.OnEndDeserializing(string restoredVersion)
        {
            --this.deserializing;
        }

        [Obsolete("This property(method) is not operational in applications that utilize DirectX hardware acceleration and/or Per-Monitor/HiDPI support.")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int FontHeight
        {
            get
            {
                PaintApiTrace.Critical("AppearanceObject.FontHeight", (string)null);
                return this.GetFontHeightCore();
            }
        }

        private int GetFontHeightCore()
        {
            long fontHashCode = AppearanceHelper.GetFontHashCode(this.Font);
            object obj = AppearanceObject.FontHeights[(object)fontHashCode];
            if (obj == null)
            {
                obj = (object)this.Font.Height;
                AppearanceObject.FontHeights[(object)fontHashCode] = obj;
            }
            return (int)obj;
        }

        private static Hashtable FontHeights
        {
            get
            {
                if (AppearanceObject.fontHeights == null)
                    AppearanceObject.fontHeights = new Hashtable();
                return AppearanceObject.fontHeights;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Size CalcDefaultTextSize()
        {
            PaintApiTrace.Critical("AppearanceObject.CalcDefaultTextSize", (string)null);
            Graphics g = GraphicsInfo.Default.AddGraphics((Graphics)null);
            try
            {
                return this.CalcDefaultTextSizeCore(g);
            }
            finally
            {
                GraphicsInfo.Default.ReleaseGraphics();
            }
        }

        private static Hashtable FontSizes
        {
            get
            {
                if (AppearanceObject.fontSizes == null)
                    AppearanceObject.fontSizes = new Hashtable();
                return AppearanceObject.fontSizes;
            }
        }

        private Size CalcDefaultTextSizeCore(Graphics g)
        {
            long fontHashCode = AppearanceHelper.GetFontHashCode(this.Font);
            object obj = AppearanceObject.FontSizes[(object)fontHashCode];
            bool flag = g.PageUnit == GraphicsUnit.Display;
            if (obj == null || !flag)
            {
                Size size = XPaint.TextSizeRound(XPaint.Graphics.CalcTextSize(g, "Wg", this.Font, TextOptions.DefaultStringFormat, 0));
                if (!(XPaint.Graphics is XPaintMixed))
                    size.Height = Math.Max(this.GetFontHeightCore(), size.Height);
                obj = (object)size;
                if (flag)
                    AppearanceObject.FontSizes[(object)fontHashCode] = obj;
            }
            return (Size)obj;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual Size CalcDefaultTextSize(Graphics g)
        {
            PaintApiTrace.NotRecommendedBecauseOfGraphics("AppearanceObject.CalcDefaultTextSize");
            return this.CalcDefaultTextSizeCore(g);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Size CalcTextSizeInt(Graphics g, string s, int width)
        {
            return this.CalcTextSizeInt(g, this.GetTextOptions().GetStringFormat(), s, width);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Size CalcTextSizeInt(Graphics g, StringFormat sf, string s, int width)
        {
            PaintApiTrace.NotRecommendedBecauseOfGraphics("AppearanceObject.CalcTextSizeInt");
            return XPaint.Graphics.CalcTextSizeInt(g, s, this.Font, sf, width);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public SizeF CalcTextSize(Graphics g, string s, int width)
        {
            return this.CalcTextSize(g, this.GetTextOptions().GetStringFormat(), s, width);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public SizeF CalcTextSize(Graphics g, StringFormat sf, string s, int width)
        {
            PaintApiTrace.NotRecommendedBecauseOfGraphics("AppearanceObject.CalcTextSize");
            return XPaint.Graphics.CalcTextSize(g, s, this.Font, sf, width);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public SizeF CalcTextSize(Graphics g, StringFormat sf, string s, int width, int height)
        {
            PaintApiTrace.NotRecommendedBecauseOfGraphics("AppearanceObject.CalcTextSize");
            return XPaint.Graphics.CalcTextSize(g, s, this.Font, sf, width, height);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public SizeF CalcTextSize(
          Graphics g,
          StringFormat sf,
          string s,
          int width,
          int height,
          out bool isCropped)
        {
            PaintApiTrace.NotRecommendedBecauseOfGraphics("AppearanceObject.CalcTextSize");
            return XPaint.Graphics.CalcTextSize(g, s, this.Font, sf, width, height, out isCropped);
        }
    }

}
