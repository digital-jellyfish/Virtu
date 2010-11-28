using System;
using Jellyfish.Library;
using Microsoft.Xna.Framework.Audio;

namespace Jellyfish.Virtu.Services
{
    public sealed class XnaAudioService : AudioService
    {
        public XnaAudioService(Machine machine, GameBase game) : 
            base(machine)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            _game = game;

            _dynamicSoundEffect.BufferNeeded += OnDynamicSoundEffectBufferNeeded;
            _game.Exiting += (sender, e) => _dynamicSoundEffect.Stop();

            _dynamicSoundEffect.SubmitBuffer(SampleZero);
            _dynamicSoundEffect.Play();
        }

        public override void SetVolume(double volume)
        {
            _dynamicSoundEffect.Volume = (float)volume;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dynamicSoundEffect.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnDynamicSoundEffectBufferNeeded(object sender, EventArgs e) // audio thread
        {
            //if (_count++ % (1000 / SampleLatency) == 0)
            //{
            //    DebugService.WriteLine("OnDynamicSoundEffectBufferNeeded");
            //}

            _dynamicSoundEffect.SubmitBuffer(Source, 0, SampleSize);
            Update();
        }

        private GameBase _game;
        private DynamicSoundEffectInstance _dynamicSoundEffect = new DynamicSoundEffectInstance(SampleRate, (AudioChannels)SampleChannels);
        //private int _count;
    }
}
