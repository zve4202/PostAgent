using FirebirdSql.Data.FirebirdClient;
using GH.Entity;
using GH.Controls.Annotations;
using Newtonsoft.Json;
using PostAgent.Domain.App;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PostAgent.Domain.Cfgs
{
    public class CfgFirebird : AbstractEntity
    {

        public CfgFirebird()
        {

        }

        
        [Display(Name = "Server", Description = "Сервер для подключения (http://web.firebirdsql.org).\r\nЕсли подключение к серверу на другом компьютере то Remote [V]", GroupName = "Server")]
        [EdiltorClass]
        public string DataSource { get; set; } = "localhost";

        [Display(Name = "Remote", Description = "Если подключение к серверу на другом компьютере то Remote [V]", GroupName = "Server")]
        [EdiltorClass(ControlType = EditorType.Check, Default = true)]
        public bool Remote { get; set; } = true;

        [Display(Name = "Port", Description = "Порт для подключения. Обычно 3050", GroupName = "Server")]
        [EdiltorClass(Default = 3050)]
        public int Port { 
            get => port; 
            set => port = value; 
        }
        private int port = 3050;
        [EdiltorClass]
        [Display(Name = "Charset", Description = "Язык для подключения. Обычно WIN1251", GroupName = "Server")]
        public string Charset { get; set; } = "WIN1251";

        [Display(Name = "Dialect", Description = "Диалект БД. Обычно 3-й", GroupName = "Server")]
        [EdiltorClass]
        public int Dialect { get; set; } = 3;

        [Display(Name = "User ID", Description = "Пользователь базы данных. Обычно SYSDBA", GroupName = "Server")]
        [EdiltorClass]
        public string UserID { get; set; } = "SYSDBA";

        [Display(Name = "Password", Description = "Пароль для подключения к базе данных", GroupName = "Server")]
        [EdiltorClass(ControlType = EditorType.Password)]
        public virtual string Password { get; set; } = "42220";

        public Dictionary<EnumBaseId, string> Pathes { get; set; } = new Dictionary<EnumBaseId, string>()
        {
            [EnumBaseId.ShopId] = "ISHOP.FDB",
            [EnumBaseId.DiscogsId] = "DISCOGS.FDB"
        };

        [Display(Name = "Display Index", Description = "Имя отправителя в письме вразных вариантах", GroupName = "Pathes")]
        [EdiltorClass(ControlType = EditorType.Combo)]
        [JsonIgnore]
        public EnumBaseId Selected { get; set; } = EnumBaseId.ShopId;
        [Display(Name = "Path to DB", Description = "Путь к базе данных включая имя файла и расширение", GroupName = "Pathes")]
        [EdiltorClass(ControlType = EditorType.TextPath, SubText = "Найти", SubToolTip = "Найти файл Базы Данных")]
        [JsonIgnore]
        public string SelectedPath
        {
            get
            {
                return Pathes[Selected];
            }
            set
            {
                Pathes[Selected] = value.Trim();
            }

        }

        public string ConnectionString()
        {
            var csb = new FbConnectionStringBuilder();
            csb.Pooling = true;
            csb.DataSource = DataSource;
            csb.Dialect = Dialect;// 3;
            csb.Port = Port;// 3050;
            csb.ServerType = FbServerType.Default;
            csb.Charset = Charset;
            csb.Database = Pathes[Selected];
            csb.UserID = UserID;
            csb.Password = Password;
            return csb.ConnectionString;
        }

    }
}
