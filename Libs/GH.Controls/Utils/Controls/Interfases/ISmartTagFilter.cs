using System.ComponentModel;

namespace GH.Controls.Utils.Controls
{
    public interface ISmartTagFilter
    {
        void SetComponent(IComponent component);

        bool FilterProperty(MemberDescriptor descriptor);

        bool FilterMethod(string MethodName, object actionMethodItem);
    }

}
