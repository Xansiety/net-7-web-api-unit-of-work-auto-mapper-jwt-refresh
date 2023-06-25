
using API.Extensions;
using API.Middlewares;
using AspNetCoreRateLimit;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//SeriLog -> Configuraci�n obtenida desde AppSettings
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

//agregamos SeriLog al sistema integrado de Loggin
//builder.Logging.ClearProviders(); //limpiar los proveedores por defecto
builder.Logging.AddSerilog(logger: logger);

builder.Services.ConfigureCors(); //desde mi extension
builder.Services.AddAplicationServices(); // Inyecci�n de dependencias
builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());
builder.Services.ConfigureRateLimiting();
builder.Services.ConfigureApiVersioning();
//JWT
builder.Services.AddJwt(configuration: builder.Configuration);


//configuraci�n para serializar respuestas en XML
//Por defecto resolver� en Application/json
builder.Services.AddControllers(opt => {
    opt.RespectBrowserAcceptHeader = true; //aceptar headers del browser
    opt.ReturnHttpNotAcceptable = true; //devolver un mensaje de error indicando que el formato solicitado no es aceptado que soporta el servidor
}).AddXmlDataContractSerializerFormatters();

//es importante que esto este despu�s de controladores para el manejo de validaciones de model state
builder.Services.AddValidationErrors();


builder.Services.AddDbContext<TiendaContext>(opt =>
{
    //var serverVersion = new MySqlServerVersion(new Version(8, 0, 30));
    //opt.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), serverVersion);

    //detect automaticamente la version del motor de base de datos
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//MiddleWare para manejo de excepciones de forma global
app.UseMiddleware<ExtendExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors/{0}");//lamamos a un controlador  //nos permite generar paginas de error personalizadas cuando ocurre un error 

app.UseIpRateLimiting();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//iniciar DB si existen migraciones pendientes
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
//    try
//    {
//        var context = services.GetRequiredService<TiendaContext>();
//        await context.Database.MigrateAsync();
//        await TiendaContextSeed.SeedAsync(context, loggerFactory);
//        await TiendaContextSeed.SeedRolesAsync(context, loggerFactory);
//    }
//    catch (Exception ex)
//    {
//        var _logger = loggerFactory.CreateLogger<Program>();
//        _logger.LogError(ex, "Ocurri� un error durante la migraci�n");
//    }
//}

app.UseCors("CorsPolicy"); //nombre de mi pol�tica

app.UseHttpsRedirection();

//siempre debe ir antes que authorization
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
