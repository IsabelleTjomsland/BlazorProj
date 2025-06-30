
// DbContext

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

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(e => e.EmployeeID);

                entity.HasMany(e => e.Schedule)
                      .WithOne(s => s.Employee)
                      .HasForeignKey(s => s.EmployeeID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Shift
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

            // Schedule
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedules");
                entity.HasKey(sc => sc.ScheduleID);
            });
        }
    }
}
