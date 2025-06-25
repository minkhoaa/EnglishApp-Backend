using CloudinaryDotNet;
using EnglishApp;
using EnglishApp.Data;
using EnglishApp.Model;
using EnglishApp.Repository;
using EnglishApp.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
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

builder.Services.AddScoped<TokenRepository, TokenGenerator>();


builder.Services.AddScoped<AuthenticationRepository, AuthenticationService>();

builder.Services.AddScoped<ILessonRepository,  LessonRepository>();
builder.Services.AddScoped<ILessonService, LessonService>();

builder.Services.AddScoped<ILessonContentRepository, LessonContentRepository>();
builder.Services.AddScoped<ILessonContentService, LessonContentService>();  

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();

builder.Services.AddScoped<IExerciseOptionService, ExerciseOptionService>();
builder.Services.AddScoped<IExerciseOptionRepository, ExerciseOptionRepository>();
builder.Services.AddScoped<IExerciseResultProgressRepository, ExerciseResultProgressRepository>();
builder.Services.AddScoped<IFlashCardRepository, FlashCardService>();
builder.Services.AddScoped<IDeckRepository, DeckService>();


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
builder.Services.AddAuthentication(options =>
{
    // Đặt default scheme là PolicyScheme tự động nhận JWT hoặc Cookie
    options.DefaultScheme = "JwtOrCookie";
    options.DefaultAuthenticateScheme = "JwtOrCookie";
    options.DefaultChallengeScheme = "JwtOrCookie";
})
.AddPolicyScheme("JwtOrCookie", "JWT or Cookie", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        // Nếu có header Bearer thì dùng JWT
        var hasBearer = context.Request.Headers["Authorization"].FirstOrDefault()?.StartsWith("Bearer ") == true;
        if (hasBearer)
            return JwtBearerDefaults.AuthenticationScheme;

        if (context.Request.Cookies.ContainsKey(".AspNetCore.Cookies"))
            return CookieAuthenticationDefaults.AuthenticationScheme;

        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
})
.AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("JWT AUTH FAILED: " + context.Exception?.ToString());
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("JWT TOKEN VALIDATED: " + context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                message = "Vui lòng đăng nhập"
            });

            Console.WriteLine("JWT CHALLENGE - Token invalid or not found");
            return context.Response.WriteAsync(result);
        }
    };

})
.AddGoogle(google =>
{
    google.ClientId = builder.Configuration["GOOGLE_SETTINGS:GOOGLE__CLIENT__ID"] ?? Environment.GetEnvironmentVariable("GOOGLE__CLIENT__ID")!;
    google.ClientSecret = builder.Configuration["GOOGLE_SETTINGS:GOOGLE__CLIENT__SECRET"] ?? Environment.GetEnvironmentVariable("GOOGLE__CLIENT__SECRET")!;
    google.CallbackPath = "/signin-google";
    google.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddFacebook(facebook =>
{
    facebook.ClientId = Environment.GetEnvironmentVariable("FACEBOOK_APP_ID")!;
    facebook.ClientSecret = Environment.GetEnvironmentVariable("FACEBOOK_APP_SECRET")!;
    facebook.CallbackPath = "/login-facebook";
    facebook.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    facebook.Scope.Add("email");
})

;
builder.Services.AddAuthorization();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

var emailConfigs = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();

builder.Services.AddFluentEmail(emailConfigs!.SenderEmail, "no reply")
    .AddSmtpSender(new System.Net.Mail.SmtpClient(emailConfigs.SmtpServer)
    {
        Port = emailConfigs.SmtpPort,
        Credentials = new NetworkCredential(emailConfigs.SenderEmail, emailConfigs.SenderPassword),
        EnableSsl = emailConfigs.EnableSSL
    }
    );


var app = builder.Build();

// Configure the HTTP request pipeline.



app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.UseCors();

app.Run();
