using Flustri.Api.Auth;
using Flustri.Core;
using Flustri.Core.Queries;
using Flustri.Core.Commands;
using Flustri.Api;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.AddAuthentication(options => options.DefaultScheme = "FlustriAuth")
    .AddScheme<FlustriAuthSchemeOptions, FlustriAuthHandler>("FlustriAuth", options => { });

builder.Services.AddDbContext<FlustriDbContext>();


builder.Services.AddTransient<ISigningService, SigningService>();
builder.Services.AddTransient<ILocksmithService, LocksmithService>();

builder.Services.AddTransient<IStartupFilter, FlustriStartup>();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1),
                PermitLimit = 10,
                QueueLimit = 0,
            }
        )
    );
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("logged-in", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 30;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 5;
    });
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("admin_rooms", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("scope", "rooms_api")
    )
    .AddPolicy("registered_rooms", policy =>
        policy
            .RequireRole("registered")
            .RequireClaim("scope", "registered_rooms"));

var app = builder.Build();

if (!builder.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => { return "Hello, world!"; }).RequireAuthorization();

// Register new user using a previously generated RegistrationRequestId
app.MapPost("/register", async (RegisterCommandRequest request, ISigningService signingService, FlustriDbContext db) => await RegisterCommand.ConsumeAsync(request, signingService, db))    
    .WithMetadata(new EndpointNameMetadata("register"));

// List rooms in server
app.MapGet("/rooms", async (int page, int pageSize, FlustriDbContext db) => await ListRoomsQuery.ConsumeAsync(new ListRoomsQueryRequest(page, pageSize), db))
    .RequireAuthorization("admin_rooms", "registered_rooms")
    .WithMetadata(new EndpointNameMetadata("list-rooms"))
    .RequireRateLimiting("logged-in");

// Create new room in server
app.MapPost("/rooms", async (CreateRoomCommandRequest request, FlustriDbContext db) => await CreateRoomCommand.ConsumeAsync(request, db))
    .RequireAuthorization("admin_rooms")
    .WithMetadata(new EndpointNameMetadata("create-room"))
    .RequireRateLimiting("logged-in");


// // List messages in room
// app.MapGet("/rooms/{roomName}/messages", (string roomName, int page, int limit) => { throw new NotImplementedException(); })
//     .RequireAuthorization("admin_rooms", "registered_rooms");

// // Create new message to room
// app.MapPost("/rooms/{roomName}/messages", (string roomName, CreateMessageRequest request) => { throw new NotImplementedException(); })
//     .RequireAuthorization("admin_rooms", "registered_rooms");

// // Update existing message in room
// app.MapPut("/rooms/{roomName}/messages/{messageId}", (string roomName, string messageId) => { throw new NotImplementedException(); })
//     .RequireAuthorization("admin_rooms", "registered_rooms");

app.Run();
