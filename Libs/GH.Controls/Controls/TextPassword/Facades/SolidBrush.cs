using System;
using System.Drawing;

namespace GH.Forms.Facades
{
    public sealed class SolidBrush : ISolidBrush, IDisposable
    {
        private readonly System.Drawing.SolidBrush native;

        public SolidBrush(Color color)
        {
            this.native = new System.Drawing.SolidBrush(color);
        }

        ~SolidBrush()
        {
            this.Dispose(false);
        }

        public Color Color
        {
            get
            {
                return this.native.Color;
            }
        }

        System.Drawing.SolidBrush ISolidBrush.Native
        {
            get
            {
                return this.native;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            this.native.Dispose();
        }
    }
}
