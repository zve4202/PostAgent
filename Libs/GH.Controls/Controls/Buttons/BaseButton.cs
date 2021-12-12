using System.Windows.Forms;

namespace GH.Forms
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
