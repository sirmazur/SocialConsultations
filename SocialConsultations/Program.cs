using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using SocialConsultations.DbContexts;
using SocialConsultations.Entities;
using SocialConsultations.Helpers;
using SocialConsultations.Services.Basic;
using SocialConsultations.Services.CommunityServices;
using SocialConsultations.Services.FieldsValidationServices;
using SocialConsultations.Services.IssueServices;
using SocialConsultations.Services.UserServices;
using System;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver =
    new CamelCasePropertyNamesContractResolver();
})
.AddXmlDataContractSerializerFormatters()
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetailsFactory = context.HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();

        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                       context.HttpContext,
                                  context.ModelState);

        problemDetails.Type = "https://soccons.com/modelvalidationproblem";
        problemDetails.Title = "One or more model validation errors occurred.";
        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
        problemDetails.Detail = "See the errors property for details.";
        problemDetails.Instance = context.HttpContext.Request.Path;

        return new UnprocessableEntityObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});

builder.Services.Configure<MvcOptions>(options =>
{
    var newtonsoftJsonOutputFormatter = options.OutputFormatters
        .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

});

var connection = String.Empty;
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
}
else
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Production.json");
}

connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")
                 ?? Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SchemaFilter<EnumSchemaFilter>();
//    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

//    options.IncludeXmlComments(xmlPath);
//});
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IFieldsValidationService, FieldsValidationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBasicRepository<User>, BasicRepository<User>>();
builder.Services.AddScoped<ICommunityService, CommunityService>();
builder.Services.AddScoped<ICommunityRepository, CommunityRepository>();
builder.Services.AddScoped<IIssueService, IssueService>();
builder.Services.AddScoped<IIssueRepository, IssueRepository>();
builder.Services.AddScoped<IBasicRepository<Issue>, BasicRepository<Issue>>();
builder.Services.AddScoped<IBasicRepository<JoinRequest>, BasicRepository<JoinRequest>>();
builder.Services.AddScoped<IBasicRepository<Community>, BasicRepository<Community>>();
builder.Services.AddScoped<IStoreKeyAccessor, StoreKeyAccessor>();
builder.Services.AddScoped<IValidatorValueInvalidator, ValidatorValueInvalidator>();
builder.Services.AddTransient<EmailSender>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ConsultationsContext>(options =>
    options.UseSqlServer(connection).EnableSensitiveDataLogging());
builder.Services.AddResponseCaching();
builder.Services.AddHttpCacheHeaders(
       (expirationModelOptions) =>
       {
           expirationModelOptions.MaxAge = 30;
           expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
       },
       (validationModelOptions) =>
       {
           validationModelOptions.MustRevalidate = true;
           validationModelOptions.Vary = ["Accept", "Accept-Language", "Accept-Encoding", "Authorization"];
       });
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
        };
    }
    );
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeLoggedIn", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();

app.UseHttpCacheHeaders();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
