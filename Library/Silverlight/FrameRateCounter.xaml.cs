using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Jellyfish.Library
{
    public sealed partial class FrameRateCounter : UserControl
    {
        public FrameRateCounter()
        {
            InitializeComponent();

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            _frameCount++;

            long time = DateTime.UtcNow.Ticks;
            if (time - _lastTime >= TimeSpan.TicksPerSecond)
            {
                _lastTime = time;
                FrameRate = _frameCount;
                _frameCount = 0;
            }
        }

        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register("FrameRate", typeof(int), typeof(FrameRateCounter), 
            new PropertyMetadata(0));

        public int FrameRate
        {
            get { return (int)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
        }

        private int _frameCount;
        private long _lastTime;
    }
}
