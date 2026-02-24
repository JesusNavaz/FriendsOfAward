using FriendsOfAward.Components;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(options =>
{
    options.DetailedErrors = true;
});

builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, FriendsOfAward.MyCustomAuthStateProvider>();


builder.Services.AddDistributedMemoryCache();


builder.Services.AddSession();

var app = builder.Build();

app.UseSession();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();