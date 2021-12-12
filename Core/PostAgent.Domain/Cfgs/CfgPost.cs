using GH.Cfg;
using GH.Forms.Annotations;
using Newtonsoft.Json;
using PostAgent.Domain.App;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PostAgent.Domain.Cfgs
{
    public class CfgPost : SectionCfg
    {
        [Display(Name = "Host", Description = "SMTP сервер. Обычно smtp.site.ru (.com) ", GroupName = "Server"), Required(ErrorMessage = "Поле не должно быть пустым")]
        [EdiltorClass(ControlType = EditorType.WebAddress)]
        public string Host { get; set; } = "smtp.yandex.ru";
        [EdiltorClass(Default = 25)]
        [Display(Name = "Port", Description = "Порт подключения. Обычно 25, 587, или 465", GroupName = "Server"), Required(ErrorMessage = "Поле не должно быть пустым")]
        public int Port { get; set; } = 25;

        [Display(Name = "User Name", Description = "Логин для подключения", GroupName = "Server"), Required(ErrorMessage = "Поле не должно быть пустым")]
        [EdiltorClass]
        public string Username { get; set; }

        [Display(Name = "Password", Description = "Пароль для подключения", GroupName = "Server"), Required(ErrorMessage = "Поле не должно быть пустым")]
        [EdiltorClass(ControlType = EditorType.Password)]
        public string Password { get; set; }
        [Display(Name = "Email", Description = "Email-адрес отправителя в письме", GroupName = "Server"), Required(ErrorMessage = "Поле не должно быть пустым")]
        [EdiltorClass(ControlType = EditorType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Display(Name = "Reply Email", Description = "Email-адрес для ответа, если отвечать нужно на другой адрес", GroupName = "Server")]
        [EdiltorClass(ControlType = EditorType.EmailAddress)]
        public string ReplyEmailAddress { get; set; }


        [Display(Name = "Display Index", Description = "Index данных отправителя в письме, вразных вариантах", GroupName = "Display Name")]
        [EdiltorClass(ControlType = EditorType.Combo)]
        [JsonIgnore]
        public EnumBaseId Selected { get; set; } = EnumBaseId.ShopId;

        [Display(Name = "Display Name", Description = "Имя отправителя в письме которое будет указано в письме", GroupName = "Display Name"), Required(ErrorMessage = "Поле не должно быть пустым")]
        [EdiltorClass]
        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                return DisplayNames[Selected];
            }
            set
            {
                DisplayNames[Selected] = value.Trim();
            }
        }
        public Dictionary<EnumBaseId, string> DisplayNames { get; set; } = new Dictionary<EnumBaseId, string>()
        {
            [EnumBaseId.ShopId] = "BRIDGENOTE",
            [EnumBaseId.DiscogsId] = "DISCOGS"
        };


    }
}
