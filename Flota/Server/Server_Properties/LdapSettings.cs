using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flota
{
    public class LdapSettings
    {
        #region Properties

        public string Host { get; set; }
        public int Port { get; set; }
        public string DN { get; set; }

        #endregion

        #region .Ctor

        public LdapSettings(string host, int port, string dn)
        {
            Host = host;
            Port = port;
            DN = dn;
        }

        #endregion
    }
}
