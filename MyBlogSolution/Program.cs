using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using MyBlog.DataAccess.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using MyBlog.Business.Validators; // Validator sýnýflarýný dahil ediyoruz
using Microsoft.OpenApi.Models;
using Serilog;
using MyBlog.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog yapýlandýrmasý
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("D:/logs/log-.txt", rollingInterval: RollingInterval.Day) // Günlük log dosyasý oluþturur
    .CreateLogger();

// Serilog'u uygulamanýn loglama saðlayýcýsý olarak ayarlayýn
builder.Host.UseSerilog();

// appsettings.json dosyasýndaki JWT ayarlarýný alalým
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]); // Gizli anahtar

// Veritabaný baðlantýsý için Connection String'i alalým
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(connectionString)); // Veritabaný baðlantýsýný ekliyoruz

// JWT Authentication'ý konfigüre edelim
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"], // Issuer doðrulamasý
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"], // Audience doðrulamasý
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Token'ýn hemen süresinin dolmasý için tolerans süresi yok
    };
});

// Authorization Servisini ekleyelim
builder.Services.AddAuthorization();  // Authorization servisini ekliyoruz

// FluentValidation'ý burada ekliyoruz
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblyContaining<LoginValidator>());

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<GalleryDTOValidator>());


// Swagger ve diðer servisleri ekleyin
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    // Swagger JWT Bearer Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Lütfen 'Bearer {token}' formatýnda JWT token girin",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Swagger anotasyonlarýný etkinleþtir
    c.EnableAnnotations();
});

// Repository ve Dependency Injection ekleyelim
builder.Services.AddScoped<IUserRepository, UserRepository>(); // IUserRepository ve UserRepository baðýmlýlýklarýný ekliyoruz
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>(); // IGalleryRepository ve GalleryRepository baðýmlýlýklarýný ekliyoruz
builder.Services.AddScoped<IGalleryService, GalleryService>(); // Service baðýmlýlýðýný ekleyin
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Middleware'ler
if (app.Environment.IsDevelopment())
{
    // Geliþtirme ortamýnda detaylý hata sayfasý göster
    app.UseDeveloperExceptionPage();
}
else
{
    // Üretim ortamýnda genel hata sayfasý göster
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Swagger middleware'lerini ekleyelim
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Blog API V1");
});

app.UseHttpsRedirection();
app.UseRouting();

// Hata yakalama middleware'i burada ekleniyor
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthentication(); // JWT Authentication'ý etkinleþtirir
app.UseAuthorization();  // Authorization'ý etkinleþtirir

app.MapControllers(); // Controller'larý haritalar ve çalýþmasýný saðlar

app.Run();
