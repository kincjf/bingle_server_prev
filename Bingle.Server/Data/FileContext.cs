using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Data
{
    public class FileContext
    {
        public long FileSize { get; set; }
        public long TotalReadSize { get; set; }
        public FileStream FileDataStream { get; set; }
        public string FilePath { get; set; }

        public FileContext()
        {
            FileSize = 0;
            TotalReadSize = 0;
            FileDataStream = null;
            FilePath = String.Empty;
        }
    }
}
