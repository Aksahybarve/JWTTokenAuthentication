using SugSession.Model;

namespace SugSession.Services.Interface
{
    public interface IUserAccountService
    {
        /// <summary>
        /// Validate User's Credentials in Sitecore database and send response with the JWT Token
        /// </summary>
        /// <param name="AuthenticateRequest">AuthenticateRequest class object holding different attributes required for login</param>
        /// <returns>APIResponse</returns>
        APILoginResponses UserLogin(AuthenticateRequest authenticateRequest, System.Net.Http.HttpRequestMessage Request);
    }
}