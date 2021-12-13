using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace GH.Controls.Utils
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BaseOptions : ViewStatePersisterCore, INotifyPropertyChanged
    {
        private static object boolFalse = (object)false;
        private static object boolTrue = (object)true;
        private int lockUpdate;
        protected internal BaseOptionChangedEventHandler ChangedCore;
        private PropertyChangedEventHandler onPropertyChanged;

        public BaseOptions(IViewBagOwner viewBagOwner, string objectPath)
          : base(viewBagOwner, objectPath)
        {
        }

        public BaseOptions()
          : this((IViewBagOwner)null, string.Empty)
        {
        }

        public virtual void Assign(BaseOptions options)
        {
        }

        public virtual void BeginUpdate()
        {
            ++this.lockUpdate;
        }

        public virtual void EndUpdate()
        {
            if (--this.lockUpdate != 0)
                return;
            this.OnChanged(BaseOptions.EmptyOptionChangedEventArgs.Instance);
        }

        public virtual void CancelUpdate()
        {
            --this.lockUpdate;
        }

        protected virtual bool IsLockUpdate
        {
            get
            {
                return this.lockUpdate != 0;
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                this.onPropertyChanged += value;
            }
            remove
            {
                this.onPropertyChanged -= value;
            }
        }

        protected internal virtual void RaisePropertyChanged(string propertyName)
        {
            if (this.onPropertyChanged == null)
                return;
            this.onPropertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnChanged(string option, bool oldValue, bool newValue)
        {
            this.OnChanged(option, oldValue ? BaseOptions.boolTrue : BaseOptions.boolFalse, newValue ? BaseOptions.boolTrue : BaseOptions.boolFalse);
        }

        protected void OnChanged(string option, object oldValue, object newValue)
        {
            this.RaisePropertyChanged(option);
            if (this.IsLockUpdate)
                return;
            this.RaiseOnChanged(new BaseOptionChangedEventArgs(option, oldValue, newValue));
        }

        protected virtual void OnChanged(BaseOptionChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.Name);
            if (this.IsLockUpdate)
                return;
            this.RaiseOnChanged(e);
        }

        protected virtual void RaiseOnChanged(BaseOptionChangedEventArgs e)
        {
            if (this.ChangedCore == null)
                return;
            this.ChangedCore((object)this, e);
        }

        protected internal bool ShouldSerialize()
        {
            return this.ShouldSerialize((IComponent)null);
        }

        public override string ToString()
        {
            return OptionsHelper.GetObjectText((object)this);
        }

        protected internal virtual bool ShouldSerialize(IComponent owner)
        {
            return UniversalTypeConverter.ShouldSerializeObject((object)this, owner);
        }

        public virtual void Reset()
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties((object)this);
            this.BeginUpdate();
            try
            {
                foreach (PropertyDescriptor propertyDescriptor in properties)
                    propertyDescriptor.ResetValue((object)this);
            }
            finally
            {
                this.EndUpdate();
            }
        }

        private sealed class EmptyOptionChangedEventArgs : BaseOptionChangedEventArgs
        {
            internal static readonly BaseOptionChangedEventArgs Instance = (BaseOptionChangedEventArgs)new BaseOptions.EmptyOptionChangedEventArgs();

            private EmptyOptionChangedEventArgs()
              : base(string.Empty, (object)null, (object)null)
            {
            }

            public override sealed object NewValue
            {
                get
                {
                    return (object)null;
                }
                set
                {
                }
            }
        }
    }
    public delegate void BaseOptionChangedEventHandler(object sender, BaseOptionChangedEventArgs e);

    public class BaseOptionChangedEventArgs : EventArgs
    {
        private string name;
        private object oldValue;
        private object newValue;

        public BaseOptionChangedEventArgs()
          : this("", (object)null, (object)null)
        {
        }

        public BaseOptionChangedEventArgs(string name, object oldValue, object newValue)
        {
            this.name = name;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public object OldValue
        {
            get
            {
                return this.oldValue;
            }
        }

        public virtual object NewValue
        {
            get
            {
                return this.newValue;
            }
            set
            {
                this.newValue = value;
            }
        }
    }

    public class OptionsHelper
    {
        public static string GetObjectText(object obj)
        {
            return OptionsHelper.GetObjectText(obj, false);
        }

        public static void SetOptionValue(object obj, string name, object value)
        {
            TypeDescriptor.GetProperties(obj)[name]?.SetValue(obj, value);
        }

        public static object GetOptionValue(object obj, string name)
        {
            return TypeDescriptor.GetProperties(obj)[name]?.GetValue(obj);
        }

        public static T GetOptionValue<T>(object obj, string name)
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(obj)[name];
            return property != null ? (T)property.GetValue(obj) : default(T);
        }

        public static string GetObjectText(object obj, bool includeSubObjects)
        {
            string str1 = string.Empty;
            try
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj))
                {
                    if (property.IsBrowsable && property.SerializationVisibility != DesignerSerializationVisibility.Hidden)
                    {
                        if (property.SerializationVisibility == DesignerSerializationVisibility.Content)
                        {
                            if (includeSubObjects)
                            {
                                object obj1 = property.GetValue(obj);
                                string str2 = obj1 != null ? obj1.ToString() : string.Empty;
                                if (!string.IsNullOrEmpty(str2))
                                {
                                    if (str1.Length > 0)
                                        str1 += ", ";
                                    str1 += string.Format("{0} = {{ {1} }}", (object)property.Name, (object)str2);
                                }
                            }
                        }
                        else if (!property.IsReadOnly && property.ShouldSerializeValue(obj))
                        {
                            if (str1.Length > 0)
                                str1 += ", ";
                            str1 += property.Name;
                            object obj1 = property.GetValue(obj);
                            str1 = !property.PropertyType.Equals(typeof(string)) ? str1 + string.Format(" = {0}", obj1) : str1 + string.Format(" = '{0}'", (object)property.Converter.ConvertToString(obj1));
                        }
                    }
                }
            }
            catch
            {
            }
            return str1;
        }
    }

    public class UniversalTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(InstanceDescriptor) || destinationType.Equals(typeof(byte[])) || base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.Equals(typeof(byte[])) || base.CanConvertFrom(context, sourceType);
        }

        protected virtual Type GetObjectType(object val)
        {
            return val.GetType();
        }

        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return value is byte[]? BinaryTypeConverter.ConvertFromBytes(value as byte[]) : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destinationType)
        {
            if (destinationType == (Type)null)
                throw new ArgumentNullException(nameof(destinationType));
            if (destinationType.Equals(typeof(byte[])))
                return (object)BinaryTypeConverter.ConvertToBytes(value);
            if (destinationType == typeof(InstanceDescriptor) && value != null)
            {
                Type objectType = this.GetObjectType(value);
                ConstructorInfo constructor = objectType.GetConstructor(new Type[0]);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
                int num = 0;
                List<PropertyDescriptor> list = (List<PropertyDescriptor>)null;
                foreach (PropertyDescriptor propertyDescriptor in properties)
                {
                    if (propertyDescriptor.SerializationVisibility != DesignerSerializationVisibility.Hidden && propertyDescriptor.ShouldSerializeValue(value))
                    {
                        if (list == null)
                            list = new List<PropertyDescriptor>();
                        list.Add(propertyDescriptor);
                        ++num;
                    }
                }
                object[] objArray = (object[])null;
                if (num > 0)
                {
                    constructor = this.FindConstructor(properties, constructor, this.GetConstructors(objectType), list);
                    objArray = this.GenerateParameters(properties, constructor, value);
                }
                if (constructor != (ConstructorInfo)null)
                    return (object)new InstanceDescriptor((MemberInfo)constructor, (ICollection)objArray);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        protected ConstructorInfo[] GetConstructors(Type ctorType)
        {
            return this.FilterConstructors(ctorType.GetConstructors());
        }

        protected virtual ConstructorInfo[] FilterConstructors(ConstructorInfo[] ctors)
        {
            return ctors;
        }

        protected string ExtractPropertyName(string valName)
        {
            string str = valName;
            if (str.StartsWith("_"))
                str = str.Substring(1);
            if (str.Length > 0)
                str = str.Substring(0, 1).ToUpper(CultureInfo.InvariantCulture) + str.Substring(1);
            return str;
        }

        protected virtual object[] GenerateParameters(
          PropertyDescriptorCollection properties,
          ConstructorInfo ctor,
          object val)
        {
            if (ctor == (ConstructorInfo)null)
                return (object[])null;
            ParameterInfo[] parameters = ctor.GetParameters();
            if (val == null || parameters.Length == 0)
                return (object[])null;
            object[] objArray = new object[parameters.Length];
            for (int index = 0; index < parameters.Length; ++index)
            {
                string propertyName = this.ExtractPropertyName(parameters[index].Name);
                PropertyDescriptor property = properties[propertyName];
                if (property == null)
                    throw new WarningException("[UniConstructor]: Can't find property " + propertyName);
                objArray[index] = property.GetValue(val);
            }
            return objArray;
        }

        protected virtual ConstructorInfo FindConstructor(
          PropertyDescriptorCollection properties,
          ConstructorInfo empty,
          ConstructorInfo[] ctors,
          List<PropertyDescriptor> list)
        {
            if (ctors == null || ctors.Length == 0 || (list == null || list.Count == 0))
                return empty;
            ConstructorInfo constructorInfo = (ConstructorInfo)null;
            int num = -1;
            foreach (ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] parameters = ctor.GetParameters();
                bool flag = true;
                foreach (PropertyDescriptor propertyDescriptor in list)
                {
                    if (!this.CheckParameter(propertyDescriptor.PropertyType, propertyDescriptor.Name, parameters))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    if (parameters.Length == list.Count)
                        return ctor;
                    if (constructorInfo == (ConstructorInfo)null || num > parameters.Length)
                    {
                        foreach (ParameterInfo parameterInfo in parameters)
                        {
                            string propertyName = this.ExtractPropertyName(parameterInfo.Name);
                            if (properties[propertyName] == null)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            constructorInfo = ctor;
                            num = parameters.Length;
                        }
                    }
                }
            }
            if (constructorInfo == (ConstructorInfo)null)
                constructorInfo = empty;
            return constructorInfo;
        }

        protected virtual bool CheckParameter(
          Type propertyType,
          string propertyName,
          ParameterInfo[] pars)
        {
            if (propertyName.Length > 0)
                propertyName = propertyName.Substring(0, 1).ToLower(CultureInfo.InvariantCulture) + propertyName.Substring(1);
            foreach (ParameterInfo par in pars)
            {
                if ((par.Name == propertyName || par.Name == "_" + propertyName) && par.ParameterType.Equals(propertyType))
                    return true;
            }
            return false;
        }

        public static void ResetObject(object checkObject)
        {
            if (checkObject == null)
                return;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(checkObject);
            if (properties == null || properties.Count == 0)
                return;
            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                if (propertyDescriptor.SerializationVisibility != DesignerSerializationVisibility.Hidden)
                    propertyDescriptor.ResetValue(checkObject);
            }
        }

        public static bool ShouldSerializeObject(object checkObject, IComponent owner)
        {
            IInheritanceService inheritanceService = owner == null || owner.Site == null ? (IInheritanceService)null : owner.Site.GetService(typeof(IInheritanceService)) as IInheritanceService;
            if (inheritanceService != null)
            {
                InheritanceAttribute inheritanceAttribute = inheritanceService.GetInheritanceAttribute(owner);
                if (inheritanceAttribute != null && inheritanceAttribute.InheritanceLevel != InheritanceLevel.NotInherited)
                    return true;
            }
            return UniversalTypeConverter.ShouldSerializeObject(checkObject);
        }

        public static bool ShouldSerializeObject(object checkObject)
        {
            if (checkObject == null)
                return false;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(checkObject);
            if (properties == null || properties.Count == 0)
                return false;
            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                if (propertyDescriptor.SerializationVisibility == DesignerSerializationVisibility.Content)
                {
                    if (UniversalTypeConverter.ShouldSerializeObject(propertyDescriptor.GetValue(checkObject)))
                        return true;
                }
                else if (propertyDescriptor.SerializationVisibility != DesignerSerializationVisibility.Hidden && propertyDescriptor.ShouldSerializeValue(checkObject))
                    return true;
            }
            return false;
        }
    }

    public class BinaryTypeConverter : TypeConverter
    {
        public BinaryTypeConverter()
        {
            GHAssemblyResolverEx.Init();
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.Equals(typeof(byte[])) || base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.Equals(typeof(byte[])) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destinationType)
        {
            return destinationType.Equals(typeof(byte[])) ? (object)BinaryTypeConverter.ConvertToBytes(value) : base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return value is byte[]? BinaryTypeConverter.ConvertFromBytes(value as byte[]) : base.ConvertFrom(context, culture, value);
        }

        internal static byte[] ConvertToBytes(object value)
        {
            if (value == null)
                return (byte[])null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ObjectConverter.BinaryFormatter.Serialize((Stream)memoryStream, value);
                return memoryStream.ToArray();
            }
        }

        internal static object ConvertFromBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
                return ObjectConverter.BinaryFormatter.Deserialize((Stream)memoryStream);
        }
    }

    public class GHAssemblyResolverEx
    {
        private static bool initialized = false;
        private static int locked = 0;
        private const int CodeRushForRoslynVersionBuild = 127;
        [ThreadStatic]
        private static Dictionary<string, Assembly> assemblies;
        private static Regex typeModuleRegEx;
        private static Regex versionRegEx;

        public static void Init()
        {
            if (!SecurityHelper.IsPermissionGranted((IPermission)new SecurityPermission(SecurityPermissionFlag.ControlAppDomain)))
                return;
            GHAssemblyResolverEx.InitInternal();
        }

        [SecuritySafeCritical]
        private static void InitInternal()
        {
            if (GHAssemblyResolverEx.initialized)
                return;
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(GHAssemblyResolverEx.OnAssemblyResolve);
            }
            catch
            {
            }
            GHAssemblyResolverEx.initialized = true;
        }

        [SecuritySafeCritical]
        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs e)
        {
            if (GHAssemblyResolverEx.locked != 0)
                return (Assembly)null;
            if (e.Name.StartsWith("DevExpress"))
            {
                if (e.Name.Contains("PublicKeyToken=null"))
                    return (Assembly)null;
                ++GHAssemblyResolverEx.locked;
                bool flag = e.Name.Contains(".Design");
                try
                {
                    Assembly assembly = GHAssemblyResolverEx.FindAssembly(e.Name, !flag);
                    if (assembly != (Assembly)null)
                        return assembly;
                    return e.Name.Contains(".resources") ? (Assembly)null : AssemblyCache.LoadWithPartialName(GHAssemblyResolverEx.GetValidAssemblyName(e.Name, !flag));
                }
                catch
                {
                }
                finally
                {
                    --GHAssemblyResolverEx.locked;
                }
            }
            return (Assembly)null;
        }

        public static Assembly FindAssembly(string name)
        {
            return GHAssemblyResolverEx.FindAssembly(name, true);
        }

        public static Assembly FindAssembly(string name, bool patchVersion)
        {
            string validShortName = GHAssemblyResolverEx.GetValidShortName(name, patchVersion);
            if (GHAssemblyResolverEx.assemblies != null && GHAssemblyResolverEx.assemblies.ContainsKey(validShortName))
                return GHAssemblyResolverEx.assemblies[validShortName];
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int index = assemblies.Length - 1; index >= 0; --index)
            {
                if (assemblies[index].FullName.StartsWith(validShortName, StringComparison.InvariantCulture))
                {
                    AssemblyName name1 = assemblies[index].GetName();
                    if (name1.Name == validShortName && name1.Version.ToString() == "17.2.7.0")
                    {
                        if (GHAssemblyResolverEx.assemblies == null)
                            GHAssemblyResolverEx.assemblies = new Dictionary<string, Assembly>();
                        GHAssemblyResolverEx.assemblies[validShortName] = assemblies[index];
                        return assemblies[index];
                    }
                }
            }
            return (Assembly)null;
        }

        private static string GetValidShortName(string assemblyName, bool patchVersion)
        {
            string validAssemblyName = GHAssemblyResolverEx.GetValidAssemblyName(assemblyName, patchVersion);
            string[] strArray = validAssemblyName.Split(',');
            return strArray.Length > 0 ? strArray[0] : validAssemblyName;
        }

        private static Regex GetTypeModuleRegEx()
        {
            if (GHAssemblyResolverEx.typeModuleRegEx == null)
                GHAssemblyResolverEx.typeModuleRegEx = new Regex("\\[(?<TypeName>[\\w\\s\\.=]+?)\\,+\\s*(?<AssemblyName>.+?)\\]", RegexOptions.Compiled);
            return GHAssemblyResolverEx.typeModuleRegEx;
        }

        internal static string GetValidTypeName(string typeName)
        {
            return string.IsNullOrEmpty(typeName) || !typeName.Contains("DevExpress") ? typeName : GHAssemblyResolverEx.GetTypeModuleRegEx().Replace(typeName, (MatchEvaluator)(target =>
            {
                Group group1 = target.Groups["AssemblyName"];
                Group group2 = target.Groups["TypeName"];
                return group1 == null || !group1.Success || (group2 == null || !group2.Success) ? target.ToString() : string.Format("[{0}, {1}]", (object)group2.Value, (object)GHAssemblyResolverEx.GetValidAssemblyName(group1.Value, true));
            }));
        }

        internal static string GetValidAssemblyName(string assemblyName)
        {
            return GHAssemblyResolverEx.GetValidAssemblyName(assemblyName, true);
        }

        internal static string GetValidAssemblyName(string assemblyName, bool patchVersion)
        {
            if (assemblyName == null)
                return assemblyName;
            string lower = assemblyName.ToLower(CultureInfo.InvariantCulture);
            if (lower.StartsWith("devexpress") && !lower.StartsWith("devexpress.expressapp") && (!lower.StartsWith("devexpress.persistence") && !lower.StartsWith("devexpress.persistent")) && !lower.StartsWith("devexpress.workflow"))
            {
                string[] typeParts = assemblyName.Split(',');
                if (typeParts != null && typeParts.Length > 1)
                {
                    for (int index = 0; index < typeParts.Length; ++index)
                        typeParts[index] = typeParts[index].Trim();
                    int versionIndex;
                    Version version = GHAssemblyResolverEx.GetVersion(typeParts, out versionIndex);
                    if (version != (Version)null && version.Build >= (int)sbyte.MaxValue)
                        return assemblyName;
                    string str = patchVersion ? GHAssemblyResolverEx.GetValidModuleName(typeParts[0]) : typeParts[0];
                    for (int index = 1; index < typeParts.Length; ++index)
                    {
                        if (index != versionIndex || !patchVersion)
                            str = str + ", " + typeParts[index];
                    }
                    assemblyName = str;
                }
                else
                    assemblyName = patchVersion ? GHAssemblyResolverEx.GetValidModuleName(assemblyName) : assemblyName;
                if (patchVersion)
                    assemblyName += ", Version=17.2.7.0";
            }
            return assemblyName;
        }

        private static Version GetVersion(string[] typeParts, out int versionIndex)
        {
            versionIndex = ((IList<string>)typeParts).FindIndex<string>((Predicate<string>)(x => x.ToLowerInvariant().StartsWith("version=", StringComparison.Ordinal)));
            if (versionIndex != -1)
            {
                try
                {
                    Version result = (Version)null;
                    if (Version.TryParse(typeParts[versionIndex].Substring("version=".Length), out result))
                        return result;
                }
                catch
                {
                }
            }
            return (Version)null;
        }

        private static Regex GetVersionRegEx()
        {
            if (GHAssemblyResolverEx.versionRegEx == null)
                GHAssemblyResolverEx.versionRegEx = new Regex(".v\\d+.\\d", RegexOptions.Compiled | RegexOptions.Singleline);
            return GHAssemblyResolverEx.versionRegEx;
        }

        internal static string GetValidModuleName(string assemblyName)
        {
            Regex versionRegEx = GHAssemblyResolverEx.GetVersionRegEx();
            if (versionRegEx.IsMatch(assemblyName))
                return versionRegEx.Replace(assemblyName, ".v17.2");
            if (assemblyName.IndexOf("3") != -1)
                return assemblyName.Replace("3", ".v17.2");
            if (assemblyName.IndexOf("4") != -1)
                return assemblyName.Replace("4", ".v17.2");
            return assemblyName.IndexOf(".Design") != -1 ? assemblyName.Replace(".Design", ".v17.2.Design") : assemblyName + ".v17.2";
        }
    }

    public static class SecurityHelper
    {
        private static bool forcePartialTrustMode;
        [ThreadStatic]
        private static PermissionCheckerSet permissionCheckerSet;

        public static bool IsPartialTrust
        {
            get
            {
                return SecurityHelper.forcePartialTrustMode || !SecurityHelper.IsPermissionGranted((IPermission)new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
            }
        }

        public static bool ForcePartialTrustMode
        {
            get
            {
                return SecurityHelper.forcePartialTrustMode;
            }
            set
            {
                SecurityHelper.forcePartialTrustMode = value;
            }
        }

        public static bool IsWeakRefAvailable
        {
            get
            {
                return SecurityHelper.IsPermissionGranted((IPermission)new WeakReferencePermission());
            }
        }

        public static bool IsUnmanagedCodeGrantedAndHasZeroHwnd
        {
            get
            {
                return SecurityHelper.IsPermissionGranted((IPermission)new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)) && GraphicsHelper.CanCreateFromZeroHwnd;
            }
        }

        public static bool IsUnmanagedCodeGrantedAndCanUseGetHdc
        {
            get
            {
                return SecurityHelper.IsPermissionGranted((IPermission)new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)) && GraphicsHelper.CanUseGetHdc;
            }
        }

        public static bool IsPermissionGranted(IPermission permission)
        {
            if (SecurityHelper.permissionCheckerSet == null)
                SecurityHelper.permissionCheckerSet = new PermissionCheckerSet();
            return SecurityHelper.permissionCheckerSet.IsGranted(permission);
        }

        public static void ForceRecheckPermissions()
        {
            SecurityHelper.permissionCheckerSet = (PermissionCheckerSet)null;
        }
    }

    internal class PermissionCheckerSet
    {
        private List<PermissionChecker> checkers = new List<PermissionChecker>();

        public bool IsGranted(IPermission permission)
        {
            PermissionChecker permissionChecker = this.GetChecker(permission);
            if (permissionChecker == null)
            {
                permissionChecker = new PermissionChecker(permission);
                this.checkers.Add(permissionChecker);
            }
            return permissionChecker.IsPermissionGranted;
        }

        private PermissionChecker GetChecker(IPermission permission)
        {
            foreach (PermissionChecker checker in this.checkers)
            {
                if (checker.Permission.GetType() == permission.GetType() && permission.IsSubsetOf(checker.Permission))
                    return checker;
            }
            return (PermissionChecker)null;
        }
    }

    public class PermissionChecker
    {
        private bool? isPermissionGranted;
        private IPermission permission;

        public IPermission Permission
        {
            get
            {
                return this.permission;
            }
        }

        public bool IsPermissionGranted
        {
            get
            {
                if (!this.isPermissionGranted.HasValue)
                    this.isPermissionGranted = new bool?(this.IsPermissionGrantedCore(this.permission));
                return this.isPermissionGranted.Value;
            }
        }

        public PermissionChecker(IPermission permission)
        {
            this.permission = permission;
        }

        private bool IsPermissionGrantedCore(IPermission permission)
        {
            try
            {
                permission.Demand();
                return true;
            }
            catch (SecurityException ex)
            {
                return false;
            }
        }
    }

    internal class WeakReferencePermission : IPermission, ISecurityEncodable
    {
        public IPermission Copy()
        {
            throw new NotImplementedException();
        }

        public void Demand()
        {
            WeakReference weakReference = new WeakReference(new object());
        }

        public IPermission Intersect(IPermission target)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IPermission target)
        {
            return target is WeakReferencePermission;
        }

        public IPermission Union(IPermission target)
        {
            throw new NotImplementedException();
        }

        public void FromXml(SecurityElement e)
        {
            throw new NotImplementedException();
        }

        public SecurityElement ToXml()
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectConverter
    {
        public static readonly ObjectConverterImplementation Instance = new ObjectConverterImplementation();

        internal static BinaryFormatter BinaryFormatter
        {
            get
            {
                return ObjectConverter.Instance.BinaryFormatter;
            }
        }

        public static string ObjectToString(object obj)
        {
            return ObjectConverter.Instance.ObjectToString(obj);
        }

        internal static string GetNextPart(string str, ref int index)
        {
            return ObjectConverter.Instance.GetNextPart(str, ref index);
        }

        internal static int GetIndexOfDelimiter(string str, int index)
        {
            return ObjectConverter.Instance.GetIndexOfDelimiter(str, index);
        }

        public static object StringToObject(string str, Type type)
        {
            return ObjectConverter.Instance.StringToObject(str, type);
        }

        public static string IntStructureToString(object obj)
        {
            return ObjectConverter.Instance.IntStructureToString(obj);
        }

        public static object IntStringToStructure(string str, Type type)
        {
            return ObjectConverter.Instance.IntStringToStructure(str, type);
        }
    }

    public class ObjectConverterImplementation
    {
        private static Type[] structTypes = new Type[6]
        {
      typeof (Point),
      typeof (PointF),
      typeof (Size),
      typeof (SizeF),
      typeof (Rectangle),
      typeof (RectangleF)
        };
        private ObjectConverters converters;
        private BinaryFormatter formatter;

        protected virtual ITypeDescriptorContext Context
        {
            get
            {
                return (ITypeDescriptorContext)null;
            }
        }

        public void CopyConvertersTo(ObjectConverterImplementation ocImplTo)
        {
            this.Converters.CopyTo(ocImplTo.Converters);
        }

        public Type ResolveType(string typeName)
        {
            return this.Converters.ResolveType(typeName);
        }

        public void RegisterConverter(IOneTypeObjectConverter converter)
        {
            this.Converters.RegisterConverter(converter);
        }

        public void UnregisterConverter(Type type)
        {
            this.Converters.UnregisterConverter(type);
        }

        public bool CanConvertToString(Type type)
        {
            return this.Converters.IsConverterExists(type);
        }

        public bool CanConvertFromString(Type type, string s)
        {
            IOneTypeObjectConverter converter = this.Converters.GetConverter(type);
            return !(converter is IOneTypeObjectConverter2) ? converter != null : ((IOneTypeObjectConverter2)converter).CanConvertFromString(s);
        }

        public string ConvertToString(object obj)
        {
            return this.Converters.ConvertToString(obj);
        }

        public object ConvertFromString(Type type, string str)
        {
            return this.Converters.ConvertFromString(type, str);
        }

        public ObjectConverterImplementation()
        {
            this.converters = new ObjectConverters();
        }

        protected virtual ObjectConverters Converters
        {
            get
            {
                return this.converters;
            }
        }

        internal BinaryFormatter BinaryFormatter
        {
            get
            {
                if (this.formatter != null)
                    return this.formatter;
                this.formatter = new BinaryFormatter();
                this.formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
                this.formatter.Binder = (SerializationBinder)new ObjectConverterImplementation.DXSerializationBinder();
                return this.formatter;
            }
        }

        private TypeConverter GetToStringConverter(Type type)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)) ? converter : (TypeConverter)null;
        }

        public virtual string ObjectToString(object obj)
        {
            if (obj == null)
                return (string)null;
            if (obj is string)
                return (string)obj;
            Type type = obj.GetType();
            if (this.CanConvertToString(type))
                return this.ConvertToString(obj);
            if (ObjectConverterImplementation.IsStructure(type))
                return this.StructureToString(obj);
            if (type.IsArray)
                return this.ArrayToString(obj, type);
            TypeConverter toStringConverter = this.GetToStringConverter(type);
            if (toStringConverter != null)
                return (string)toStringConverter.ConvertTo(this.Context, CultureInfo.InvariantCulture, obj, typeof(string));
            return !this.IsTypeSerializable(type) ? (string)null : this.SerialzeWithBinaryFormatter(obj);
        }

        protected internal virtual bool IsTypeSerializable(Type type)
        {
            return type.IsSerializable;
        }

        protected virtual string SerialzeWithBinaryFormatter(object obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            this.BinaryFormatter.Serialize((Stream)memoryStream, obj);
            return this.ToBase64String(memoryStream.ToArray());
        }

        private string ArrayToString(object obj, Type t)
        {
            Array array = (Array)obj;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("~Xtra#Array").Append(array.Length).Append(", ");
            for (int index = 0; index < array.Length; ++index)
                stringBuilder.Append(this.ArrayElementToString(array.GetValue(index)).Replace(",", ",,")).Append(", ");
            return stringBuilder.ToString();
        }

        private string ArrayElementToString(object obj)
        {
            if (obj == null)
                return "null";
            Type type = obj.GetType();
            if (!type.IsPrimitive)
            {
                switch (obj)
                {
                    case string _:
                    case DateTime _:
                    case Decimal _:
                        break;
                    default:
                        string str = this.ObjectToString(obj);
                        if (str == null)
                            throw new Exception(string.Format("Class {0} should either have a TypeConverter that can convert to/from a string, or implement the ISerializable interface to solve the problem.", (object)obj.GetType()));
                        return TypeCode.Object.ToString() + ", " + type.AssemblyQualifiedName.Replace(",", ",,") + ", " + str.Replace(",", ",,");
                }
            }
            return DXTypeExtensions.GetTypeCode(type).ToString() + ", " + this.PrimitiveToString(obj).Replace(",", ",,");
        }

        private string PrimitiveToString(object obj)
        {
            return obj is double num ? num.ToString("R", (IFormatProvider)CultureInfo.InvariantCulture) : Convert.ToString(obj, (IFormatProvider)CultureInfo.InvariantCulture);
        }

        internal string GetNextPart(string str, ref int index)
        {
            int startIndex = index;
            int indexOfDelimiter = this.GetIndexOfDelimiter(str, index);
            string str1;
            if (indexOfDelimiter < 0)
            {
                index = str.Length;
                str1 = str.Substring(startIndex);
            }
            else
            {
                index = indexOfDelimiter + 2;
                str1 = str.Substring(startIndex, indexOfDelimiter - startIndex);
            }
            return str1.Replace(",,", ",");
        }

        internal int GetIndexOfDelimiter(string str, int index)
        {
            for (int index1 = index; index1 < str.Length - 1; ++index1)
            {
                if (str[index1] == ',')
                {
                    switch (str[index1 + 1])
                    {
                        case ' ':
                            return index1;
                        case ',':
                            ++index1;
                            continue;
                        default:
                            continue;
                    }
                }
            }
            return -1;
        }

        private object StringToArray(string str, Type type)
        {
            str = str.Substring("~Xtra#Array".Length);
            int result = 0;
            int index1 = 0;
            int index2 = 0;
            if (!int.TryParse(this.GetNextPart(str, ref index1), out result))
                return (object)null;
            Array array = !(type == typeof(object)) ? Array.CreateInstance(type.GetElementType(), result) : (Array)new object[result];
            for (; index1 < str.Length && index2 < result; ++index2)
            {
                string nextPart = this.GetNextPart(str, ref index1);
                array.SetValue(this.StringToArrayElement(nextPart), index2);
            }
            return (object)array;
        }

        private object StringToArrayElement(string part)
        {
            int index = 0;
            string nextPart1 = this.GetNextPart(part, ref index);
            if (nextPart1 == "null")
                return (object)null;
            TypeCode typeCode = (TypeCode)Enum.Parse(typeof(TypeCode), nextPart1, false);
            if (typeCode != TypeCode.Object)
                return Convert.ChangeType((object)this.GetNextPart(part, ref index), typeCode, (IFormatProvider)CultureInfo.InvariantCulture);
            string nextPart2 = this.GetNextPart(part, ref index);
            return this.StringToObject(this.GetNextPart(part, ref index), Type.GetType(nextPart2));
        }

        private string ToBase64String(byte[] array)
        {
            return "~Xtra#Base64" + Convert.ToBase64String(array);
        }

        private object Base64ToObject(string str, Type type)
        {
            return this.BinaryFormatter.Deserialize((Stream)new MemoryStream(Convert.FromBase64String(str.Substring("~Xtra#Base64".Length))));
        }

        public virtual object StringToObject(string str, Type type)
        {
            if (type.Equals(typeof(string)))
                return (object)str;
            if (string.IsNullOrEmpty(str))
                return (object)null;
            if (str.StartsWith("~Xtra#Base64"))
                return this.Base64ToObject(str, type);
            if (str.StartsWith("~Xtra#Array"))
                return this.StringToArray(str, type);
            if (this.CanConvertFromString(type, str))
                return this.ConvertFromString(type, str);
            if (type.IsEnum)
                return ObjectConverterImplementation.EnumToObject(str, type);
            if (ObjectConverterImplementation.IsStructure(type))
                return this.StringToStructure(str, type);
            TypeConverter toStringConverter = this.GetToStringConverter(type);
            if (toStringConverter == null)
                return Convert.ChangeType((object)str, type, (IFormatProvider)CultureInfo.InvariantCulture);
            object obj = toStringConverter.ConvertFrom(this.Context, CultureInfo.InvariantCulture, (object)str);
            return ObjectConverterImplementation.IsNullable(type) || obj != null && type.IsAssignableFrom(obj.GetType()) ? obj : Convert.ChangeType(obj, type, (IFormatProvider)CultureInfo.InvariantCulture);
        }

        private static bool IsNullable(Type t)
        {
            return Nullable.GetUnderlyingType(t) != (Type)null;
        }

        private static bool IsStructure(Type t)
        {
            return Array.IndexOf<Type>(ObjectConverterImplementation.structTypes, t) != -1;
        }

        public string IntStructureToString(object obj)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                if (!propertyDescriptor.IsReadOnly && propertyDescriptor.PropertyType.IsPrimitive)
                {
                    string str = this.ObjectToString(propertyDescriptor.GetValue(obj)) ?? string.Empty;
                    stringBuilder.AppendFormat("{2}{0}={1}", (object)propertyDescriptor.Name, (object)str, stringBuilder.Length > 0 ? (object)"," : (object)string.Empty);
                }
            }
            return stringBuilder.ToString();
        }

        public object IntStringToStructure(string str, Type type)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            object instance = Activator.CreateInstance(type);
            string[] strArray = str.Split(',', '=');
            for (int index = 0; index < strArray.Length; index += 2)
            {
                PropertyDescriptor propertyDescriptor = properties[strArray[index]];
                object obj = this.StringToObject(strArray[index + 1], propertyDescriptor.PropertyType);
                propertyDescriptor.SetValue(instance, obj);
            }
            return instance;
        }

        private string StructureToString(object obj)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                if (!propertyDescriptor.IsReadOnly && propertyDescriptor.PropertyType.IsPrimitive)
                {
                    string str = this.ObjectToString(propertyDescriptor.GetValue(obj)) ?? string.Empty;
                    stringBuilder.AppendFormat("@{2},{0}={1}", (object)propertyDescriptor.Name, (object)str, (object)str.Length);
                }
            }
            return stringBuilder.ToString();
        }

        private object StringToStructure(string str, Type type)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            object instance = Activator.CreateInstance(type);
            int startIndex1 = 0;
            while (startIndex1 < str.Length)
            {
                int num = str.IndexOf("@", startIndex1);
                if (num != -1)
                {
                    startIndex1 = num + 1;
                    string[] strArray = str.Substring(startIndex1).Split(',', '=');
                    if (strArray != null && strArray.Length >= 2)
                    {
                        int int32 = Convert.ToInt32(strArray[0]);
                        string index = strArray[1];
                        int startIndex2 = startIndex1 + str.Substring(startIndex1).IndexOf("=") + 1;
                        PropertyDescriptor propertyDescriptor = properties[index];
                        if (propertyDescriptor != null)
                        {
                            string str1 = str.Substring(startIndex2, int32);
                            startIndex1 = startIndex2 + int32;
                            object obj = this.StringToObject(str1, propertyDescriptor.PropertyType);
                            propertyDescriptor.SetValue(instance, obj);
                        }
                    }
                    else
                        break;
                }
                else
                    break;
            }
            return instance;
        }

        private static object EnumToObject(string str, Type type)
        {
            return Enum.Parse(type, str, false);
        }

        private class ColorConverter : IOneTypeObjectConverter
        {
            private System.Drawing.ColorConverter colorConverter;

            public Type Type
            {
                get
                {
                    return typeof(Color);
                }
            }

            public ColorConverter()
            {
                this.colorConverter = new System.Drawing.ColorConverter();
            }

            public string ToString(object obj)
            {
                return this.colorConverter.ConvertToInvariantString(obj);
            }

            public object FromString(string str)
            {
                return this.colorConverter.ConvertFromInvariantString(str);
            }
        }

        private class DXSerializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (typeName == "DevExpress.XtraPivotGrid.PivotGridFieldFilterValues")
                    assemblyName = "DevExpress.PivotGrid.v17.2.Core";
                typeName = GHAssemblyResolverEx.GetValidTypeName(typeName);
                assemblyName = GHAssemblyResolverEx.GetValidAssemblyName(assemblyName);
                Assembly assembly = this.GetAssembly(assemblyName);
                return assembly != (Assembly)null ? assembly.GetType(typeName, false) : (Type)null;
            }

            private Assembly GetAssembly(string assemblyName)
            {
                Assembly assembly = (Assembly)null;
                try
                {
                    if (assemblyName.ToLower().StartsWith("devexpress"))
                        assembly = GHAssemblyResolverEx.FindAssembly(assemblyName);
                    return assembly != (Assembly)null ? assembly : Assembly.Load(assemblyName, (Evidence)null);
                }
                catch
                {
                    return (Assembly)null;
                }
            }
        }
    }

}
