using Microsoft.EntityFrameworkCore;
using Bemanning_System.Backend.Models;

namespace Bemanning_System.Backend.Data
{
    public class StaffingContext : DbContext
    {
        public StaffingContext(DbContextOptions<StaffingContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // User
            // -------------------------
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.UserID);

                entity.Property(u => u.FirstName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(u => u.LastName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(u => u.Email)
                      .HasMaxLength(256)
                      .IsRequired();

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                // Detta är din jobb/yrkesroll-kolumn i Users-tabellen (om du behåller den)
                entity.Property(u => u.Role)
                      .HasMaxLength(100);

                // SystemRoll för accesskontroll (Admin, Employee)
                entity.Property(u => u.SystemRole)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(u => u.IsActive)
                      .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                      .HasColumnType("datetime2");

                entity.Property(u => u.UpdatedAt)
                      .HasColumnType("datetime2");
            });

            // -------------------------
            // Employee
            // -------------------------
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(e => e.EmployeeID);

                entity.Property(e => e.FirstName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.LastName)
                      .HasMaxLength(100)
                      .IsRequired();

                // Yrkesroll / befattning (Läkare, Receptionist osv.)
                entity.Property(e => e.Role)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Email)
                      .HasMaxLength(256)
                      .IsRequired();

                entity.Property(e => e.PasswordHash)
                      .IsRequired();

                // FK till Users
                entity.HasOne(e => e.User)
                      .WithOne(u => u.Employee!)     // En User kan ha 0..1 Employee; se kommentar nedan
                      .HasForeignKey<Employee>(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade); // ändra vid behov
            });

            // OBS om du hellre vill tillåta *flera* Employees per User (ovanligt), byt till:
            // .WithMany(u => u.Employees)
            // och ändra navigation i User (ICollection<Employee>).

            // -------------------------
            // Shift
            // -------------------------
            modelBuilder.Entity<Shift>(entity =>
            {
                entity.ToTable("Shifts");
                entity.HasKey(s => s.ShiftID);

                entity.Property(s => s.ShiftDate).HasColumnName("ShiftDate");
                entity.Property(s => s.StartTime).HasColumnName("StartTime");
                entity.Property(s => s.EndTime).HasColumnName("EndTime");
                entity.Property(s => s.Description).HasColumnName("Description");

                entity.HasMany(s => s.Schedules)
                      .WithOne(sc => sc.Shift)
                      .HasForeignKey(sc => sc.ShiftID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -------------------------
            // Schedule
            // -------------------------
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedules");
                entity.HasKey(sc => sc.ScheduleID);

                // Relation till Employee (du hade den i Employee-mappningen ovan via .HasMany)
                entity.HasOne(sc => sc.Employee)
                      .WithMany(e => e.Schedule)
                      .HasForeignKey(sc => sc.EmployeeID)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
