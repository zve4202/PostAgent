using GH.Entity;
using GH.Forms.Annotations;
using GH.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace GH.Forms
{

    public static class ControlsHelper
    {
        public static Field[] GetFields<T>(string[] except = null, bool withKey = false)
                    where T : AbstractEntity

        {
            List<Field> fields = new List<Field>();

            foreach (PropertyInfo info in typeof(T).GetProperties().Where(x =>
            {
                var att = x.GetCustomAttribute<DisplayAttribute>();
                if (null == att)
                    return false;

                if (except != null && except.Contains(x.Name))
                    return false;

                return true;
            }))
            {
                fields.Add(new Field(info));
            }

            return fields.ToArray();
        }

        public static KeyValuePair<object, string>[] GetEnum(Type fieldType)
        {
            Dictionary<object, string> keys = new Dictionary<object, string>();
            try
            {
                foreach (Enum item in Enum.GetValues(fieldType))
                {
                    if (!keys.ContainsKey(item))
                        keys.Add(item, item.GetDescription());
                }
            }
            catch (Exception)
            {

                throw;
            }
            return keys.ToArray();
        }

        public static Label CreateLabel(Field field, int top, int left, int height = 0, int width = 0)
        {
            return CreateControl<Label>(field, top, left, height, width);
        }


        public static T CreateControl<T>(Field field, int top, int left, int height = 0, int width = 0)
        {
            object obj = Activator.CreateInstance<T>();
            if (obj is Control control)
            {
                control.Name = $"{typeof(T).Name.ToLower()}{field.Name.ToUpper()}";
                control.AutoSize = false;
                control.Top = top;
                control.Left = left;
                control.TabStop = field.ReadOnly;


                if (height > 0)
                {
                    control.Height = height;
                }
                if (width > 0)
                {
                    control.Width = width;
                }


                SetToolTip(control, field);

                if (control is Label label)
                {
                    label.Height = height - 1;
                    label.TextAlign = ContentAlignment.MiddleRight;
                    label.Text = field.Caption;
                    label.BackColor = Color.CornflowerBlue;
                    label.ForeColor = Color.White;
                }
                else
                if (control is NumericUpDown spin)
                {
                    spin.Maximum = int.MaxValue;
                    spin.UpDownAlign = LeftRightAlignment.Left;
                    spin.ReadOnly = field.ReadOnly;
                    spin.Tag = nameof(spin.Value);
                }
                else
                if (control is DateTimePicker date)
                {
                    date.Enabled = !field.ReadOnly;
                    date.Format = DateTimePickerFormat.Short;
                    if (field.FieldType == typeof(TimeSpan))
                        date.Format = DateTimePickerFormat.Time;
                    date.Tag = nameof(date.Value);
                }
                else
                if (control is TextBox text)
                {
                    if (field.MaxLength > 0)
                        text.MaxLength = field.MaxLength;

                    text.ReadOnly = field.ReadOnly;
                    text.Tag = nameof(text.Text);

                    if (field.EditorType == EditorType.Memo)
                    {
                        text.Multiline = true;
                        text.ScrollBars = ScrollBars.Both;
                    }
                }
                else
                if (control is TextPassword pass)
                {
                    if (field.MaxLength > 0)
                        pass.MaxLength = field.MaxLength;

                    pass.UseSystemPasswordChar = true;
                    pass.ReadOnly = field.ReadOnly;
                    pass.Tag = nameof(pass.Text);
                }
                else
                if (control is ComboBox combo)
                {
                    combo.Enabled = !field.ReadOnly;
                    combo.DropDownStyle = ComboBoxStyle.DropDown;
                    combo.Tag = nameof(combo.SelectedValue);
                    if (field.FieldType.IsEnum)
                    {
                        combo.DropDownStyle = ComboBoxStyle.DropDownList;
                        combo.DataSource = GetEnum(field.FieldType);
                        combo.ValueMember = "Key";
                        combo.DisplayMember = "Value";
                    }
                }
                else
                if (control is CheckBox check)
                {
                    check.Enabled = !field.ReadOnly;
                    check.AutoSize = false;
                    check.Tag = nameof(check.Checked);
                    check.Text = field.ToolTip;
                }
                else
                if (control is Button button)
                {
                    button.Text = field.Caption;
                    button.TabStop = false;
                    button.FlatStyle = FlatStyle.Popup;
                    button.Tag = nameof(button.Tag);
                }
                else
                if (control is TextPath textPath)
                {
                    textPath.Tag = nameof(textPath.Text);
                }

                if (control.Tag == null)
                    control.Tag = nameof(control.Text);
            }
            return (T)obj;
        }

        public static Control CreateControl(Field field, int top, int left, int height = 0, int width = 0)
        {
            Control control = null;
            switch (field.EditorType)
            {
                case EditorType.Text:
                    if (field.FieldType == typeof(int) || field.FieldType == typeof(double))
                        control = CreateControl<NumericUpDown>(field, top, left, height, width);
                    else
                    if (field.FieldType == typeof(DateTime) || field.FieldType == typeof(TimeSpan))
                        control = CreateControl<DateTimePicker>(field, top, left, height, width);
                    else
                        control = CreateControl<TextBox>(field, top, left, height, width);
                    break;
                case EditorType.TextPath:
                    control = CreateControl<TextPath>(field, top, left, height, width);
                    break;
                case EditorType.EmailAddress:
                    control = CreateControl<TextBox>(field, top, left, height, width);
                    break;
                case EditorType.WebAddress:
                    control = CreateControl<TextBox>(field, top, left, height, width);
                    break;
                case EditorType.Password:
                    control = CreateControl<TextPassword>(field, top, left, height, width);
                    break;
                case EditorType.Memo:
                    control = CreateControl<TextBox>(field, top, left, height, width);
                    break;
                case EditorType.Combo:
                    control = CreateControl<ComboBox>(field, top, left, height, width);
                    break;
                case EditorType.Check:
                    control = CreateControl<CheckBox>(field, top, left, height, width);
                    break;
                case EditorType.Button:
                    control = CreateControl<Button>(field, top, left, height, width);
                    break;
                default:
                    break;
            }

            return control;
        }

        public static void SetToolTip(Control control, Field field)
        {
            SetToolTip(control, field.ToolTip);
        }

        public static void SetToolTip(Control control, string hint)
        {
            ToolTip toolTip = LayoutHelper.components.Components.OfType<ToolTip>().FirstOrDefault();

            if (toolTip == null)
            {
                toolTip = new ToolTip()
                {
                    AutoPopDelay = 5000,
                    InitialDelay = 500,
                    ReshowDelay = 1000,
                    ShowAlways = true,
                };

                LayoutHelper.components.Add(toolTip);
            }

            toolTip.SetToolTip(control, hint);
        }

    }
}
