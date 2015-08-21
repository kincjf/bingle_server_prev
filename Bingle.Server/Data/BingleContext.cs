using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Data
{
    /// <summary>
    /// 접속자에 대한 설정 정보를 관리함
    /// (접속자별 리소스 저장 폴더 위치, 언어 타입 관리, 임시 저장 파일 정보)
    /// </summary>
    public class BingleContext
    {
        private SslProtocols _dataSecureProtocol = SslProtocols.None;

        public SslProtocols DataSecureProtocol
        {
            get { return _dataSecureProtocol; }
            set { _dataSecureProtocol = value; }
        }

        public string UserName { get; set; }

        public Encoding charset { get; set; }

        public string UserFilePath { get; set; }

        public string TempFileName { get; set; }

        public long TempFileSize { get; set; }

        /// <summary>
        /// method of setting fileSavePath
        /// "/root" - absolute path
        /// ".", "" - current executed path
        /// "image" - relative path
        /// </summary>
        public BingleContext(string serverRootPath, string userFilePath)
        {
            charset = Encoding.UTF8;
            UserFilePath = Path.Combine(serverRootPath, userFilePath);          /// relative path
            TempFileName = string.Empty;
            TempFileSize = 0;
        }
    }
}
