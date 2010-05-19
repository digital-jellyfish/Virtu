using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Jellyfish.Virtu.Services;
using Jellyfish.Virtu.Settings;
using Microsoft.Win32;

namespace Jellyfish.Virtu
{
    public sealed partial class MainWindow : Window, IDisposable
    {
        public MainWindow()
        {
            InitializeComponent();

            _storageService = new WpfStorageService(_machine);
            _keyboardService = new WpfKeyboardService(_machine, this);
            _gamePortService = new GamePortService(_machine); // not connected
            _audioService = new WpfAudioService(_machine, this);
            _videoService = new WpfVideoService(_machine, this, _image);

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
                using (var stream = File.OpenRead(dialog.FileName))
                {
                    _machine.Pause();
                    _machine.DiskII.Drives[drive].InsertDisk(dialog.FileName, stream, false);
                    var settings = _machine.Settings.DiskII;
                    if (drive == 0)
                    {
                        settings.Disk1.Name = dialog.FileName;
                    }
                    else
                    {
                        settings.Disk2.Name = dialog.FileName;
                    }
                    _machine.Unpause();
                }
            }
        }

        private Machine _machine = new Machine();

        private StorageService _storageService;
        private KeyboardService _keyboardService;
        private GamePortService _gamePortService;
        private AudioService _audioService;
        private VideoService _videoService;
    }
}
