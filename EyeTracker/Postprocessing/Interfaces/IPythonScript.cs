using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postprocessing.Interfaces
{
    public interface IPythonScript
    {
        string GetScriptPath();

        void CallScript(string csv_path, string sessionName);

        string GetResultsPath(string sessionName);
    }
}
