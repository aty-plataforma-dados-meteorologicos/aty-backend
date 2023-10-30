using Microsoft.Extensions.Configuration;
using AtyBackend.Infrastructure.Data.Identity;
using AtyBackend.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5121); // Ouvir em qualquer IP na porta 5121
});
var MyAllowSpecificOrigins = "https://localhost:5121";

// Add services to the container.
builder.Services.AddInfrastructureAPI(builder.Configuration);
//ativar autenticacao e validar o token
builder.Services.AddInfrastructureJWT(builder.Configuration);
builder.Services.AddInfrastructureSwagger();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHttpsRedirection(options =>
//{
//    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
//    options.HttpsPort = 443;
//});

// builder to use infra ioc seetroles

//CORS changes
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:5121")
                          .AllowAnyOrigin()
                          .AllowAnyHeader()
                          .WithMethods("GET", "POST", "PUT", "DELETE");
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
}

app.UseHttpsRedirection();

app.UseStatusCodePages();
app.UseRouting();
app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
