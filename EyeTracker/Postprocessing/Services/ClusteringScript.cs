using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postprocessing.Services
{
    public class ClusteringScript: PythonScript
    {
        public new string GetScriptPath()
        {
            string projectPath = GetPostprocessingProjectPath();

            return Path.Join(projectPath, "Scripts", "coordinate_clustering.py");
        }
    }
}
