using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.MetaData
{
    public class StatusData
    {
        public const byte NOT_FRAGMENTED = 0x00;
        public const byte FRAGMENTED = 0x01;

        public const byte NOT_LASTMSG = 0x00;
        public const byte LASTMSG = 0x01;

        public const int DataConnectionAccepted_150 = 150;
        public const int RequestActionCompleted_200 = 200;
        public const int TransferCompleted_226 = 226;
    }
}
