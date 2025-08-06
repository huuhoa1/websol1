using Microsoft.EntityFrameworkCore;
using webapp1.Data;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using webapp1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add GraphQL client
var graphQLEndpoint = builder.Configuration["GraphQL:Endpoint"];
builder.Services.AddSingleton<GraphQLHttpClient>(sp =>
    new GraphQLHttpClient(graphQLEndpoint!, new SystemTextJsonSerializer()));

// Add GraphQL services
builder.Services.AddScoped<ICountryService, CountryService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
