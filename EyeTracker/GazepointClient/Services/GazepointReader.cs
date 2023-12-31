﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using GazepointClient.Model;

namespace GazepointClient.Services
{
    public class GazepointReader
    {
        public SignalConfiguration SignalConfiguration { get; set; }

        private Dictionary<string, Type> TypeMapping { get; set; }

        // Dict where input signal name is the key and the value is a list of the signal's corresponding objects
        // In it is also a special key called ROI_LABEL that represents the label at a single timestamp
        public Dictionary<string, List<object>> SignalObjectsDict { get; set; }

        public GazepointReader()
        {
            string workingDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string configPath = Path.Join(projectDirectory, "config.yaml");

            string yamlString = File.ReadAllText(configPath);
            using var strReader = new StringReader(yamlString);

            var yamlDeserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            SignalConfiguration = yamlDeserializer.Deserialize<SignalConfiguration>(strReader);

            TypeMapping = new Dictionary<string, Type>
            {
                { "int", typeof(int) },
                { "float", typeof(double) },
                { "bool", typeof(int) }  // booleans will be only 1 or 0 anyways and it's much easier to work this way
            };

            SignalObjectsDict = new Dictionary<string, List<object>>();
            foreach (string signalName in SignalConfiguration.InputSignals)
            {
                SignalObjectsDict.Add(signalName, new List<object>());
            };
            SignalObjectsDict.Add("ROI_LABEL", new List<object>());
        }

        private static string WriteSignalXMLLine(string signalName)
        {
            return $"<SET ID=\"{signalName}\" STATE=\"1\" />\r\n";
        }

        public string WriteSignalXMLSignalConfiguration()
        {
            string xml = "";
            foreach (string signalName in SignalConfiguration.InputSignals)
            {
                xml += WriteSignalXMLLine(signalName);
            }

            xml += "<SET ID=\"ENABLE_SEND_DATA\" STATE=\"1\" />\r\n";

            return xml;
        }

        private object ParseSingleOutputSignal<T>(string incomingData)
        {
            T outputSignal = (T)Activator.CreateInstance(typeof(T));

            string outputSignalName = typeof(T).Name;
            string yamlSignalName = "ENABLE_SEND_" + outputSignalName.ToUpper();

            Dictionary<string, List<string>> signal = SignalConfiguration.SignalOutputs[yamlSignalName];

            foreach (var item in signal["params"].Zip(signal["types"], (first, second) => (first, second)))
            {
                var startindex = incomingData.IndexOf($"{item.first}=\"") + $"{item.first}=\"".Length;
                var endindex = incomingData.IndexOf("\"", startindex);

                typeof(T).GetProperty(item.first).SetValue(
                    outputSignal, Convert.ChangeType(
                            incomingData.Substring(startindex, endindex - startindex), TypeMapping[item.second]
                        )
                );
            }

            return outputSignal;
        }

        private string SignalTypeFromSignalName(string signalName)
        {
            string snippedName = signalName.Split("ENABLE_SEND_")[1];

            if (snippedName.Split("_").Length > 1)
            {
                // for i.e. POG_Best and others like that
                var underscoreSplit = snippedName.Split("_");
                return underscoreSplit[0] + "_" + char.ToUpper(underscoreSplit[1][0]) + underscoreSplit[1][1..].ToLower();
            }

            return char.ToUpper(snippedName[0]) + snippedName[1..].ToLower();
        }

        // append to a dict with keys being the signal objects from the config
        // and values being lists of those objects gotten from the tracker
        public void ParseIncomingDataLine(string incomingData)
        {
            foreach (string signalName in SignalConfiguration.InputSignals)
            {
                string signalTypeName = SignalTypeFromSignalName(signalName);
                Type signalType = Type.GetType("GazepointClient.Model." + signalTypeName);

                MethodInfo method = typeof(GazepointReader).GetMethod("ParseSingleOutputSignal", BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo generic = method.MakeGenericMethod(signalType);

                object result = generic.Invoke(new GazepointReader(), new object[] { incomingData });

                SignalObjectsDict[signalName].Add(result);
            }
        }
    }
}
