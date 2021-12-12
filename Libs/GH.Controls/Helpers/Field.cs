using GH.Controls.Annotations;
using GH.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows.Forms;

namespace GH.Controls
{
    public struct Field
    {
        public AbstractEntity Entity { get; private set; }
        public readonly EditorType EditorType;
        public readonly bool Key;
        public readonly string Name;
        public readonly string Caption;
        public readonly string ToolTip;
        public readonly object Value;
        public readonly Type FieldType;
        public readonly string DisplayFormat;
        public readonly string Format;
        public readonly string Group;
        public readonly object Default;
        public readonly bool ReadOnly;
        public readonly bool Required;
        public readonly CharacterCasing CharacterCasing;
        public readonly int MaxLength;

        public Field(PropertyInfo propInfo, AbstractEntity entity = null) : this()
        {
            Entity = entity;

            DisplayAttribute dispAtt = propInfo.GetCustomAttribute<DisplayAttribute>();
            EdiltorClassAttribute classAtt = propInfo.GetCustomAttribute<EdiltorClassAttribute>();

            Key = null != propInfo.GetCustomAttribute<KeyAttribute>();

            Caption = dispAtt.Name + ":";
            ToolTip = dispAtt.Description;
            Group = dispAtt.GroupName;

            EditorType = classAtt.ControlType;
            Default = classAtt.Default;
            ReadOnly = Key || classAtt.ReadOnly;
            Required = Key || classAtt.Required;
            CharacterCasing = classAtt.CharacterCasing;
            MaxLength = classAtt.MaxLength;



            Name = propInfo.Name;
            FieldType = propInfo.PropertyType;

            if (FieldType == typeof(int))
            {
                DisplayFormat = "{0:N0}";
                Format = "{N0}";
            }
            else
            if (FieldType == typeof(double))
            {
                DisplayFormat = "{0:f2}";
                Format = "{f2}";
            }
            else
            if (FieldType == typeof(DateTime))
            {
                DisplayFormat = "{d}";
                Format = "{d}";
            }
            else
            {
                DisplayFormat = null;
                Format = null;
            }

            if (entity != null)
                Value = propInfo.GetValue(entity);
            else
                Value = null;

        }
    }
}
