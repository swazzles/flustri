using Flustri.Api;
using Flustri.Api.Auth;
using Flustri.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IServer, Server>();

builder.Services.AddAuthentication(options => options.DefaultScheme = "FlustriAuth")
    .AddScheme<FlustriAuthSchemeOptions, FlustriAuthHandler>("FlustriAuth", options => { });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!builder.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.MapGet("/", () => { return "Hello, world!"; }).RequireAuthorization();

// List rooms in server
app.MapGet("/rooms", (int page, int limit) => { throw new NotImplementedException(); })
    .RequireAuthorization();

// Create new room in server
app.MapPost("/rooms", (CreateRoomRequest request) => { throw new NotImplementedException(); } )
    .RequireAuthorization();


// List messages in room
app.MapGet("/rooms/{roomName}/messages", (string roomName, int page, int limit) => { throw new NotImplementedException(); })
    .RequireAuthorization();

// Create new message to room
app.MapPost("/rooms/{roomName}/messages", (string roomName, CreateMessageRequest request) => { throw new NotImplementedException(); })
    .RequireAuthorization();

// Update existing message in room
app.MapPut("/rooms/{roomName}/messages/{messageId}", (string roomName, string messageId) => { throw new NotImplementedException(); })
    .RequireAuthorization();

app.Run();
