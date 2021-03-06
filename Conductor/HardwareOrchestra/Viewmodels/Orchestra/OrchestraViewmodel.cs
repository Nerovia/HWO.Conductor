﻿using Conductor.App.Resources;
using HardwareOrchestra.Resources.Orchestra;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.UI.Core;

namespace HardwareOrchestra.Viewmodels.Orchestra
{
    public sealed class OrchestraViewmodel : BindableBase
    {
        #region Constructor



        public OrchestraViewmodel()
        {
            conductor = new Resources.Orchestra.Conductor();
            conductor.OrchestraStateChanged += Conductor_OrchestraStateChanged;
            conductor.InsturmentsChanged += Conductor_InsturmentsChanged;
            conductor.ConnectionChanged += Conductor_ConnectionChanged;
            conductor.QueueRequest = Conductor_QueueRequest;
        }



        #endregion

        #region Properties



        public DeviceInformation SelectedDevice
        {
            get => _SelectedDevice;
            set 
            { 
                if (Set(ref _SelectedDevice, value))
                {
                    if (IsConnected)
                        Disconnect();
                }
            }
        }
        private DeviceInformation _SelectedDevice;



        public SheetmusicViewmodel SelectedSheet
        {
            get => _SelectedSheet;
            set 
            {
                if (Set(ref _SelectedSheet, value))
                    if (_SelectedSheet != null)
                        LoadPiece();
            }
        }
        private SheetmusicViewmodel _SelectedSheet;



        public InstrumentViewmodel[] Instruments
        {
            get => _Instruments;
            set 
            {
                if (Set(ref _Instruments, value))
                    OnPropertyChanged(nameof(CanPlay));
            }
        }
        private InstrumentViewmodel[] _Instruments;



        public ChannelViewmodel[] Channels
        {
            get => _Channels;
            set { Set(ref _Channels, value); }
        }
        private ChannelViewmodel[] _Channels;



        public TimeSpan PieceDuration
        {
            get => _PieceDuration;
            set { Set(ref _PieceDuration, value); }
        }
        private TimeSpan _PieceDuration;



        public OrchestraState OrchestraState
        {
            get => conductor.OrchestraState;
        }


        public bool IsConnected
        {
            get => conductor.IsConnected;
        }

        public bool CanPlay
        {
            get => 
                Piece != null &&
                Instruments != null &&
                IsConnected &&
                (OrchestraState == OrchestraState.Rest || OrchestraState == OrchestraState.Perform);
        }


        public bool IsLoaded
        {
            get => _IsLoaded;
            set 
            {
                if (_IsLoaded != value)
                {
                    _IsLoaded = value;
                    OnPropertyChanged(nameof(CanPlay));
                }
            }
        }
        private bool _IsLoaded;




        public Tone[] Piece
        {
            get => _Piece;
            set 
            { 
                if (_Piece != value)
                {
                    _Piece = value;
                    Stop();
                    OnPropertyChanged(nameof(CanPlay));
                }
            }
        }
        private Tone[] _Piece;


        #endregion

        #region Private Fields

        private uint currentTone;

        private readonly Resources.Orchestra.Conductor conductor;




        #endregion

        #region Public Fields



        public async void Connect()
        {
            if (SelectedDevice is null)
                return;

            if (IsConnected)
                return;

            string portName;
            int baudRate;
            using (var serialDevice = await SerialDevice.FromIdAsync(SelectedDevice.Id))
            {
                if (serialDevice is null)
                    return;
                portName = serialDevice.PortName;
                baudRate = (int)serialDevice.BaudRate;
            }
            conductor.Connect(portName, baudRate);
        }


        public void Disconnect()
        {
            if (!IsConnected)
                return;

            conductor.Disconnect();
        }


        public async void Play()
        {
            if (!CanPlay)
                return;

            if (OrchestraState != OrchestraState.Rest)
                return;

            if (currentTone >= Piece.Length)
                currentTone = 0;

            await conductor.ChangeState(OrchestraState.Perform);
        }


        public async void Conclude()
        {
            await conductor.ChangeState(OrchestraState.Conclude);
        }


        public async void Pause()
        {
            await conductor.ChangeState(OrchestraState.Rest);
        }


        public async void Stop()
        {
            currentTone = 0;
            conductor.ClearQueue();
            await conductor.ChangeState(OrchestraState.Rest);
        }

        public void GoTo(uint currentTone)
        {
            Stop();
            if (currentTone < Piece.Length)
                currentTone = (uint)Piece.Length;
            this.currentTone = currentTone;
        }


