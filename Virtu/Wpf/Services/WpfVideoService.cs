using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Jellyfish.Virtu.Services
{
    public sealed class WpfVideoService : VideoService
    {
        public WpfVideoService(Machine machine, UserControl page, Image image) : 
            base(machine)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            _page = page;
            _image = image;
            _image.Source = _bitmap;

            _page.Loaded += (sender, e) => SetWindowSizeToContent();
            _page.SizeChanged += (sender, e) => SetImageSize();
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow")]
        public override void SetPixel(int x, int y, uint color)
        {
            _pixels[y * BitmapWidth + x] = color;
            _pixelsDirty = true;
        }

        public override void Update() // main thread
        {
            if (_isFullScreen != IsFullScreen)
            {
                var window = Window.GetWindow(_page);
                if (IsFullScreen)
                {
                    window.ResizeMode = ResizeMode.NoResize;
                    window.WindowStyle = WindowStyle.None;
                    window.WindowState = WindowState.Maximized;
                }
                else
                {
                    window.WindowState = WindowState.Normal;
                    window.WindowStyle = WindowStyle.SingleBorderWindow;
                    window.ResizeMode = ResizeMode.CanResize;
                }
                _isFullScreen = IsFullScreen;
            }

            if (_pixelsDirty)
            {
                _pixelsDirty = false;
                _bitmap.WritePixels(BitmapRect, _pixels, BitmapStride, 0);
            }
        }

        private void SetImageSize()
        {
            int uniformScale = Math.Max(1, Math.Min((int)_page.RenderSize.Width / BitmapWidth, (int)_page.RenderSize.Height / BitmapHeight));
            _image.Width = uniformScale * BitmapWidth;
            _image.Height = uniformScale * BitmapHeight;
            Machine.Video.DirtyScreen();
        }

        private void SetWindowSizeToContent()
        {
            if (!_sizedToContent)
            {
                _sizedToContent = true;
                var window = Application.Current.MainWindow;
                var size = window.DesiredSize;
                window.Width = size.Width;
                window.Height = size.Height;
            }
        }

        private const int BitmapWidth = 560;
        private const int BitmapHeight = 384;
        private const int BitmapDpi = 96;
        private static readonly PixelFormat BitmapPixelFormat = PixelFormats.Bgr32;
        private static readonly int BitmapStride = (BitmapWidth * BitmapPixelFormat.BitsPerPixel + 7) / 8;
        private static readonly Int32Rect BitmapRect = new Int32Rect(0, 0, BitmapWidth, BitmapHeight);

        private UserControl _page;
        private Image _image;
        private WriteableBitmap _bitmap = new WriteableBitmap(BitmapWidth, BitmapHeight, BitmapDpi, BitmapDpi, BitmapPixelFormat, null);
        private uint[] _pixels = new uint[BitmapWidth * BitmapHeight];
        private bool _pixelsDirty;
        private bool _isFullScreen;
        private bool _sizedToContent;
    }
}
