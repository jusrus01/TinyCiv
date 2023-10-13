using System;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using TinyCiv.Client.Code.UnitDecorators;

namespace TinyCiv.Client.Code.Decorators
{
    public class BorderFlashDecorator : BorderDecorator
    {
        private bool isFlashing = false;
        private DispatcherTimer flashTimer;
        private Brush originalBackgroundColor;

        public BorderFlashDecorator(BorderObject borderObject) : base(borderObject)
        {
            flashTimer = new DispatcherTimer();
            flashTimer.Interval = TimeSpan.FromMilliseconds(1000);
            flashTimer.Tick += FlashTimer_Tick;
            originalBackgroundColor = wrappee.BackgroundBrush;
        }

        public override void ApplyBorderEffects()
        {
            isFlashing = true;
            flashTimer.Start();
            base.ApplyBorderEffects();
        }

        public override void RemoveBorderEffects()
        {
            isFlashing = false;
            flashTimer.Stop();
            base.RemoveBorderEffects();
        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            if (isFlashing)
            {
                wrappee.BackgroundBrush = originalBackgroundColor;
                wrappee.BorderThickness = new Thickness(2);
            }
            else
            {
                wrappee.BackgroundBrush = Brushes.Transparent;
                wrappee.BorderThickness = new Thickness(0);
            }

            isFlashing = !isFlashing;
        }
    }
}
