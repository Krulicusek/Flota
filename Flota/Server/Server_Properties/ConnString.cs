using Flota.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flota
{
    public static class ConnString
    {
        public static bool TryGetConnectionString(IConfiguration configuration, string dbSection, out string connString)
        {
            connString = null;

            IConfigurationSection serversSection = configuration.GetSection("Servers");
            IConfigurationSection s = serversSection.GetSection(dbSection);

            string host = s.GetValue<string>("Host");
            string db = s.GetValue<string>("Database");
            string usrnm = s.GetValue<string>("Username");
            string pwdEncrypted = s.GetValue<string>("Password");

            if (String.IsNullOrWhiteSpace(host)
                || String.IsNullOrWhiteSpace(db)
                || String.IsNullOrWhiteSpace(usrnm)
                || String.IsNullOrWhiteSpace(pwdEncrypted))
                return false;

            string pwd = Hashing.Decrypt(pwdEncrypted);

            connString = $"Host={host};Database={db};Username={usrnm};Password={pwd}";

            return true;
        }
    }
}
