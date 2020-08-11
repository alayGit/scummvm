using ManagedCommon.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace ConfigStore
{
    public class JsonConfigStore: IConfigurationStore<System.Enum> 
    {
        Dictionary<string, Dictionary<Enum, string>> _cache;
        public JsonConfigStore()
        {
            _cache = new Dictionary<string, Dictionary<Enum, string>>();
        }

        public V GetValue<V>(Enum key) where V : IConvertible
        {
            Dictionary<Enum, string> configValues;
            string store = key.GetType().Name;

            _cache.TryGetValue(store, out configValues);

            if(configValues == null)
            {
                configValues = new Dictionary<Enum, string>();

                IEnumerable<KeyValuePair<string, string>> values = JsonConvert
                    .DeserializeObject<Dictionary<string, string>>(GetConfigJson(store));

                foreach(KeyValuePair<string, string> kvp in values)
                {
                    configValues[(Enum)System.Enum.Parse(key.GetType(), kvp.Key)] = kvp.Value;
                }

                _cache[store] = configValues;

            }

            if(!configValues.ContainsKey(key))
            {
                throw new Exception($"There is no key '{key}' in store '{store}'");
            }

            return (V) Convert.ChangeType(configValues[key], typeof(V));
        }

        public string GetValue(Enum key)
        {
            return GetValue<string>(key);
        }

        private string GetConfigJson(string store)
        {
            string result = File.ReadAllText(Path.Combine(ConfigurationManager.AppSettings.Get(Constants.ConfigFilesLocationConfigKey), $"{store}.json"));

            if(result == null)
            {
                throw new Exception($"Attempting to access a non existant resource store {store}");
            }

            return result;
        }
    }
}
