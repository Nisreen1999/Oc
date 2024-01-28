using Course.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Values;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ContextClass>();
builder.Services.AddControllers();
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//       .AddJwtBearer(options =>
//       {
//           options.Authority = "https://localhost:7059";
//           options.Audience = "https://localhost:7035";
//           //options.TokenValidationParameters = new TokenValidationParameters
//           //{
//           //  //  NameClaimType = "name",
//           //    RoleClaimType = "role"
//           //};
//       });
string key = "Je1YcVG+dmXh5VZWI5EgAVPbToMPlIsS48wDqDkgzQU='"; //this should be same which is used while creating token      
var issuer = "SecureApi";  //this should be same which is used while creating token  

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = issuer,
      ValidAudience = issuer,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
  };

  options.Events = new JwtBearerEvents
  {
      OnAuthenticationFailed = context =>
      {
          if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
          {
              context.Response.Headers.Add("Token-Expired", "true");
          }
          return Task.CompletedTask;
      }
  };
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

app.UseAuthentication(); // Add this line to enable JWT token authentication

app.UseAuthorization();

app.MapControllers();



app.Run();
