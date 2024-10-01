using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using MyBlog.DataAccess.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using MyBlog.Business.Validators; // Validator s�n�flar�n� dahil ediyoruz
using Microsoft.OpenApi.Models;
using Serilog;
using MyBlog.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog yap�land�rmas�
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("D:/logs/log-.txt", rollingInterval: RollingInterval.Day) // G�nl�k log dosyas� olu�turur
    .CreateLogger();

// Serilog'u uygulaman�n loglama sa�lay�c�s� olarak ayarlay�n
builder.Host.UseSerilog();

// appsettings.json dosyas�ndaki JWT ayarlar�n� alal�m
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]); // Gizli anahtar

// Veritaban� ba�lant�s� i�in Connection String'i alal�m
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(connectionString)); // Veritaban� ba�lant�s�n� ekliyoruz

// JWT Authentication'� konfig�re edelim
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
        ValidIssuer = jwtSettings["Issuer"], // Issuer do�rulamas�
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"], // Audience do�rulamas�
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Token'�n hemen s�resinin dolmas� i�in tolerans s�resi yok
    };
});

// Authorization Servisini ekleyelim
builder.Services.AddAuthorization();  // Authorization servisini ekliyoruz

// FluentValidation'� burada ekliyoruz
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblyContaining<LoginValidator>());

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<GalleryDTOValidator>());


// Swagger ve di�er servisleri ekleyin
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
        Description = "L�tfen 'Bearer {token}' format�nda JWT token girin",
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

    // Swagger anotasyonlar�n� etkinle�tir
    c.EnableAnnotations();
});

// Repository ve Dependency Injection ekleyelim
builder.Services.AddScoped<IUserRepository, UserRepository>(); // IUserRepository ve UserRepository ba��ml�l�klar�n� ekliyoruz
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>(); // IGalleryRepository ve GalleryRepository ba��ml�l�klar�n� ekliyoruz
builder.Services.AddScoped<IGalleryService, GalleryService>(); // Service ba��ml�l���n� ekleyin
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Middleware'ler
if (app.Environment.IsDevelopment())
{
    // Geli�tirme ortam�nda detayl� hata sayfas� g�ster
    app.UseDeveloperExceptionPage();
}
else
{
    // �retim ortam�nda genel hata sayfas� g�ster
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

app.UseAuthentication(); // JWT Authentication'� etkinle�tirir
app.UseAuthorization();  // Authorization'� etkinle�tirir

app.MapControllers(); // Controller'lar� haritalar ve �al��mas�n� sa�lar

app.Run();
