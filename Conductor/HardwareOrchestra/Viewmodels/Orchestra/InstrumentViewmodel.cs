using Conductor.App.Resources;
using HardwareOrchestra.Resources.Orchestra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareOrchestra.Viewmodels.Orchestra
{
    public class InstrumentViewmodel : BindableBase
    {
        public InstrumentViewmodel(InstrumentType type, byte number)
        {
            Type = type;
            Number = number;
        }

        public string Name
        {
            get => Type.GetStringValue();
        }

        public string Id
        {
            get => ((int)Type).ToString();
        }

        public InstrumentType Type { get; set; }

        public byte Number { get; set; }








        public ChannelViewmodel AssignedChannel
        {
            get => _AssignedChannel;
            set
            {
                if (_AssignedChannel != value)
                {
                    if (_AssignedChannel != null)
                        _AssignedChannel.AssignedInstrument = null;
                    
                    _AssignedChannel = value;

                    if (_AssignedChannel != null)
                    {
                        if (_AssignedChannel.AssignedInstrument != null)
                            _AssignedChannel.AssignedInstrument.AssignedChannel = null;
                        _AssignedChannel.AssignedInstrument = this;
                    }
                        

                    OnPropertyChanged(nameof(AssignedChannel));
                }
            }
        }
        private ChannelViewmodel _AssignedChannel;
    }
}
