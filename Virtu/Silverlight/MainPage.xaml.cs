using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Jellyfish.Virtu.Services;

namespace Jellyfish.Virtu
{
    public sealed partial class MainPage : UserControl, IDisposable
    {
        public MainPage()
        {
            InitializeComponent();

            _debugService = new SilverlightDebugService(_machine, this);
            _storageService = new IsolatedStorageService(_machine);
            _keyboardService = new SilverlightKeyboardService(_machine, this);
            _gamePortService = new GamePortService(_machine); // not connected
            _audioService = new SilverlightAudioService(_machine, this, _media);
            _videoService = new SilverlightVideoService(_machine, this, _image);

            _machine.Services.AddService(typeof(DebugService), _debugService);
            _machine.Services.AddService(typeof(StorageService), _storageService);
            _machine.Services.AddService(typeof(KeyboardService), _keyboardService);
            _machine.Services.AddService(typeof(GamePortService), _gamePortService);
            _machine.Services.AddService(typeof(AudioService), _audioService);
            _machine.Services.AddService(typeof(VideoService), _videoService);

            Loaded += (sender, e) => _machine.Start();
            CompositionTarget.Rendering += OnCompositionTargetRendering;
            Application.Current.Exit += (sender, e) => _machine.Stop();

            _disk1Button.Click += (sender, e) => OnDiskButtonClick(0);
            _disk2Button.Click += (sender, e) => OnDiskButtonClick(1);
        }

        public void Dispose()
        {
            _machine.Dispose();
            _debugService.Dispose();
            _storageService.Dispose();
            _keyboardService.Dispose();
            _gamePortService.Dispose();
            _audioService.Dispose();
            _videoService.Dispose();
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            _keyboardService.Update();
            _gamePortService.Update();
            _videoService.Update();
        }

        private void OnDiskButtonClick(int drive)
        {
            var dialog = new OpenFileDialog() { Filter = "Disk Files (*.dsk;*.nib)|*.dsk;*.nib|All Files (*.*)|*.*" };

            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                using (var stream = dialog.File.OpenRead())
                {
                    _machine.Pause();
                    _machine.DiskII.Drives[drive].InsertDisk(dialog.File.Name, stream, false);
                    _machine.Unpause();
                }
            }
        }

        private Machine _machine = new Machine();

        private DebugService _debugService;
        private StorageService _storageService;
        private KeyboardService _keyboardService;
        private GamePortService _gamePortService;
        private AudioService _audioService;
        private VideoService _videoService;
    }
}
