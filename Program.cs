using System.Text;
using Interview_Calendar.Data;
using Interview_Calendar.Filters;
using Interview_Calendar.Helpers;
using Interview_Calendar.Models;
using Interview_Calendar.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();
        var app = builder.Build();
        app.Configure();
        app.Run();
    }

    //Use Extension methods to initialize Services
    private static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.DictionaryKeyPolicy = null;
        }).AddNewtonsoftJson();

        builder.Services.AddControllers(c =>
        {
            c.Filters.Add<InterviewCalendarExceptionFilter>();
        }).AddNewtonsoftJson();

        builder.Services.Configure<UserDbConfiguration>(builder.Configuration.GetSection("InterviewCalendarDb"));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddSingleton<IInterviewerService, InterviewerService>();
        builder.Services.AddSingleton<ICandidateService, CandidateService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton(typeof(AddUserService<,,>));
        builder.Services.AddSingleton<PasswordHasher>();



        var config = builder.Configuration;

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = config["JwtSettings:Issuer"],
                ValidAudience = config["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(IdentityData.InterviewerUserClaim, p =>
                p.RequireClaim(IdentityData.InterviewerUserPolicyName, "true")
            );
            options.AddPolicy(IdentityData.CandidateUserClaim, p =>
                p.RequireClaim(IdentityData.CandidateUserPolicyName, "true")
            );
        });
    }

    private static void Configure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}