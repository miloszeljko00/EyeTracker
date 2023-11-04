using Postprocessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postprocessing.Services
{
    public class PythonScript: IPythonScript
    {
        public void CallScript(string csv_path, string sessionName)
        {
            var script = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"{GetScriptPath()} --csv_path={csv_path} --save_path={GetRecordingStatisticsDirectoryPath()} --session_name={sessionName}",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                }
            };

            script.Start();
            script.WaitForExit();
        }

        public string GetResultsPath(string sessionName)
        {
            throw new NotImplementedException();
        }

        public string GetScriptPath()
        {
            throw new NotImplementedException();
        }

        protected string GetPostprocessingProjectPath()
        {
            string workingDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            return Path.Join(projectDirectory, "..", "PostProcessing");
        }

        protected string GetRecordingStatisticsDirectoryPath()
        {
            return Path.Join(GetPostprocessingProjectPath(), "recording_statistics");
        }
    }
}
