using System.ComponentModel;

namespace GH.Controls.Utils
{
    public class OptionsLayoutBase : BaseOptions
    {
        private string layoutVersion = "";
        private static OptionsLayoutBase fullLayout;

        [DevExpressDataLocalizedDescription("OptionsLayoutBaseFullLayout")]
        [AutoFormatDisable]
        [NotifyParentProperty(true)]
        public static OptionsLayoutBase FullLayout
        {
            get
            {
                if (OptionsLayoutBase.fullLayout == null)
                    OptionsLayoutBase.fullLayout = new OptionsLayoutBase();
                return OptionsLayoutBase.fullLayout;
            }
        }

        [DevExpressDataLocalizedDescription("OptionsLayoutBaseLayoutVersion")]
        [AutoFormatDisable]
        [DefaultValue("")]
        [NotifyParentProperty(true)]
        public string LayoutVersion
        {
            get
            {
                return this.layoutVersion;
            }
            set
            {
                this.layoutVersion = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool ShouldSerializeCore(IComponent owner)
        {
            return this.ShouldSerialize(owner);
        }

        public override void Assign(BaseOptions source)
        {
            base.Assign(source);
            if (!(source is OptionsLayoutBase optionsLayoutBase))
                return;
            this.LayoutVersion = optionsLayoutBase.LayoutVersion;
        }

        [TypeConverter(typeof (ExpandableObjectConverter))]
  public class BaseOptions : ViewStatePersisterCore, INotifyPropertyChanged
  {
    private static object boolFalse = (object) false;
    private static object boolTrue = (object) true;
    private int lockUpdate;
    protected internal BaseOptionChangedEventHandler ChangedCore;
    private PropertyChangedEventHandler onPropertyChanged;

    public BaseOptions(IViewBagOwner viewBagOwner, string objectPath)
      : base(viewBagOwner, objectPath)
    {
    }

    public BaseOptions()
      : this((IViewBagOwner) null, string.Empty)
    {
    }

    public virtual void Assign(BaseOptions options)
    {
    }

    public virtual void BeginUpdate()
    {
      ++this.lockUpdate;
    }

    public virtual void EndUpdate()
    {
      if (--this.lockUpdate != 0)
        return;
      this.OnChanged(BaseOptions.EmptyOptionChangedEventArgs.Instance);
    }

    public virtual void CancelUpdate()
    {
      --this.lockUpdate;
    }

    protected virtual bool IsLockUpdate
    {
      get
      {
        return this.lockUpdate != 0;
      }
    }

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
      add
      {
        this.onPropertyChanged += value;
      }
      remove
      {
        this.onPropertyChanged -= value;
      }
    }

    protected internal virtual void RaisePropertyChanged(string propertyName)
    {
      if (this.onPropertyChanged == null)
        return;
      this.onPropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected void OnChanged(string option, bool oldValue, bool newValue)
    {
      this.OnChanged(option, oldValue ? BaseOptions.boolTrue : BaseOptions.boolFalse, newValue ? BaseOptions.boolTrue : BaseOptions.boolFalse);
    }

    protected void OnChanged(string option, object oldValue, object newValue)
    {
      this.RaisePropertyChanged(option);
      if (this.IsLockUpdate)
        return;
      this.RaiseOnChanged(new BaseOptionChangedEventArgs(option, oldValue, newValue));
    }

    protected virtual void OnChanged(BaseOptionChangedEventArgs e)
    {
      this.RaisePropertyChanged(e.Name);
      if (this.IsLockUpdate)
        return;
      this.RaiseOnChanged(e);
    }

    protected virtual void RaiseOnChanged(BaseOptionChangedEventArgs e)
    {
      if (this.ChangedCore == null)
        return;
      this.ChangedCore((object) this, e);
    }

    protected internal bool ShouldSerialize()
    {
      return this.ShouldSerialize((IComponent) null);
    }

    public override string ToString()
    {
      return OptionsHelper.GetObjectText((object) this);
    }

    protected internal virtual bool ShouldSerialize(IComponent owner)
    {
      return UniversalTypeConverter.ShouldSerializeObject((object) this, owner);
    }

    public virtual void Reset()
    {
      PropertyDescriptorCollection properties = TypeDescriptor.GetProperties((object) this);
      this.BeginUpdate();
      try
      {
        foreach (PropertyDescriptor propertyDescriptor in properties)
          propertyDescriptor.ResetValue((object) this);
      }
      finally
      {
        this.EndUpdate();
      }
    }

    private sealed class EmptyOptionChangedEventArgs : BaseOptionChangedEventArgs
    {
      internal static readonly BaseOptionChangedEventArgs Instance = (BaseOptionChangedEventArgs) new BaseOptions.EmptyOptionChangedEventArgs();

      private EmptyOptionChangedEventArgs()
        : base(string.Empty, (object) null, (object) null)
      {
      }

      public override sealed object NewValue
      {
        get
        {
          return (object) null;
        }
        set
        {
        }
      }
    }
  }
    }

}
