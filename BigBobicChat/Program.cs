using BigBobicChat;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Database>();
builder.Services.AddSignalR();
WebApplication app = builder.Build();

Database database = app.Services.GetRequiredService<Database>();
database.CreateTables();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHub<ChatHub>("/chat");
app.Run();
