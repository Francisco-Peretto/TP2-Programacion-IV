using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TP2_Programming_IV.Services;
using TP2_Programming_IV.Repositories;
using TP2_Programacion_IV.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- EF Core ----------
var connString =
    builder.Configuration.GetConnectionString("Default")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connString));

// ---------- AutoMapper ----------
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ---------- Authentication (Cookies + JWT with policy switch) ----------
var jwtKey =
    builder.Configuration["Secrets:JWT"]
    ?? builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT secret not configured (Secrets:JWT or Jwt:Key).");

// Policy scheme decides per-request whether to use Cookie or Bearer
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "MultiAuth";
        options.DefaultChallengeScheme = "MultiAuth";
    })
    .AddPolicyScheme("MultiAuth", "CookieOrBearer", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            return (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                ? JwtBearerDefaults.AuthenticationScheme
                : CookieAuthenticationDefaults.AuthenticationScheme;
        };
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
    {
        opt.Cookie.Name = "tp_auth";
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        opt.Cookie.SameSite = SameSiteMode.Strict;
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan = TimeSpan.FromDays(1);
        opt.LoginPath = "/api/auth/login";
        opt.LogoutPath = "/api/auth/logout";

        // ⬇️ important for APIs
        opt.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
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

builder.Services.AddSwaggerGen(c =>
{
    var scheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Paste only the JWT (no 'Bearer ' prefix).",
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { scheme, Array.Empty<string>() }
    });
});



// ---------- DI: Repositories ----------
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<UserRepository>();

// ---------- DI: Services ----------
builder.Services.AddScoped<CourseServices>();
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<AuthServices>();
builder.Services.AddScoped<AdminServices>();
builder.Services.AddScoped<RoleServices>();

// ---------- Utils ----------
builder.Services.AddScoped<IEncoderServices, EncoderServices>();

// ---------- MVC + Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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

    // Roles
    var neededRoles = new[] { "Admin", "User" };
    foreach (var r in neededRoles)
        if (!await ctx.Roles.AnyAsync(x => x.Name == r))
            ctx.Roles.Add(new Domain.Entities.Role { Name = r });
    await ctx.SaveChangesAsync();

    var adminRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
    var userRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "User");
    if (adminRole == null || userRole == null)
        throw new InvalidOperationException("No se pudieron asegurar los roles Admin/User.");

    // Users
    if (!await ctx.Users.AnyAsync())
    {
        var enc = scope.ServiceProvider.GetRequiredService<IEncoderServices>();

        var admin = new Domain.Entities.User
        {
            UserName = "admin",
            Email = "admin@demo.com",
            Password = enc.Hash("Admin123!"),
            RoleId = adminRole.Id
        };

        var user = new Domain.Entities.User
        {
            UserName = "user",
            Email = "user@demo.com",
            Password = enc.Hash("User123!"),
            RoleId = userRole.Id
        };

        ctx.Users.AddRange(admin, user);
        await ctx.SaveChangesAsync();
    }

    // Courses
    if (!await ctx.Courses.AnyAsync())
    {
        ctx.Courses.AddRange(
            new Domain.Entities.Course { Name = "React from scratch", Description = "Intro a React" },
            new Domain.Entities.Course { Name = ".NET API with EF Core", Description = "APIs with EF Core" }
        );
        await ctx.SaveChangesAsync();
    }
}

app.Run();
