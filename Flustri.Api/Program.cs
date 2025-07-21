using Flustri.Api.Auth;
using Flustri.Core;
using Flustri.Core.Queries;
using Flustri.Core.Commands;
using Flustri.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.AddAuthentication(options => options.DefaultScheme = "FlustriAuth")
    .AddScheme<FlustriAuthSchemeOptions, FlustriAuthHandler>("FlustriAuth", options => { });

builder.Services.AddDbContext<FlustriDbContext>();

builder.Services.AddTransient<ILocksmith, Locksmith>(_ => new Locksmith(new LocksmithOptions(FlustriKeyDerivationAlgorithm.HkdfSha512, 256, 32)));

builder.Services.AddTransient<IStartupFilter, FlustriStartup>();

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

// List rooms in server
app.MapGet("/rooms", async (int page, int pageSize, FlustriDbContext db) => await ListRoomsConsumer.ConsumeAsync(new ListRoomsQuery(page, pageSize), db))
    .RequireAuthorization("admin_rooms", "registered_rooms");

// Create new room in server
app.MapPost("/rooms", async (CreateRoomRequest request, FlustriDbContext db) => await CreateRoomConsumer.ConsumeAsync(request, db))
    .RequireAuthorization("admin_rooms");


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
