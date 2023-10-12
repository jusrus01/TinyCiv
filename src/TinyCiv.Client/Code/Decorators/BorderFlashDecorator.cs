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
        private GameObject gameObject;

        public BorderFlashDecorator(GameObject gameObject) : base(gameObject)
        {
            flashTimer = new DispatcherTimer();
            flashTimer.Interval = TimeSpan.FromMilliseconds(1000); 
            flashTimer.Tick += FlashTimer_Tick;
        }

        new public void ApplyBorderEffects()
        {
            isFlashing = true;
            flashTimer.Start();
        }

        new public void RemoveBorderEffects()
        {
            isFlashing = false;
            flashTimer.Stop();
        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            if (isFlashing)
            {
                gameObject.BorderThickness = new Thickness(2);
            }
            else
            { 
                gameObject.BorderThickness = new Thickness(0);
            }

            isFlashing = !isFlashing;
        }
    }
}
