using Microsoft.EntityFrameworkCore;
using MoMA_thias.web.Components;
using MoMA_thias.web.Data;
using MoMA_thias.web.Repository;
using MoMA_thias.web.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Register db and repositories
builder.Services.RegisterRepositories();

// Register services
builder.Services.RegisterServices();


var app = builder.Build();


// MVP: Create database if not exists
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<ControlHub>("/controlhub");

app.Run();