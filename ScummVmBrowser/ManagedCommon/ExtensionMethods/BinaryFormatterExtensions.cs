using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.ExtensionMethods
{
    public static class BinaryFormatterExtensions
    {
        public static T FromBinary<T>(this byte[] binary)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new MemoryStream(binary))
            {
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static byte[] ToBinary<T>(this T obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);

                return memoryStream.ToArray();
            }
        }
    }
}
