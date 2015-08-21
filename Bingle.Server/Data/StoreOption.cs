using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Data
{
    public class StoreOption
    {
        private bool _appendOriginalFile = true;

        /// <summary>
        /// Gets or sets a value indicating whether [append original file].
        /// </summary>
        /// <value><c>true</c> if [append original file]; otherwise, <c>false</c>.</value>
        public bool AppendOriginalFile
        {
            get { return _appendOriginalFile; }
            set { _appendOriginalFile = value; }
        }

        private long _totalRead = 0;

        /// <summary>
        /// Gets or sets the total read.
        /// </summary>
        /// <value>The total read.</value>
        public long TotalRead
        {
            get { return _totalRead; }
            set { _totalRead = value; }
        }

        public StoreOption(long totalRead)
        {
            TotalRead = totalRead;
        }
    }
}
