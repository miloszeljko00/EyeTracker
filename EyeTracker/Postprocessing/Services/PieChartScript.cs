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
    public class PieChartScript : PythonScript
    {
        public new string GetScriptPath()
        {
            string projectPath = GetPostprocessingProjectPath();

            return Path.Join(projectPath, "Scripts", "region_piechart.py");
        }
    }
}
