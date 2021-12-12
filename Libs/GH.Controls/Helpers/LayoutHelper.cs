using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using GH.Entity;

namespace GH.Forms
{
    public static class LayoutHelper
    {
        public static IContainer components;
        public static void CreateEntityGroup<T>(AbstrctControl control)
            where T : AbstractEntity

        {
            components = ObjectHelper.GetComponents(control) ?? new Container();

            var panels = new Dictionary<string, Panel>();
            FieldsPanel panel = null;

            foreach (var field in ControlsHelper.GetFields<T>())
            {
                if (panel == null || !panels.ContainsKey(field.Group))
                {
                    panel = BasePanel.CreateGroupPanel(nameof(panel) + field.Group.ToUpper(), control);
                    panels.Add(field.Group, panel);
                }
                panel.AddField(field);
            }
            BasePanel.CreateOkCancelPanel(control);
            FieldsPanel.AjastLayoutWithParent(control);
        }

        public static void CreateViewGroup<T>(AbstrctControl control)
            where T : AbstractEntity

        {
            var panels = new Dictionary<string, Panel>();
            FieldsPanel panel = null;

            foreach (var field in ControlsHelper.GetFields<T>())
            {
                if (panel == null || !panels.ContainsKey(field.Group))
                {
                    panel = BasePanel.CreateGroupPanel(nameof(panel) + field.Group.ToUpper(), control);
                    panels.Add(field.Group, panel);
                }
                panel.AddField(field);
            }
            BasePanel.CreateStartPanel(control);
            FieldsPanel.AjastLayoutWithParent(control);
        }

        public static int TextWidth(string text, Font f)
        {
            Bitmap bitmap = new Bitmap(1, 1);
            Graphics grph = Graphics.FromImage(bitmap);

            SizeF bounds = grph.MeasureString(text, f);
            return (int)Math.Floor(bounds.Width) + 1;
        }
    }
}
