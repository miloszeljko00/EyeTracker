using GazepointClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient.Services
{
    public class CSVExporter : IDataExporter
    {
        
        private static string MakeCSVHeader(Dictionary<string, List<object>> SignalObjectsDict)
        {
            string header = String.Empty;
            
            foreach(string signalName in SignalObjectsDict.Keys)
            {
                object signalObject = SignalObjectsDict[signalName].First();

                foreach(PropertyInfo propertyInfo in signalObject.GetType().GetProperties())
                {
                    header += String.Format("{0},", propertyInfo.Name);
                }
            }

            return header[..^1] + "\n";
        }

        private static string MakeCSVData(Dictionary<string, List<object>> SignalObjectsDict)
        {
            string data = String.Empty;

            int numberOfRows = SignalObjectsDict[SignalObjectsDict.Keys.First()].Count;

            for(int row=0; row < numberOfRows; row++)
            {
                string rowData = String.Empty;

                foreach(string signalName in SignalObjectsDict.Keys)
                {
                    object signalObjectAtRow = SignalObjectsDict[signalName][row];

                    foreach(PropertyInfo propertyInfo in signalObjectAtRow.GetType().GetProperties())
                    {
                        rowData += String.Format("{0},", propertyInfo.GetValue(signalObjectAtRow));
                    }
                }

                data += rowData[..^1] + "\n";
            }

            return data[..^1];
        }

        public void ExportData(Dictionary<string, List<object>> SignalObjectsDict, string savePath)
        {
            string header = MakeCSVHeader(SignalObjectsDict);
            string data = MakeCSVData(SignalObjectsDict);

            string csv = header + data;

            File.WriteAllText(savePath, csv);
        }
    }
}
