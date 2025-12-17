using Ecomm.Core.Configurations;
using Ecomm.Core.Validators;
using Ecomm.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Infrastructure Services like DbContext, Identity, Repositories, etc.
builder.Services.AddInfrastructure(builder.Configuration);


// Add CORS policy 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
        //  policy.WithOrigins("https://frontend.example.com")
        //.AllowAnyHeader()
        //.AllowAnyMethod()
        //.AllowCredentials();

    });
});
//Bind TokenOptions for DI (REQUIRED)
builder.Services.Configure<TokenOptions>(
    builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
var tokenOptions = builder.Configuration.GetSection("Jwt").Get<TokenOptions>()
                   ?? throw new InvalidOperationException("Jwt section is missing in configuration.");

var keyBytes = Encoding.UTF8.GetBytes(tokenOptions.SecretKey);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true; // enforce HTTPS in production
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidIssuer = tokenOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = tokenOptions.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(tokenOptions.ClockSkewSeconds),
        RequireExpirationTime = true
    };

});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "EcommApi v1");
        options.RoutePrefix = string.Empty;
    });
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
