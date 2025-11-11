using System.Text;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TP2_Programming_IV.Repositories;
using TP2_Programming_IV.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- EF Core ----------
var connString =
    builder.Configuration.GetConnectionString("Default")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(connString));

// ---------- AutoMapper ----------
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ---------- Authentication (Cookies + JWT) ----------
var jwtKey =
    builder.Configuration["Secrets:JWT"]
    ?? builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT secret not configured (Secrets:JWT or Jwt:Key).");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
    {
        opt.LoginPath = "/api/auth/login";
        opt.LogoutPath = "/api/auth/logout";
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan = TimeSpan.FromDays(1);
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ---------- DI: Repositories & Services ----------
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoleRepository>();

builder.Services.AddScoped<CourseServices>();
builder.Services.AddScoped<AuthServices>();
builder.Services.AddScoped<AdminServices>();

// Make sure AuthServices can receive IEncoderServices
builder.Services.AddSingleton<IEncoderServices, EncoderServices>();

// ---------- MVC + Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------- Middleware pipeline ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ---------- Seed inicial ----------
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await ctx.Database.MigrateAsync();

    // === Ensure Roles ===
    var neededRoles = new[] { "Admin", "User" };
    foreach (var r in neededRoles)
    {
        if (!await ctx.Roles.AnyAsync(x => x.Name == r))
            ctx.Roles.Add(new Domain.Entities.Role { Name = r });
    }
    await ctx.SaveChangesAsync();

    var adminRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
    var userRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "User");
    if (adminRole == null || userRole == null)
        throw new InvalidOperationException("No se pudieron asegurar los roles Admin/User.");

    // === Ensure Users ===
    if (!await ctx.Users.AnyAsync())
    {
        var enc = scope.ServiceProvider.GetRequiredService<IEncoderServices>();

        var admin = new Domain.Entities.User
        {
            UserName = "admin",
            Email = "admin@demo.com",
            Password = enc.Hash("Admin123!")
        };
        admin.Role = adminRole;


        var user = new Domain.Entities.User
        {
            UserName = "user",
            Email = "user@demo.com",
            Password = enc.Hash("User123!")
        };
        user.Role = userRole;

        ctx.Users.AddRange(admin, user);
        await ctx.SaveChangesAsync();
    }

    // === Ensure Courses ===
    if (!await ctx.Courses.AnyAsync())
    {
        ctx.Courses.AddRange(
            new Domain.Entities.Course { Name = "React from scratch", Description = "Intro a React" },
            new Domain.Entities.Course { Name = ".NET API with EF Core", Description = "APIs with EF Core"}
        );
        await ctx.SaveChangesAsync();
    }
}

app.Run();

