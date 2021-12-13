using System.Runtime.InteropServices;

namespace GH.Controls.Utils
{
    [ComVisible(false)]
    public interface ISerializableLayoutEx
    {
        bool AllowProperty(OptionsLayoutBase options, string propertyName, int id);

        void ResetProperties(OptionsLayoutBase options);
    }

}
