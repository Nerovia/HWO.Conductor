using HardwareOrchestra.Viewmodels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace HardwareOrchestra
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewmodel Viewmodel = App.Viewmodel;

        public MainPage()
        {
            this.InitializeComponent();
        }


        // !!! Testing !!!
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Viewmodel?.Orchestra?.Play();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Viewmodel?.Orchestra?.Pause();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Viewmodel?.Orchestra?.GoTo(0);
        }
        // !!! Testing !!!
    }
}
