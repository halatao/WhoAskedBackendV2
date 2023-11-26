using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using WhoAskedBackend.Data;
using WhoAskedBackend.Services;
using WhoAskedBackend.Services.ContextServices;
using WhoAskedBackend.Services.Messaging;
using WhoAskedBackend.Services.WebSocket;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
SecurityService securityService = new(builder.Configuration);
builder.Services.AddTransient<WhoAskedContext>();
builder.Services.AddSingleton<WebSocketHandler>();

builder.Services.AddTransient<QueueService>();
builder.Services.AddSingleton<SecurityService>();
builder.Services.AddSingleton<BrokerConnection>();
builder.Services.AddSingleton<QueueStorage>();
builder.Services.AddTransient<QueueProvider>();


builder.Services.AddTransient<MessageProvider>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<UserInQueueService>();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    // key from config
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
    };
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(options =>
    options.WithOrigins("http://localhost:4200/").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseWebSockets();


await app.RunAsync();