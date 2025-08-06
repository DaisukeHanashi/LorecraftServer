using Lorecraft_API.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Lorecraft_API.Data.Repository.Interface;
using Lorecraft_API.StaticFactory;
using Lorecraft_API.Resources.Handler;
using Lorecraft_API.Factory.Interface;
using Lorecraft_API.Factory;
using Lorecraft_API.Manager;
using System.Text.Json;
using Lorecraft_API.Helper;
using Lorecraft_API.Resources;
using Lorecraft_API.InternalModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using Lorecraft_API.Service;

namespace Lorecraft_API.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static void InstallBaseService(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddHttpContextAccessor();
            services.AddLogger(config);
            services.ConfigureSections(config);
            services.AddCryptoAlgorithms(config);
            services.AddDataService();
            services.AddCustomAuthentication(config, env);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
                options.AddPolicy("AllowLocalOrigin", builder =>
                builder
                .WithOrigins("http://localhost")
                .SetIsOriginAllowed(option => true)
                .AllowAnyMethod()
                .AllowAnyHeader());
            });
            services.AddControllers(options => options.RespectBrowserAcceptHeader = true)
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            });


            ConfigureDapper();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(setup =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }

                };
                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
            });
        }

        private static void ConfigureSections(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<TokenAuthentication>(config.GetSection("TokenAuthentication"));
        }

        private static void AddLogger(this IServiceCollection services, IConfiguration config)
        {
            services.AddLogging(log =>
            {
                log.AddConfiguration(config.GetSection("Logging"));
                log.AddConsole();
                log.AddDebug();
            });
        }

        private static void AddCryptoAlgorithms(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<Argon2ParameterFactory>();
            services.AddSingleton<CryptoManager>();
            // CryptoManager.SetUp(config.GetRequiredSection("TokenAuthentication")); 


        }

        private static void AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {

            var token = configuration.GetTokenAuthentication();

            services.AddSingleton<TokenProviderOptionsFactory>();
            services.AddSingleton<TokenValidationParametersFactory>();

            var tokenValidationParametersFactory = services.BuildServiceProvider().GetRequiredService<TokenValidationParametersFactory>();
            var tokenValidationParameters = tokenValidationParametersFactory.Create();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {

                options.TokenValidationParameters = tokenValidationParameters;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[Constants.Authorization];
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(Constants.AuthenticationScheme, options =>
            {
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest,
                    Name = $"{environment.ApplicationName}_{token.CookieName}"
                };

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }
                };

                var tokenProviderOptionsFactory = services.BuildServiceProvider().GetRequiredService<TokenProviderOptionsFactory>();
                options.TicketDataFormat = new CustomJwtDataFormat(SecurityAlgorithms.HmacSha256Signature, tokenValidationParameters, configuration, tokenProviderOptionsFactory);
            });

            services.AddSingleton<Store.CookieTicketStore>();



            services.AddAuthorizationBuilder()
            .AddDefaultPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser())
            .AddPolicy("AdminOnly", policy => policy.RequireAuthenticatedUser().RequireRole("Admin"));

            services.AddScoped<AuthenticationManager>();

        }

        private static void ConfigureDapper()
        {
            Dapper.SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        }

        private static void AddDataService(this IServiceCollection services)
        {
            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ApplicationException("The connection string is null");

                return new SqlConnectionFactory(connectionString);

            });

            //Repository
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ILoreRepository, LoreRepository>();
            services.AddScoped<ICharacterRepository, CharacterRepository>();
            services.AddScoped<IPersonRespository, PersonRepository>();
            services.AddScoped<IConceptRepository, ConceptRepository>();
            services.AddScoped<IFactionRepository, FactionRepository>();
            services.AddScoped<IContextRepository, ContextRepository>();
            services.AddScoped<IStoryRepository, StoryRepository>();
            services.AddScoped<IDescriptionRepository, DescriptionRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();


            //Service
            services.AddScoped<ICharacterService, CharacterService>();

            //Factory
            services.AddScoped<IAccountFactory, AccountFactory>();
            services.AddScoped<ILoreFactory, LoreFactory>();
            services.AddScoped<ICharacterFactory, CharacterFactory>();
            services.AddScoped<IConceptFactory, ConceptFactory>();
            services.AddScoped<IStoryFactory, StoryFactory>();
            services.AddScoped<IContextFactory, ContextFactory>();
            services.AddScoped<IFactionFactory, FactionFactory>();
            services.AddScoped<IDescriptionFactory, DescriptionFactory>();
            services.AddScoped<ICountryFactory, CountryFactory>();

        }
    }
}