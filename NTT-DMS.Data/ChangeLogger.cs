using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace NTT_DMS.Data
{
    public interface IChangeLogger
    {
        List<Log> GetAuditRecordsForChange(EntityEntry dbEntry, string userId);
    }

    public class ChangeLogger : IChangeLogger
    {

        public List<Log> GetAuditRecordsForChange(EntityEntry dbEntry, string userId)
        {
                var logs = new List<Log>();
                var changeTime = DateTime.Now;
                var tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
                var tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
                var keyProperty = dbEntry.Entity.GetType().GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());
                var keyName = keyProperty?.Name ?? "Id";
                //var recordId = keyProperty != null ? dbEntry.OriginalValues.GetValue<object>(keyName)?.ToString() : null;   

                if (dbEntry.State == EntityState.Added)
                {
                    logs.Add(CreateLogEntry("Add", tableName, "", "", "*ALL", "", dbEntry.CurrentValues.ToObject().ToString(), userId, changeTime));
                }
                else if (dbEntry.State == EntityState.Deleted)
                {
                    logs.Add(CreateLogEntry("Delete", tableName, "", "", "*ALL", dbEntry.OriginalValues.ToObject().ToString(), "", userId, changeTime));
                }
                else if (dbEntry.State == EntityState.Modified)
                {
                    var originalValues = dbEntry.GetDatabaseValues();
                    foreach (var propertyName in dbEntry.OriginalValues.Properties.Select(p => p.Name))
                    {
                        var originalValue = originalValues[propertyName];
                        var currentValue = dbEntry.CurrentValues[propertyName];
                        if (!Equals(originalValue, currentValue))
                        {
                            logs.Add(CreateLogEntry("Modify", tableName, "", "", propertyName, originalValue?.ToString(), currentValue?.ToString(), userId, changeTime));
                        }
                    }
                }

                return logs;
        }

        private Log CreateLogEntry(string eventType, string tableName, string recordId, string actionId, string columnName, string originalValue, string newValue, string userId, DateTime changeTime)
        {
            return new Log
            {
                LogID = Guid.NewGuid(),
                EventType = eventType,
                TableName = tableName,
                RecordID = recordId,
                ActionID = actionId,
                ColumnName = columnName,
                OriginalValue = originalValue,
                NewValue = newValue,
                Created_by = userId,
                Created_date = changeTime
            };
        }
    }
}
