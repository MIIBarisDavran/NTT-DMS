using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NTT_DMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTT_DMS.Service
{
    public class LoggerService
    {
        private readonly DMSContext _context;

        public LoggerService(DMSContext context)
        {
            _context = context;
        }

        public List<Log> GetAll(string str)
        {
            var _logs = _context.Logs.ToList();
            if (!string.IsNullOrEmpty(str))
            {
                var searchedItems = _logs
            .Where(x => x.LogID.ToString().Contains(str, StringComparison.OrdinalIgnoreCase)
                     || x.NewValue.Contains(str, StringComparison.OrdinalIgnoreCase)
                     || x.OriginalValue.Contains(str, StringComparison.OrdinalIgnoreCase)
                     || x.EventType.Contains(str, StringComparison.OrdinalIgnoreCase)
                     || x.RecordID.Contains(str, StringComparison.OrdinalIgnoreCase)
                     || x.TableName.Contains(str, StringComparison.OrdinalIgnoreCase)
                     || x.ActionID.Contains(str, StringComparison.OrdinalIgnoreCase)
                     || x.ColumnName.Contains(str, StringComparison.OrdinalIgnoreCase)
                     || x.Created_by.Contains(str, StringComparison.OrdinalIgnoreCase)
                     || x.Created_date.ToString().Contains(str, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.Created_date)
            .ToList();
                return searchedItems;
            }
            return _logs.OrderByDescending(x => x.Created_date).ToList();
        }

    }
}
