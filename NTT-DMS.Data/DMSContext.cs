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

        public DMSContext(DbContextOptions<DMSContext> options) : base(options)
        {
            
        }

        // Custom SaveChangesAsync method to log changes
        public async Task<int> SaveChangesAsync(string userId)
        {
            var logList = new List<Log>();
            // Get all Added/Deleted/Modified entities
            foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                // For each changed record, get the audit record entries and add them
                //foreach (Log log in GetAuditRecordsForChange(ent, userId))
                //{
                //    this.Logs.Add(log);
                //}
                logList.AddRange(GetAuditRecordsForChange(ent, userId));
            }
            this.Logs.AddRange(logList);

            var result = await base.SaveChangesAsync();

            // Call the original SaveChanges(), which will save both the changes made and the audit records
            

            //await base.SaveChangesAsync();
            return result;

        }

        private List<Log> GetAuditRecordsForChange(EntityEntry dbEntry, string userId)
        {
            List<Log> result = new List<Log>();
            DateTime changeTime = DateTime.Now;

            // Get the Table() attribute, if one exists
            TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            string keyName = dbEntry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;
            //string keyValue = dbEntry.State switch
            //{
            //    EntityState.Added => dbEntry.CurrentValues.GetValue<object>(keyName)?.ToString(),
            //    EntityState.Deleted => dbEntry.OriginalValues.GetValue<object>(keyName)?.ToString(),
            //    EntityState.Modified => dbEntry.OriginalValues.GetValue<object>(keyName)?.ToString(),
            //    _ => null
            //};

            if (dbEntry.State == EntityState.Added)
            {
                // For Inserts, just add the whole record
                result.Add(new Log()
                {
                    LogID = Guid.NewGuid(),
                    EventType = "A", // Added
                    TableName = tableName,
                    RecordID = "",
                    ColumnName = "*ALL",
                    NewValue = dbEntry.CurrentValues.ToObject().ToString(),
                    Created_by = userId,
                    Created_date = changeTime
                });
            }
            else if (dbEntry.State == EntityState.Deleted)
            {
                // Same with deletes, do the whole record
                result.Add(new Log()
                {
                    LogID = Guid.NewGuid(),
                    EventType = "D", // Deleted
                    TableName = tableName,
                    RecordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                    ColumnName = "*ALL",
                    NewValue = dbEntry.OriginalValues.ToObject().ToString(),
                    Created_by = userId,
                    Created_date = changeTime
                });
            }
            else if (dbEntry.State == EntityState.Modified)
            {
                foreach (string propertyName in dbEntry.OriginalValues.Properties.Select(p => p.Name))
                {
                    // For updates, we only want to capture the columns that actually changed
                    if (!Equals(dbEntry.OriginalValues[propertyName], dbEntry.CurrentValues[propertyName]))
                    {
                        result.Add(new Log()
                        {
                            LogID = Guid.NewGuid(),
                            EventType = "M",    // Modified
                            TableName = tableName,
                            RecordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                            ColumnName = propertyName,
                            OriginalValue = dbEntry.OriginalValues[propertyName]?.ToString(),
                            NewValue = dbEntry.CurrentValues[propertyName]?.ToString(),
                            Created_by = userId,
                            Created_date = changeTime
                        });
                    }
                }
            }

            return result;
        }

    }
}
