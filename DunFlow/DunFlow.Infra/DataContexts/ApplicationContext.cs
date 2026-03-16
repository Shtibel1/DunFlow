using DunFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Infra.DataContexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }


        public DbSet<WorkTask> WorkTasks { get; set; }
        public DbSet<AppUser> Users { get; set; }
    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkTask>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.HasOne(d => d.AssignedUser)
                    .WithMany(p => p.AssignedTasks)
                    .HasForeignKey(d => d.AssignedUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CustomFieldsJson)
                    .HasColumnType("nvarchar(max)");
            });

            SeedUsers(modelBuilder);
        }

        private void SeedUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser { Id = 1, FullName = "Nadav"},
                new AppUser { Id = 2, FullName = "Nashef" },
                new AppUser { Id = 3, FullName = "Dun" }
            );
        }
    }
}
