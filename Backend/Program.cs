using Microsoft.EntityFrameworkCore;
using Bemanning_System.Backend.Data; // Namespace för StaffingContext
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Lägg till DbContext och koppla till connection string
builder.Services.AddDbContext<StaffingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WorkScheduleDB")));

// Lägg till stöd för controllers
builder.Services.AddControllers();

// ✅ Lägg till CORS — NOTERA HTTPS på frontend-URL!
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5031") // frontend (https!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger/OpenAPI för API-dokumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Bemanning API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Aktivera CORS (måste ligga före Authorization)
app.UseCors();

app.UseAuthorization();

// Koppla controller-rutter
app.MapControllers();

app.Run();
