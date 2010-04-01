using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Jellyfish.Virtu.Services
{
    public sealed class WpfVideoService : VideoService
    {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public WpfVideoService(Machine machine, Window window, Image image) : 
            base(machine)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            _window = window;
            _image = image;

            _image.Source = _bitmap;
            SetImageSize();

            SystemEvents.DisplaySettingsChanged += (sender, e) => SetImageSize();
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "y*560")]
        public override void SetPixel(int x, int y, uint color)
        {
            _pixels[y * BitmapWidth + x] = color;
            _pixelsDirty = true;
        }

        public override void Update() // main thread
        {
            if (_window.IsActive && (_isFullScreen != IsFullScreen))
            {
                if (IsFullScreen)
                {
                    _window.SizeToContent = SizeToContent.Manual;
                    _window.Topmost = true;
                    _window.WindowStyle = WindowStyle.None;
                    _window.WindowState = WindowState.Maximized;
                }
                else
                {
                    _window.WindowState = WindowState.Normal;
                    _window.WindowStyle = WindowStyle.SingleBorderWindow;
                    _window.Topmost = false;
                    _window.SizeToContent = SizeToContent.WidthAndHeight;
                }
                _isFullScreen = IsFullScreen;
                SetImageSize();
            }

            if (_pixelsDirty)
            {
                _pixelsDirty = false;
                _bitmap.WritePixels(BitmapRect, _pixels, BitmapStride, 0);
            }
        }

        private void SetImageSize()
        {
            int uniformScale = IsFullScreen ? Math.Min((int)SystemParameters.PrimaryScreenWidth / BitmapWidth, (int)SystemParameters.PrimaryScreenHeight / BitmapHeight) : 
                Math.Min((int)SystemParameters.FullPrimaryScreenWidth / BitmapWidth, (int)SystemParameters.FullPrimaryScreenHeight / BitmapHeight);
            _image.Width = uniformScale * BitmapWidth;
            _image.Height = uniformScale * BitmapHeight;
        }

        private const int BitmapWidth = 560;
        private const int BitmapHeight = 384;
        private const int BitmapDpi = 96;
        private static readonly PixelFormat BitmapPixelFormat = PixelFormats.Bgr32;
        private static readonly int BitmapStride = (BitmapWidth * BitmapPixelFormat.BitsPerPixel + 7) / 8;
        private static readonly Int32Rect BitmapRect = new Int32Rect(0, 0, BitmapWidth, BitmapHeight);

        private Window _window;
        private Image _image;
        private WriteableBitmap _bitmap = new WriteableBitmap(BitmapWidth, BitmapHeight, BitmapDpi, BitmapDpi, BitmapPixelFormat, null);
        private uint[] _pixels = new uint[BitmapWidth * BitmapHeight];
        private bool _pixelsDirty;
        private bool _isFullScreen;
    }
}
