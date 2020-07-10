using HardwareOrchestra.Viewmodels;
using HardwareOrchestra.Viewmodels.Orchestra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Benutzersteuerelement" wird unter https://go.microsoft.com/fwlink/?LinkId=234236 dokumentiert.

namespace HardwareOrchestra.Views.UserControls
{
    public sealed partial class ConcertmasterControl : UserControl
    {
        public ConcertmasterControl()
        {
            this.InitializeComponent();
        }


        public OrchestraViewmodel Orchestra
        {
            get { return (OrchestraViewmodel)GetValue(OrchestraProperty); }
            set { SetValue(OrchestraProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orchestra.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrchestraProperty =
            DependencyProperty.Register("Orchestra", typeof(OrchestraViewmodel), typeof(ConcertmasterControl), new PropertyMetadata(0));





        public DeviceFinderViewmodel DeviceFinder
        {
            get { return (DeviceFinderViewmodel)GetValue(DeviceFinderProperty); }
            set { SetValue(DeviceFinderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeviceFinder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeviceFinderProperty =
            DependencyProperty.Register("DeviceFinder", typeof(DeviceFinderViewmodel), typeof(ConcertmasterControl), new PropertyMetadata(0));






        private void ConnectDisconnectButtonClick(object sender, RoutedEventArgs e)
        {
            if (Orchestra.IsConnected)
                Orchestra.Disconnect();
            else
                Orchestra.Connect();
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ItemsControl).ItemsSource = Orchestra.Channels;
        }

        private void AvailableDevicesComboBox_DropDownOpened(object sender, object e)
        {
            DeviceFinder?.Refresh();
        }
    }

    public class TitleTemplate
    {
        public string Text { get; set; }
        public string Glyph { get; set; }
        public bool IsGlyphVisible { get; set; } = true;
    }


    public class SwitchTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FalseTemplate { get; set; }
        public DataTemplate TrueTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is bool && (bool)item) return TrueTemplate;
            else return FalseTemplate;
        }
    }
}