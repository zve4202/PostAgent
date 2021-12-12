using System;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Windows.Forms;

namespace GH.Context
{
    public class NamedPipeManager
    {
        public static string NamedPipeName { get; } = "Pipe_" + Path.GetFileNameWithoutExtension(Application.ExecutablePath);
        public event Action<string> ReceiveString;

        private const string EXIT_STRING = "__EXIT__";
        public const string ACTIVE_STRING = "__ACTIVE__";

        private BackgroundWorker worker;

        public void Start()
        {
            worker = new BackgroundWorker(); 
            worker.WorkerReportsProgress = true;
            worker.DoWork += BackgroundWorker_DoWork;
            worker.ProgressChanged += BackgroundWorker_ProgressChanged;
            worker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            worker.RunWorkerAsync(); 
        }

        public void Stop()
        {
            Write(EXIT_STRING);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.Dispose();
            worker = null;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ReceiveString == null)
                return;

            RunContext.Invoke(() =>
            {
                ReceiveString.Invoke(e.UserState as string);
            });
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                string result;
                using (var server = new NamedPipeServerStream(NamedPipeName))
                {
                    server.WaitForConnection();

                    using (StreamReader reader = new StreamReader(server))
                        result = reader.ReadToEnd();
                }

                if (result == EXIT_STRING)
                    break;

                worker.ReportProgress(0, result);
            }
        }

        public static bool Write(string[] text, int connectTimeout = 300)
        {
            using (var client = new NamedPipeClientStream(NamedPipeName))
            {
                try
                {
                    client.Connect(connectTimeout);
                }
                catch
                {
                    return false;
                }

                if (!client.IsConnected)
                    return false;

                using (StreamWriter writer = new StreamWriter(client))
                {
                    foreach (var a in text)
                        writer.Write(a + '\n');
                    writer.Flush();
                }
            }
            return true;
        }

        public static bool Write(string text, int connectTimeout = 300)
        {
            using (var client = new NamedPipeClientStream(NamedPipeName))
            {
                try
                {
                    client.Connect(connectTimeout);
                }
                catch
                {
                    return false;
                }

                if (!client.IsConnected)
                    return false;

                using (StreamWriter writer = new StreamWriter(client))
                {
                    writer.Write(text);
                    writer.Flush();
                }
            }
            return true;
        }
    }
}
