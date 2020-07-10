using Conductor.App.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace HardwareOrchestra.Viewmodels
{
    public sealed class DeviceFinderViewmodel : BindableBase
    {
        #region Constructor



        public DeviceFinderViewmodel()
        {
            Refresh();
        }



        #endregion

        #region Properties



        public DeviceInformation[] Devices
        {
            get => _Devices;
            set { Set(ref _Devices, value); }
        }
        private DeviceInformation[] _Devices;



        #endregion

        #region Private Fields



        // No Private Fields



        #endregion

        #region Public Fields



        // No Public Fields



        #endregion

        #region Private Methodes



        // No Private Methodes



        #endregion

        #region Public Methodes



        public async void Refresh()
        {
            string aqs = SerialDevice.GetDeviceSelector();
            var deviceCollection = await DeviceInformation.FindAllAsync(aqs);
            Devices = deviceCollection.ToArray();
        }



        #endregion

        #region Events



        // No Events



        #endregion


    }
}
