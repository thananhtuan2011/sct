using EF_Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using StackExchange.Redis;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks;
using Serilog.Sinks.File;
using System;
using System.Globalization;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

var filter = new Func<LogEvent, bool>(evt => evt.Properties.ContainsKey("IP_Port_CurrentNode"));
Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .Enrich.WithThreadId()
        .Enrich.WithThreadName()
        .Enrich.WithMachineName()
        .Enrich.WithProcessId()
        .Enrich.WithProcessName()
        .Enrich.WithMemoryUsage()
        .MinimumLevel.Verbose()
        .WriteTo.Console()
        .WriteTo.File($"logs/log-{DateTime.Now:yyyy-MM-dd}.txt",
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {IP_Port_CurrentNode} {Duration} {ErrorCode} {ErrorDescription} {TransactionStatus} {Message}{NewLine}{Exception}",
            formatProvider: new CultureInfo("en-US"),
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 104857600,
            retainedFileCountLimit: 90)
        .Filter.ByIncludingOnly(filter)
        .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationOptions
{
    AbortOnConnectFail = false,
    Ssl = false,
    Password = builder.Configuration["REDIS_PASS"],
};
config.EndPoints.Add(builder.Configuration["REDIS_HOST"], int.Parse(builder.Configuration["REDIS_PORT"].ToString()));
// Add services to the container.
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(config);
// Lấy DB
IDatabase db = redis.GetDatabase(1);
// Ping thử
if (db.Ping().TotalSeconds > 150)
{
    Console.WriteLine("Server Redis không hoạt động");
}

// Lưu dữ liệu
db.StringSet("hello", "Xin chào Redis");
// Lấy tất cả các key
var c = db.StringGet("hello");
Console.WriteLine(c);
ISubscriber sub = redis.GetSubscriber();
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);


builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddDbContext<SoHoa_SoCongThuongContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           );
var domains = new string[] { builder.Configuration["AllowedHosts"] };
builder.Services.AddCors(p => p.AddPolicy("AllowOrigin", builder =>
{

    builder.WithOrigins(domains).AllowAnyMethod().AllowAnyHeader();
}));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Please enter into field the word 'Bearer' following by space and JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        //Scheme = "bearer", // must be lower case
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                                        {
                                            {securityScheme, new string[] { }}
                                        });
});



builder.Host.UseSerilog();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowOrigin");

//app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Upload")),
    RequestPath = "/Upload"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "logs")),
    RequestPath = "/logs"
});



app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "{RequestBody}";
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null)
        {
            return LogEventLevel.Error;
        }
        else if (httpContext.Response.StatusCode >= 500)
        {
            return LogEventLevel.Error;
        }
        else if (httpContext.Response.StatusCode >= 400)
        {
            return LogEventLevel.Warning;
        }
        else
        {
            return LogEventLevel.Information;
        }
    };
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("StartTime", DateTime.UtcNow);
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
        diagnosticContext.Set("RequestHeaders", httpContext.Request.Headers);
        diagnosticContext.Set("RequestBody", httpContext.Request.Body);
        diagnosticContext.Set("ResponseHeaders", httpContext.Response.Headers);
        diagnosticContext.Set("ResponseBody", httpContext.Response.Body);
        diagnosticContext.Set("IP_Port_CurrentNode", $"{httpContext.Connection.RemoteIpAddress}:{httpContext.Connection.RemotePort.ToString(CultureInfo.InvariantCulture)}_{Environment.MachineName}");
    };
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
