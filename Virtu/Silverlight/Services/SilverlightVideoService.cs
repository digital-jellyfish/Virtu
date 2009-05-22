using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Jellyfish.Virtu.Services
{
    public sealed class SilverlightVideoService : VideoService
    {
        public SilverlightVideoService(Image image)
        {
            _image = image;
            SetImageSize();

            _bitmap = new WriteableBitmap(BitmapWidth, BitmapHeight, BitmapPixelFormat);
            _pixels = new uint[BitmapWidth * BitmapHeight];

            Application.Current.Host.Content.Resized += (sender, e) => SetImageSize();
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "y*560")]
        public override void SetPixel(int x, int y, uint color)
        {
            _pixels[y * BitmapWidth + x] = color;
            _pixelsDirty = true;
        }

        public override void Update()
        {
            if (Application.Current.RunningOffline && /*_window.IsActive &&*/ (_isFullScreen != IsFullScreen))
            {
                if (IsFullScreen) // SL is missing out of browser window control
                {
                    //_window.SizeToContent = SizeToContent.Manual;
                    //_window.Topmost = true;
                    //_window.WindowStyle = WindowStyle.None;
                    //_window.WindowState = WindowState.Maximized;
                }
                else
                {
                    //_window.WindowState = WindowState.Normal;
                    //_window.WindowStyle = WindowStyle.SingleBorderWindow;
                    //_window.Topmost = false;
                    //_window.SizeToContent = SizeToContent.WidthAndHeight;
                }
                _isFullScreen = IsFullScreen;
            }

            if (_pixelsDirty)
            {
                _pixelsDirty = false;
                _bitmap.Lock();
                for (int i = 0; i < BitmapWidth * BitmapHeight; i++)
                {
                    _bitmap[i] = (int)_pixels[i];
                }
                _bitmap.Invalidate();
                _bitmap.Unlock();
                _image.Source = _bitmap; // shouldn't have to set source each frame; SL bug?
            }
        }

        private void SetImageSize()
        {
            Content content = Application.Current.Host.Content;
            int uniformScale = Math.Min((int)content.ActualWidth / BitmapWidth, (int)content.ActualHeight / BitmapHeight);
            _image.Width = uniformScale * BitmapWidth;
            _image.Height = uniformScale * BitmapHeight;
        }

        private const int BitmapWidth = 560;
        private const int BitmapHeight = 384;
        private static readonly PixelFormat BitmapPixelFormat = PixelFormats.Bgr32;

        private Image _image;
        private WriteableBitmap _bitmap;
        private uint[] _pixels;
        private bool _pixelsDirty;
        private bool _isFullScreen;
    }
}
