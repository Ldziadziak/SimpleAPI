using Microsoft.EntityFrameworkCore;
using Serilog;
using SimpleAPI.DbContexts;
using SimpleAPI.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(CustomerMapper).Assembly);
builder.Services.AddLocalServices();
builder.Services.AddMvc().AddNewtonsoftJson(); //for JsonPatch
builder.Services.AddControllersWithViews();
builder.Services.AddEntityFrameworkSqlite().AddDbContext<CustomerContext>();
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

#region db things
using (var db = new CustomerContext())
{
    try
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.Database.Migrate();
    }
    catch { }
}
#endregion

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Home}/{id?}");
app.Run();