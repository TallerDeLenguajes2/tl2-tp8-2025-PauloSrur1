var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session + DI
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// DI registrations
builder.Services.AddScoped<tl2_tp8_2025_PauloSrur1.Interfaces.IProductoRepository, Repositories.ProductoRepository>();
builder.Services.AddScoped<tl2_tp8_2025_PauloSrur1.Interfaces.IPresupuestoRepository, Repositories.PresupuestoRepository>();
builder.Services.AddScoped<tl2_tp8_2025_PauloSrur1.Interfaces.IUserRepository, Repositories.UsuarioRepository>();
builder.Services.AddScoped<tl2_tp8_2025_PauloSrur1.Interfaces.IAuthenticationService, tl2_tp8_2025_PauloSrur1.Services.AuthenticationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// El orden del middleware es importante para el manejo de sesión y autorización
app.UseSession();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
