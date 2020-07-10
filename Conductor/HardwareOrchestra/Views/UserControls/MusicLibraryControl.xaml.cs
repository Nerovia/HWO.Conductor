using Conductor.App.Resources;
using HardwareOrchestra.Viewmodels;
using HardwareOrchestra.Viewmodels.Orchestra;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class MusicLibraryControl : UserControl
    {
        #region Dependency Properties



        public OrchestraViewmodel Orchestra
        {
            get { return (OrchestraViewmodel)GetValue(OrchestraProperty); }
            set { SetValue(OrchestraProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orchestra.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrchestraProperty =
            DependencyProperty.Register("Orchestra", typeof(OrchestraViewmodel), typeof(MusicLibraryControl), new PropertyMetadata(null));



        public MusicLibraryViewmodel MusicLibrary
        {
            get { return (MusicLibraryViewmodel)GetValue(MusicLibraryProperty); }
            set { SetValue(MusicLibraryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MusicLibrary.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MusicLibraryProperty =
            DependencyProperty.Register("MusicLibrary", typeof(MusicLibraryViewmodel), typeof(MusicLibraryControl), new PropertyMetadata(null, MusicLibraryChanged));



        private async static void MusicLibraryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as MusicLibraryControl;
            var musicLibrary = e.NewValue as MusicLibraryViewmodel;
            musicLibrary.PropertyChanged += instance.MusicLibrary_PropertyChanged;
            if (musicLibrary != null)
                await musicLibrary.Open();
            instance.UpdataCollectionViews();
        }

        private void MusicLibrary_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MusicLibraryViewmodel.Sheetmusic))
                UpdataCollectionViews();
        }



        #endregion

        #region Private Fields



        private AdvancedCollectionView allSheetsCollectionView;

        private AdvancedCollectionView favouriteSheetsCollectionView;



        #endregion

        #region Constructor



        public MusicLibraryControl()
        {
            this.InitializeComponent();
        }



        #endregion

        #region Private Methodes



        private void UpdataCollectionViews()
        {
            if (MusicLibrary is null || MusicLibrary.Sheetmusic is null)
            {
                allSheetsCollectionView = null;
                favouriteSheetsCollectionView = null;
            }
            else
            {
                var titleSortAscending = new SortDescription("Title", SortDirection.Ascending);

                allSheetsCollectionView = new AdvancedCollectionView(MusicLibrary.Sheetmusic, true);
                allSheetsCollectionView.SortDescriptions.Add(titleSortAscending);
                MusicExplorerItemsControl.ItemsSource = allSheetsCollectionView;

                favouriteSheetsCollectionView = new AdvancedCollectionView(MusicLibrary.Sheetmusic, true);
                favouriteSheetsCollectionView.SortDescriptions.Add(titleSortAscending);
                favouriteSheetsCollectionView.Filter = x => (x as SheetmusicViewmodel).IsFavourite;
                favouriteSheetsCollectionView.ObserveFilterProperty("IsFavourite");
                FavouritesItemsControl.ItemsSource = favouriteSheetsCollectionView;
            }
        }


        private async void AddMusicFile_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            filePicker.FileTypeFilter.Add(".mid");

            StorageFile file = await filePicker.PickSingleFileAsync();
            if (file != null)
            {
                MusicLibrary.Add(file, file.DisplayName, null, true);
            }
        }


        private void RemoveMusicFile_Click(object sender, RoutedEventArgs e)
        {
            MusicLibrary.Remove((sender as FrameworkElement).DataContext as SheetmusicViewmodel);
        }


        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var text = sender.Text.Trim();
            if (string.IsNullOrEmpty(text))
                allSheetsCollectionView.Filter = null;
            else
                allSheetsCollectionView.Filter = x => (x as SheetmusicViewmodel).Title.ToLower().Contains(text.ToLower());
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Orchestra.SelectedSheet = (sender as FrameworkElement).DataContext as SheetmusicViewmodel;
        }



        #endregion
    }



    public class MusicInfoViewModel : BindableBase
    {
        public ParameterlessCommandHandler PointerEnteredCommand { get; private set; }

        public ParameterlessCommandHandler PointerExitedCommand { get; private set; }

        public MusicInfoViewModel()
        {
            PointerEnteredCommand = new ParameterlessCommandHandler(() => { IsHoveredOver = true; });
            PointerExitedCommand = new ParameterlessCommandHandler(() => { IsHoveredOver = false; });
        }

        public string Title { get; set; } = "Test Piece";


        public bool IsHoveredOver
        {
            get => _IsHoveredOver;
            set { Set(ref _IsHoveredOver, value); }
        }
        private bool _IsHoveredOver;


    }
}
