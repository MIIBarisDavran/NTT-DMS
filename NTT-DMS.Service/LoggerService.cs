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
            .Where(x => x.LogID.ToString().Contains(str)
                     || x.NewValue.Contains(str)
                     || x.OriginalValue.Contains(str)
                     || x.EventType.Contains(str)
                     || x.RecordID.Contains(str)
                     || x.TableName.Contains(str)
                     || x.ActionID.Contains(str)
                     || x.ColumnName.Contains(str)
                     || x.Created_by.Contains(str)
                     || x.Created_date.ToString().Contains(str))
            .OrderByDescending(x => x.Created_date)
            .ToList();
                return searchedItems;
            }
            return _logs.OrderByDescending(x => x.Created_date).ToList();
        }

    }
}
