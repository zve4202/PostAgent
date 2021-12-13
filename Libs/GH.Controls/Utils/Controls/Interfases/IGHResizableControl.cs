using System;
using System.Drawing;

namespace GH.Controls.Utils.Controls
{
    public interface IGHResizableControl
    {
        Size MinSize { get; }

        Size MaxSize { get; }

        event EventHandler Changed;

        bool IsCaptionVisible { get; }
    }

}
