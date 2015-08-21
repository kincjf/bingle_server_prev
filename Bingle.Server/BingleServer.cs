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
            string fileRootPathNode = config.Options.GetValue("fileRootPath", string.Empty);

            string dataPortNode = config.Options.GetValue("dataPort");

            if (string.IsNullOrEmpty(dataPortNode))
            {
                Logger.Error("Parameter 'dataPort' is required!");
                return false;
            }

            if (fileRootPathNode.StartsWith("."))
            {
                fileRootPathNode = fileRootPathNode.TrimStart('.');
            }
            
            int dataPort = Convert.ToInt32(dataPortNode);

            ServerContext = new ServerContext(fileRootPathNode, dataPort);
            ServiceProvider = new BingleServiceProvider();

            Console.WriteLine("File Save Path - \"{0}\"", ServerContext.RootPath);
            Logger.Info("File Save Path - \"" + ServerContext.RootPath + "\"");
            
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
