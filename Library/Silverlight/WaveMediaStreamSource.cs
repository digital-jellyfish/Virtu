using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media;

namespace Jellyfish.Library
{
    public sealed class WaveMediaStreamSourceUpdateEventArgs : EventArgs
    {
        private WaveMediaStreamSourceUpdateEventArgs()
        {
        }

        public static WaveMediaStreamSourceUpdateEventArgs Create(byte[] buffer, int bufferSize)
        {
            _instance.Buffer = buffer;
            _instance.BufferSize = bufferSize;

            return _instance;  // use singleton; avoids garbage
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] Buffer { get; private set; }
        public int BufferSize { get; private set; }

        private static readonly WaveMediaStreamSourceUpdateEventArgs _instance = new WaveMediaStreamSourceUpdateEventArgs();
    }

    public sealed class WaveMediaStreamSource : MediaStreamSource, IDisposable
    {
        public WaveMediaStreamSource(int sampleRate, int sampleChannels, int sampleBits, int sampleSize, int sampleLatency)
        {
            _bufferSize = sampleSize;
            _buffer = new byte[_bufferSize];
            _bufferStream = new MemoryStream(_buffer);
            _waveFormat = new WaveFormat(sampleRate, sampleChannels, sampleBits);
            AudioBufferLength = sampleLatency; // ms; avoids audio delay
        }

        public void Dispose()
        {
            _bufferStream.Dispose();
        }

        protected override void CloseMedia()
        {
            _audioDescription = null;
        }

        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            var handler = Update;
            if (handler != null)
            {
                handler(this, WaveMediaStreamSourceUpdateEventArgs.Create(_buffer, _bufferSize));
            }

            var sample = new MediaStreamSample(_audioDescription, _bufferStream, 0, _bufferSize, _timestamp, _emptySampleDict);
            _timestamp += _bufferSize * 10000000L / _waveFormat.AverageBytesPerSec; // 100 ns

            ReportGetSampleCompleted(sample);
        }

        protected override void OpenMediaAsync()
        {
            _timestamp = 0;

            var sourceAttributes = new Dictionary<MediaSourceAttributesKeys, string>() { { MediaSourceAttributesKeys.Duration, "0" }, { MediaSourceAttributesKeys.CanSeek, "false" } };
            var streamAttributes = new Dictionary<MediaStreamAttributeKeys, string>() { { MediaStreamAttributeKeys.CodecPrivateData, _waveFormat.ToHexString() } };
            _audioDescription = new MediaStreamDescription(MediaStreamType.Audio, streamAttributes);
            var availableStreams = new List<MediaStreamDescription>() { _audioDescription };

            ReportOpenMediaCompleted(sourceAttributes, availableStreams);
        }

        protected override void SeekAsync(long seekToTime)
        {
            ReportSeekCompleted(seekToTime);
        }

        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<WaveMediaStreamSourceUpdateEventArgs> Update;

        private byte[] _buffer;
        private int _bufferSize;
        private MemoryStream _bufferStream;
        private WaveFormat _waveFormat;
        private long _timestamp;
        private MediaStreamDescription _audioDescription;
        private Dictionary<MediaSampleAttributeKeys, string> _emptySampleDict = new Dictionary<MediaSampleAttributeKeys, string>();
    }
}
