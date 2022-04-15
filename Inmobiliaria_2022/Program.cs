var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
//_________________________________________________________________________2
//using Microsoft.AspNetCore.Authentication.Cookies;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//  .AddCookie(options =>
//  {
//      options.LoginPath = "/Usuarios/Login";//a donde redirigir para login
//                                            //LoginPath Si yo trato de acceder a un lugar que no tengo accesso porque todavia no me autentique me redirige al Path 
//      options.LogoutPath = "/Usuarios/Logout";//idem para logout()
//      options.AccessDeniedPath = "/Home/Restringido";//idem para rec restringidos(Estoy logeadopero no tengo acceso a esse sservicio)
//  });
//builder.Services.Configure<CookiePolicyOptions>(options =>
//{
//    options.CheckConsentNeeded = context => true;
//    options.MinimumSameSitePolicy = SameSiteMode.None;
//});
////3_Crear politicas de autorizacion y se usan en el contoller
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("Administrador", policy =>
//    policy.RequireRole("Administrador", "SuperAdministrador")
//    //policy.RequireClaim(ClaimTypes.Role, "Administrador")  requiere q el usuario tenga cpmp rol el adminitrador
//    );
//});


//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseCookiePolicy();
//app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
//________________________________1__________________________
//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>//el sitio web valida con cookie
//    {
//        options.LoginPath = "/Usuarios/Login";
//        options.LogoutPath = "/Usuarios/Logout";
//        options.AccessDeniedPath = "/Home/Restringido";
//    });
//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

////app.UseHttpsRedirection();
////app.UseStaticFiles();

////app.UseRouting();

////app.UseAuthorization();
//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseCookiePolicy();
//app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();





//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute("login", "entrar/{**accion}", new { controller = "Usuarios", action = "Login" });
//    endpoints.MapControllerRoute("rutaFija", "ruteo/{valor}", new { controller = "Home", action = "Ruta", valor = "defecto" });
//    endpoints.MapControllerRoute("fechas", "{controller=Home}/{action=Fecha}/{anio}/{mes}/{dia}");
//    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

//});

//app.Run();
