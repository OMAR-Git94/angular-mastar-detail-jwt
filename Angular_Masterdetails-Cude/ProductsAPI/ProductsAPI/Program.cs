using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProductsAPI.Data;
using ProductsAPI.Models;
using System.Text;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb")));
builder.Services.AddDbContext<ProductsAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductsAPIContext") ?? throw new InvalidOperationException("Connection string 'ProductsAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region CORS
builder.Services.AddCors(setUp =>
{
    setUp.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
#endregion
#region Auth Config
builder.Services.AddIdentity<IdentityUser, IdentityRole>(op =>
{
    op.Password.RequireDigit = false;
    op.Password.RequiredLength = 6;
    op.Password.RequireNonAlphanumeric = false;
    op.Password.RequireUppercase = false;
    op.Password.RequireLowercase = false;

})
   .AddEntityFrameworkStores<AppDbContext>()
   .AddDefaultTokenProviders();



#endregion
#region JWT Config
builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
    .AddJwtBearer(op =>
    {
        op.SaveToken = true;
        op.RequireHttpsMetadata = false;
        op.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidAudience = builder.Configuration["Jwt:Site"],
            ValidIssuer = builder.Configuration["Jwt:Site"],

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"] ?? ""))
        };
    });
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("CorsPolicy");
app.MapControllers();

app.Run();
