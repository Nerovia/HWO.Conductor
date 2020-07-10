using HardwareOrchestra.Viewmodels.Orchestra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HardwareOrchestra.Viewmodels
{
    public sealed class MainViewmodel
    {
        #region Properties



        /// <summary>
        /// Gets a singleton instance of the MainViewmodel.
        /// </summary>
        public static MainViewmodel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_Instance == null)
                        _Instance = new MainViewmodel();
                    return _Instance;
                }
            }
        }
        private static readonly object padlock = new object();
        private static MainViewmodel _Instance = null;


        /// <summary>
        /// Gets or sets the main <see cref="OrchestraViewmodel"/>.
        /// </summary>
        public OrchestraViewmodel Orchestra { get; private set; }


        /// <summary>
        /// Gets or sets the main <see cref="MusicLibraryViewmodel"/>.
        /// </summary>
        public MusicLibraryViewmodel MusicLibrary { get; private set; }


        /// <summary>
        /// Gets or sets the main <see cref="DeviceFinderViewmodel"/>.
        /// </summary>
        public DeviceFinderViewmodel DeviceFinder { get; private set; }



        #endregion

        #region Constructor



        /// <summary>
        /// Initializes a new MainViewmodel
        /// </summary>
        private MainViewmodel()
        {
            Task.WaitAll(Task.Run(async () =>
            {
                MusicLibrary = new MusicLibraryViewmodel(await ApplicationData.Current.LocalFolder.CreateFolderAsync("MusicLibrary", CreationCollisionOption.OpenIfExists));
            }));
            Orchestra = new OrchestraViewmodel();
            DeviceFinder = new DeviceFinderViewmodel();
            App.Current.Suspending += OnAppSuspending;
        }




        #endregion

        #region Private Methodes



        /// <summary>
        /// Gets called when the App suspends
        /// </summary>
        private async void OnAppSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            await MusicLibrary.Save();
            MusicLibrary.Close();
        }



        #endregion
    }
}