        private Tone Conductor_QueueRequest()
        {
            if (currentTone >= Piece.Length)
            {
                Conclude();
                return null;
            }

            Tone tone = new Tone();
            tone.duration = Piece[currentTone].duration;
            tone.instrument = Piece[currentTone].instrument;
            tone.note = Piece[currentTone].note;

            var assignedInstrument = Channels[Piece[currentTone].instrument].AssignedInstrument;
            if (assignedInstrument is null)
                tone.instrument = 255;
            else
                tone.instrument = assignedInstrument.Number;

            currentTone++;
            return tone;
        }



        #endregion

        #region Private Methodes



        private async void LoadPiece()
        {
            // Read Midi
            var midi = await SelectedSheet.TryGetMidiAsync();
            if (midi is null)
                return;


            // Get Notes & Channels
            var tracks = midi.GetTrackChunks().ToArray();
            var tempoMap = midi.GetTempoMap();

            var tones = new List<Tone>();
            var channels = new ChannelViewmodel[tracks.Count()];

            byte trackNumber = 0;
            foreach (TrackChunk track in midi.GetTrackChunks())
            {
                foreach (Note note in track.GetNotes())
                {
                    tones.Add(new Tone()
                    {
                        duration = (int)note.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000,
                        note = note.NoteNumber,
                        instrument = trackNumber,
                    });

                    tones.Add(new Tone()
                    {
                        duration = (int)note.EndTimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000,
                        note = 0,
                        instrument = trackNumber,
                    });
                }
                channels[trackNumber] = new ChannelViewmodel(trackNumber);
                trackNumber++;
            }
            tones.Sort((x, y) => x.duration.CompareTo(y.duration));

            
            for (int n = 0; n < tones.Count() - 1; n++)
            {
                tones.ElementAt(n).duration = tones[n + 1].duration - tones[n].duration;
            }
            tones.Last().duration = 10;


            Piece = tones.ToArray();
            Channels = channels;
        }


        //private async void LoadPiece()
        //{
        //    // Read Midi
        //    var midi = await SelectedSheet.TryGetMidiAsync();
        //    if (midi is null)
        //        return;

        //    // Get Channels
        //    var channels = midi.GetChannels().ToArray();
        //    ChannelViewmodel[] channelViewmodels = new ChannelViewmodel[channels.Count()];
        //    for (byte n = 0; n < channelViewmodels.Length; n++)
        //    {
        //        channelViewmodels[n] = new ChannelViewmodel(channels[n]);
        //        if (n == byte.MaxValue)
        //            break;
        //    }

        //    // Get Tones
        //    var tempomap = midi.GetTempoMap();
        //    midi.RemoveTimedEvents((TimedEvent x) => !(x.Event is NoteEvent));
        //    var events = midi.GetTimedEvents().ToArray();
        //    var piece = new Tone[events.Count()];

        //    for (int n = 0; n < events.Length; n++)
        //    {
                
        //        var note = events[n].Event as NoteEvent;

        //        if (note is NoteOnEvent)
        //            piece[n].note = note.NoteNumber;
        //        else
        //            piece[n].note = 0;

        //        piece[n].instrument = note.Channel;

        //        piece[n].duration = (int)(note.DeltaTime * tempomap.Tempo.Last((ValueChange<Tempo> x) => events[n].Time >= x.Time).Value.MicrosecondsPerQuarterNote / 480 / 1000);
        //    }

        //    // Get Duration
        //    Piece = piece;
        //    PieceDuration = events.Last().TimeAs<MetricTimeSpan>(tempomap);

        //    return;
        //}


        private async void Conductor_ConnectionChanged(Resources.Orchestra.Conductor sender, bool isConnected)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                OnPropertyChanged(nameof(IsConnected));
                OnPropertyChanged(nameof(CanPlay));
            });
        }


        private async void Conductor_InsturmentsChanged(Resources.Orchestra.Conductor sender, InstrumentType[] instruments)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (instruments is null)
                {
                    Instruments = null;
                }
                else
                {
                    var instrumentViewmodels = new InstrumentViewmodel[instruments.Length];
                    for (byte n = 0; n < (byte)instruments.Length; n++)
                    {
                        instrumentViewmodels[n] = new InstrumentViewmodel(instruments[n], n);
                    }
                    Instruments = instrumentViewmodels;
                }
            });
        }


        private async void Conductor_OrchestraStateChanged(Resources.Orchestra.Conductor sender, OrchestraState newState, OrchestraState oldState)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                OnPropertyChanged(nameof(OrchestraState));
                OnPropertyChanged(nameof(CanPlay));
                if (oldState == OrchestraState.Conclude && newState == OrchestraState.Rest)
                    currentTone = 0;
            });
        }



        #endregion

        #region Public Methodes



        // Strat here --->



        #endregion

        #region Events



        // Strat here --->



        #endregion
    }
}
