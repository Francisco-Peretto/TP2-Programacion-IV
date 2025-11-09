using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TP2_Programacion_IV.Repositories;
using TP2_Programacion_IV.Services;
using Domain.Entities;
using Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// EF Core
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

// DI repos y servicios
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoleRepository>();

builder.Services.AddScoped<CourseServices>();
builder.Services.AddScoped<AuthServices>();
builder.Services.AddScoped<AdminServices>();
builder.Services.AddSingleton<EncoderServices>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed inicial
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await ctx.Database.MigrateAsync();

    // === Ensure Roles ===
    var neededRoles = new[] { "Admin", "User" };
    foreach (var r in neededRoles)
    {
        if (!await ctx.Roles.AnyAsync(x => x.Name == r))
            ctx.Roles.Add(new Role { Name = r });
    }
    await ctx.SaveChangesAsync();

    // Leer roles ya persistidos (fallar explícitamente si no están)
    var adminRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
    var userRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "User");
    if (adminRole == null || userRole == null)
        throw new InvalidOperationException("No se pudieron asegurar los roles Admin/User.");

    // === Ensure Users ===
    if (!await ctx.Users.AnyAsync())
    {
        var enc = scope.ServiceProvider.GetRequiredService<EncoderServices>();

        var admin = new User
        {
            UserName = "admin",
            Email = "admin@demo.com",
            Password = enc.Hash("Admin123!")
        };
        admin.Roles.Add(adminRole);

        var user = new User
        {
            UserName = "user",
            Email = "user@demo.com",
            Password = enc.Hash("User123!")
        };
        user.Roles.Add(userRole);

        ctx.Users.AddRange(admin, user);
        await ctx.SaveChangesAsync();
    }

    // === Ensure Courses ===
    if (!await ctx.Courses.AnyAsync())
    {
        ctx.Courses.AddRange(
            new Course { Name = "React desde cero", Description = "Intro a React", Category = "Web", Price = 0m },
            new Course { Name = ".NET API con EF Core", Description = "APIs con EFCore", Category = "Backend", Price = 100m }
        );
        await ctx.SaveChangesAsync();
    }
}

app.Run();
