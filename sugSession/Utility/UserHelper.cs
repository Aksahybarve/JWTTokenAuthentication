using Sitecore.Security.Accounts;
using SugSession.Model;
using System;
using System.Web.Security;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Sitecore.Services.Infrastructure.Web.Http.Security;
using System.Collections.Generic;
using System.Linq;

namespace SugSession.Utility
{
    public class UserHelper
    {
        public static MembershipUser FindUserByEmailAndDomain(string emailToMatch, string sitecoreDomain)
        {
            var userCollection = Membership.FindUsersByEmail(emailToMatch);
            foreach (MembershipUser user in userCollection)
            {
                if (!user.Email.Equals(emailToMatch, StringComparison.OrdinalIgnoreCase)) continue;
                User sitecoreUser = User.FromName(user.UserName, false);
                var domain = sitecoreUser.Domain;
                if (domain == null) return null;
                var domainName = domain.Name;
                if (domainName == sitecoreDomain)
                    return user;
            }
            return null;
        }

        public static bool ValidateToken(ITokenProvider Provider, System.Net.Http.HttpRequestMessage Request)
        {
            var isValid = false;
            if (Request.Headers.Contains("token"))
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("token");
                var token = headerValues.FirstOrDefault();
                isValid = Provider.ValidateToken(token).IsValid;
            }
            return isValid;
        }

        public static UserClaims GetUserClaims(ITokenProvider Provider, System.Net.Http.HttpRequestMessage Request)
        {
            var UserClaims = new UserClaims();
            if (Request.Headers.Contains("token"))
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("token");
                var token = headerValues.FirstOrDefault();
                var claims = Provider.ValidateToken(token).Claims;

                foreach (var _claim in claims)
                {
                    if (_claim.Type == "userid")
                    {
                        UserClaims.UserId = _claim.Value;
                    }
                    if (_claim.Type == "useremail")
                    {
                        UserClaims.EmailId = _claim.Value;
                    }
                }

            }
            return UserClaims;
        }
    }
}
