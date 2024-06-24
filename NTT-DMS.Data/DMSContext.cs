using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace NTT_DMS.Data
{
    public class DMSContext: DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Document> Documents { get; set; }

        public virtual DbSet<Log> Logs { get; set; }

        private readonly IChangeLogger _changeLogger;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Categories)
                .WithOne(c => c.Users)
                .HasForeignKey(c => c.UsersUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Documents)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UsersUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Documents)
                .WithOne(d => d.Category)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DMSContext(DbContextOptions<DMSContext> options, IChangeLogger changeLogger) : base(options)
        {
            _changeLogger = changeLogger;
        }

        public async Task SaveChangesAsync(string userId)
        {
            var logs = new List<Log>();
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Deleted || e.State == EntityState.Modified))
            {
                logs.AddRange(_changeLogger.GetAuditRecordsForChange(entry, userId));
            }
            if (logs.Any())
            {
               Logs.AddRange(logs);
            }

            await base.SaveChangesAsync();
        }

        public void LogUserLogin(string userEmail)
        {
            var log = new Log
            {
                LogID = Guid.NewGuid(),
                EventType = "Login",
                TableName = "User",
                RecordID = userEmail,
                ActionID = "Login",
                ColumnName = "UserEmail",
                OriginalValue = "",
                NewValue = "",
                Created_by = userEmail,
                Created_date = DateTime.Now
            };

            Logs.Add(log);
            SaveChanges();
        }

        public void CustomLogAction(string userEmail, string eventType, string tableName, string ColumnName)
        {
            var log = new Log
            {
                LogID = Guid.NewGuid(),
                EventType = eventType,
                TableName = tableName,
                RecordID = userEmail,
                ActionID = eventType,
                ColumnName = ColumnName,
                OriginalValue = "",
                NewValue = "",
                Created_by = userEmail,
                Created_date = DateTime.Now
            };

            Logs.Add(log);
            SaveChanges();
        }

    }
}
