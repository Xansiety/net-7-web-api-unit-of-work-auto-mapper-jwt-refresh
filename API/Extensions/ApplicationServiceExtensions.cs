using AspNetCoreRateLimit;
using Core.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace API.Extensions;
public static class ApplicationServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            builder.AllowAnyOrigin() //WithOrigins("https://dominio.com")
            .AllowAnyMethod() // WithMethods("Get,Post")
            .AllowAnyHeader()); //WithHeaders("accept", "content-type")
        });


    // Implementación de dependencias
    public static void AddAplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //generic
        //services.AddScoped<IProductoRepository, ProductoRepository>();
        //services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        //services.AddScoped<IMarcaRepository, MarcaRepository>();
        services.AddScoped<IUnityOfWork, UnitOfWork>(); // Unidad de trabajo
    }


    // Rate Limit
    //Establecer un limite de peticiones 
    // https://github.com/stefanprodan/AspNetCoreRateLimit#readme
    public static void ConfigureRateLimiting(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddInMemoryRateLimiting();

        //configuración
        services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = true;
            options.HttpStatusCode = 429; // cuando se sobrepase el limite de peticiones
            options.RealIpHeader = "X-Real-IP";  //encabezado que leeremos
            options.GeneralRules = new List<RateLimitRule>
            {
                new RateLimitRule {
                    Endpoint = "*", // a todos los endpoints
                    Period = "10s", // por un periodo de cada 10 segundos
                    Limit = 2 //un limite de 2 peticiones en ese intervalo
                }
            };
        });
    }

    public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0); //opción de version por default
            opt.AssumeDefaultVersionWhenUnspecified = true; // usar version por defecto
            //opt.ApiVersionReader = new QueryStringApiVersionReader("ver"); //establecer version mediante queryString
            //opt.ApiVersionReader = new HeaderApiVersionReader("X-Version"); // establecer version mediante Header
            opt.ApiVersionReader = ApiVersionReader.Combine( //Combinar versionamiento queryString o Header lo definirá el cliente
                new QueryStringApiVersionReader("ver"),
                 new HeaderApiVersionReader("X-Version")
            );
            opt.ReportApiVersions = true;
        });
    }

}

