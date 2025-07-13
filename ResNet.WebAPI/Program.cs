using System.Text;
using Infrastructure;
using Infrastructure.AutoMapper;
using Infrastructure.Data;
using Infrastructure.Data.Seeder;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ResNet.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

// 1️⃣ DbContext
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connection));

// 2️⃣ Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// 3️⃣ JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// 4️⃣ Services
builder.Services.AddInfrastructure();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(InfrastructureProfile));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen();

// ✅ 5️⃣ CORS: Разрешаем ВСЕМ
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// 6️⃣ Migration & Seeding
await using var scope = app.Services.CreateAsyncScope();
var services = scope.ServiceProvider;

var context = services.GetRequiredService<DataContext>();
await context.Database.MigrateAsync();

var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
await DefaultRoles.SeedAsync(roleManager);
await DefaultUser.SeedAsync(userManager);

// app.Services.RegisterRecurringJobs();

// 7️⃣ Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

// CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Настраиваем порт
app.Urls.Add("http://0.0.0.0:5001");

app.Run();
