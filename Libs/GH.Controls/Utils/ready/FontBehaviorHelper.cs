using Microsoft.Win32;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace GH.Controls.Utils
{
    public class FontBehaviorHelper
    {
        private static Font tahomaFont = (Font)null;
        private static bool userPreferencesSubscribed = false;
        private static bool fontPreferencesInitialized = false;
        private static List<object> defaultFonts;

        internal static WindowsFormsFontBehavior GetFontBehavior()
        {
            WindowsFormsFontBehavior formsFontBehavior = WindowsFormsSettings.FontBehavior;
            if (formsFontBehavior == WindowsFormsFontBehavior.Default)
                formsFontBehavior = WindowsFormsFontBehavior.UseTahoma;
            return formsFontBehavior;
        }

        internal static Font GetDefaultFont()
        {
            if (FontBehaviorHelper.ForcedFromSettingsFont != null)
                return FontBehaviorHelper.ForcedFromSettingsFont;
            switch (FontBehaviorHelper.GetFontBehavior())
            {
                case WindowsFormsFontBehavior.UseControlFont:
                    return SystemFonts.DefaultFont;
                case WindowsFormsFontBehavior.UseTahoma:
                    return FontBehaviorHelper.GetTahomaFont();
                case WindowsFormsFontBehavior.UseWindowsFont:
                    return SystemFonts.MessageBoxFont;
                case WindowsFormsFontBehavior.ForceWindowsFont:
                    return SystemFonts.MessageBoxFont;
                case WindowsFormsFontBehavior.ForceTahoma:
                    return FontBehaviorHelper.GetTahomaFont();
                default:
                    return SystemFonts.DefaultFont;
            }
        }

        private static Font GetTahomaFont()
        {
            if (FontBehaviorHelper.tahomaFont == null)
                FontBehaviorHelper.tahomaFont = FontBehaviorHelper.CreateTahomaFont();
            return FontBehaviorHelper.tahomaFont;
        }

        private static Font CreateTahomaFont()
        {
            try
            {
                return new Font(new FontFamily("Tahoma"), SystemFonts.DefaultFont.Size);
            }
            catch
            {
            }
            return SystemFonts.DefaultFont;
        }

        public static void CheckFont()
        {
            if (FontBehaviorHelper.fontPreferencesInitialized)
                return;
            FontBehaviorHelper.Update();
        }

        public static void Update()
        {
            FontBehaviorHelper.fontPreferencesInitialized = true;
            switch (FontBehaviorHelper.GetFontBehavior())
            {
                case WindowsFormsFontBehavior.UseControlFont:
                    FontBehaviorHelper.UpdateDefaultFont(SystemFonts.DefaultFont);
                    AppearanceObject.ResetDefaultFont();
                    FontBehaviorHelper.UnsubscribeUserPreferences();
                    break;
                case WindowsFormsFontBehavior.UseTahoma:
                    AppearanceObject.ResetDefaultFont();
                    FontBehaviorHelper.UpdateDefaultFont((Font)null);
                    FontBehaviorHelper.UnsubscribeUserPreferences();
                    break;
                case WindowsFormsFontBehavior.UseWindowsFont:
                    AppearanceObject.ResetDefaultFont();
                    FontBehaviorHelper.UpdateDefaultFont((Font)null);
                    FontBehaviorHelper.UnsubscribeUserPreferences();
                    break;
                case WindowsFormsFontBehavior.ForceWindowsFont:
                case WindowsFormsFontBehavior.ForceTahoma:
                    AppearanceObject.ResetDefaultFont();
                    FontBehaviorHelper.UpdateDefaultFont(FontBehaviorHelper.GetDefaultFont());
                    FontBehaviorHelper.SubscribeUserPreferences();
                    break;
            }
        }

        private static void UnsubscribeUserPreferences()
        {
            if (!FontBehaviorHelper.userPreferencesSubscribed)
                return;
            FontBehaviorHelper.userPreferencesSubscribed = false;
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(FontBehaviorHelper.OnUserPreferencesChanged);
        }

        private static void SubscribeUserPreferences()
        {
            if (FontBehaviorHelper.userPreferencesSubscribed)
                return;
            FontBehaviorHelper.userPreferencesSubscribed = true;
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(FontBehaviorHelper.OnUserPreferencesChanged);
        }

        private static void OnUserPreferencesChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            FontBehaviorHelper.tahomaFont = (Font)null;
            FontBehaviorHelper.Update();
        }

        private static void UpdateDefaultFont(Font font)
        {
            if (font == null && Control.DefaultFont.Equals((object)SystemFonts.DefaultFont))
                return;
            FieldInfo field1 = typeof(Control).GetField("defaultFont", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo field2 = typeof(Control).GetField("defaultFontHandleWrapper", BindingFlags.Static | BindingFlags.NonPublic);
            field1.SetValue((object)null, (object)font);
            if (FontBehaviorHelper.defaultFonts == null)
                FontBehaviorHelper.defaultFonts = new List<object>();
            FontBehaviorHelper.defaultFonts.Add(field2.GetValue((object)null));
            field2.SetValue((object)null, (object)null);
        }

        internal static Font ForcedFromSettingsFont { get; set; }
    }

}
