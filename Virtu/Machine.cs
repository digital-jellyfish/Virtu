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
            Keyboard = new Keyboard(this);
            GamePort = new GamePort(this);
            Cassette = new Cassette(this);
            Speaker = new Speaker(this);
            Video = new Video(this);
            NoSlotClock = new NoSlotClock(this);

            var emptySlot = new PeripheralCard(this);
            Slot1 = emptySlot;
            Slot2 = emptySlot;
            Slot3 = emptySlot;
            Slot4 = emptySlot;
            Slot5 = emptySlot;
            Slot6 = new DiskIIController(this);
            Slot7 = emptySlot;

            Slots = new Collection<PeripheralCard> { null, Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7 };
            Components = new Collection<MachineComponent> { Cpu, Memory, Keyboard, GamePort, Cassette, Speaker, Video, Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7 };

            Thread = new Thread(Run) { Name = "Machine" };
        }

        public void Dispose()
        {
            _pauseEvent.Close();
            _unpauseEvent.Close();
        }

        public void Reset()
        {
            Components.ForEach(component => component.Reset()); // while machine starting or paused
        }

        public void Start()
        {
            _storageService = Services.GetService<StorageService>();
            _storageService.Load(MachineSettings.FileName, stream => Settings.Deserialize(stream));

            State = MachineState.Starting;
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
            _unpauseEvent.Set();
            Thread.IsAliveJoin();
            State = MachineState.Stopped;

            if (_storageService != null)
            {
                _storageService.Save(MachineSettings.FileName, stream => Settings.Serialize(stream));
            }
        }

        public DiskIIController FindDiskIIController()
        {
            for (int i = 7; i >= 1; i--)
            {
                var diskII = Slots[i] as DiskIIController;
                if (diskII != null)
                {
                    return diskII;
                }
            }

            return null;
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
        public Keyboard Keyboard { get; private set; }
        public GamePort GamePort { get; private set; }
        public Cassette Cassette { get; private set; }
        public Speaker Speaker { get; private set; }
        public Video Video { get; private set; }
        public NoSlotClock NoSlotClock { get; private set; }

        public PeripheralCard Slot1 { get; private set; }
        public PeripheralCard Slot2 { get; private set; }
        public PeripheralCard Slot3 { get; private set; }
        public PeripheralCard Slot4 { get; private set; }
        public PeripheralCard Slot5 { get; private set; }
        public PeripheralCard Slot6 { get; private set; }
        public PeripheralCard Slot7 { get; private set; }

        public Collection<PeripheralCard> Slots { get; private set; }
        public Collection<MachineComponent> Components { get; private set; }

        public Thread Thread { get; private set; }

        private AutoResetEvent _pauseEvent = new AutoResetEvent(false);
        private AutoResetEvent _unpauseEvent = new AutoResetEvent(false);

        private StorageService _storageService;
    }
}
