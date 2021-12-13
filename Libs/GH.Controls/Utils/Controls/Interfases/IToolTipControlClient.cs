using System.Drawing;
using System.Runtime.InteropServices;

namespace GH.Controls.Utils.Controls
{
    [ComVisible(false)]
    public interface IToolTipControlClient
    {
        ToolTipControlInfo GetObjectInfo(Point point);

        bool ShowToolTips { get; }
    }

}
