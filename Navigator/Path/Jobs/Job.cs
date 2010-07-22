using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using Navigator.Containers;
using Navigator.UI.Attributes;
using VIControls.Commands.Interfaces;

namespace Navigator.Path.Jobs
{
    public abstract class Job : ISummaryString, IDescriptionString, INavigable
    {
        private readonly JobProgress _jobProgress;
        private Action<string> _updateSummary;

        public abstract string Summary { get; }

        public abstract void Execute();

        protected Job(IContainer container)
        {
            _jobProgress = container.Get<JobProgress>();
        }

        protected void UpdateSummary(string summary)
        {
            if (_updateSummary == null)
                _updateSummary = _jobProgress.GetPropertyUpdater<ISummaryString, string>(s => s.Summary, Summary);

            _updateSummary(summary);
        }

        protected void WriteToLog(string name, string message)
        {
            var fileName = GetType().FullName + "." + name + "." + DateTime.Now.ToString("MM.dd.yyyy.HH.mm.ss.fffffff") + ".txt";
            var streamWriter = new StreamWriter(fileName);
            streamWriter.Write(message);
            streamWriter.Flush();
            streamWriter.Close();

            var processStartInfo = new ProcessStartInfo {FileName = fileName, Verb = "open"};
            Process.Start(processStartInfo);
        }

        public string Description
        {
            get { return "This is a job!"; }
        }

        public void Navigate()
        {
            try
            {
                var threadStart = new ThreadStart(Execute);
                var thread = new Thread(threadStart);
                thread.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void NavigateToCurrentChild()
        {
            throw new NotImplementedException();
        }
    }
}