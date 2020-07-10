using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Input;

namespace HardwareOrchestra.Views.Templates
{
    public class ContentToggleButton : Button
    {
        public object OnContent
        {
            get { return (object)GetValue(OnContentProperty); }
            set { SetValue(OnContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnContentProperty =
            DependencyProperty.Register("OnContent", typeof(object), typeof(ContentToggleButton), new PropertyMetadata(null));


        

        public object OffContent
        {
            get { return (object)GetValue(OffContentProperty); }
            set { SetValue(OffContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffContentProperty =
            DependencyProperty.Register("OffContent", typeof(object), typeof(ContentToggleButton), new PropertyMetadata(null));



        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ContentToggleButton), new PropertyMetadata(false, OnIsCheckedChanged));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ContentToggleButton;
            instance.UpdateContent();
        }

        public ContentToggleButton()
        {

            Loaded += ContentToggleButton_Loaded;
        }

        private void ContentToggleButton_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateContent();
        }

        protected void UpdateContent()
        {
            if (IsChecked.Equals(true))
            {
                base.Content = OnContent;
            }
            else
            {
                base.Content = OffContent;
            }
        }


        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            IsChecked = !IsChecked;
        }
    }
}
