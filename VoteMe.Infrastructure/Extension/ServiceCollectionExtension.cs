using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Amazon.SimpleEmail;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Domain.Entities;
using VoteMe.Infrastructure.AWS;
using VoteMe.Infrastructure.Consumers;
using VoteMe.Infrastructure.Consumers.Auth;
using VoteMe.Infrastructure.Consumers.Candidate;
using VoteMe.Infrastructure.Consumers.Election;
using VoteMe.Infrastructure.Consumers.Organization;
using VoteMe.Infrastructure.Consumers.Voting;
using VoteMe.Infrastructure.Data;
using VoteMe.Infrastructure.Jobs;
using VoteMe.Infrastructure.Jwt;
using VoteMe.Infrastructure.Repositories;
using VoteMe.Infrastructure.Repository;
using VoteMe.Infrastructure.Services;
using VoteMe.Infrastructure.Settings;
using System.Net.Security;

namespace VoteMe.Infrastructure.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddVoteMeInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found. Check user secrets.");
            }
            services.Configure<SuperAdminSettings>(configuration.GetSection("SuperAdmin"));

            ////AWS Settings
            //services.Configure<AwsSettings>(configuration.GetSection("AWS"));

            //JWT Settings
            var jwtSettings = services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            //CloudinarySettings
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

            //AWS SES Client
            services.AddScoped<IAmazonSimpleEmailService>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<AwsSettings>>().Value;

                return new AmazonSimpleEmailServiceClient(
                    settings.AccessKeyId,
                    settings.SecretAccessKey,
                    Amazon.RegionEndpoint.GetBySystemName(settings.Region)
                );
            });

            //DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                sqlOptions.CommandTimeout(60)));

            //Identity
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            //Redis

            var options = new ConfigurationOptions
            {
                EndPoints = { "redis-12945.c247.eu-west-1-1.ec2.cloud.redislabs.com:12945" },
                User = "default",
                Password = "AecLgrRPnPFtL44kZmydioaQsDh3z2WW",
                Ssl = false,          // ← try without SSL
                AbortOnConnectFail = false,
                ConnectTimeout = 10000,
                SyncTimeout = 5000,
            };

            services.AddStackExchangeRedisCache(cacheOptions =>
            {
                cacheOptions.ConfigurationOptions = options;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

                if (jwtSettings == null)
                     throw new Exception("JWT is null");
                
                if (string.IsNullOrEmpty(jwtSettings.Key))
                     throw new Exception("JWT Key is missing!");
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                              Encoding.UTF8.GetBytes(jwtSettings.Key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager = context.HttpContext.RequestServices   
                            .GetRequiredService<UserManager<AppUser>>();

                        var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var tokenVersion = context.Principal?.FindFirst("tokenVersion")?.Value;

                        if (userId == null || tokenVersion == null)
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);
                        if (user == null || user.TokenVersion.ToString() != tokenVersion)
                        {
                            context.Fail("Token is no longer valid");
                            return;
                        }
                    }
                };
            });

            //Swagger configuration
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "VoteMe API",
                    Version = "v1",
                    Description = "Online Voting Platform API"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
                 });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin"));
                options.AddPolicy("OrgAdmin", policy => policy.RequireRole("OrgAdmin"));
                options.AddPolicy("Voter", policy => policy.RequireRole("Voter"));
                options.AddPolicy("OrgAdminOrSuperAdmin", policy => policy.RequireRole("OrgAdmin", "SuperAdmin"));
                options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
            });

            //Hangfire
            services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            //Jobs
            services.AddScoped<IElectionJobService, ElectionJobService>();
            services.AddScoped<IElectionScheduler, ElectionScheduler>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ICandidateRepository, CandidateRepository>();
            services.AddScoped<IElectionCategoryRepository, ElectionCategoryRepository>();
            services.AddScoped<IElectionRepository, ElectionRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<ITokenService, TokenService>();

            // Consumers
            services.AddHostedService<UserRegisteredConsumer>();
            //services.AddHostedService<AuditLogConsumer>();
            services.AddHostedService<PasswordChangedConsumer>();
            services.AddHostedService<OrganizationCreatedConsumer>();
            services.AddHostedService<MemberJoinedConsumer>();
            services.AddHostedService<MemberRemovedConsumer>();
            services.AddHostedService<ElectionCreatedConsumer>();
            services.AddHostedService<ElectionOpenedConsumer>();
            services.AddHostedService<ElectionClosedConsumer>();
            services.AddHostedService<ElectionUpdatedConsumer>();
            services.AddHostedService<VoteCastConsumer>();
            services.AddHostedService<VoteChangedConsumer>();
            services.AddHostedService<CandidateAddedConsumer>();
            services.AddHostedService<CandidateUpdatedConsumer>();
            services.AddHostedService<CandidateDeletedConsumer>();

            services.AddAuthorization();
            services.AddHttpContextAccessor();

            return services;
        } 
    }
}
