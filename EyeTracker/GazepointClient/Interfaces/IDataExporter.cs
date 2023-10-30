using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient.Interfaces
{
    public interface IDataExporter
    {
        void ExportData(Dictionary<string, List<object>> SignalObjectsDict, string savePath);
    }
}
