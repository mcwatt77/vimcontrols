using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Navigator.Containers;

namespace Navigator.Path.Jobs
{
    public class RunCmd : Job
    {
        private readonly CommandInfo _info;

        public RunCmd(CommandInfo info, IContainer container) : base(container)
        {
            _info = info;
        }

        public override string Summary
        {
            get { return "Run Command: " + _info.CommandName; }
        }

        public override void Execute()
        {
            var threadStart = new ThreadStart(RunJob);
            var thread = new Thread(threadStart);

            var startTime = DateTime.Now;

            thread.Start();
            while (thread.IsAlive)
            {
                UpdateSummary("Running: " + _info.CommandName + " (" + (int)DateTime.Now.Subtract(startTime).TotalSeconds + ")");
                Thread.Sleep(1000);
            }
            UpdateSummary("Run Command: " + _info.CommandName + " (Complete)");
        }

        private void RunJob()
        {
            var commandParts = _info.Command.Split(' ');
            var command = commandParts.First();
            var arguments = commandParts.Skip(1).Aggregate("", (a, s) => a + " " + s);

            if (command.ToLower().IndexOf(".bat") > 0)
            {
                arguments = " /c " + command + arguments;
                command = "cmd";
            }

            var processStartInfo = new ProcessStartInfo(command)
                                       {
                                           WorkingDirectory = _info.Directory.FullName,
                                           UseShellExecute = false,
                                           ErrorDialog = false,
                                           CreateNoWindow = true,
                                           Arguments = arguments,
                                           RedirectStandardOutput = true
                                       };
            var process = Process.Start(processStartInfo);
            WriteToLog(_info.CommandName.Replace(" ", "_"), process.StandardOutput.ReadToEnd());
        }
    }
}