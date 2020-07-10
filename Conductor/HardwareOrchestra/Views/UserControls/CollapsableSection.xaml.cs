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

// Die Elementvorlage "Benutzersteuerelement" wird unter https://go.microsoft.com/fwlink/?LinkId=234236 dokumentiert.

namespace HardwareOrchestra.Views.UserControls
{
    public sealed partial class CollapsableSection : UserControl
    {


        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(CollapsableSection), new PropertyMetadata(true, IsOpenChanged));

        private static void IsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var section = d as CollapsableSection;
            if (section.IsOpen) section.OpenCloseIcon.Glyph = "\uE972";
            else section.OpenCloseIcon.Glyph = "\uE974";
        }

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Glyph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register("Glyph", typeof(string), typeof(CollapsableSection), new PropertyMetadata(null, GlyphChanged));

        private static void GlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as CollapsableSection;
            if (e.NewValue is null) instance.Icon.Visibility = Visibility.Collapsed;
            else instance.Icon.Visibility = Visibility.Visible;
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CollapsableSection), new PropertyMetadata(null));



        public object SectionContent
        {
            get { return (object)GetValue(SectionContentProperty); }
            set { SetValue(SectionContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SectionContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionContentProperty =
            DependencyProperty.Register("SectionContent", typeof(object), typeof(CollapsableSection), new PropertyMetadata(null));




        public CollapsableSection()
        {
            this.InitializeComponent();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            

        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsOpen = !IsOpen;
        }
    }
}
