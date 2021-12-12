using GH.Cfg;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GH.Context
{
    public class RunContext: ApplicationContext        
    {
        private static Mutex _mutex;
        private static NamedPipeManager namedPipe;
        protected static RunContext Instance;
        private static CfgApp cfgApp = null;
        public static CfgApp GetCfgApp()
        {
            if (cfgApp == null)
            {
                cfgApp = CfgApp.Load();
            }

            return cfgApp;
        }

        public static void SaveCfgApp()
        {
            if (cfgApp != null)
            {
                CfgApp.Save(cfgApp);
            }
        }

        private static void Activate()
        {
            if (Instance != null && Instance.MainForm != null)
            {
                if (Instance.MainForm.WindowState == FormWindowState.Minimized)
                    Instance.MainForm.WindowState = FormWindowState.Normal;
                Instance.MainForm.Activate();
            }
        }
        internal static void Invoke(Action action)
        {
            if (Instance != null && Instance.MainForm != null)
            {
                try
                {
                    if (Instance.MainForm.InvokeRequired)
                        Instance.MainForm.Invoke(action);
                    else
                        action.Invoke();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error");
                }
            }
            else
                Log.Error($"{nameof(Instance)} == null or {nameof(Instance.MainForm)} == null", "Error");

        }

        private static void PipeReceiveString(string obj)
        {
            switch (obj)
            {
                case NamedPipeManager.ACTIVE_STRING:
                    Activate();
                    break;
                default:
                    break;
            }
        }
        public static void Run<T>() where T : Form
        {

            string m_name = "Mutex_" + Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            _mutex = new Mutex(true, m_name, out bool RuningNow);

            if (RuningNow)
            {
                namedPipe = new NamedPipeManager();
                namedPipe.ReceiveString += PipeReceiveString;
                namedPipe.Start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (CreateAppContext<RunContext,T>())
                {
                    AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e)
                    {
                        Log.Error((string)e.ExceptionObject, "Error");
                    };


                    Application.ThreadException += delegate (Object sender, ThreadExceptionEventArgs e)
                    {
                        Log.Error(e.Exception, "Error");
                        Environment.Exit(0);
                    };
                }
                else
                    Environment.Exit(0);
            }
            else
            {
                NamedPipeManager.Write(NamedPipeManager.ACTIVE_STRING);
                Application.Exit();
            }


            Application.Run(Instance);
        }


        private static bool CreateAppContext<C,T>() where C: RunContext where T : Form
        {
            try
            {
                Instance = Activator.CreateInstance<C>();
                Instance.MainForm = Activator.CreateInstance<T>(); ;                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                Instance = null;
            }

            return Instance != null;
        }

        public EventHandler CfgAppCreate;

        public RunContext()
        {
            OnCreate();
        }

        public virtual void OnCreate()
        {
            CfgApp.OnCreate += CfgAppCreate;
        }
    }
}
