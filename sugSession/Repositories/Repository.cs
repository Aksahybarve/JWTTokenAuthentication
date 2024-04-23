using Sitecore.Diagnostics;
using SugSession.BusinessLogic;
using SugSession.BusinessLogic.Models;
using SugSession.Model;
using SugSession.Repositories.Interface;
using System;
using System.Collections.Generic;

namespace SugSession.Repositories
{
    public class Repository : IRepository
    {
        readonly DatabaseInteractionHelper TBhelper = new DatabaseInteractionHelper();

        public APIResponses SaveArticles(string articleIDs, UserClaims claims)
        {
            APIResponses resp = new APIResponses() { Success = false, ErrorMessage = "" };
            try
            {
                string[] articleIdlist = { "" };
                if (!string.IsNullOrEmpty(articleIDs))
                {
                    articleIdlist = articleIDs.Split(',');
                }
                foreach (string article in articleIdlist)
                {
                    SavedArticles articles = new SavedArticles
                    {
                        ArticleId = article,
                        LoggedInUserId = Guid.Parse(claims.UserId)
                    };
                    TBhelper.CreateArticle(articles, claims);
                }
                resp.Success = true;
                resp.SuccessMessage = Sitecore.Globalization.Translate.Text("ARTICLE_CREATED");
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = "Something Went Wrong";
                resp.Success = false;
                Log.Error("Error in SugSession.Repositories.Repository.SaveArticles", ex.InnerException, this);
            }
            return resp;
        }

        public List<SavedArticles> GetArticles(UserClaims claims)
        {
            var myArticlesList = new List<SavedArticles>();
            try
            {
                if (claims != null)
                {
                    var userID = new Guid(claims.UserId);
                    myArticlesList = TBhelper.GetArticles(userID);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in SugSession.Repositories.Repository.GetArticles", ex.InnerException, this);
            }
            return myArticlesList;
        }
    }
}