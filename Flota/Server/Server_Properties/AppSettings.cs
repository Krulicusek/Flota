using Flota.Shared;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flota
{
    public class AppSettings
    {
        #region Members

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        IConfiguration Configuration;
        public List<string> TestUsers;
        public string TestUsersPassword;
        public List<string> AllowedUsers;
        public string SecurityLogin;
        
        #endregion

        #region Properties

        public bool IsTest { get; private set; }
        public ServerSettings ServerSettings { get; private set; }
        public LdapSettings LdapSettings { get; private set; }

        #endregion

        #region .Ctor

        public AppSettings(IConfiguration configuration)
        {
            try
            {
                Configuration = configuration;
                IsTest = configuration.GetValue<bool>("IsTest");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion

        #region Methods

        public void GetServerSettings()
        {
            try
            {
                IConfigurationSection serversSection = Configuration.GetSection("Servers");
                string serverMode = IsTest ? "FrontDeskTest" : "FrontDesk";
                IConfigurationSection serverDataSection = serversSection.GetSection(serverMode);
                string host = serverDataSection.GetValue<string>("Host");
                string db = serverDataSection.GetValue<string>("Database");
                string usrnm = serverDataSection.GetValue<string>("Username");
                string pwdEncrypted = serverDataSection.GetValue<string>("Password");

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(db) || string.IsNullOrEmpty(usrnm) || string.IsNullOrEmpty(pwdEncrypted))
                {
                    throw new Exception("Server data invalid");
                }

                string pwd = Hashing.Decrypt(pwdEncrypted);
                ServerSettings = new ServerSettings(serverMode, host, db, usrnm, pwd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return;
            }
        }

        public void GetTestUsers()
        {
            try
            {
                if (IsTest)
                {
                    string[] users = Configuration.GetSection("TestUsersData:TestUsers").Get<string[]>();
                    string password = Configuration.GetSection("TestUsersData:Password").Get<string>();

                    if (users == null || users.Length == 0 || string.IsNullOrEmpty(password))
                    {
                        throw new Exception("TestUsers data invalid");
                    }

                    TestUsers = users.ToList();
                    TestUsersPassword = password;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return;
            }
        }

        public void GetAllowedUsers()
        {
            try
            {
                if (!IsTest)
                {
                    string[] users = Configuration.GetSection("Allowed").Get<string[]>();
                    string securityLogin = Configuration.GetSection("SecurityLogin").Get<string>();

                    if (users == null || users.Length == 0 || string.IsNullOrEmpty(securityLogin))
                    {
                        throw new Exception("AllowedUsers data invalid");
                    }

                    AllowedUsers = users.ToList();
                    SecurityLogin = securityLogin;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return;
            }
        }

        public void GetLdapSettings()
        {
            try
            {
                if (!IsTest)
                {
                    string host = Configuration.GetSection("LdapSettings:Host").Get<string>();
                    int port = Configuration.GetSection("LdapSettings:Port").Get<int>();
                    string dn = Configuration.GetSection("LdapSettings:DN").Get<string>();

                    if (string.IsNullOrEmpty(host) || port == 0 || string.IsNullOrEmpty(dn))
                    {
                        throw new Exception("LDAP data invalid");
                    }

                    LdapSettings = new LdapSettings(host, port, dn);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return;
            }
        }

        #endregion
    }
}
