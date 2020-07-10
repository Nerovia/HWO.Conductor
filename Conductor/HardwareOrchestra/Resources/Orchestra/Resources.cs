using Conductor.App.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareOrchestra.Resources.Orchestra
{
    #region Enums



    /// <summary>
    /// Defines the states the orchestra can set to.
    /// </summary>
    public enum OrchestraState : byte
    {
        Rest = 0,
        Tune = 1,
        Perform = 2,
        Interrupted = 3,
    }



    /// <summary>
    /// Defines the different orchestra instruments and their display name.
    /// </summary>
    public enum InstrumentType : byte
    {
        
        [StringValue("Unidentified")]
        Unidentified = 0,
        [StringValue("Steppermotor")]
        Steppermotor = 1,
        [StringValue("Piezobuzzer")]
        Piezo = 2,
    }



    #endregion

    #region Interfaces



    /// <summary>
    /// Provides the functionallity to convert a object to a orchestra instruction.
    /// </summary>
    public interface IConductable
    {
        string ToInstruction();
    }



    #endregion

    #region Delegates



    /// <summary>
    /// Handles the action when the conductor requenst a tone.
    /// </summary>
    /// <returns></returns>
    public delegate Tone? QueueRequestHandler();


    public delegate void ConnectionChangedEventHandler(Conductor sender, bool isConnected);

    public delegate void ConnectionErrorEventHandler(Conductor sender);

    public delegate void OrchestraStateChanged(Conductor sender, OrchestraState orchestraState);

    public delegate void InsturmentsChangedEventHandler(Conductor sender, InstrumentType[] instruments);


    #endregion

    #region Static



    /// <summary>
    /// Defines the keywords for communication between conductor and concertmaster.
    /// </summary>
    public static class Command
    {
        public const char Separator = '$';

        public const char ChangeState = 'M';
        public const char QueueTone = 'B';
        public const char TuneTone = 'T';
        public const char ClearQueue = 'C';
        public const char Event = 'E';
        public const char InsturmentsChanged = 'I';
        public const char Handshake = 'H';
    }



    #endregion

    #region Structs



    /// <summary>
    /// Defines the properties of a orchestra tone-instruction.
    /// </summary>
    public struct Tone : IConductable
    {
        public byte note;
        public byte instrument;
        public int duration;

        public string ToInstruction()
        {
            return (
                note.ToString() + Command.Separator +
                instrument.ToString() + Command.Separator +
                duration.ToString());
        }
    }



    #endregion
}
