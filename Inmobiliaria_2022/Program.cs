using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//1- Se configura el servicio
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>//el sitio web valida con cookie ,una vez creada la cookie el usario ya debe estar logeado
    {
        options.LoginPath = "/Usuarios/Login";
        options.LogoutPath = "/Usuarios/Logout";
        options.AccessDeniedPath = "/Home/Restringido";
    });
    
    builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
//3_Crear politicas de autorizacion y se usan en el contoller
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy =>
    policy.RequireRole("Administrador", "SuperAdministrador")
    //policy.RequireClaim(ClaimTypes.Role, "Administrador")  /*requiere q el usuario tenga cpmp rol el adminitrador*/
    );
});
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
//app.UseCookiePolicy();
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
});
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("login", "entrar/{**accion}", new { controller = "Usuarios", action = "Login" });
    endpoints.MapControllerRoute("rutaFija", "ruteo/{valor}", new { controller = "Home", action = "Ruta", valor = "defecto" });
    endpoints.MapControllerRoute("fechas", "{controller=Home}/{action=Fecha}/{anio}/{mes}/{dia}");
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

});

app.Run();
