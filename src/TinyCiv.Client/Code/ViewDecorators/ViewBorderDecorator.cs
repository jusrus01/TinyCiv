using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using TinyCiv.Client.Code.MVVM.View;
using System.Configuration;

namespace TinyCiv.Client.Code.ViewDecorators
{
    public class ViewBorderDecorator : ViewDecorator
    {
        public ViewBorderDecorator(ToggleView decoratedView) : base(decoratedView) { }

        public override void Show()
        {
            base.Show();
            ApplyBorder();
        }

        private void ApplyBorder()
        {
            var border = new Border
            {
                BorderThickness = new Thickness(4),
                BorderBrush = Brushes.Red
            };
            border.Child = _decoratedView;
            _decoratedView.Content = border;
        }
    }
}
