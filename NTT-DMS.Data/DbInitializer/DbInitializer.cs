using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTT_DMS.Data.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly DMSContext _context;
        public DbInitializer(DMSContext db)
        {
            _context = db;
        }

        public void Initialize()
        {

            try
            {
                if(_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
