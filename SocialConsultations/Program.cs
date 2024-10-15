using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using SocialConsultations.DbContexts;
using SocialConsultations.Helpers;
using System;
using System.Reflection;

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

builder.Services.Configure<MvcOptions>(options =>
{
    var newtonsoftJsonOutputFormatter = options.OutputFormatters
        .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<EnumSchemaFilter>();
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ConsultationsContext>(options =>
    options.UseSqlServer(connection));
builder.Configuration.AddEnvironmentVariables();
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
