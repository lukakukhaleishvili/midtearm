using Microsoft.EntityFrameworkCore;
using Reddit;
using Reddit.Mapper;
using System.Text.Json.Serialization;
using Reddit.Middlewares;
using Reddit.Filters;
using Microsoft.Extensions.Options;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(option => option.Filters.Add<ModelValidationActionFilter>());

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; 
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplcationDBContext>(options => {
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteDb"));
    options.UseLazyLoadingProxies();
    options.LogTo(Console.WriteLine, LogLevel.Information);
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
               builder => builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader());
});
builder.Services.AddSingleton<IMapper, Mapper>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Type", "application/json");
    await next();
});


app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapGet("/throws", (context) => throw new Exception("Unexpected error occured on the server."));


app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
