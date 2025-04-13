using FluentValidation;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Repository;
using Registry.Services;
using Registry.Validator;

namespace Registry
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
        {
            var connectionStrings = config.GetConnectionString("DefaultConnection") ?? throw new Exception();
            return services
                .AddSingleton(new ConnectionString(connectionStrings))
                .AddDbContext<TradesManDbContext>()
                .AddScoped<IRepositoryUser, RepositoryUser>();
        }

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            return services.AddScoped(x =>
            {
                return ActivatorUtilities.CreateInstance<AuthenticationService>(x,
                    config["JwtSettings:Key"] ?? throw new Exception("No key specified"),
                    TimeSpan.FromHours(1));
            });
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            return services.AddSingleton<IValidator<Client>, ClientValidator>();
        }

        public static IApplicationBuilder UseServiceExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                try
                {
                    await next(context);
                }
                catch (NameAlreadyUsedException ex)
                {
                    // CONFLICT STATUS CODE
                    context.Response.StatusCode = 409;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = ex.Message
                    });
                }
                catch (ServiceException ex)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = ex.Message
                    });
                }
            });
        }
    }
}
