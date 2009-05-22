using System.IO;

namespace Jellyfish.Library
{
    public static class FileHelpers
    {
        public static byte[] ReadAllBytes(string path)
        {
#if SILVERLIGHT || XBOX || ZUNE
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return stream.ReadAllBytes();
            }
#else
            return File.ReadAllBytes(path);
#endif
        }
    }
}
