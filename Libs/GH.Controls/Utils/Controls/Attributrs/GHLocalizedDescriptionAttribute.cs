using System;
using System.ComponentModel;
using System.Resources;

namespace GH.Controls.Utils.Controls
{
    [AttributeUsage(AttributeTargets.All)]
    internal class GHLocalizedDescriptionAttribute : DescriptionAttribute
    {
        private static ResourceManager rm;
        private bool loaded;

        public GHLocalizedDescriptionAttribute(string name)
          : base(name)
        {
        }

        public override string Description
        {
            get
            {
                if (!this.loaded)
                {
                    this.loaded = true;
                    if (GHLocalizedDescriptionAttribute.rm == null)
                    {
                        lock (typeof(GHLocalizedDescriptionAttribute))
                        {
                            if (GHLocalizedDescriptionAttribute.rm == null)
                                GHLocalizedDescriptionAttribute.rm = new ResourceManager("DevExpress.Utils.Descriptions", typeof(GHLocalizedDescriptionAttribute).Assembly);
                        }
                    }
                    this.DescriptionValue = GHLocalizedDescriptionAttribute.rm.GetString(base.Description);
                }
                return base.Description;
            }
        }
    }

}
