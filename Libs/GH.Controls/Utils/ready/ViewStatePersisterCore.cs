namespace GH.Controls.Utils
{
    public class ViewStatePersisterCore
  {
    private IViewBagOwner viewBagOwner;
    private string objectPath;

    public ViewStatePersisterCore()
      : this((IViewBagOwner) null)
    {
    }

    public ViewStatePersisterCore(IViewBagOwner viewBagOwner)
      : this(viewBagOwner, string.Empty)
    {
    }

    public ViewStatePersisterCore(IViewBagOwner viewBagOwner, string objectPath)
    {
      this.viewBagOwner = viewBagOwner;
      this.objectPath = objectPath;
    }

    protected IViewBagOwner ViewBagOwner
    {
      get
      {
        return this.viewBagOwner;
      }
    }

    protected virtual string ViewBagObjectPath
    {
      get
      {
        return this.objectPath == null ? this.GetType().Name : this.objectPath;
      }
    }

    protected virtual T GetViewBagProperty<T>(string name, T value)
    {
      return this.viewBagOwner == null ? value : this.viewBagOwner.GetViewBagProperty<T>(this.ViewBagObjectPath, name, value);
    }

    protected virtual void SetViewBagProperty<T>(string name, T defaultValue, T value)
    {
      if (this.viewBagOwner == null)
        return;
      this.viewBagOwner.SetViewBagProperty<T>(this.ViewBagObjectPath, name, defaultValue, value);
    }
  }
}
