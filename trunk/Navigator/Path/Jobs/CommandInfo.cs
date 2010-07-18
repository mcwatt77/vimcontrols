using System.IO;

namespace Navigator.Path.Jobs
{
    public class CommandInfo
    {
        public CommandInfo(string directory)
        {
            Directory = new DirectoryInfo(directory);
        }

        public CommandInfo(DirectoryInfo directoryInfo)
        {
            Directory = directoryInfo;
        }

        public DirectoryInfo Directory { get; set; }
        public string Command { get; set; }
        public string CommandName { get; set; }
    }
}