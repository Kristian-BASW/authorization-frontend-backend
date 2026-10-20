using System.Text;
using DataAccess;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Security;
using Security.Services;
using Security.Services.Authorization;
using Security.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureInfrastructureServices(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db");
builder.Services.ConfigureDomainServices(builder.Configuration["JwtSecret"]!);
builder.Services.AddControllers();



builder.Services.AddCors(s => s
    .AddPolicy("CorsPolicy", t => t.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()));



var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapControllers();

app.Run();
