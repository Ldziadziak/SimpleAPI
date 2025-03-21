using Serilog;
using SimpleAPI.DbContexts;
using SimpleAPI.Extensions;
using SimpleAPI.Infrastructure;
using SimpleAPI.Utils;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
  options.ReturnHttpNotAcceptable = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(CustomerMapper).Assembly);
builder.Services.AddLocalServices();

builder.Services.AddMvc().AddNewtonsoftJson();

builder.Services.AddControllersWithViews();

builder.Services.AddEntityFrameworkSqlite().AddDbContext<CustomerContext>();

#region Konfiguracja wersjonowania API
builder.Services.AddApiVersioning(config =>
{
  config.DefaultApiVersion = new ApiVersion(2, 0);
  config.ReportApiVersions = true;
});
#endregion

#region Konfiguracja Swaggera
builder.Services.AddSwaggerGen(options =>
{
  options.DocInclusionPredicate((version, apiDescription) =>
  {
    var versions = apiDescription.ActionDescriptor
        .EndpointMetadata
        .OfType<ApiVersionAttribute>()
        .SelectMany(attr => attr.Versions);

    return versions.Any(v => $"v{v.ToString()}" == version);
  });

  options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API v1", Version = "v1" });
  options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API v2", Version = "v2" });
});

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
#endregion

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

#region Migracja bazy danych
var services = app.Services;
await DatabaseInitializer.MigrateDatabaseAsync(services);
#endregion

#region Konfiguracja Swaggera i wersji API w środowisku deweloperskim
var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    var versionDescriptions = apiVersionProvider.ApiVersionDescriptions
          .OrderByDescending(desc => desc.ApiVersion)
          .Select(description => (
              Url: $"/swagger/{description.GroupName}/swagger.json",
              Name: description.GroupName.ToUpperInvariant()))
          .ToList();

    versionDescriptions.ForEach(endpoint =>
          options.SwaggerEndpoint(endpoint.Url, endpoint.Name));
  });
}
#endregion

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Home}/{id?}");

await app.RunAsync();
