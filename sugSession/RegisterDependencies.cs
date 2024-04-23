using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using SugSession.Repositories;
using SugSession.Repositories.Interface;
using SugSession.Services;
using SugSession.Services.Interface;

namespace SugSession
{
    public class RegisterDependencies : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(UserAccountController));
            serviceCollection.AddTransient<IUserAccountService, UserAccountService>();
            serviceCollection.AddTransient<IRepository, Repository>();
        }
    }
}