using SugSession.BusinessLogic.Models;
using SugSession.Model;
using System.Collections.Generic;

namespace SugSession.Repositories.Interface
{
    public interface IRepository
    {
        APIResponses SaveArticles(string articleIDs, UserClaims claims);

        List<SavedArticles> GetArticles(UserClaims claims);
    }
}