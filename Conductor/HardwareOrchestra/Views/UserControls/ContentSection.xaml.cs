using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// Die Elementvorlage "Benutzersteuerelement" wird unter https://go.microsoft.com/fwlink/?LinkId=234236 dokumentiert.

namespace HardwareOrchestra.Views.UserControls
{
    public sealed partial class ContentSection : UserControl
    {
        public ContentSection()
        {
            this.InitializeComponent();
        }



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ContentSection), new PropertyMetadata(null));



        public object SectionContent
        {
            get { return (object)GetValue(SectionContentProperty); }
            set { SetValue(SectionContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SectionContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionContentProperty =
            DependencyProperty.Register("SectionContent", typeof(object), typeof(ContentSection), new PropertyMetadata(null));

















        public ObservableCollection<UIElement> ControlItems { get; set; } = new ObservableCollection<UIElement>();




    }
}
