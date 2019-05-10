
using System;
using System.IO;
using System.Reflection;
using EasyWords.Client.Models;
using Newtonsoft.Json;

namespace EasyWords.Client
{
    public class ConfigurationProvider
    {

        public ConfigurationProvider()
        {
        }

        public static Configuration ResolveConfiguration()
        {
            var stream = typeof(ConfigurationProvider).GetTypeInfo().Assembly.GetManifestResourceStream("EasyWords.Client.appsettings.json");
            using (StreamReader reader = new StreamReader(stream))
            {
                var configurationObject = JsonConvert.DeserializeObject<Configuration>(reader.ReadToEnd());

                return configurationObject;

            }
        }
    }
}
