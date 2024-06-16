using System.Text;
using APBD11Login.Context;
using APBD11Login.Middlewares;
using APBD11Login.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<DbaseContext>(
    options => options.UseSqlServer("Server=localhost,1433;Database=master;User Id=sa;Password=bazaTestowa1234;Trust server certificate= true"));
builder.Services.AddScoped<IMedicamentsService, MedicamentsService>();

var secretKey = builder.Configuration["JwtSettings:SecretKey"];
var validIssuer = builder.Configuration["JwtSettings:Issuer"];
var validAudience = builder.Configuration["JwtSettings:Audience"];

Console.WriteLine(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,  
        ValidateAudience = true, 
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2),
        ValidIssuer = validIssuer, 
        ValidAudience = validAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    opt.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-expired", "true");
            }
            return Task.CompletedTask;
        }
    };
}).AddJwtBearer("IgnoreTokenExpirationScheme",opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,  
        ValidateAudience = true,
        ValidateLifetime = false,
        ClockSkew = TimeSpan.FromMinutes(2),
        ValidIssuer = validIssuer, 
        ValidAudience = validAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<ErrorHandlerMiddleWare>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

