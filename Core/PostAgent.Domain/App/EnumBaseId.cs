using System.ComponentModel;

namespace PostAgent.Domain.App
{
    public enum EnumBaseId
    {
        [Description("Отношение к I-SHOP")]
        ShopId,
        [Description("Отношение к DISCOGS")]
        DiscogsId
    }
}
