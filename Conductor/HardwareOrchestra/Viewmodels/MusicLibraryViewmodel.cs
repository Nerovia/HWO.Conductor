using Conductor.App.Resources;
using HardwareOrchestra.Models.Orchestra;
using HardwareOrchestra.Resources.Orchestra;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HardwareOrchestra.Viewmodels
{
    public sealed class MusicLibraryViewmodel : BindableBase
    {
        #region Constructor



        /// <summary>
        /// Initializes a new MusicLibrary. 
        /// </summary>
        /// <param name="libraryFolder"></param>
        public MusicLibraryViewmodel(StorageFolder libraryFolder)
        {
            if (libraryFolder is null)
                throw new ArgumentNullException("libraryFolder cannot be null.");

            this.libraryFolder = libraryFolder;
        }



        #endregion

        #region Properties



        /// <summary>
        /// Provides a bindable collection of <see cref="SheetmusicViewmodel"/>. Is null when the library is closed.
        /// </summary>
        public ObservableCollection<SheetmusicViewmodel> Sheetmusic
        {
            get => _Sheetmusic;
            private set { Set(ref _Sheetmusic, value); }
        }
        private ObservableCollection<SheetmusicViewmodel> _Sheetmusic = null;


        /// <summary>
        /// Provides a bindable property which indicates that an item has been added, removed or edited.
        /// </summary>
        public bool IsManipulated
        {
            get => _IsManipulated;
            set { Set(ref _IsManipulated, value); }
        }
        private bool _IsManipulated;



        #endregion

        #region Private Fields



        private readonly StorageFolder libraryFolder;
        private StorageFile libraryFile;
        private const string libraryFileName = "SheetData.dat";
        
        


        #endregion

        #region Public Methodes



        /// <summary>
        /// Deserializes the library data and loads the saved sheetmusic.
        /// </summary>
        /// <returns></returns>
        public async Task Open()
        {
            if (Sheetmusic != null) return;

            if (libraryFile is null)
                libraryFile = await libraryFolder.CreateFileAsync(libraryFileName, CreationCollisionOption.OpenIfExists);

            List<Sheetmusic> sheetsmusic = null;

            using (var fileStream = await libraryFile.OpenStreamForWriteAsync())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    sheetsmusic = (List<Sheetmusic>)formatter.Deserialize(fileStream);
                }
                catch (SerializationException e)
                {
                    //throw e;
                }
                finally
                {
                    fileStream.Close();
                }
            }

            
            Sheetmusic = new ObservableCollection<SheetmusicViewmodel>();
            if (sheetsmusic != null)
                foreach (var model in sheetsmusic)
                {
                    Sheetmusic.Add(new SheetmusicViewmodel(model));
                }

            IsManipulated = false;
        }


        /// <summary>
        /// Serializes the current state of the library.
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            if (IsManipulated is false)
                return;

            if (libraryFile is null)
                libraryFile = await libraryFolder.CreateFileAsync(libraryFileName, CreationCollisionOption.OpenIfExists);

            List<Sheetmusic> sheetsmusic = new List<Sheetmusic>();
            foreach(var viewmodel in Sheetmusic)
            {
                sheetsmusic.Add(viewmodel.Model);
            }

            using (var fileStream = await libraryFile.OpenStreamForWriteAsync())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(fileStream, sheetsmusic);
                }
                catch (SerializationException e)
                {
                    throw e;
                }
                finally
                {
                    fileStream.Close();
                }
            }

            IsManipulated = false;
        }


        /// <summary>
        /// Closes the library and discards all unsaved changes.
        /// </summary>
        public void Close()
        {
            Sheetmusic = null;
            IsManipulated = false;
        }


        /// <summary>
        /// Adds sheetmusic to the library using a parameters.
        /// </summary>
        /// <param name="sheetmusic"></param>
        /// <returns></returns>
        public async void Add(StorageFile midifile, string title, List<string> tags = null, bool isImported = false)
        {
            if (Sheetmusic is null)
                throw new Exception("library is closed");

            if (midifile is null)
                throw new ArgumentNullException("midifile cannot be null");

            if (midifile.FileType != ".mid" &&
                midifile.FileType != ".midi") 
                throw new ArgumentException("midifile has invalid datatype.");

            if (isImported)
                midifile = await midifile.CopyAsync(libraryFolder, title + midifile.FileType, NameCollisionOption.GenerateUniqueName);

            Sheetmusic.Add(new SheetmusicViewmodel(new Models.Orchestra.Sheetmusic(midifile.Path, title, tags, false, isImported)));

            IsManipulated = true;
        }



        /// <summary>
        /// Removes a certain sheetmusic from the library.
        /// </summary>
        /// <param name="sheetmusic"></param>
        /// <returns></returns>
        public bool Remove(SheetmusicViewmodel sheetmusic)
        {
            if (Sheetmusic is null)
                throw new Exception("library is closed");

            var removed = Sheetmusic.Remove(sheetmusic);

            if (removed)
                IsManipulated = true;

            return removed;
        }



        #endregion
    }



    public sealed class SheetmusicViewmodel : BindableBase
    {
        #region Constructor



        public SheetmusicViewmodel(Sheetmusic model)
        {
            Model = model;
        }



        #endregion

        #region Properties



        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public Sheetmusic Model
        {
            get => _Model;
            private set { Set(ref _Model, value); }
        }
        private Sheetmusic _Model;


        /// <summary>
        /// Gets or sets the Title Property of the model.
        /// </summary>
        public string Title
        {
            get => Model.Title;
            set
            {
                if (Model.Title != value)
                {
                    Model.Title = value;
                    OnPropertyChanged("Title");
                }
            }
        }


        /// <summary>
        /// Gets or sets the IsFavourite Property of the model.
        /// </summary>
        public bool IsFavourite
        {
            get => Model.IsFavourite;
            set
            {
                if (Model.IsFavourite != value)
                {
                    Model.IsFavourite = value;
                    OnPropertyChanged("IsFavourite");
                }
            }
        }



        #endregion

        #region Private Fields




        #endregion

        #region Public Fields



        public Task<MidiFile> TryGetMidiAsync()
        {
            try
            {
                return Task.FromResult(MidiFile.Read(Model.Path));
            }
            catch
            {
                return null;
            }
        }



        #endregion

        #region Private Methodes



        // Strat here --->



        #endregion

        #region Public Methodes



        // Strat here --->



        #endregion

        #region Events




        #endregion
    }
}
