namespace WhereDidTheMoneyGo.AttachedProperties
{
    using System;
    using Windows.UI.Xaml;

    public class AnimationsProperties
    {
        public static bool GetShowHideValue(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowHideProperty);
        }

        public static void SetShowHideValue(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowHideProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty. 
        // This enables animation, styling, binding, etc...

        public static readonly DependencyProperty ShowHideProperty =
            DependencyProperty.RegisterAttached("ShowHide",
                                                typeof(bool),
                                                typeof(UIElement),
                                                new PropertyMetadata(true, new PropertyChangedCallback(HandleShowHideChange)));

        //TODO: Validate If the user is a idiot, and presses the button 6000 times/second to brake the animation
        private static void HandleShowHideChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            if (element == null)
            {
                return;
            }

            var timer = new DispatcherTimer();
            if (element.Opacity >= 1 && element.Visibility == Visibility.Visible)
            {
                timer.Tick += (sender, args) =>
                {
                    if (element.Opacity <= 0)
                    {
                        timer.Stop();
                        element.Visibility = Visibility.Collapsed;
                        return;
                    }
                    element.Opacity -= 0.1;

                };
                timer.Interval = TimeSpan.FromMilliseconds(25);
                timer.Start();
            }
            else
            {
                element.Opacity = 0.1;
                element.Visibility = Visibility.Visible;
                timer.Tick += (sender, args) =>
                {
                    if (element.Opacity >= 1)
                    {
                        timer.Stop();
                        return;
                    }
                    element.Opacity += 0.1;

                };
                timer.Interval = TimeSpan.FromMilliseconds(25);
                timer.Start();
            }
        }
    }
}
