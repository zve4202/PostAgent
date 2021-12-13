using System.Windows.Forms;

namespace GH.Forms
{
    public class DialogButton : Button
    {
        public static DialogButton GetDialogButton(DialogResult dialog) 
        {
            var btn = new DialogButton();
            btn.DialogResult = dialog;
            btn.Name = dialog.ToString() + "Button";
            return btn;
        }
        public DialogButton()
        {
            TabStop = false;
            FlatStyle = FlatStyle.Flat;
            Cursor = Cursors.Hand;
        }
    }
}
