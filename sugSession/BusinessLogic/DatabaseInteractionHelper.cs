using SugSession.BusinessLogic.Models;
using SugSession.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SugSession.BusinessLogic
{
    public class DatabaseInteractionHelper
    {
        /// <summary>
        /// Creates article record in DB
        /// </summary>
        /// <param name="article">Article object to create record in DB</param>
        /// <param name="user">Logged In User</param>
        public void CreateArticle(SavedArticles article, UserClaims user)
        {
            using (var context = new CDBContext())
            {
                var articleID = context.Articles.Where(x => x.ArticleId == article.ArticleId && x.CreatedBy == user.EmailId).Select(x => x.ID).FirstOrDefault();
                if (articleID == 0)
                {
                    context.Articles.Add(article);
                    context.SaveChanges(user);
                }
            }
        }

        /// <summary>
        /// Fetches article records from DB for a particular user
        /// </summary>
        /// <param name="userId">Logged In User ID</param>
        public List<SavedArticles> GetArticles(Guid userId)
        {
            var articles = new List<SavedArticles>();
            using (var context = new CDBContext())
            {
                articles = context.Articles.Where(x => x.LoggedInUserId == userId).ToList();
            }
            return articles;
        }
    }
}