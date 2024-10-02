using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using MyBlog.DataAccess.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using MyBlog.Business.Validators;
using Microsoft.OpenApi.Models;
using Serilog;
using MyBlog.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog yapýlandýrmasý
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("z:/logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Serilog'u uygulamanýn loglama saðlayýcýsý olarak ayarlayýn
builder.Host.UseSerilog();

// appsettings.json dosyasýndaki JWT ayarlarýný alalým
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

// Veritabaný baðlantýsý için Connection String'i alalým
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(connectionString));

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// FluentValidation'ý burada ekliyoruz
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<LoginValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<GalleryDTOValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<ArticleValidator>();
    });

// Swagger ve diðer servisleri ekleyin
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

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

    c.EnableAnnotations();
});

// Repository ve Dependency Injection ekleyelim
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Middleware'ler
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
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

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
