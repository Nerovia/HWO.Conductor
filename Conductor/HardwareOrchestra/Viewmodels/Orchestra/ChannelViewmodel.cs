using Conductor.App.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareOrchestra.Viewmodels.Orchestra
{
    public sealed class ChannelViewmodel : BindableBase
    {
        #region Constructor



        public ChannelViewmodel(byte number)
        {
            Number = number;
        }



        #endregion

        #region Properties

        public void ResetAssignedInstrument()
        {
            if (AssignedInstrument is null)
                return;

            AssignedInstrument.AssignedChannel = null;
            AssignedInstrument = null;
        }



        public InstrumentViewmodel AssignedInstrument { get; set; }


        public byte Number
        {
            get => _Number;
            set { Set(ref _Number, value); }
        }
        private byte _Number;

        #endregion
    }
}
