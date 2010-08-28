using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace Jellyfish.Library
{
    public sealed class WaveMediaStreamSource : MediaStreamSource, IDisposable
    {
        public WaveMediaStreamSource(int sampleRate, int sampleChannels, int sampleBits, int sampleSize, int sampleLatency, Action<byte[], int> updater)
        {
            _bufferSize = sampleSize;
            _buffer = new byte[_bufferSize];
            _bufferStream = new MemoryStream(_buffer);
            _waveFormat = new WaveFormat(sampleRate, sampleChannels, sampleBits);
            AudioBufferLength = sampleLatency; // ms; avoids audio delay
            _updater = updater;
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
            _updater(_buffer, _bufferSize);

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

        private byte[] _buffer;
        private int _bufferSize;
        private MemoryStream _bufferStream;
        private Action<byte[], int> _updater;
        private WaveFormat _waveFormat;
        private long _timestamp;
        private MediaStreamDescription _audioDescription;
        private Dictionary<MediaSampleAttributeKeys, string> _emptySampleDict = new Dictionary<MediaSampleAttributeKeys, string>();
    }
}
