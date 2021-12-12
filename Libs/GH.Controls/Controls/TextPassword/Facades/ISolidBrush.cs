using System;

namespace GH.Forms.Facades
{
    public interface ISolidBrush : IDisposable
    {
        System.Drawing.SolidBrush Native { get; }
    }
}
