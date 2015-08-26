using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.MetaData
{
    /// <summary>
    /// Header정보, 통신 결과에 대한 정보
    /// </summary>
    public class StatusData
    {
        public const byte NOT_FRAGMENTED = 0x00;
        public const byte FRAGMENTED = 0x01;

        public const byte NOT_LASTMSG = 0x00;
        public const byte LASTMSG = 0x01;

        public const int DataConnectionAccepted_150 = 150;

        public const int RequestActionCompleted_200 = 200;
        public const int TransferCompleted_226 = 226;

        public const int DataConnectionCannotOpen_420 = 420;
        public const int DataConnectionError_426 = 426;

        public const int InputFileError_551 = 551;
        public const int OutputFileError_551 = 551;
        public const int PortInvalid_552 = 552;
    }
}
