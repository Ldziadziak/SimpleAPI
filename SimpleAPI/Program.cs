using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SimpleAPI.DbContexts;
using SimpleAPI.Extensions;
using SimpleAPI.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(CustomerMapper).Assembly);
builder.Services.AddLocalServices();
builder.Services.AddMvc().AddNewtonsoftJson(); //for JsonPatch
builder.Services.AddControllersWithViews();
builder.Services.AddEntityFrameworkSqlite().AddDbContext<CustomerContext>();

#region api default version
builder.Services.AddApiVersioning(config =>
 {
     config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(2, 0);
     config.ReportApiVersions = true;
 });
#endregion

#region swagger things
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
#endregion

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

#region db things
using (var db = new CustomerContext())
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    try
    {
        //it will crash if database exists
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.Database.Migrate();
        logger.LogInformation("DB updated!");
    }
    catch
    {

        logger.LogInformation("DB not updated!");
    }
}
#endregion

#region swagger/versioning
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
app.Run();