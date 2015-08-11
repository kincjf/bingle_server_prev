using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Common
{
    public static class StringUtil
    {
        public static string ReverseSlash(string message, char slash)
        {
            if (string.IsNullOrEmpty(message))
                return string.Empty;

            if (slash == '\\')
            {
                return message.Replace('\\', '/');
            }
            else if (slash == '/')
            {
                return message.Replace('/', '\\');
            }
            else
                return message;
        }

        public static string GetParentDirectory(string dir, char seperator)
        {
            if (seperator != '\\')
                seperator = '/';

            if (string.IsNullOrEmpty(dir))
                return string.Empty;

            if (dir[dir.Length - 1] == seperator)
                dir = dir.Substring(0, dir.Length - 1);

            int pos = dir.LastIndexOf(seperator);

            if (pos < 0)
                return string.Empty;
            else
                return dir.Substring(0, pos);
        }
    }
}
