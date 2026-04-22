using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.ServiceCategories.Queries.GetCategories;
using ServiceMatch.Domain.Interfaces;
using ServiceMatch.Infrastructure.BackgroundServices;
using ServiceMatch.Infrastructure.Options;
using ServiceMatch.Infrastructure.Persistence;
using ServiceMatch.Infrastructure.Persistence.Repositories;
using ServiceMatch.Infrastructure.Services;

namespace ServiceMatch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IServiceProviderRepository, ServiceProviderRepository>();
        services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();

        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        var emailProvider = config["Email:Provider"] ?? "logging";
        if (emailProvider == "acs")
        {
            services.Configure<AcsOptions>(opts => config.GetSection("Email:Acs").Bind(opts));
            services.AddScoped<IEmailService, AcsEmailService>();
        }
        else
        {
            services.AddScoped<IEmailService, LoggingEmailService>();
        }

        services.Configure<AnthropicOptions>(opts => config.GetSection("Anthropic").Bind(opts));
        services.AddHttpClient<IAiChatService, AnthropicChatService>();

        services.AddMemoryCache();
        services.AddHttpClient("anthropic");
        services.AddHostedService<OffersFeedAgentService>();

        return services;
    }
}
