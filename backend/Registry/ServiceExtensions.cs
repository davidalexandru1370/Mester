using FluentValidation;
using Registry.Errors.Repositories;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Repository;
using Registry.Services;
using Registry.Services.Interfaces;
using Registry.Validator;
using System.Net;

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
            return services.AddScoped<IUserService>(x =>
                {
                    return ActivatorUtilities.CreateInstance<UserService>(x,
                        config["JwtSettings:Key"] ?? throw new Exception("No key specified"),
                        TimeSpan.FromHours(1));
                })
                .AddScoped<ITradesManService, TradesManService>()
                .AddScoped<IImageService, ImageService>()
                .AddScoped<IDataSeedingService, DataSeedingService>();
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            return services.AddSingleton<IValidator<User>, UserValidator>();
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
                catch (NotFoundException ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = ex.Message
                    });
                }
                catch (UnauthorizedException ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = ex.Message
                    });

                }
                catch (Exception ex) when (ex is ServiceException || ex is ApplicationException)
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
