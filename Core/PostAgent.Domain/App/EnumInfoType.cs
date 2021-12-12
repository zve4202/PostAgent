using System.ComponentModel;

namespace PostAgent.Domain.App
{
    public enum InfoType
    {
        [Description("INFO")]
        Info,
        [Description("ERROR")]
        Error
    }
}
