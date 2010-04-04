using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Jellyfish.Library
{
    public static class XmlSerializerHelpers
    {
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T Deserialize<T>(Stream stream)
        {
            return Deserialize<T>(stream, null);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T Deserialize<T>(Stream stream, string defaultNamespace)
        {
            using (var reader = XmlReader.Create(stream))
            {
                var serializer = new XmlSerializer(typeof(T), defaultNamespace);
                return (T)serializer.Deserialize(reader);
            }
        }

        public static void Serialize<T>(Stream stream, T instance)
        {
            Serialize<T>(stream, instance, null);
        }

        public static void Serialize<T>(Stream stream, T instance, string defaultNamespace)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                var serializer = new XmlSerializer(typeof(T), defaultNamespace);
                serializer.Serialize(writer, instance);
            }
        }
    }
}
