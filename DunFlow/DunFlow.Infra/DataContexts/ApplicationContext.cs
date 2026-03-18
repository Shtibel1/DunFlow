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

        public DbSet<AppUser> Users { get; set; }
        public DbSet<WorkTask> WorkTasks { get; set; }
        public DbSet<WorkTaskType> WorkTaskTypes { get; set; }
        public DbSet<WorkTaskStatus> WorkTaskStatuses { get; set; }
        public DbSet<FormField> FormFields { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkTask>(entity =>
            {
                entity.ToTable("WorkTasks", b => b.IsTemporal());
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.WorkTaskType)
                    .WithMany(p => p.WorkTasks)
                    .HasForeignKey(d => d.WorkTaskTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.AssignedUser)
                    .WithMany(p => p.AssignedTasks)
                    .HasForeignKey(d => d.AssignedUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CustomFieldsJson)
                    .HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<WorkTaskStatus>(entity =>
            {
                entity.HasOne(d => d.WorkTaskType)
                    .WithMany(p => p.Statuses)
                    .HasForeignKey(d => d.WorkTaskTypeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FormField>(entity =>
            {
                entity.HasOne(d => d.WorkTaskStatus)
                    .WithMany(p => p.FormFields)
                    .HasForeignKey(d => d.WorkTaskStatusId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            SeedUsers(modelBuilder);
            SeedMetadata(modelBuilder);
        }

        private void SeedUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser { Id = 1, FullName = "Nadav" },
                new AppUser { Id = 2, FullName = "Nashef" },
                new AppUser { Id = 3, FullName = "Dun" }
            );
        }

        private void SeedMetadata(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkTaskType>().HasData(
                new WorkTaskType { Id = 1, Name = "Development", Description = "Software Development Task" },
                new WorkTaskType { Id = 2, Name = "Procurement", Description = "Purchasing and Procurement" }
            );

            modelBuilder.Entity<WorkTaskStatus>().HasData(
                new WorkTaskStatus { Id = 1, WorkTaskTypeId = 1, StatusValue = 1, DisplayName = "Created", IsFinal = false },
                new WorkTaskStatus { Id = 2, WorkTaskTypeId = 1, StatusValue = 2, DisplayName = "Specification Completed", IsFinal = false },
                new WorkTaskStatus { Id = 3, WorkTaskTypeId = 1, StatusValue = 3, DisplayName = "Development Completed", IsFinal = false },
                new WorkTaskStatus { Id = 4, WorkTaskTypeId = 1, StatusValue = 4, DisplayName = "Distribution Completed", IsFinal = true },

                new WorkTaskStatus { Id = 5, WorkTaskTypeId = 2, StatusValue = 1, DisplayName = "Created", IsFinal = false },
                new WorkTaskStatus { Id = 6, WorkTaskTypeId = 2, StatusValue = 2, DisplayName = "Supplier Offers Received", IsFinal = false },
                new WorkTaskStatus { Id = 7, WorkTaskTypeId = 2, StatusValue = 3, DisplayName = "Purchase Completed", IsFinal = true }
            );

            modelBuilder.Entity<FormField>().HasData(
                new FormField { Id = 1, WorkTaskStatusId = 2, Key = "SpecificationText", Label = "Specification Details", ControlType = "textarea", IsRequired = true },
                new FormField { Id = 2, WorkTaskStatusId = 3, Key = "BranchName", Label = "Branch Name", ControlType = "text", IsRequired = true },
                new FormField { Id = 3, WorkTaskStatusId = 4, Key = "VersionNumber", Label = "Release Version", ControlType = "text", IsRequired = true },

                new FormField { Id = 4, WorkTaskStatusId = 6, Key = "PriceQuote1", Label = "Price Quote 1", ControlType = "text", IsRequired = true },
                new FormField { Id = 5, WorkTaskStatusId = 6, Key = "PriceQuote2", Label = "Price Quote 2", ControlType = "text", IsRequired = true },
                new FormField { Id = 6, WorkTaskStatusId = 7, Key = "ReceiptString", Label = "Receipt String", ControlType = "text", IsRequired = true }
            );
        }
    }
}
