using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Jellyfish.Library;
using Jellyfish.Virtu.Services;

namespace Jellyfish.Virtu
{
    public enum MachineState { Stopped = 0, Starting, Running, Pausing, Paused, Stopping }

    public sealed class Machine : IDisposable
    {
        public Machine()
        {
            Events = new MachineEvents();
            Services = new MachineServices();

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
            Components = new Collection<MachineComponent> { Cpu, Memory, Keyboard, GamePort, Cassette, Speaker, Video, NoSlotClock, Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7 };

            Thread = new Thread(Run) { Name = "Machine" };
        }

        public void Dispose()
        {
            _pauseEvent.Close();
            _unpauseEvent.Close();
        }

        public void Reset()
        {
            foreach (var component in Components)
            {
                //_debugService.WriteLine("Resetting component '{0}'", component.GetType().Name);
                component.Reset();
                //_debugService.WriteLine("Reset component '{0}'", component.GetType().Name);
            }
        }

        public void Start()
        {
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
            if (Thread.IsAlive)
            {
                Thread.Join();
            }
            State = MachineState.Stopped;
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

        private void Initialize()
        {
            foreach (var component in Components)
            {
                //_debugService.WriteLine("Initializing component '{0}'", component.GetType().Name);
                component.Initialize();
                //_debugService.WriteLine("Initialized component '{0}'", component.GetType().Name);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void LoadState()
        {
            _storageService.Load(Machine.LastStateFileName, stream =>
            {
                try
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        string stateSignature = reader.ReadString();
                        var stateVersion = new Version(reader.ReadString());
                        if ((stateSignature == StateSignature) && (stateVersion == new Version(Machine.Version))) // avoid state version mismatch (for now)
                        {
                            foreach (var component in Components)
                            {
                                //_debugService.WriteLine("Loading component '{0}' state", component.GetType().Name);
                                component.LoadState(reader, stateVersion);
                                //_debugService.WriteLine("Loaded component '{0}' state", component.GetType().Name);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                }
            });
        }

        private void Uninitialize()
        {
            foreach (var component in Components)
            {
                //_debugService.WriteLine("Uninitializing component '{0}'", component.GetType().Name);
                component.Uninitialize();
                //_debugService.WriteLine("Uninitialized component '{0}'", component.GetType().Name);
            }
        }

        private void SaveState()
        {
            _storageService.Save(Machine.LastStateFileName, stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(StateSignature);
                    writer.Write(Machine.Version);
                    foreach (var component in Components)
                    {
                        //_debugService.WriteLine("Saving component '{0}' state", component.GetType().Name);
                        component.SaveState(writer);
                        //_debugService.WriteLine("Saved component '{0}' state", component.GetType().Name);
                    }
                }
            });
        }

        private void Run() // machine thread
        {
            _debugService = Services.GetService<DebugService>();
            _storageService = Services.GetService<StorageService>();

            Initialize();
            Reset();
            LoadState();

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

            SaveState();
            Uninitialize();
        }

        public const string Version = "0.9.1.0";

        public MachineEvents Events { get; private set; }
        public MachineServices Services { get; private set; }
        public MachineState State { get { return _state; } private set { _state = value; } }

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

        private const string LastStateFileName = "LastState.bin";
        private const string StateSignature = "Virtu";

        private DebugService _debugService;
        private StorageService _storageService;
        private volatile MachineState _state;

        private AutoResetEvent _pauseEvent = new AutoResetEvent(false);
        private AutoResetEvent _unpauseEvent = new AutoResetEvent(false);
    }
}
