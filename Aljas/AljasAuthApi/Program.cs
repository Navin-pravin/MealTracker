using AljasAuthApi.Services;
using AljasAuthApi.Config;
using AljasAuthApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using ProjectHierarchyApi.Services;
using RabbitMQ.Client;
using System.Net.WebSockets;
using System.Threading.Tasks;

// ✅ Create Builder
var builder = WebApplication.CreateBuilder(args);

// ✅ Load MongoDB Settings
var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>() ?? new MongoDbSettings
{
    ConnectionString = "mongodb://172.16.100.67:27017",
    DatabaseName = "Aljas",
    EmployeesCollectionName = "Employees",
    SubcontractorCollectionName = "Subcontractors"
};

// ✅ Load JWT Settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings
{
    SecretKey = "a54bc4a85f99f01f4fa91cd540653bbdd06a4cd325e23e1c228cf46531fbfb24",
    Issuer = "AljasIssuer",
    Audience = "AljasAudience",
    TokenExpirationMinutes = 60
};

// ✅ Load Redis Settings
var redisSettings = builder.Configuration.GetSection("RedisSettings").Get<RedisSettings>() ?? new RedisSettings
{
    ConnectionString = "43.204.40.208:6380",
    StreamKey = "user-events"
};

// ✅ Load RabbitMQ Settings
var rabbitMQSettings = builder.Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>() ?? new RabbitMQSettings
{
    HostName = "43.204.40.208",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

// ✅ Register MongoDB Client and Database
var mongoClient = new MongoClient(mongoSettings.ConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoSettings.DatabaseName);
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

// ✅ Register MongoDB Collections
builder.Services.AddSingleton(sp => mongoDatabase.GetCollection<User>("Users"));
builder.Services.AddSingleton(sp => mongoDatabase.GetCollection<Employee>(mongoSettings.EmployeesCollectionName));
builder.Services.AddSingleton(sp => mongoDatabase.GetCollection<SubContractor>(mongoSettings.SubcontractorCollectionName));

// ✅ Register Configuration Settings
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton(redisSettings);
builder.Services.AddSingleton(rabbitMQSettings);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ✅ Register RabbitMQ Connection
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    return new ConnectionFactory()
    {
        HostName = rabbitMQSettings.HostName,
        Port = rabbitMQSettings.Port,
        UserName = rabbitMQSettings.UserName,
        Password = rabbitMQSettings.Password
    };
});

// ✅ Register Services
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<EmployeeService>();
builder.Services.AddSingleton<SubContractorService>();
builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<LocationService>();
builder.Services.AddSingleton<CanteenService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<DeviceService>();
builder.Services.AddSingleton<VisitorService>();
builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddSingleton<ExtrasService>();
builder.Services.AddSingleton<RoleAccessService>();
builder.Services.AddSingleton<CanteenConfigurationService>();
builder.Services.AddSingleton<RoleService>();
builder.Services.AddSingleton<EmployeeReportService>();
builder.Services.AddSingleton<CouponService>();
builder.Services.AddSingleton<RoleHierarchyService>();
builder.Services.AddSingleton<DashboardService>();
builder.Services.AddSingleton<RawDataService>();

// ✅ Register MealCount WebSocket Background Service
builder.Services.AddSingleton<MealCountWebSocketService>();

// ✅ Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Aljas Authentication API",
        Version = "v1"
    });
});

// ✅ Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

// ✅ Set up Kestrel for API to listen on port 5221
builder.WebHost.UseKestrel()
    .UseUrls("http://0.0.0.0:5221");

// ✅ Build App
var app = builder.Build();

// ✅ Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meal Tracking API");
    c.RoutePrefix = string.Empty;
});

// ✅ Enable WebSockets
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
};
app.UseWebSockets(webSocketOptions);

// ✅ WebSocket Endpoint for Meal Count Updates
app.Map("/ws/mealcount", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var webSocketHandler = context.RequestServices.GetRequiredService<MealCountWebSocketService>();
        await webSocketHandler.HandleWebSocketAsync(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

// ✅ Middleware Setup
app.UseCors("AllowAnyOrigin");
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map Controllers
app.MapControllers();
// ✅ Start WebSocket background change stream listener
var mealSocketService = app.Services.GetRequiredService<MealCountWebSocketService>();
_ = mealSocketService.StartAsync(); // Fire-and-forget

// ✅ Run App
app.Run();
