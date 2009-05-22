using System.Collections.ObjectModel;
using System.Threading;
using Jellyfish.Library;
using Jellyfish.Virtu.Services;
using Jellyfish.Virtu.Settings;

namespace Jellyfish.Virtu
{
    public class Machine
    {
        public Machine()
        {
            Thread = new Thread(Run) { Name = "Machine" };
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
        }

        public void Reset()
        {
            Components.ForEach(component => component.Reset());
        }

        public void Start()
        {
            _storageService = Services.GetService<StorageService>();
            _storageService.Load(MachineSettings.FileName, stream => Settings.Deserialize(stream));
            Thread.Start();
        }

        public void Stop()
        {
            _stopPending = true;
            Thread.Join();
            _storageService.Save(MachineSettings.FileName, stream => Settings.Serialize(stream));
        }

        private void Run()
        {
            Components.ForEach(component => component.Initialize());
            Reset();

            do
            {
                Events.RaiseEvents(Cpu.Execute());
            }
            while (!_stopPending);

            Components.ForEach(component => component.Uninitialize());
        }

        public Thread Thread { get; private set; }
        public MachineEvents Events { get; private set; }
        public MachineServices Services { get; private set; }
        public MachineSettings Settings { get; private set; }
        public Collection<MachineComponent> Components { get; private set; }
        public Cpu Cpu { get; private set; }
        public Memory Memory { get; private set; }
        public DiskII DiskII { get; private set; }
        public Keyboard Keyboard { get; private set; }
        public GamePort GamePort { get; private set; }
        public Cassette Cassette { get; private set; }
        public Speaker Speaker { get; private set; }
        public Video Video { get; private set; }

        private StorageService _storageService;
        private bool _stopPending;
    }
}
