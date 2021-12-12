using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace GH.Controls.Facades
{
    public class Graphics : IGraphics, IDisposable
    {
        private readonly System.Drawing.Graphics native;

        public static explicit operator System.Drawing.Graphics(Graphics graphics)
        {
            return graphics.native;
        }

        public Graphics(Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            this.Control = control;
            this.native = this.Control.CreateGraphics();
        }

        ~Graphics()
        {
            this.Dispose(false);
        }

        public Control Control { get; }

        public TextRenderingHint TextRenderingHint
        {
            get
            {
                return this.native.TextRenderingHint;
            }
            set
            {
                this.native.TextRenderingHint = value;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            this.native.Dispose();
        }

        public void DrawString(string @string, Font font, ISolidBrush solidBrush, PointF point)
        {
            this.native.DrawString(@string, font, (Brush)solidBrush.Native, point);
        }

        public void FillRectangle(ISolidBrush solidBrush, RectangleF rectangle)
        {
            this.native.FillRectangle((Brush)solidBrush.Native, rectangle);
        }

        public SizeF MeasureString(string @string, Font font)
        {
            return this.native.MeasureString(@string, font);
        }
    }
}
