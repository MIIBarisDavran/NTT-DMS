using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

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
            var utcNow = DateTime.UtcNow;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"); // UTC+3
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
            var tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            var tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
            var keyProperty = dbEntry.Entity.GetType().GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());
            var keyName = keyProperty?.Name ?? "Id";
            //var recordId = keyProperty != null ? dbEntry.OriginalValues.GetValue<object>(keyName)?.ToString() : null;   

            if (dbEntry.State == EntityState.Added)
            {
                logs.Add(CreateLogEntry("Add", tableName, "", "", "*ALL", "", SerializeObject(dbEntry.CurrentValues), userId, localTime));
            }
            else if (dbEntry.State == EntityState.Deleted)
            {
                logs.Add(CreateLogEntry("Delete", tableName, "", "", "*ALL", SerializeObject(dbEntry.CurrentValues), "", userId, localTime));
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
                        logs.Add(CreateLogEntry("Modify", tableName, "", "", propertyName, originalValue?.ToString(), currentValue?.ToString(), userId, localTime));
                    }
                }
            }

            return logs;
        }
        private string SerializeObject(PropertyValues propertyValues)
        {
            var properties = propertyValues.Properties.Select(p => new { Name = p.Name, Value = propertyValues[p.Name] }).ToList();
            return JsonConvert.SerializeObject(properties, Formatting.Indented);
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
