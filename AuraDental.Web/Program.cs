using AuraDental.Aplicacion;
using AuraDental.Dominio.Interfaces;
using AuraDental.Infraestructura;
using AuraDental.Infraestructura.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPersonalService, PersonalService>();

// Registro del DbContext con la cadena de conexión
builder.Services.AddDbContext<AuraDentalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuraDentalConnection")));

// Registro del repositorio genérico
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Registro del servicio de autenticación
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IServicioService, ServicioService>();
builder.Services.AddScoped<IAgendaService, AgendaService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IPaisService, PaisService>();
builder.Services.AddHttpClient<IEmailService, EmailService>();

builder.Services.AddScoped<IExpedienteService, ExpedienteService>();
builder.Services.AddScoped<IResenaService, ResenaService>();
// Habilitar el servicio de sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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
app.UseRouting();

// El middleware de sesión colocado estratégicamente después de Routing y antes de Authorization
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();