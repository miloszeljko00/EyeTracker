using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace GazepointClient.Model
{
    public class SignalConfiguration
    {
        public List<string> InputSignals { get; set; } = new List<string>();
        public Dictionary<string, Dictionary<string, List<string>>> SignalOutputs { get; set; } = new Dictionary<string, Dictionary<string, List<string>>>();
    }
}
