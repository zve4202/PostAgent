using System.ComponentModel;
using System.Drawing;

namespace GH.Controls.Utils.Controls
{
    public class ToolTipControlInfo
    {
        private int interval = -1;
        private bool applyCursorOffset = true;
        private ToolTipLocation toolTipLocation;
        private Point toolTipPosition;
        private bool immediateToolTip;
        private object _object;
        private string text;
        private string title;
        private ToolTipIconType iconType;
        private ToolTipType toolTipType;
        private Image toolTipImage;

        public ToolTipControlInfo()
          : this((object)null, "")
        {
        }

        public ToolTipControlInfo(object _object, string text, ToolTipIconType iconType)
          : this(_object, text, false, iconType)
        {
        }

        public ToolTipControlInfo(object _object, string text)
          : this(_object, text, false, ToolTipIconType.None)
        {
        }

        public ToolTipControlInfo(object _object, string text, string title, ToolTipIconType iconType)
          : this(_object, text, title, false, iconType)
        {
        }

        public ToolTipControlInfo(object _object, string text, string title)
          : this(_object, text, title, false, ToolTipIconType.None)
        {
        }

        public ToolTipControlInfo(
          object _object,
          string text,
          bool immediateToolTip,
          ToolTipIconType iconType)
          : this(_object, text, "", immediateToolTip, iconType)
        {
        }

        public ToolTipControlInfo(
          object _object,
          string text,
          string title,
          bool immediateToolTip,
          ToolTipIconType iconType)
          : this(_object, text, title, immediateToolTip, iconType)
        {
        }

        public ToolTipControlInfo(
          object _object,
          string text,
          string title,
          bool immediateToolTip,
          ToolTipIconType iconType)
        {
            this._object = _object;
            this.text = text;
            this.title = title;
            this.immediateToolTip = immediateToolTip;
            this.iconType = iconType;
            this.toolTipLocation = ToolTipLocation.Default;
            this.toolTipPosition = new Point(-10000, -10000);
            this.ForcedShow = DefaultBoolean.Default;
            this.ToolTipIndent = 16;
        }

        [GHLocalizedDescription("ToolTipControlInfoForcedShow")]
        public bool ForcedShow { get; set; }

        [GHLocalizedDescription("ToolTipControlInfoHideHintOnMouseMove")]
        public bool HideHintOnMouseMove { get; set; }

        protected internal bool ApplyCursorOffset
        {
            get
            {
                return this.applyCursorOffset;
            }
            set
            {
                this.applyCursorOffset = value;
            }
        }

        [GHLocalizedDescription("ToolTipControlInfoToolTipImage")]
        public Image ToolTipImage
        {
            get
            {
                return this.toolTipImage;
            }
            set
            {
                this.toolTipImage = value;
            }
        }

        [GHLocalizedDescription("ToolTipControlInfoToolTipLocation")]
        public ToolTipLocation ToolTipLocation
        {
            get
            {
                return this.toolTipLocation;
            }
            set
            {
                this.toolTipLocation = value;
            }
        }

        [GHLocalizedDescription("ToolTipControlInfoToolTipPosition")]
        public Point ToolTipPosition
        {
            get
            {
                return this.toolTipPosition;
            }
            set
            {
                this.toolTipPosition = value;
            }
        }

        [GHLocalizedDescription("ToolTipControlInfoSuperTip")]
        public SuperToolTip SuperTip
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

        [GHLocalizedDescription("ToolTipControlInfoInterval")]
        public int Interval
        {
            get
            {
                return this.interval;
            }
            set
            {
                this.interval = value;
            }
        }

        [GHLocalizedDescription("ToolTipControlInfoText")]
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

        public ToolTipAnchor ToolTipAnchor { get; set; }

        [GHLocalizedDescription("ToolTipControlInfoObjectBounds")]
        public Rectangle ObjectBounds { get; set; }

        [GHLocalizedDescription("ToolTipControlInfoToolTipType")]
        public ToolTipType ToolTipType
        {
            get
            {
                return this.toolTipType;
            }
            set
            {
                this.toolTipType = value;
            }
        }

        [GHLocalizedDescription("ToolTipControlInfoTitle")]
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

        public virtual void Normalize()
        {
            this.Text = ToolTipControlInfo.NormalizeSimpleText(this.text);
            this.Title = ToolTipControlInfo.NormalizeSimpleText(this.title);
        }

        protected internal static string NormalizeSimpleText(string input)
        {
            if (input == null)
                return (string)null;
            int startIndex = input.IndexOf(char.MinValue);
            return startIndex != -1 ? input.Remove(startIndex) : input;
        }

        [GHLocalizedDescription("ToolTipControlInfoObject")]
        public object Object
        {
            get
            {
                return this._object;
            }
            set
            {
                this._object = value;
            }
        }

        [GHLocalizedDescription("ToolTipControlInfoImmediateToolTip")]
        public bool ImmediateToolTip
        {
            get
            {
                return this.immediateToolTip;
            }
            set
            {
                this.immediateToolTip = value;
            }
        }

        [GHLocalizedDescription("ToolTipControlInfoIconType")]
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

        [GHLocalizedDescription("ToolTipControlInfoAllowHtmlText")]
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

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int ToolTipIndent { get; set; }
    }

}
