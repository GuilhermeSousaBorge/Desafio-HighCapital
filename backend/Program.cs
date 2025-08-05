using backend.Data;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", // Para desenvolvimento local
                "http://localhost:3000", // Caso use porta diferente
                "http://frontend:80",
                "http://localhost:8080",  // ← Adicione esta
                "https://localhost:8080", // ← E esta se usar HTTPS
                "http://127.0.0.1:8080"// Para comunicação entre containers Docker
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Importante para cookies/auth se usar
    });
});


builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
var useMock = builder.Configuration.GetValue<bool>("OpenAI:UseMock");

if (useMock)
    builder.Services.AddScoped<IOpenAiService, MockOpenAiService>();
else
    builder.Services.AddScoped<IOpenAiService, OpenAiService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    context.Database.EnsureCreated(); // Cria o banco se não existir
    // Ou use: context.Database.Migrate(); // se tiver migrations
}

app.UseCors("AllowFrontend");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatGPT API V1");
        c.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection(); // s� fora do container
}



app.UseAuthorization();

app.MapControllers();

app.Urls.Add("http://+:8080");

app.Run();
