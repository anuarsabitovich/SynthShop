using System.Text;
using SynthShop.Infrastructure.Data;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SynthShop.Mapper.Profiles;
using SynthShop.Core.Services;
using SynthShop.Validations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Constants;
using SynthShop.Domain.Settings;
using SynthShop.Middleware;
using SynthShop.Extensions;
using SynthShop.Infrastructure.Data.Seed;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var allowedOrigins = config["CorsSettings:AllowedOrigins"];

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
        .Enrich.WithCorrelationIdHeader(LogConstants.CorrelationHeader));


builder.Services.Configure<PagingSettings>(config.GetSection(nameof(PagingSettings)));

builder.Services.AddCors();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var tokenValidationParameters = new TokenValidationParameters()
{
    // TODO refactor to use options
    ValidIssuer = config["JwtSettings:Issuer"],
    ValidAudience = config["JwtSettings:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true
};

builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => { x.TokenValidationParameters = tokenValidationParameters; });

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserProvider, UserProvider>();
builder.Services.AddHeaderPropagation(options => options.Headers.Add(LogConstants.CorrelationHeader));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<CorrelationHeaderSwaggerOperationFilter>();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Synth Shop API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddHealthChecks().AddDbContextCheck<MainDbContext>();
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CategoryValidator>());


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSerilogRequestLogging();
}


app.UseExceptionHandler("/Error");
app.UseHttpsRedirection();

app.UseHsts();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(opt =>
{
    opt.AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(allowedOrigins)
        .AllowCredentials();
});


app.UseHeaderPropagation();

app.MapControllers();
app.MapHealthChecks("/health");
using var serviceScope = app.Services.CreateScope();
var seeder = serviceScope.ServiceProvider.GetRequiredService<Runner>();
seeder.SeedAsync().GetAwaiter().GetResult();

app.Run();