using System;
using System.Windows.Forms;

namespace GH.Forms.Annotations
{
    public class EdiltorClassAttribute : Attribute
    {
        public EditorType ControlType { get; set; } = EditorType.Text;
        public EditorType SubControlType { get; set; } = EditorType.Button;
        public string SubText { get; set; }
        public string SubToolTip { get; set; }
        public bool Required { get; set; } = false;
        public bool ReadOnly { get; set; } = false;
        public object Default { get; set; } = null;
        public int MaxLength { get; set; } = 0;
        public CharacterCasing CharacterCasing { get; set; }

    }
}
