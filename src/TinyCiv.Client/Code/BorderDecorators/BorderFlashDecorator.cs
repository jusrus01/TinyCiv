using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public class BorderFlashDecorator : BorderDecorator
    {
        private Color ogBackground;
        private Color ogBorderBrush;
        private Storyboard backgroundAnimation;
        private Storyboard borderAnimation;
        private BorderProperties decoratedBorder;

        public BorderFlashDecorator(IBorderObject decoratedObject) : base(decoratedObject) { }

        public override BorderProperties ApplyEffects()
        {
            decoratedBorder = base.ApplyEffects();
            ogBackground = ConvertBrushToColor(decoratedBorder.BackgroundBrush);
            ogBorderBrush = ConvertBrushToColor(decoratedBorder.BorderBrush);

            if (decoratedBorder != null)
            {
                TimeSpan duration = TimeSpan.FromSeconds(1);
                var targerBackgroundColor = Color.FromArgb((byte)(ogBackground.A - ogBackground.A * 0.5), ogBackground.R, ogBackground.G, ogBackground.B);
                var targetBorderColor = Color.FromArgb((byte)(ogBorderBrush.A - ogBorderBrush.A * 0.5), ogBorderBrush.R, ogBorderBrush.G, ogBorderBrush.B);

                AnimateBackgroundColor(ogBackground, targerBackgroundColor, duration);
                AnimateBorderColor(ogBorderBrush, targetBorderColor, duration);
            }

            return decoratedBorder;
        }

        public override BorderProperties RemoveEffects()
        {
            backgroundAnimation.Stop();
            borderAnimation.Stop();
            //RemoveBackgroundColorAnimation();
            //RemoveBorderColorAnimation();
            return base.RemoveEffects();
        }

        public void AnimateBackgroundColor(Color fromColor, Color toColor, TimeSpan duration)
        {
            //var animation = new ColorAnimation
            //{
            //    From = fromColor,
            //    To = toColor,
            //    Duration = duration,
            //    AutoReverse = true,
            //    RepeatBehavior = RepeatBehavior.Forever,
            //};

            //SolidColorBrush brush = new SolidColorBrush(fromColor);
            //decoratedBorder.BackgroundBrush = brush;

            //brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            var animation = new ColorAnimation
            {
                From = fromColor,
                To = toColor,
                Duration = duration,
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
            };

            backgroundAnimation = new Storyboard();
            backgroundAnimation.Children.Add(animation);

            Storyboard.SetTarget(animation, decoratedBorder.BackgroundBrush);
            Storyboard.SetTargetProperty(animation, new PropertyPath(SolidColorBrush.ColorProperty));

            backgroundAnimation.Begin();
        }

        public void AnimateBorderColor(Color fromColor, Color toColor, TimeSpan duration)
        {
            var animation = new ColorAnimation
            {
                From = fromColor,
                To = toColor,
                Duration = duration,
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
            };

            SolidColorBrush brush = new SolidColorBrush(fromColor);
            decoratedBorder.BorderBrush = brush;

            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        //public void RemoveBackgroundColorAnimation()
        //{
        //    decoratedBorder.BeginAnimation(Border.BackgroundProperty, null);
        //}

        //public void RemoveBorderColorAnimation()
        //{
        //    decoratedBorder.BeginAnimation(Border.BorderBrushProperty, null);
        //}

        public Color ConvertBrushToColor(Brush brush)
        {
            if (brush is SolidColorBrush solidColorBrush)
            {
                return solidColorBrush.Color;
            }
            return Colors.Transparent;
        }
    }
}
