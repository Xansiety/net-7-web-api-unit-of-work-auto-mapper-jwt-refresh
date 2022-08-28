
using API.Extensions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureCors(); //desde mi extension
builder.Services.AddAplicationServices(); // Inyección de dependencias
builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());


builder.Services.AddControllers();

builder.Services.AddDbContext<TiendaContext>(opt =>
{
    var serverVersion = new MySqlServerVersion(new Version(8, 0, 30));
    opt.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), serverVersion);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
//    }
//    catch (Exception ex)
//    {
//        var logger = loggerFactory.CreateLogger<Program>();
//        logger.LogError(ex, "Ocurrió un error durante la migración");
//    }
//}

app.UseCors("CorsPolicy"); //nombre de mi política

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
