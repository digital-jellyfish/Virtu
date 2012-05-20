using System;
using System.ComponentModel;
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

            if (!DesignerProperties.IsInDesignTool)
            {
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

        public void WriteLine(string message)
        {
            _debugText.Text += message;
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
                StorageService.LoadFile(dialog.File, stream => _machine.BootDiskII.Drives[drive].InsertDisk(dialog.File.Name, stream, false));
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
