using System.Windows.Forms;

namespace GH.Controls
{
    public class BaseButton : Button
    {
        public BaseButton()
        {
            TabStop = false;
            FlatStyle = FlatStyle.Flat;
            Cursor = Cursors.Hand;
        }
    }
}
