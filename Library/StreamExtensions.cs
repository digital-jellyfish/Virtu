using System;
using System.IO;

namespace Jellyfish.Library
{
    public static class StreamExtensions
    {
        public static byte[] ReadBlock(this Stream stream, int count)
        {
            return ReadBlock(stream, new byte[count], 0, count);
        }

        public static byte[] ReadBlock(this Stream stream, byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            return ReadBlock(stream, buffer, 0, buffer.Length);
        }

        public static byte[] ReadBlock(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            int total = 0;
            int read;
            do
            {
                total += read = stream.Read(buffer, offset + total, count - total);
            }
            while ((read > 0) && (total < count));

            if (total < count)
            {
                throw new EndOfStreamException();
            }

            return buffer;
        }

        public static void SkipBlock(this Stream stream, int count)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (stream.CanSeek)
            {
                stream.Seek(count, SeekOrigin.Current);
            }
            else
            {
                const int BufferSize = 1024;
                byte[] buffer = new byte[BufferSize];

                int total = 0;
                int read;
                do
                {
                    total += read = stream.Read(buffer, 0, Math.Min(count - total, BufferSize));
                }
                while ((read > 0) && (total < count));
            }
        }
    }
}
