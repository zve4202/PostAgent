using System;

namespace GH.Controls.Facades
{
    public interface ISolidBrush : IDisposable
    {
        System.Drawing.SolidBrush Native { get; }
    }
}
