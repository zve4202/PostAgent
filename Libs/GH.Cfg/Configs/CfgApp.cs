using GH.Entity;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GH.Cfg
{
    public class AppCfg
    {
        public readonly Dictionary<string, SectionCfg> Configs;
        public AppCfg()
        {
            Configs = new Dictionary<string, SectionCfg>();
            AddConfig<FormCfg>();
        }
        private static string FileName
        {
            get
            {
                var name = Process.GetCurrentProcess().ProcessName;
                return $"{name}.ini";
            }
        }
        public void AddConfig<T>() where T : SectionCfg
        {
            var cfg = Activator.CreateInstance<T>();
            Configs.Add(typeof(T).Name, cfg);
        }


        public T Get<T>() where T : SectionCfg
        {
            Configs.TryGetValue(typeof(T).Name, out SectionCfg config);
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
        
        internal void Save()
        {
            Save(this);
        }

        public static void Save(AppCfg cfg)
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
        public static AppCfg Load()
        {
            FileInfo _fileInfo = new FileInfo(FileName);
            if (!_fileInfo.Exists)
            {
                return new AppCfg();
            }

            string json = File.ReadAllText(_fileInfo.FullName, Encoding.UTF8);
            if (string.IsNullOrEmpty(json))
            {
                return new AppCfg();
            }

            try
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.All,
                };
                return JsonConvert.DeserializeObject<AppCfg>(json);
            }
            catch (Exception err)
            {
                Log.Error(err, "Error");
                return new AppCfg();
            }
        }

    }
}
