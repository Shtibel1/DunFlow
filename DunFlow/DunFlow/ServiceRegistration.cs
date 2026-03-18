using DunFlow.Application.Helpers;
using DunFlow.Application.Interfaces;
using DunFlow.Application.Services;
using DunFlow.Domain.Interfaces;
using DunFlow.Infra.Repositories;

namespace DunFlow
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services)
        {
            services.AddScoped<IWorkTaskRepository, WorkTaskRepository>();
            services.AddScoped<IWorkTaskService, WorkTaskService>();
            
            services.AddScoped<IWorkTaskMetadataRepository, WorkTaskMetadataRepository>();
            services.AddScoped<IWorkTaskMetadataService, WorkTaskMetadataService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

            return services;
        }
    }
}
