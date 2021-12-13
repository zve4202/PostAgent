namespace GH.Controls.Utils
{
    public interface IViewBagOwner
    {
        T GetViewBagProperty<T>(string objectPath, string propertyName, T value);

        void SetViewBagProperty<T>(string objectPath, string propertyName, T defaultValue, T value);
    }
}
