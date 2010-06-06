using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Jellyfish.Library
{
    public static class XmlSerializerHelpers
    {
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static T Deserialize<T>(Stream stream, string defaultNamespace = null)
        {
            using (var reader = XmlReader.Create(stream))
            {
                var serializer = new XmlSerializer(typeof(T), defaultNamespace);
                return (T)serializer.Deserialize(reader);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static void Serialize<T>(Stream stream, T instance, string defaultNamespace = null)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                var serializer = new XmlSerializer(typeof(T), defaultNamespace);
                serializer.Serialize(writer, instance);
            }
        }
    }
}
