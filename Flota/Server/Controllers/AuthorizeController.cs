using Flota.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flota.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthorizeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            User currentUser = new User();

            if (User.Identity.IsAuthenticated)
            {
                currentUser.Username = User.FindFirstValue(ClaimTypes.Name);
            }

            return await Task.FromResult(currentUser);
        }

        [HttpGet("Logout")]
        public async Task<ActionResult<String>> LogOutUser()
        {
            await HttpContext.SignOutAsync();
            return "Success";
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Validate([FromBody] UsrPwd usrPwd)
        {
            AppSettings appSettings = new AppSettings(_configuration);
            if (appSettings == null)
            {
                throw new Exception("AppSettings null");
            }

            bool? isTest = appSettings.IsTest;

            if (!isTest.HasValue)
            {
                return null;
            }

            if (isTest == true)
            {
                List<string> testUsers = appSettings.TestUsers;

                if (testUsers == null || testUsers.Count == 0)
                {
                    appSettings.GetTestUsers();
                    testUsers = appSettings.TestUsers;
                }

                if (testUsers.Contains(usrPwd.Username) && appSettings.TestUsersPassword == usrPwd.Pwd)
                {
                    string role = string.Empty;

                    if (usrPwd.Username == "frontDeskUser")
                    {
                        role = "Rec";
                    }
                    else if (usrPwd.Username == "securityUser")
                    {
                        role = "Sec";
                    }
                    else if (usrPwd.Username == "officeArticlesUser")
                    {
                        role = "Office";
                    }
                    else
                    {
                        return null;
                    }

                    User user = new User
                    {
                        Username = usrPwd.Username,
                        Role = role,
                    };
                    var claim = new Claim(ClaimTypes.Name, usrPwd.Username);
                    var claimsIdentity = new ClaimsIdentity(new[] { claim });
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return await Task.FromResult(user);
                }

                return null;
            }
            else
            {
                List<string> allowedUsers = appSettings.AllowedUsers;

                if (allowedUsers == null || allowedUsers.Count == 0)
                {
                    appSettings.GetAllowedUsers();
                    allowedUsers = appSettings.AllowedUsers;
                }

                LdapSettings ldapSettings = appSettings.LdapSettings;

                if (ldapSettings == null)
                {
                    appSettings.GetLdapSettings();
                    ldapSettings = appSettings.LdapSettings;
                }

                Novell.Directory.Ldap.ILdapConnection ldapConnection = Ldap.GetConnection(ldapSettings, usrPwd.Username, usrPwd.Pwd);

                if (ldapConnection != null && allowedUsers.Contains(usrPwd.Username))
                {
                    User user =  new User
                    {
                        Username = usrPwd.Username,
                        Role = appSettings.SecurityLogin == usrPwd.Username ? "Sec" : "Office"
                    };
                    var claim = new Claim(ClaimTypes.Name, usrPwd.Username);
                    var claimsIdentity = new ClaimsIdentity(new [] { claim }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return await Task.FromResult(user);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

