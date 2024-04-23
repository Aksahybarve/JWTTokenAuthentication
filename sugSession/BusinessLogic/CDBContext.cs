using System;
using System.Data.Entity;
using SugSession.BusinessLogic.Models;
using SugSession.Model;

namespace SugSession.BusinessLogic
{
    public class CDBContext : DbContext
    {
        public CDBContext() : base("name=CDBContext")
        {

        }

        public DbSet<SavedArticles> Articles { get; set; }

        public int SaveChanges(UserClaims user)
        {
            var now = DateTime.UtcNow;

            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is SavedArticles entity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.CreatedDate = now;
                            entity.UpdatedDate = now;
                            entity.CreatedBy = user.EmailId;
                            entity.UpdatedBy = user.EmailId;
                            break;
                        case EntityState.Modified:
                            Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                            Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                            entity.UpdatedDate = now;
                            entity.UpdatedBy = user.EmailId;
                            break;
                    }
                }
            }
            return base.SaveChanges();
        }
    }
}