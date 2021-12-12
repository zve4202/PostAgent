using System;
using System.Drawing;
using System.Drawing.Text;

namespace GH.Controls.Facades
{
    public interface IGraphics : IDisposable
    {
        TextRenderingHint TextRenderingHint { get; set; }

        void DrawString(string @string, Font font, ISolidBrush solidBrush, PointF point);

        void FillRectangle(ISolidBrush solidBrush, RectangleF rectangle);

        SizeF MeasureString(string @string, Font font);
    }
}
