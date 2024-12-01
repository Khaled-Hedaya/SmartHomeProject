using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SmartHomeProject.Data;
using SmartHomeProject.Hubs;
using SmartHomeProject.Middleware;
using SmartHomeProject.Services;
using SmartHomeProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Smart Home API", 
        Version = "v1",
        Description = "API for Smart Home Device Management"
    });
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ItemStateValidator>();

builder.Services.AddSignalR();


// Add Logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
 //   logging.AddFile(builder.Configuration.GetSection("Logging:File"));
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Home API V1");
    c.RoutePrefix = "api-docs"; // This will serve the Swagger UI at /api-docs
    // Optional: Add basic authentication
    c.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
    {
        ["activated"] = true
    };
});


// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
    
//}
//else
//{
//    app.UseExceptionHandler("/Errors");
//    app.UseHsts();
//}


app.UseStaticFiles();
app.UseDefaultFiles();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseGlobalExceptionHandler(); // Add global exception handler
app.UseAuthorization();
app.UseRouting();
app.MapHub<SmartHomeHub>("/smarthomeHub");
app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();