//#define TEST
using GH.Controls.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostAgent.Domain
{
    [Table("V_LETTERS_LIST")]
    public class Letter
    {
        [Key]
        [Column("PR_ID")]
        [Display(Name = "Id", Description = "№ письма", GroupName = "Base")]
        [EdiltorClass]
        public int Id { get; set; }
        [Column("C_NAME")]
        [Display(Name = "Name", Description = "Имя получателя", GroupName = "Base")]
        [EdiltorClass]
        public string DisplayName { get; set; }
        [Column("C_EMAIL")]
#if TEST
        [Display(Name = "Email", Description = "Email получателя", GroupName = "Base")]
        [EdiltorClass(ControlType = EditorType.EmailAddress)]
        public string EmailAddress
        {
            get
            {
                return "zve4202@yandex.ru";
            }
            set
            {
                _emailAddress = value;
            }
        }
        private string _emailAddress;
#else
        [Display(Name = "Email", Description = "Email получателя", GroupName = "Base")]
        [EdiltorClass(ControlType = EditorType.EmailAddress)]
        public string EmailAddress { get; set; }
#endif

        [Column("PR_SUBJECT")]
        [Display(Name = "Subject", Description = "Тема сообщения", GroupName = "Base")]
        [EdiltorClass]
        public string Subject { get; set; }
        [Column("PR_BODY")]
        [Display(Name = "Body", Description = "Текст сообщения", GroupName = "Base")]
        [EdiltorClass(ControlType = EditorType.Memo)]
        public string Body { get; set; }
        [Column("PR_ATTACHMENT")]
        [Display(Name = "Attachment", Description = "Прикрепление к письму", GroupName = "Base")]
        [EdiltorClass]
        public string Attachment { get; set; }
        [Column("PR_SENDED")]
        [Display(Name = "Sended", Description = "Отправлено", GroupName = "Base")]
        [EdiltorClass(ControlType = EditorType.Check)]
        public bool Sended { get; set; }
        [NotMapped]
        [Display(Name = "Send it", Description = "Отправить", GroupName = "Base")]
        [EdiltorClass(ControlType = EditorType.Check, Default = true)]
        public bool NeedSend { get; set; }
    }
}

