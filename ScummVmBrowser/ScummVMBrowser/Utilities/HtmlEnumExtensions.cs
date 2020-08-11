using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ScummVMBrowser.Utilities
{
    public static class EnumExtensions
    {
        public static String EnumToString<T>() where T : struct, IConvertible
        {
            var values = Enum.GetValues(typeof(T)).Cast<int>();
            var enumDictionary = values.ToDictionary(value => Enum.GetName(typeof(T), value));

            return JsonConvert.SerializeObject(enumDictionary);
        }
    }
}