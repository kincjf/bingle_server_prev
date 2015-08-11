using Bingle.Server.Data;
using Bingle.Server.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Command
{
    /// <summary>
    /// Console 상에서 packet 내용을 Console.WriteLine을 이용하여 보여줌
    /// </summary>
    public class ConsoleLogger
    {     
        public static void GetProtocolLog(BingleProtocol protocol)
        {
            Console.WriteLine("Key : {0}", protocol.Key);
            Console.WriteLine("BodyLength : {0}", protocol.Header.BodyLength);
            Console.WriteLine("Fragmented : {0}", protocol.Header.Fragmented);
            Console.WriteLine("LastMsg : {0}", protocol.Header.LastMsg);
            Console.WriteLine("Sequence : {0}", protocol.Header.Sequence);

            if(protocol.Key.Equals(CommandList.STAT))
            {
                Console.WriteLine("ResponseCode : {0}", IPAddress.NetworkToHostOrder(BitConverter.ToInt32(protocol.Body, 0)));
            }
            else if (protocol.Key.Equals(CommandList.STOR))
            {
                Console.WriteLine("Length of BinaryData : {0}", protocol.Body.Length);
            }
            else if (protocol.Key.Equals(CommandList.TYPE))
            {
                Console.WriteLine("FileSize : {0}", IPAddress.NetworkToHostOrder(BitConverter.ToInt64(protocol.Body, 0)));
            }
            else
            {
                Console.WriteLine("Body is Empty");
            }

            Console.WriteLine("------------------");
        }
    }
}
