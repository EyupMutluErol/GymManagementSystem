using GymManagementSystem.Business.Abstract;
using GymManagementSystem.Business.Concrete;
using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.DataAccess.Concrete.EntityFramework;
using GymManagementSystem.DataAccess.Context;
using GymManagementSystem.Entities.Concrete;
using GymManagementSystem.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GymContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false;          
    options.Password.RequireLowercase = false;    
    options.Password.RequireUppercase = false;      
    options.Password.RequireNonAlphanumeric = false; 
    options.Password.RequiredLength = 3;
})
.AddEntityFrameworkStores<GymContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<CustomIdentityErrorDescriber>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Giriþ yapmamýþ kullanýcýyý buraya yönlendir
    options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisi yetmeyen kullanýcýyý buraya yönlendir
});

// > DataAccess Katmaný (Repositoryler)
builder.Services.AddScoped<IAppUserRepository, EfAppUserRepository>();
builder.Services.AddScoped<IGymRepository, EfGymRepository>();
builder.Services.AddScoped<IServiceRepository, EfServiceRepository>();
builder.Services.AddScoped<IAppointmentRepository, EfAppointmentRepository>();
builder.Services.AddScoped<ITrainerServiceRepository, EfTrainerServiceRepository>();

// > Business Katmaný (Servisler)
builder.Services.AddScoped<IAppUserService, AppUserManager>();
builder.Services.AddScoped<IGymService, GymManager>();
builder.Services.AddScoped<IServiceService, ServiceManager>();
builder.Services.AddScoped<IAppointmentService, AppointmentManager>();
builder.Services.AddScoped<ITrainerServiceService, TrainerServiceManager>();




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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
