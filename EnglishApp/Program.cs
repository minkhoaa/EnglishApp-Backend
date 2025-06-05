using CloudinaryDotNet;
using EnglishApp;
using EnglishApp.Data;
using EnglishApp.Model;
using EnglishApp.Repository;
using EnglishApp.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Security.Authentication.ExtendedProtection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



DotNetEnv.Env.Load();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EnglishApp API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Nhập JWT Bearer token (chỉ phần token, không kèm 'Bearer ').",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme, // "Bearer"
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});


builder.Services.AddCors(option => option.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Configuration["ConnectionStrings:MyDB"] = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__MYDB");


builder.Configuration["JWT:ValidAudience"] = Environment.GetEnvironmentVariable("JWT__VALIDAUDIENCE");
builder.Configuration["JWT:ValidIssuer"] = Environment.GetEnvironmentVariable("JWT__VALIDISSUER");
builder.Configuration["JWT:SecretKey"] = Environment.GetEnvironmentVariable("JWT__SECRETKEY");

builder.Configuration["EmailSettings:SmtpServer"] = Environment.GetEnvironmentVariable("EMAILSETTINGS__SMTPSERVER");
builder.Configuration["EmailSettings:SmtpPort"] = Environment.GetEnvironmentVariable("EMAILSETTINGS__SMTPPORT");
builder.Configuration["EmailSettings:SenderEmail"] = Environment.GetEnvironmentVariable("EMAILSETTINGS__SENDEREMAIL");
builder.Configuration["EmailSettings:SenderPassword"] = Environment.GetEnvironmentVariable("EMAILSETTINGS__SENDERPASSWORD");
builder.Configuration["EmailSettings:EnableSSL"] = Environment.GetEnvironmentVariable("EMAILSETTINGS__ENABLESSL");

builder.Configuration["CloudinarySettings:CloudName"] = Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__CLOUDNAME");
builder.Configuration["CloudinarySettings:ApiKey"] = Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__APIKEY");
builder.Configuration["CloudinarySettings:ApiSecret"] = Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__APISECRET");

builder.Services.AddDbContext<EnglishAppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("MyDB")));
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<EnglishAppDbContext>().AddDefaultTokenProviders();




builder.Services.AddScoped<AuthenticationRepository, AuthenticationService>();
builder.Services.AddScoped<LessonRepository,  LessonService>();
builder.Services.AddScoped<CategoryRepository, CategoryService>();



builder.Services.AddSingleton(option =>
{
    var settings = option.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(
        settings.CloudName,
        settings.ApiKey,
        settings.ApiSecret
        );
    return new Cloudinary(account);
});

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.RequireHttpsMetadata = false;
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
    };
});


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

var emailConfigs = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();

builder.Services.AddFluentEmail(emailConfigs!.SenderEmail, "no reply")
    .AddSmtpSender(new System.Net.Mail.SmtpClient(emailConfigs.SmtpServer)
    {
        Port = emailConfigs.SmtpPort,
        Credentials = new NetworkCredential(emailConfigs.SenderEmail, emailConfigs.SenderPassword),
        EnableSsl = emailConfigs.EnableSSL
    });


var app = builder.Build();

// Configure the HTTP request pipeline.



app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.UseCors();

app.Run();
