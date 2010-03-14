using System;
using System.Collections.ObjectModel;
using System.Threading;
using Jellyfish.Library;
using Jellyfish.Virtu.Services;
using Jellyfish.Virtu.Settings;

namespace Jellyfish.Virtu
{
    public enum MachineState { Stopped = 0, Starting, Running, Pausing, Paused, Stopping }

    public sealed class Machine : IDisposable
    {
        public Machine()
        {
            Events = new MachineEvents();
            Services = new MachineServices();
            Settings = new MachineSettings();

            Cpu = new Cpu(this);
            Memory = new Memory(this);
            DiskII = new DiskII(this);
            Keyboard = new Keyboard(this);
            GamePort = new GamePort(this);
            Cassette = new Cassette(this);
            Speaker = new Speaker(this);
            Video = new Video(this);
            Components = new Collection<MachineComponent> { Cpu, Memory, DiskII, Keyboard, GamePort, Cassette, Speaker, Video };

            Thread = new Thread(Run) { Name = "Machine" };
        }

        public void Dispose()
        {
            _pauseEvent.Close();
            _unpauseEvent.Close();
        }

        public void Reset()
        {
            Components.ForEach(component => component.Reset());
        }

        public void Start()
        {
            _storageService = Services.GetService<StorageService>();
            _storageService.Load(MachineSettings.FileName, stream => Settings.Deserialize(stream));

            State = MachineState.Starting;
            Services.ForEach(service => service.Start());

            Thread.Start();
        }

        public void Pause()
        {
            State = MachineState.Pausing;
            _pauseEvent.WaitOne();
            State = MachineState.Paused;
        }

        public void Unpause()
        {
            State = MachineState.Running;
            _unpauseEvent.Set();
        }

        public void Stop()
        {
            State = MachineState.Stopping;
            Services.ForEach(service => service.Stop());

            _pauseEvent.Set();
            _unpauseEvent.Set();
            Thread.Join();
            State = MachineState.Stopped;

            _storageService.Save(MachineSettings.FileName, stream => Settings.Serialize(stream));
        }

        private void Run() // machine thread
        {
            Components.ForEach(component => component.Initialize());
            Reset();

            State = MachineState.Running;
            do
            {
                do
                {
                    Events.HandleEvents(Cpu.Execute());
                }
                while (State == MachineState.Running);

                if (State == MachineState.Pausing)
                {
                    _pauseEvent.Set();
                    _unpauseEvent.WaitOne();
                }
            }
            while (State != MachineState.Stopping);

            Components.ForEach(component => component.Uninitialize());
        }

        public MachineEvents Events { get; private set; }
        public MachineServices Services { get; private set; }
        public MachineSettings Settings { get; private set; }
        public MachineState State { get; private set; }

        public Cpu Cpu { get; private set; }
        public Memory Memory { get; private set; }
        public DiskII DiskII { get; private set; }
        public Keyboard Keyboard { get; private set; }
        public GamePort GamePort { get; private set; }
        public Cassette Cassette { get; private set; }
        public Speaker Speaker { get; private set; }
        public Video Video { get; private set; }
        public Collection<MachineComponent> Components { get; private set; }

        public Thread Thread { get; private set; }

        private AutoResetEvent _pauseEvent = new AutoResetEvent(false);
        private AutoResetEvent _unpauseEvent = new AutoResetEvent(false);

        private StorageService _storageService;
    }
}
