using System.Runtime.InteropServices;
using System.Text;

namespace Jellyfish.Library
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class WaveFormat
    {
        public WaveFormat(int sampleRate, int sampleChannels, int sampleBits)
        {
            _formatTag = WaveFormatPcm;
            _samplesPerSec = sampleRate;
            _channels = (short)sampleChannels;
            _bitsPerSample = (short)sampleBits;
            _blockAlign = (short)(sampleChannels * sampleBits / 8);
            _averageBytesPerSec = sampleRate * _blockAlign;
        }

        public string ToHexString() // little endian
        {
            var builder = new StringBuilder();

            builder.AppendHex(_formatTag);
            builder.AppendHex(_channels);
            builder.AppendHex(_samplesPerSec);
            builder.AppendHex(_averageBytesPerSec);
            builder.AppendHex(_blockAlign);
            builder.AppendHex(_bitsPerSample);
            builder.AppendHex(_size);

            return builder.ToString();
        }

        public int SamplesPerSec { get { return _samplesPerSec; } } // no auto props
        public int Channels { get { return _channels; } }
        public int BitsPerSample { get { return _bitsPerSample; } }
        public int AverageBytesPerSec { get { return _averageBytesPerSec; } }

        private const int WaveFormatPcm = 1;

        private short _formatTag;
        private short _channels;
        private int _samplesPerSec;
        private int _averageBytesPerSec;
        private short _blockAlign;
        private short _bitsPerSample;
        private short _size;
    }
}
