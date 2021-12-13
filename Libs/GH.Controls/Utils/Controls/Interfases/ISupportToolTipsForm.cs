using System.Windows.Forms;

namespace GH.Controls.Utils.Controls
{
    public interface ISupportToolTipsForm
    {
        bool ShowToolTipsWhenInactive { get; }

        bool ShowToolTipsFor(Form form);
    }

}
