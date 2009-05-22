using System;
using System.Windows;
using System.Windows.Media;
using Jellyfish.Virtu.Services;

namespace Jellyfish.Virtu
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _storageService = new WpfStorageService();
            _keyboardService = new WpfKeyboardService(this);
            _gamePortService = new GamePortService(); // not connected
            _audioService = new AudioService(); // not connected
            _videoService = new WpfVideoService(this, _image);

            _machine = new Machine();
            _machine.Services.AddService(typeof(StorageService), _storageService);
            _machine.Services.AddService(typeof(KeyboardService), _keyboardService);
            _machine.Services.AddService(typeof(GamePortService), _gamePortService);
            _machine.Services.AddService(typeof(AudioService), _audioService);
            _machine.Services.AddService(typeof(VideoService), _videoService);

            Loaded += MainWindow_Loaded;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            Application.Current.Exit += MainApp_Exit;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _machine.Start();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            _keyboardService.Update();
            _gamePortService.Update();
            _audioService.Update();
            _videoService.Update();
        }

        private void MainApp_Exit(object sender, ExitEventArgs e)
        {
            _machine.Stop();
        }

        private StorageService _storageService;
        private KeyboardService _keyboardService;
        private GamePortService _gamePortService;
        private AudioService _audioService;
        private VideoService _videoService;

        private Machine _machine;
    }
}
