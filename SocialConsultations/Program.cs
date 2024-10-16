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
using SocialConsultations.Services.FieldsValidationServices;
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
    connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
}
else
{
    connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
}


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
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ConsultationsContext>(options =>
    options.UseSqlServer(connection));
builder.Services.AddResponseCaching();
builder.Services.AddHttpCacheHeaders(
       (expirationModelOptions) =>
       {
           expirationModelOptions.MaxAge = 180;
           expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
       },
       (validationModelOptions) =>
       {
           validationModelOptions.MustRevalidate = true;
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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpCacheHeaders();

app.MapControllers();

app.Run();
