using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Data
{
    public interface IBingleProtocol
    {
        byte[] GetBytes();
        int GetSize();
    }
}
