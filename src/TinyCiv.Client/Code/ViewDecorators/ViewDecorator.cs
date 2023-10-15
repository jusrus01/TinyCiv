using System;
using System.Windows;
using TinyCiv.Client.Code.MVVM.View;

namespace TinyCiv.Client.Code.ViewDecorators
{
    public abstract class ViewDecorator : ToggleView
    {
        protected ToggleView _decoratedView;

        protected ViewDecorator(ToggleView decoratedView)
        {
            _decoratedView = decoratedView;
        }

        public override void Hide()
        {
            _decoratedView.Visibility = Visibility.Collapsed;
        }

        public override void Show()
        {
            _decoratedView.Visibility = Visibility.Visible;
        }
    }
}
