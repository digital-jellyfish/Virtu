using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Jellyfish.Library;

namespace Jellyfish.Virtu.Services
{
    public sealed class SilverlightVideoService : VideoService
    {
        public SilverlightVideoService(Machine machine, UserControl page, Image image) : 
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

            _page.LayoutUpdated += (sender, e) => SetWindowSizeToContent();
            _page.SizeChanged += (sender, e) => SetImageSize();
        }

        public override void SetFullScreen(bool isFullScreen)
        {
            _page.Dispatcher.Send(() =>
            {
                var application = Application.Current;
                if (application.IsRunningOutOfBrowser)
                {
                    var content = application.Host.Content;
                    if (content.IsFullScreen != isFullScreen)
                    {
                        content.IsFullScreen = isFullScreen;
                    }
                }
            });
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow")]
        public override void SetPixel(int x, int y, uint color)
        {
            _pixels[y * BitmapWidth + x] = (int)color;
            _pixelsDirty = true;
        }

        public override void Update() // main thread
        {
            if (_pixelsDirty)
            {
                _pixelsDirty = false;
                for (int i = 0; i < BitmapWidth * BitmapHeight; i++)
                {
                    _bitmap.Pixels[i] = _pixels[i];
                }
                _bitmap.Invalidate();
            }
        }

        private void SetImageSize(bool swapOrientation = false)
        {
            int uniformScale = Math.Max(1, swapOrientation ? Math.Min((int)_page.RenderSize.Height / BitmapWidth, (int)_page.RenderSize.Width / BitmapHeight) : 
                Math.Min((int)_page.RenderSize.Width / BitmapWidth, (int)_page.RenderSize.Height / BitmapHeight));
            _image.Width = uniformScale * BitmapWidth;
            _image.Height = uniformScale * BitmapHeight;
        }

        private void SetWindowSizeToContent()
        {
            var application = Application.Current;
            if (application.IsRunningOutOfBrowser && !_sizedToContent)
            {
                _sizedToContent = true;
                var window = application.MainWindow;
                var size = application.RootVisual.DesiredSize;
                window.Width = size.Width;
                window.Height = size.Height;
            }
        }

        private const int BitmapWidth = 560;
        private const int BitmapHeight = 384;

        private UserControl _page;
        private Image _image;
        private WriteableBitmap _bitmap = new WriteableBitmap(BitmapWidth, BitmapHeight);
        private int[] _pixels = new int[BitmapWidth * BitmapHeight];
        private bool _pixelsDirty;
        private bool _sizedToContent;
    }
}
