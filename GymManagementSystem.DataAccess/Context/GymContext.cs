using GymManagementSystem.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DataAccess.Context;

public class GymContext:IdentityDbContext<AppUser,IdentityRole<int>,int>
{
    public GymContext(DbContextOptions<GymContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Gym> Gyms { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<TrainerService> TrainerServices { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- İLİŞKİ YAPILANDIRMALARI ---

        // 1. Gym - Service (Gym silinirse hizmetler de silinsin -> Cascade)
        builder.Entity<Service>()
            .HasOne(s => s.Gym)
            .WithMany(g => g.Services)
            .HasForeignKey(s => s.GymId)
            .OnDelete(DeleteBehavior.Cascade);

        // 2. Gym - Trainer (Gym silinirse hocalar silinmesin -> Restrict)
        builder.Entity<AppUser>()
            .HasOne(u => u.Gym)
            .WithMany(g => g.Trainers)
            .HasForeignKey(u => u.GymId)
            .OnDelete(DeleteBehavior.Restrict);

        // 3. TrainerService (Çoktan Çoka & Cascade)
        builder.Entity<TrainerService>()
            .HasKey(x => new { x.AppUserId, x.ServiceId });

        builder.Entity<TrainerService>()
            .HasOne(ts => ts.AppUser)
            .WithMany(u => u.TrainerServices) // AppUser'a eklediğimiz listeyi bağladık
            .HasForeignKey(ts => ts.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TrainerService>()
            .HasOne(ts => ts.Service)
            .WithMany(s => s.TrainerServices)
            .HasForeignKey(ts => ts.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. Randevu Güvenliği (Restrict - Veri Kaybını Önle)
        builder.Entity<Appointment>()
            .HasOne(a => a.Member)
            .WithMany()
            .HasForeignKey(a => a.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Trainer)
            .WithMany()
            .HasForeignKey(a => a.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Service)
            .WithMany()
            .HasForeignKey(a => a.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
