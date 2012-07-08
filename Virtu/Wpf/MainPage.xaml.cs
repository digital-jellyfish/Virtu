using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Jellyfish.Virtu.Services;
using Microsoft.Win32;

namespace Jellyfish.Virtu
{
    public sealed partial class MainPage : UserControl, IDisposable
    {
        public MainPage()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _debugService = DebugService.Default;
                _storageService = new WpfStorageService(_machine);
                _keyboardService = new WpfKeyboardService(_machine, this);
                _gamePortService = new GamePortService(_machine); // not connected
                _audioService = new WpfAudioService(_machine, this);
                _videoService = new WpfVideoService(_machine, this, _image);

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

        public void WriteMessage(string message)
        {
            _debugText.Text += message + Environment.NewLine;
            _debugScrollViewer.UpdateLayout();
            _debugScrollViewer.ScrollToVerticalOffset(double.MaxValue);
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
                _machine.Pause();
                StorageService.LoadFile(dialog.FileName, stream => _machine.BootDiskII.Drives[drive].InsertDisk(dialog.FileName, stream, false));
                _machine.Unpause();
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
