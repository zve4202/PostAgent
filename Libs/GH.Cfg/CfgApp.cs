using GH.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace GH.Cfg
{
    public class CfgApp
    {
        public readonly Dictionary<string, AbstractEntity> Configs;
        public static EventHandler OnCreate = null;
        public CfgApp()
        {
            Configs = new Dictionary<string, AbstractEntity>();
            OnCreate?.Invoke(this, EventArgs.Empty);
        }
        private static string FileName
        {
            get
            {
                var name = Process.GetCurrentProcess().ProcessName;
                return $"{name}.ini";
            }
        }
        public void AddConfig<T>() where T: AbstractEntity
        {
            var cfg = Activator.CreateInstance<T>();
            Configs.Add(typeof(T).Name, cfg);
        }


        public T Get<T>() where T: AbstractEntity
        {            
            Configs.TryGetValue(typeof(T).Name, out AbstractEntity config);
            try
            {
                if (config == null)
                {
                    AddConfig<T>();
                    config = Configs[typeof(T).Name];
                }
                return (T)config;
            }
            catch 
            {
                AddConfig<T>();
                config = Configs[typeof(T).Name];
                return (T)config;
            }
        }

        public static void Save(CfgApp cfg)
        {
            FileInfo _fileInfo = new FileInfo(FileName);

            Directory.CreateDirectory(Path.GetDirectoryName(_fileInfo.FullName));
            if (_fileInfo.Exists)
                File.Delete(_fileInfo.FullName);

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All,
            };

            string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
            try
            {
                File.WriteAllText(_fileInfo.FullName, json, System.Text.Encoding.UTF8);
            }
            catch (Exception err)
            {
                Log.Error(err, "Error");
            }            
        }
        public static CfgApp Load()
        {
            FileInfo _fileInfo = new FileInfo(FileName);
            if (!_fileInfo.Exists)
            {
                return new CfgApp();
            }

            string json = File.ReadAllText(_fileInfo.FullName, Encoding.UTF8);
            if (string.IsNullOrEmpty(json))
            {
                return new CfgApp();
            }

            try
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.All,
                };
                return JsonConvert.DeserializeObject<CfgApp>(json);
            }
            catch (Exception err)
            {
                Log.Error(err, "Error");
                return new CfgApp();
            }
        }

    }
}
