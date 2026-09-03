using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GestorContas.Web.Data.AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Add Conciliação Services & Strategy (DDD)
builder.Services.AddScoped<GestorContas.Web.Services.Conciliacao.IConciliacaoExtratoStrategy, GestorContas.Web.Services.Conciliacao.MatchExatoConciliacaoStrategy>();
builder.Services.AddScoped<GestorContas.Web.Services.Conciliacao.IConciliacaoBancariaService, GestorContas.Web.Services.Conciliacao.ConciliacaoBancariaService>();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

// Ensure the database is created and apply migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<GestorContas.Web.Data.AppDbContext>();
        context.Database.EnsureCreated();

        // Garantir inclusão da nova coluna DescricaoNoExtrato se ainda não existir no SQLite existente
        try
        {
            context.Database.ExecuteSqlRaw(@"
                SELECT DescricaoNoExtrato FROM Lancamentos LIMIT 1;
            ");
        }
        catch
        {
            context.Database.ExecuteSqlRaw(@"
                ALTER TABLE Lancamentos ADD COLUMN DescricaoNoExtrato TEXT;
            ");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating/updating the database.");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
