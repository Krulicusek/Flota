using AutoMapper.Configuration;
using Flota.Shared;
using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flota
{
    public class Ldap
    {
        private static ILdapConnection _conn;

        public static ILdapConnection GetConnection(LdapSettings settings, string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(login) || !ValidateLdapSettings(settings))
            {
                return null;
            }

            LdapConnection ldapConn = _conn as LdapConnection;

            if (ldapConn == null)
            {
                // Creating an LdapConnection instance 
                ldapConn = new LdapConnection();
                try
                {
                    ldapConn.Connect(settings.Host, settings.Port);
                    ldapConn.Bind(settings.DN + login, password);
                }
                catch (LdapException)
                {
                    return null;
                }
            }

            return ldapConn;
        }

        private static bool ValidateLdapSettings(LdapSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Host) || settings.Port == 0 || string.IsNullOrEmpty(settings.DN))
            {
                return false;
            }

            return true;
        }
    }
}
