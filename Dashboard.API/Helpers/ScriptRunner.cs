using System;
using System.Diagnostics;
using System.IO;

namespace Dashboard.API.Helpers
{
    public static class ScriptRunner
    {
        /// <summary>
		/// Runs a process file.
		/// </summary>
		/// <param name="filePath">Full file path to process to run</param>
		/// <param name="arguments">Arguments to pass to the process (eg: "/a /b")</param>
		public static void RunProcess(string filePath, string arguments = "")
        {
            // Check inputs
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("ScriptRunner.Runprocess: Empty filepath.");
            }

            System.Diagnostics.Process p = new System.Diagnostics.Process();

            // Set file location and working directory.
            try
            {
                p.StartInfo.FileName = Path.GetFullPath(filePath);
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(p.StartInfo.FileName);
            }
            catch (Exception e)
            {
                throw new Exception("ScriptRunner.Runprocess: Invalid filepath.\r\nDetails:\r\n" + e.Message);
            }

            // If there are aruments, pass them to the process
            if (!string.IsNullOrWhiteSpace(arguments))
            {
                try
                {
                    p.StartInfo.Arguments = arguments;
                }
                catch (Exception e)
                {
                    throw new Exception("ScriptRunner.Runprocess: Failed to set arguments.\r\nDetails:\r\n" + e.Message);
                }
            }

            // Not sure
            p.StartInfo.UseShellExecute = false;

            // Execute
            p.Start();
        }

        /// <summary>
        /// Runs a command in a Command Prompt
        /// </summary>
        /// <param name="command">The command to run, with argumets</param>
        /// <returns></returns>
        public static string RunCommand(string command)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.Arguments = "/C " + command;

            startInfo.FileName = "cmd.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            process.StartInfo = startInfo;

            process.Start();

            StreamReader reader = process.StandardOutput;
            string output = reader.ReadToEnd();

            process.WaitForExit();

            return output;
        }

		/// <summary>
        /// Starts a command in a Command Prompt, doesn't wait for exit
        /// </summary>
        /// <param name="command">The command to run, with argumets</param>
        /// <returns></returns>
        public static void StartCommand(string command)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.Arguments = "/C " + command;

            startInfo.FileName = "cmd.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            // startInfo.RedirectStandardOutput = true;
            process.StartInfo = startInfo;

            process.Start();

            return;
        }

        /// <summary>
        /// Runs multiple commands (separated with '&') in a single Command Prompt
        /// </summary>
        /// <param name="commandArray">The commands to run, with arguments</param>
        /// <returns></returns>
        public static string RunCommand(string[] commandArray)
        {
            string command = "";
            for (int i = 0; i < commandArray.Length; i++)
            {
                command += commandArray[i];
                if (i != commandArray.Length - 1)
                {
                    command += '&';
                }
            }

            return RunCommand(command);
        }
    }
}