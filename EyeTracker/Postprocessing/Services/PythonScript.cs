using Postprocessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postprocessing.Services
{
    public class PythonScript
    {
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
