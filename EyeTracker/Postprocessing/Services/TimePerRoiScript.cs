using Postprocessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postprocessing.Services
{
    public class TimePerRoiScript : PythonScript
    {

        public new string GetResultsPath(string sessionName)
        {
            string projectPath = GetPostprocessingProjectPath();

            return Path.Join(projectPath, "recording_statistics", sessionName + ".png");
        }

        public new string GetScriptPath()
        {
            string projectPath = GetPostprocessingProjectPath();

            return Path.Join(projectPath, "Scripts", "time_per_region.py");
        }
    }
}
