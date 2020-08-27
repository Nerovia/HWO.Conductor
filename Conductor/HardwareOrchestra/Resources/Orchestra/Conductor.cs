using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace HardwareOrchestra.Resources.Orchestra
{
    public class Conductor
    {
        #region Constructor



        public Conductor()
        {
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPortDataReceived;
        }



        #endregion

        #region Properties



        public bool IsConnected
        {
            get => _IsConnected;
            private set
            {
                if (_IsConnected != value)
                {
                    _IsConnected = value;
                    OnConnectionChanged(_IsConnected);
                }
            }
        }
        private bool _IsConnected = false;
       

        public InstrumentType[] Instruments
        {
            get => _Instruments;
            set
            { 
                if (_Instruments != value)
                {
                    _Instruments = value;
                    OnInsturmentsChanged(_Instruments);
                }
            }
        }
        private InstrumentType[] _Instruments;


        public OrchestraState OrchestraState
        {
            get => _OrchestraState;
            set
            {
                if (_OrchestraState != value)
                {
                    var oldState = _OrchestraState;
                    _OrchestraState = value;
                    OnOrchestraStateChanged(_OrchestraState, oldState);
                }
            }
        }
        private OrchestraState _OrchestraState;



        #endregion

        #region Private Fields



        private SerialPort serialPort;



        #endregion

        #region Public Fields



        // Strat here --->



        #endregion

        #region Private Methodes



        /// <summary>
        /// Tries to send a command to the concertmaster with additional conductable data.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="instruction"></param>
        private void TryConduct(char command, IConductable conductable)
        {
            TryConduct(command, conductable.ToInstruction());
        }


        /// <summary>
        /// Tries to send a command to the concertmaster without any additional data.
        /// </summary>
        /// <param name="command"></param>
        private void TryConduct(char command)
        {
            TryConduct(command, "");
        }


        /// <summary>
        /// Tries to send a command to the concertmaster with additional data.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="instruction"></param>
        private void TryConduct(char command, string instruction)
        {
            if (!serialPort.IsOpen)
                return;

            try
            {
                Debug.WriteLine("<- " + command + instruction);
                serialPort.WriteLine(command + instruction);
            }
            catch
            {
                if (!serialPort.IsOpen)
                {
                    IsConnected = false;
                    OnConnectionError();
                }
            }
        }


        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (true)
            {
                // Read from the serialbuffer.
                var message = serialPort.ReadLine().Trim().Trim('$');
                Debug.WriteLine("-> " + message);


                // Check if contains anything.
                if (message.Length < 1)
                    return;

                // Aquire command and any additional parameters.
                char command = message[0];
                message = message.Remove(0, 1);
                string[] parameters = null;
                if (message.Length > 0)
                    parameters = message.Split('$');

                // Execute the requested action.
                switch (command)
                {
                    case Command.QueueTone:
                        var tone = OnQueueRequest();
                        if (tone != null)
                            QueueTone(tone);
                        break;

                    case Command.Handshake:
                        IsConnected = true;
                        break;

                    case Command.ChangeState:
                        try
                        {
                            OrchestraState = (OrchestraState)byte.Parse(parameters[0]);
                            stateChanged?.SetResult(OrchestraState);
                        }
                        catch
                        {

                        }
                        break;

                    case Command.InsturmentsChanged:
                        if (parameters is null)
                        {
                            Instruments = null;
                            return;
                        }
                        var instruments = new InstrumentType[parameters.Length];
                        for (byte n = 0; n < instruments.Length; n++)
                        {
                            try
                            {
                                instruments[n] = (InstrumentType)byte.Parse(parameters[n]);
                            }
                            catch
                            {
                                instruments[n] = InstrumentType.Unidentified;
                            }
                        }
                        Instruments = instruments;
                        break;
                }
                if (serialPort.BytesToRead == 0)
                    return;
            }
        }


        #endregion

        #region Public Methodes



        /// <summary>
        /// Tries to open a connection to a concertmaster.
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="BuadRate"></param>
        public void Connect(string portName, int baudRate)
        {
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.Open();
            TryConduct(Command.Handshake);
        }


        /// <summary>
        /// Closes the current connection to a concertmaster.
        /// </summary>
        public async void Disconnect()
        {
            await ChangeState(OrchestraState.Rest);

            serialPort.Close();
            IsConnected = false;
            Instruments = null;
        }


        /// <summary>
        /// Insturcts the concertmaster to change the orchestras state to the requested value.
        /// </summary>
        /// <param name="state"></param>
        public async Task ChangeState(OrchestraState state)
        {
            TryConduct(Command.ChangeState, ((byte)state).ToString());
            stateChanged = new TaskCompletionSource<OrchestraState>();
            await stateChanged.Task;
            stateChanged = null;
        }
        private TaskCompletionSource<OrchestraState> stateChanged;


        /// <summary>
        /// Instructs the concertmaster to tune a specified tone.
        /// </summary>
        /// <param name="tone"></param>
        public void TuneTone(Tone tone)
        {
            TryConduct(Command.TuneTone, tone);
        }


        /// <summary>
        /// Instucts the concertmastert to queue a tone for performing.
        /// </summary>
        /// <param name="tone"></param>
        public void QueueTone(Tone tone)
        {
            TryConduct(Command.QueueTone, tone);
        }


        /// <summary>
        /// Clears the internal performing-queue of the concertmaster.
        /// </summary>
        public void ClearQueue()
        {
            TryConduct(Command.ClearQueue);
        }


        public void Handshake()
        {
            TryConduct(Command.Handshake);
        }



        #endregion

        #region Delegates



        public QueueRequestHandler QueueRequest;
        protected virtual Tone OnQueueRequest()
        {
            return QueueRequest.Invoke();
        }



        #endregion

        #region Events



        public event ConnectionChangedEventHandler ConnectionChanged;
        protected virtual void OnConnectionChanged(bool isConnected)
        {
            ConnectionChanged?.Invoke(this, isConnected);
        }


        public event ConnectionErrorEventHandler ConnectionError;
        protected virtual void OnConnectionError()
        {
            ConnectionError?.Invoke(this);
        }



        public event OrchestraStateChanged OrchestraStateChanged;
        protected virtual void OnOrchestraStateChanged(OrchestraState newState, OrchestraState oldState)
        {
            OrchestraStateChanged?.Invoke(this, newState, oldState);
        }



        public event InsturmentsChangedEventHandler InsturmentsChanged;
        protected virtual void OnInsturmentsChanged(InstrumentType[] instruments)
        {
            InsturmentsChanged?.Invoke(this, instruments);
        }
           


        #endregion
    }
}
