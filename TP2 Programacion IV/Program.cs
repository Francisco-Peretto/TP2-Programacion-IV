using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TP2_Programacion_IV.Config;
using TP2_Programacion_IV.Repositories;
using TP2_Programacion_IV.Services;
using TP2_Programacion_IV.Models.User;
using TP2_Programacion_IV.Models.Role;
using TP2_Programacion_IV.Models.Course;

var builder = WebApplication.CreateBuilder(args);

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
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
    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await ctx.Database.MigrateAsync();

    if (!ctx.Roles.Any())
    {
        ctx.Roles.AddRange(new Role { Name = "Admin" }, new Role { Name = "User" });
        await ctx.SaveChangesAsync();
    }

    if (!ctx.Users.Any())
    {
        var enc = scope.ServiceProvider.GetRequiredService<EncoderServices>();
        var adminRoleId = ctx.Roles.First(r => r.Name == "Admin").Id;
        var userRoleId = ctx.Roles.First(r => r.Name == "User").Id;

        ctx.Users.AddRange(
            new User { Email = "admin@demo.com", PasswordHash = enc.Hash("Admin123!"), RoleId = adminRoleId },
            new User { Email = "user@demo.com", PasswordHash = enc.Hash("User123!"), RoleId = userRoleId }
        );
        await ctx.SaveChangesAsync();
    }

    if (!ctx.Courses.Any())
    {
        ctx.Courses.AddRange(
            new Course { Title = "React desde cero", Category = "Web", Price = 0 },
            new Course { Title = ".NET API con EF Core", Category = "Backend", Price = 100 }
        );
        await ctx.SaveChangesAsync();
    }
}

app.Run();
