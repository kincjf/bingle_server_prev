using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Data
{
    public class ServerContext
    {
        public Encoding charset { get; set; }

        public string FileRootPath { get; set; }

        public string TempFileDirectory { get; set; }

        public string ImageFileDirectory { get; set; }

        /// <summary>
        /// method of setting fileSavePath
        /// "/root" - absolute path
        /// ".", "" - current executed path
        /// "image" - relative path
        /// </summary>
        public ServerContext()
        {
            charset = Encoding.UTF8;
            FileRootPath = String.Empty;          /// relative path
            TempFileDirectory = "Temp";                /// relative path
            ImageFileDirectory = "Image";              /// relative path
        }
    }
}
