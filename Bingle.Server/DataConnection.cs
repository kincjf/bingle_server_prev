using Bingle.Server.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server
{
    class DataConnection
    {
        private string _address = string.Empty;

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private int _port = 0;

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public Socket Client { get; private set; }

        private Socket _listener;

        public SslProtocols SecureProtocol { get; set; }

        private BingleSession _session;

        private bool _isClosed = false;

        private Stream _stream;

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Stream GetStream(BingleContext context)
        {
            if (_stream == null)
            {
                InitStream(context);
            }

            return _stream;
        }

        private void InitStream(BingleContext context)
        {
            switch (SecureProtocol)
            {
                case (SslProtocols.Tls):
                case (SslProtocols.Ssl3):
                case (SslProtocols.Ssl2):
                    SslStream sslStream = new SslStream(new NetworkStream(Client), false);
                    sslStream.AuthenticateAsServer(_session.AppServer.Certificate, false, SslProtocols.Default, true);
                    _stream = sslStream as Stream;
                    break;
                default:
                    _stream = new NetworkStream(Client);
                    break;
            }
        }

        public DataConnection(BingleSession session, Socket listenSocket, int port)
        {
            _session = session;
            _address = session.Config.Ip;
            SecureProtocol = session.Context.DataSecureProtocol;
            _listener = listenSocket;
            _port = port;
        }

        public Task RunDataConnection()
        {
            var taskSource = new TaskCompletionSource<bool>();
            SocketAsyncEventArgs acceptEventArgs = new SocketAsyncEventArgs();
            acceptEventArgs.UserToken = taskSource;
            acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(acceptEventArgs_Completed);

            /// @link https://msdn.microsoft.com/ko-kr/library/system.net.sockets.socket.acceptasync(v=vs.110).aspx
            /// - 설명 부분 참조
            /// 
            /// AcceptAsync 실행시 별도의 thread에서 작업을 수행한다고 한다.
            /// I/O 작업이 정확히 어떤 것을 말하는지는 잘 모르겠지만
            /// 보류(진행이 되지 않았음)중일 경우에는(true), EventHandler를 통하여 EventHandler를 통하여 ProcessAccept가 호출된다.
            /// 동기적으로 완료된 경우에는(true) 분기문을 통하여 ProcessAccept가 실행된다.
            /// msdn : I/O 작업이 보류 중인 경우 true를 반환합니다.
            /// 	작업이 완료되면 context 매개 변수에 대한 SocketAsyncEventArgs.Completed 이벤트가 발생합니다.
            if (!_listener.AcceptAsync(acceptEventArgs))
                ProcessAccept(acceptEventArgs);
            return taskSource.Task;
        }

        void acceptEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        void ProcessAccept(SocketAsyncEventArgs e)
        {
            this.Client = e.AcceptSocket;

            var taskSource = e.UserToken as TaskCompletionSource<bool>;

            try
            {
                InitStream(_session.Context);
                taskSource.SetResult(true);		// 작업 실행 완료
            }
            catch (Exception exc)
            {
                taskSource.SetException(exc);
            }

            /// @link https://msdn.microsoft.com/ko-kr/library/system.net.sockets.socket.acceptasync(v=vs.110).aspx
            /// - 설명 부분 참조
            /// 
            /// [Socket.AcceptAsync(e)] 이전에 [e.AcceptSocket = null]을 해주지 않았기 때문에
            /// 새로운 Socket이 만들어졌음.
            /// 그래서 기존(m_listener)리소스를 해제하기 위함임(정확한 설명은 아니며 추측임).
            /// msdn : SocketAsyncEventArgs.AcceptSocket 속성이 null인 경우,
            /// 	새 Socket은 현재 Socket과 동일한 AddressFamily, SocketType 및 ProtocolType을 갖도록 생성되고
            /// 	SocketAsyncEventArgs.AcceptSocket 속성으로 설정됩니다.
            StopListener();
        }

        void StopListener()
        {
            if (_listener != null)
            {
                try
                {
                    _listener.Close();
                }
                catch (Exception)
                {
                }
                finally
                {
                    _listener = null;
                }
            }
        }

        private static Socket TryListenSocketPort(IPAddress address, int tryPort)
        {
            Socket listener = null;

            try
            {
                IPEndPoint endPoint = new IPEndPoint(address, tryPort);
                listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(endPoint);
                listener.Listen(1);
                return listener;
            }
            catch (Exception)
            {
                if (listener != null)
                {
                    try
                    {
                        listener.Close();
                    }
                    catch (Exception)
                    {

                    }
                }

                return null;
            }
        }

        internal static bool TryOpenDataConnection(BingleSession session, int port, out DataConnection dataConnection)
        {
            IPAddress ipAddress = session.LocalEndPoint.Address;
            var listenSocket = TryListenSocketPort(ipAddress, port);

            if (listenSocket != null)
            {
                dataConnection = new DataConnection(session, listenSocket, port);
                return true;
            }

            dataConnection = null;
            return false;
        }

        /// 현재는 Passive mode는 지원하지 않음
        //internal static bool TryOpenDataConnection(BingleSession session, out DataConnection dataConnection)
        //{
        //    dataConnection = null;

        //    int tryPort = session.AppServer.BingleServiceProvider.GetRandomPort();
        //    int previousPort = tryPort;
        //    int tryTimes = 0;

        //    IPAddress ipAddress = session.LocalEndPoint.Address;

        //    while (true)
        //    {
        //        var listenSocket = TryListenSocketPort(ipAddress, tryPort);

        //        if (listenSocket != null)
        //        {
        //            dataConnection = new DataConnection(session, listenSocket, tryPort);
        //            return true;
        //        }

        //        tryTimes++;

        //        if (tryTimes > 5)
        //        {
        //            return false;
        //        }

        //        tryPort = session.AppServer.BingleServiceProvider.GetRandomPort();

        //        if (previousPort == tryPort)
        //        {
        //            return false;
        //        }
        //    }
        //}

        private const string DELIM = " ";
        private const string NEWLINE = "\r\n";

        private static readonly CultureInfo m_DateTimeCulture = CultureInfo.GetCultureInfo("en-US");

        private string GetListTimeString(DateTime time)
        {
            if (time.Year == DateTime.Now.Year)
                return time.ToString("MMM dd hh:mm", m_DateTimeCulture);
            else
                return time.ToString("MMM dd yyyy", m_DateTimeCulture);
        }

        private string GetFixedLength(long length)
        {
            string size = length.ToString();

            size = size.PadLeft(11, ' ');

            return size;
        }

        public virtual void Close()
        {
            StopListener();

            if (Client != null && !_isClosed)
            {
                try
                {
                    Client.Shutdown(SocketShutdown.Both);
                }
                catch (Exception e)
                {
                    _session.Logger.Error(e);
                }

                try
                {
                    Client.Close();
                }
                catch (Exception e)
                {
                    _session.Logger.Error(e);
                }
                finally
                {
                    Client = null;
                    _session = null;
                    _isClosed = true;
                    OnClose();
                }
            }
        }

        /// <summary>
        /// Called when [close].
        /// </summary>
        protected virtual void OnClose()
        {
            _isClosed = true;
        }
    }
}
