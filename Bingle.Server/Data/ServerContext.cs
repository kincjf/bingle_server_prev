using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Data
{
    public class ServerContext
    {
        public string RootPath { get; set; }

        public string TempFileDirectory { get; set; }

        public string ImageFileDirectory { get; set; }

        public int DataPort { get; set; }

        /// <summary>
        /// method of setting fileSavePath
        /// "/root" - absolute path
        /// ".", "" - current executed path
        /// "image" - relative path
        /// </summary>
        public ServerContext(string fileRootPath, int dataPort)
        {
            RootPath = String.Empty;          /// relative path
            TempFileDirectory = "Temp";                /// relative path
            ImageFileDirectory = "Image";              /// relative path
            DataPort = dataPort;
        }
    }
}
