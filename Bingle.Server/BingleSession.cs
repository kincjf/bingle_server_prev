using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using Bingle.Server.Data;

namespace Bingle.Server
{
    public class BingleSession : AppSession<BingleSession, BingleProtocol>
    {
        public string userId { internal set; get; }

        public FileContext FileContext { get; private set; }
        
        public new BingleServer AppServer
        {
            get { return (BingleServer)base.AppServer; }
        }

        public override void Close()
        {
            base.Close();
        }

        protected override void OnInit()
        {
            FileContext = new FileContext();
            base.OnInit();
        }

        protected override void OnSessionStarted()
        {
            Console.WriteLine("Connection Successful");
        }

        protected override void HandleUnknownRequest(BingleProtocol requestInfo)
        {
            Console.WriteLine("Unknown request");
        }

        protected override void HandleException(Exception e)
        {
            Console.WriteLine("Application error: {0}", e.Message);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            Console.WriteLine("disconnected - {0}", reason);
            base.OnSessionClosed(reason);
        }
    }
}
