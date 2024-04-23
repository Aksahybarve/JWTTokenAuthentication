using SugSession.Model;
using System;
using Sitecore.Security.Accounts;
using System.Web.Security;
using SugSession.Utility;
using System.Security.Claims;
using Sitecore.Diagnostics;
using Sitecore.Services.Infrastructure.Security;
using Sitecore.Services.Infrastructure.Web.Http.Security;
using SugSession.Services.Interface;

namespace SugSession.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly ITokenProvider _tokenProvider;

        public UserAccountService()
        {
            _tokenProvider = new SigningTokenProvider();
        }

        public APILoginResponses UserLogin(AuthenticateRequest authenticateRequest, System.Net.Http.HttpRequestMessage Request)
        {
            APILoginResponses resp = new APILoginResponses() { Success = false };
            try
            {
                if (string.IsNullOrEmpty(authenticateRequest.Email) || string.IsNullOrEmpty(authenticateRequest.Password) || string.IsNullOrEmpty(authenticateRequest.Encrypted))
                {
                    resp.ErrorMessage = "Provide Credentails";
                    return resp;
                }

                MembershipUser memberShipUser = UserHelper.FindUserByEmailAndDomain(authenticateRequest.Email, "extranet");
                if (memberShipUser != null)
                {
                    var islocked = false;
                    if (memberShipUser != null)
                    {
                        islocked = memberShipUser.IsLockedOut;
                    }

                    string Password = UserHelper.EncryptDecryptPassword(authenticateRequest);

                    var result = Sitecore.Security.Authentication.AuthenticationManager.Login(memberShipUser.UserName, Password, true);

                    if (!result)
                    {
                        if (islocked)
                        {
                            resp.ErrorMessage = Sitecore.Globalization.Translate.Text("LOCK_ITEM");
                            resp.ErrorMessageField = "general";
                            return resp;
                        }
                        else
                        {
                            resp.ErrorMessage = Sitecore.Globalization.Translate.Text("PASSWORD_INVALID");
                            resp.ErrorMessageField = "password";
                            return resp;
                        }
                    }
                    else
                    {
                        resp = GenerateLoginToken(resp, memberShipUser.Email);
                        resp.Success = true;
                        return resp;
                    }
                }
                else
                {
                    resp.ErrorMessage = Sitecore.Globalization.Translate.Text("EMAIL_ADDRESS_NOT_EXIST");
                    resp.ErrorMessageField = "email";
                    return resp;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in SugSession.Services Login: " + ex, this);
                resp.Success = false;
                resp.InnerException = ex.Message.ToString();
                return resp;
            }
        }

        public APILoginResponses GenerateLoginToken(APILoginResponses resp, string email)
        {
            string token = string.Empty;
            MembershipUser memberShipUser = UserHelper.FindUserByEmailAndDomain(email, "extranet");
            if (memberShipUser != null)
            {
                token = this._tokenProvider.GenerateToken(new Claim[3]
                {
                                new Claim("user", memberShipUser.UserName),
                                new Claim("userid", memberShipUser.ProviderUserKey.ToString()),
                                new Claim("useremail", memberShipUser.Email)
                });
            }
            if (token != null)
            {
                resp.SuccessMessage = Sitecore.Globalization.Translate.Text("SUCCESSFUL_LOGIN");
                resp.UserToken = token;
            }
            return resp;
        }
    }
}