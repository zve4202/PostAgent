using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace GH.Controls.Utils.Controls
{
    public class Utility
    {
        internal static ProductKind[] productList = new ProductKind[2]
        {
      ProductKind.XtraReports,
      ProductKind.XPO
        };
        internal static string[] productText = new string[2]
        {
      "XtraReports",
      "eXpressPersistentObjects"
        };
        protected static TraceSwitch licensingSwitch = new TraceSwitch("DXAbout", "");
        private static bool staticAboutShown = false;
        internal static bool? exp = new bool?();
        private static bool isDesignMode;
        private static bool? isDebuggerAttached;

        private static string ReadVersion(Guid id, int version)
        {
            RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("Licenses\\" + id.ToString());
            object obj = (object)null;
            if (registryKey == null)
                return (string)null;
            try
            {
                obj = registryKey.GetValue(version.ToString());
            }
            finally
            {
                registryKey.Close();
            }
            return obj?.ToString().Trim();
        }

        private static int ReadVersionEx(Guid id, int version)
        {
            RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("Licenses\\" + id.ToString());
            if (registryKey == null)
                return -1;
            try
            {
                return (int)registryKey.GetValue(version.ToString(), (object)-1);
            }
            catch
            {
                return -1;
            }
            finally
            {
                registryKey.Close();
            }
        }

        public static int IsOnlyWin()
        {
            return Utility.IsOnly("Windows Forms");
        }

        public static int IsOnlyWeb()
        {
            return Utility.IsOnly("ASP.NET");
        }

        public static int IsOnlyWpf()
        {
            return Utility.IsOnly("WPF Components");
        }

        public static int IsOnly(string platform)
        {
            try
            {
                Guid guid = new Guid("{BAA304DF-1198-4BB5-AFCB-14801F755AA6}");
                RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("Licenses\\" + guid.ToString());
                if (registryKey == null)
                    return 0;
                object obj = registryKey.GetValue(172.ToString() + platform, (object)0);
                registryKey.Close();
                if (obj == null)
                    return 0;
                if ((int)obj == 1)
                    return 1;
                if ((int)obj == 2)
                    return 2;
            }
            catch
            {
            }
            return 0;
        }

        public static string GetExpiredText(int width, int height)
        {
            string str = "Your trial period has expired.\r\nVisit www.devexpress.com to purchase a copy and activate your license.";
            if (width < 100 || height < 50)
                str = "Expired.";
            Type type = Type.GetType("DevExpress.Data.Properties.Resources");
            if (type != (Type)null)
            {
                string propertyText = Utility.GetPropertyText(type, width < 100 || height < 50 ? "ExpiredTextShort" : "ExpiredTextLong");
                if (!string.IsNullOrEmpty(propertyText))
                    str = propertyText;
            }
            return str;
        }

        private static string GetPropertyText(Type type, string name)
        {
            PropertyInfo property = type.GetProperty(name, BindingFlags.Static | BindingFlags.NonPublic);
            return property != (PropertyInfo)null ? string.Format("{0}", property.GetValue((object)null, new object[0])) : string.Empty;
        }

        public static string GetRegisteredText()
        {
            string str1 = string.Empty;
            UserData info = Utility.GetInfo();
            if (info == null)
                return string.Empty;
            for (int index = 0; index < Utility.productList.Length; ++index)
            {
                ProductKind product = Utility.productList[index];
                if (info.IsLicensed(product))
                {
                    string str2 = str1 + Utility.productText[index];
                    if (info.IsLicensedSource(product))
                        str2 += " (with source)";
                    str1 = str2 + "\r\n";
                }
            }
            return str1;
        }

        public static string GetTrialText()
        {
            string str = string.Empty;
            UserData info = Utility.GetInfo();
            for (int index = 0; index < Utility.productList.Length; ++index)
            {
                ProductKind product = Utility.productList[index];
                if (info == null || !info.IsLicensed(product))
                    str = str + Utility.productText[index] + "\r\n";
            }
            return str;
        }

        public static void TraceWithCallStack(string message)
        {
            if (!Utility.licensingSwitch.TraceInfo)
                return;
            StackTrace stackTrace = new StackTrace();
            Trace.WriteLine(DateTime.Now.ToString());
            Trace.WriteLine(message + Environment.NewLine + stackTrace.ToString());
        }

        public static bool IsDesignMode
        {
            get
            {
                return Utility.isDesignMode;
            }
            set
            {
                Utility.isDesignMode = value;
                if (!value)
                    return;
                UAlgo.DesignMode();
            }
        }

        public static bool GetAllowStaticAbout()
        {
            Utility.TraceWithCallStack("GetAllowStaticAbout invoked");
            if (Utility.staticAboutShown || !Utility.ShouldShowAbout())
                return false;
            Utility.staticAboutShown = true;
            try
            {
                Assembly entryAssembly = Assembly.GetEntryAssembly();
                AssemblyName assemblyName = entryAssembly != (Assembly)null ? entryAssembly.GetName() : (AssemblyName)null;
                if (assemblyName != null)
                {
                    if (assemblyName.Name.ToLower() == "lc")
                        return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ShouldShowAbout()
        {
            try
            {
                return Utility.ShouldShowAboutCore();
            }
            catch
            {
                return true;
            }
        }

        public static bool IsDebuggerAttached
        {
            get
            {
                if (Utility.isDebuggerAttached.HasValue)
                    return Utility.isDebuggerAttached.Value;
                try
                {
                    Utility.isDebuggerAttached = new bool?(Debugger.IsAttached);
                }
                catch
                {
                    Utility.isDebuggerAttached = new bool?(false);
                }
                return Utility.isDebuggerAttached.Value;
            }
        }

        private static bool ShouldShowAboutCore()
        {
            Utility.TraceWithCallStack("ShouldShowAboutCore invoked");
            if (!Utility.IsDesignMode && !Utility.IsDebuggerAttached || Utility.IsExpired())
                return true;
            string name = "LastAboutShowedTime";
            RegistryKey registryKey = (RegistryKey)null;
            try
            {
                registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\DevExpress\\Components\\", true) ?? Registry.CurrentUser.CreateSubKey("SOFTWARE\\DevExpress\\Components\\");
            }
            catch
            {
            }
            if (registryKey == null)
                return true;
            string text = registryKey.GetValue(name, (object)null) as string;
            DateTimeConverter dateTimeConverter = new DateTimeConverter();
            bool flag;
            if (text == null)
            {
                flag = true;
            }
            else
            {
                try
                {
                    flag = Math.Abs((DateTime.Now - (DateTime)dateTimeConverter.ConvertFromString((ITypeDescriptorContext)null, CultureInfo.InvariantCulture, text)).TotalMinutes) >= 30.0;
                }
                catch
                {
                    flag = true;
                }
            }
            if (flag)
                registryKey.SetValue(name, (object)dateTimeConverter.ConvertToString((ITypeDescriptorContext)null, CultureInfo.InvariantCulture, (object)DateTime.Now), RegistryValueKind.String);
            Utility.TraceWithCallStack("ShouldShowAboutCore result is:" + flag.ToString());
            return flag;
        }

        public static ProductKind GetDXperienceKind()
        {
            UserData info = Utility.GetInfo();
            if (info == null)
                return ProductKind.Default;
            if (info.IsLicensed(ProductKind.DXperienceEnt))
                return ProductKind.DXperienceEnt;
            return info.IsLicensed(ProductKind.DXperiencePro) ? ProductKind.DXperiencePro : ProductKind.Default;
        }

        public static bool IsExpired()
        {
            if (Utility.exp.HasValue)
                return Utility.exp.Value;
            UserData infoEx = Utility.GetInfoEx();
            Utility.exp = new bool?(infoEx == null || infoEx.IsExpired);
            return Utility.exp.Value;
        }

        public static int DaysLeft()
        {
            if (Utility.IsExpired())
                return 0;
            UserData infoEx = Utility.GetInfoEx();
            return infoEx != null ? Math.Max(1, (int)infoEx.expiration.Subtract(DateTime.Now).TotalDays) : 0;
        }

        public static bool IsLic()
        {
            UserData infoEx = Utility.GetInfoEx();
            return infoEx != null && !infoEx.IsTrial && infoEx.UserNo >= 0;
        }

        public static UserData GetInfoEx()
        {
            return Utility.GetInfo() ?? new UserData(Utility.GetTicks());
        }

        internal static DateTime GetTicks()
        {
            int num = Utility.ReadVersionEx(new Guid(1863287401, (short)5398, (short)17606, (byte)189, (byte)48, (byte)14, (byte)144, (byte)190, (byte)39, (byte)135, (byte)28), 172);
            try
            {
                if (num < 1)
                    return new DateTime(2013, 5, 22).AddDays(30.0);
                int day = num / 1234;
                int month = num % 1234 / 38;
                return new DateTime(2000 + num % 1234 % 38, month, day).AddDays(30.0);
            }
            catch
            {
                return new DateTime(2013, 5, 22).AddDays(30.0);
            }
        }

        public static UserData GetInfo()
        {
            string key = Utility.ReadVersion(new Guid(58230061U, (ushort)54679, (ushort)18994, (byte)182, (byte)217, (byte)104, (byte)10, (byte)22, (byte)163, (byte)205, (byte)166), 172);
            if (key == null)
                return (UserData)null;
            UserData userData = UserInfoChecker.Default.Parse(key);
            if (userData == null || !userData.IsValid)
                return (UserData)null;
            userData.expiration = Utility.GetTicks();
            return userData;
        }

        public static bool IsFreeOfferOnly(ProductKind kind, ProductInfoStage stage)
        {
            if (stage != ProductInfoStage.Registered)
                return false;
            UserData info = Utility.GetInfo();
            if (info == null || !info.IsValid || (!info.IsLicensed(ProductKind.FreeOffer) || kind == ProductKind.FreeOffer))
                return false;
            kind &= ~ProductKind.FreeOffer;
            return !info.IsLicensed(kind) || kind == ProductKind.FreeOffer;
        }

        public static string GetSerial(ProductKind kind, ProductInfoStage stage)
        {
            if (stage != ProductInfoStage.Registered)
                return string.Empty;
            UserData info = Utility.GetInfo();
            if (info == null || !info.IsValid || kind != ProductKind.Default && !info.IsLicensed(kind) && ((kind & ProductKind.FreeOffer) != ProductKind.FreeOffer || !info.IsLicensed(ProductKind.FreeOffer) && !info.IsLicensed(kind & ~ProductKind.FreeOffer)))
                return string.Empty;
            return string.Format("{2}, {0}/{1} (#{3})", (object)info.UserNo, (object)info.KeyNumber, (object)info.UserName, (object)info.licensedProducts);
        }
    }

}
