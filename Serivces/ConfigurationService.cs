using Microsoft.Extensions.Configuration;
using NetifePanel.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.Storage;

namespace NetifePanel.Serivces
{
    public class ConfigurationService : IConfigurationService
    {
        public IConfiguration Configuration { get; private set; } 

        public ConfigurationService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void UpdateCommonValue(string sectionPathKey, string value)
        {
            var path = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "Config"), "Settings.json");
            var json = File.ReadAllText(path);
            JsonNode jNode = JsonNode.Parse(json);
            string[] sections = sectionPathKey.Split(':');
            SetNestedValue(jNode, sections, value);
            string output = jNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, output);
        }

        private void SetNestedValue(JsonNode node, string[] sections, string value, int index = 0)
        {
            string key = sections[index];
            if (index == sections.Length - 1)
            {
                if (node is JsonObject jObj)
                {
                    jObj[key] = value;
                }
            }
            else
            {
                if (node[key] is JsonNode childNode)
                {
                    SetNestedValue(childNode, sections, value, index + 1);
                }
            }
        }
    }
}
