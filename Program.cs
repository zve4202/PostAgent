using GH.Context;
using PostAgent.forms;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace PostAgent
{
    class ThreadIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "ThreadId", Thread.CurrentThread.ManagedThreadId));
        }
    }

    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var name = Process.GetCurrentProcess().ProcessName;
            name = $"{name}Log-.txt";
            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadIdEnricher())
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.File(name, 
                    rollingInterval: RollingInterval.Day, 
                    outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}")
                .CreateLogger();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            RunContext.Run<MasterForm>();
        }        
    }
}
