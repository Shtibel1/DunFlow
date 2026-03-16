using DunFlow.Application.Interfaces;
using DunFlow.Application.Services;
using DunFlow.Application.Strategies;
using DunFlow.Application;
using DunFlow.Domain.Interfaces;
using DunFlow.Infra.Repositories;
using DunFlow.Application.Helpers;

namespace DunFlow
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services)
        {
            services.AddScoped<IWorkTaskRepository, WorkTaskRepository>();
            services.AddScoped<IWorkTaskService, WorkTaskService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<ITaskTypeStrategy, ProcurementStrategy>();
            services.AddScoped<ITaskTypeStrategy, DevelopmentStrategy>();

            services.AddScoped<ITaskStrategyResolver, TaskStrategyResolver>();

            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

            return services;
        }
    }
}
