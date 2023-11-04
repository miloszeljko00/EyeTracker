using Postprocessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Postprocessing.Services
{
    public class PieChartScript : PythonScript, IPythonScript
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
            string projectPath = GetPostprocessingProjectPath();

            return Path.Join(projectPath, "recording_statistics", sessionName + ".png");
        }

        public string GetScriptPath()
        {
            string projectPath = GetPostprocessingProjectPath();

            return Path.Join(projectPath, "Scripts", "region_piechart.py");
        }
    }
}
