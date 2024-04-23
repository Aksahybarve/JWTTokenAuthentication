using Sitecore.Services.Infrastructure.Web.Http.Security;
using System.Net.Http;
using System.Net;
using SugSession.Services.Interface;
using System.Web.Http;
using SugSession.Model;
using SugSession.Utility;
using System.Web;
using SugSession.Repositories;
using SugSession.Repositories.Interface;
using Sitecore.Services.Infrastructure.Web.Http;

namespace SugSession
{
    public class UserAccountController : ServicesApiController
    {
        private IUserAccountService _userAccountService;
        private ITokenProvider _iTokenProvider;
        private IRepository _repository;

        public UserAccountController(IUserAccountService userAccountService, ITokenProvider tokenProvider, IRepository repository)
        {
            _userAccountService = userAccountService;
            _iTokenProvider = tokenProvider;
            _repository = repository;
        }

        [HttpPost]
        public IHttpActionResult UserLogin(AuthenticateRequest authenticateRequest)
        {
            HttpResponseMessage httpResponse;
            APIResponses apiResponse = _userAccountService.UserLogin(authenticateRequest, Request);

            httpResponse = Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            return ResponseMessage(httpResponse);
        }

        [HttpPost]
        public IHttpActionResult CreateArticles()
        {
            bool isValid = UserHelper.ValidateToken(_iTokenProvider, Request);
            HttpResponseMessage httpResponse;
            if (isValid)
            {
                UserClaims userClaim = UserHelper.GetUserClaims(_iTokenProvider, Request);
                var FormData = HttpContext.Current.Request;
                var articleID = FormData.Form["articleIDS"];
                APIResponses resp = _repository.SaveArticles(articleID, userClaim);
                httpResponse = Request.CreateResponse(HttpStatusCode.OK, resp);
                return ResponseMessage(httpResponse);
            }
            else
            {
                httpResponse = Request.CreateResponse(HttpStatusCode.OK, "Invalid Token");
                return ResponseMessage(httpResponse);
            }
        }

        [HttpGet]
        public IHttpActionResult GetArticlesList()
        {
            bool isValid = UserHelper.ValidateToken(_iTokenProvider, Request);
            HttpResponseMessage httpResponse;
            if (isValid)
            {
                UserClaims userClaim = UserHelper.GetUserClaims(_iTokenProvider, Request);
                var myArticles = _repository.GetArticles(userClaim);
                httpResponse = Request.CreateResponse(HttpStatusCode.OK, myArticles);
                return ResponseMessage(httpResponse);
            }
            else
            {
                httpResponse = Request.CreateResponse(HttpStatusCode.OK, "Invalid Token");
                return ResponseMessage(httpResponse);
            }
        }
    }
}