using System.Reflection;

namespace OnxAdmin.API.Agents.OnspringAdministrator;

static class ServiceCollectionExtensions
{
  public static IServiceCollection AddOnspringAdministratorAgent(this IServiceCollection services)
  {
    services.AddScoped<IOnspringAdministratorAgent, OnspringAdministratorAgent>();

    var toolInterface = typeof(IOnspringAdministratorTool);

    var types = Assembly.GetExecutingAssembly().DefinedTypes
      .Where(type => type.IsAbstract is false && type.ImplementedInterfaces.Contains(toolInterface));

    foreach (var type in types)
    {
      services.AddTransient(toolInterface, type);
    }

    return services;
  }
}