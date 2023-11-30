using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postprocessing.Interfaces
{
    public interface IPythonScript
    {
        string GetScriptPath(string scriptPath);

        void CallScript(string scriptName, string csv_path, string sessionName);

        string GetResultsPath(string sessionName);
    }
}
