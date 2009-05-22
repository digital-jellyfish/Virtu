using System.IO;

namespace Jellyfish.Library
{
    public static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            int count = (int)stream.Length;
            byte[] buffer = new byte[count];
            ReadBlock(stream, buffer, 0, count);

            return buffer;
        }

        public static int ReadBlock(this Stream stream, byte[] buffer, int offset, int count)
        {
            int total = 0;
            int read;
            do
            {
                total += read = stream.Read(buffer, offset + total, count - total);
            }
            while ((read > 0) && (total < count));

            return total;
        }
    }
}
