using GH.Entity;
using GH.Forms.Annotations;
using System.ComponentModel.DataAnnotations;

namespace PostAgent.Domain
{
    public class SendContext : AbstractEntity
    {

        [Display(Name = "Отправить", Description = "Отправить почту", GroupName = nameof(SendContext))]
        [EdiltorClass(ControlType = EditorType.ButtonStart)]
        public bool Begin
        {
            get => _begin;
            set
            {
                _begin = value;
                _stop = !_begin;
            }
        }
        private bool _begin = true;

        [Display(Name = "Остановить", Description = "Остановить отправку почты", GroupName = nameof(SendContext))]
        [EdiltorClass(ControlType = EditorType.ButtonStop)]
        public bool Stop
        {
            get => _stop;
            set
            {
                _stop = value;
            }
        }
        private bool _stop = false;

    }
}
