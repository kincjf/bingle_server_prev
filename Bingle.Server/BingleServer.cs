using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.Common;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine;
using Bingle.Server.Data;
using Bingle.Server.Filter;
using System.IO;

namespace Bingle.Server
{
	public class BingleServer : AppServer<BingleSession, BingleProtocol>
	{
        public ServerContext ServerContext { get; private set; }

        public BingleServiceProvider ServiceProvider { get; private set; }

        public BingleServer()
            : base(new DefaultReceiveFilterFactory<FileReceiveFilter, BingleProtocol>())
        {

        }
        
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            ServerContext = new ServerContext();
            ServiceProvider = new BingleServiceProvider();

            ServerContext.FileRootPath = config.Options.GetValue("fileRootPath");

            if (string.IsNullOrEmpty(ServerContext.FileRootPath))
            {
                Console.WriteLine("File Save Path - \"{0}\"", ServerContext.FileRootPath);
                Logger.Info("File Save Path - \"" + ServerContext.FileRootPath + "\"");
            }
            else if (ServerContext.FileRootPath.StartsWith("."))
            {
                ServerContext.FileRootPath = ServerContext.FileRootPath.TrimStart('.');
            }

            string tempDirectory = ServiceProvider.GetStoragePath(
                ServerContext, ServerContext.TempFileDirectory);
            string imageDirectory = ServiceProvider.GetStoragePath(
                ServerContext, ServerContext.ImageFileDirectory);

            if (Directory.Exists(tempDirectory) == false)
            {
                Directory.CreateDirectory(tempDirectory);
            }

            if(Directory.Exists(imageDirectory) == false)
            {
                Directory.CreateDirectory(imageDirectory);
            }

            return base.Setup(rootConfig, config);
        }

        protected override void OnStarted()
        {
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }
	}
}
