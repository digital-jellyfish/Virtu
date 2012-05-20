using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Jellyfish.Library
{
    public static class StreamExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static int ReadBlock(this Stream stream, byte[] buffer, int offset = 0, int minCount = int.MaxValue)
        {
            return ReadBlock(stream, buffer, offset, int.MaxValue, minCount);
        }

        public static int ReadBlock(this Stream stream, byte[] buffer, int offset, int count, int minCount)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            count = Math.Min(count, buffer.Length - offset);
            minCount = Math.Min(minCount, buffer.Length - offset);

            int total = 0;
            int read;
            do
            {
                total += read = stream.Read(buffer, offset + total, count - total);
            }
            while ((read > 0) && (total < count));

            if (total < minCount)
            {
                throw new EndOfStreamException();
            }

            return total;
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
                var buffer = new byte[BufferSize];
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
