using Microsoft.Extensions.Configuration;
using NTT_DMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTT_DMS.Service
{
    public class UserService
    {
        private readonly DMSContext _context;
        private readonly IConfiguration _config;

        public UserService(DMSContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /*
         * GET LIST OF USERS
         */
        public List<User> GetAll()
        {
            var _users = _context.Users.ToList();
            return _users;
        }

        /*
         * CREATE USER
         */
        public bool Create(User user)
        {
            bool status;
            User item = new User();
            item.UserName = user.UserName;
            item.UserEmail = user.UserEmail;
            item.password = user.password;
            item.UserRole = user.UserRole;
            try
            {
                _context.Users.Add(item);
                _context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                var exp = ex;
                status = false;
            }
            return status;
        }

        /*
         * DELETE USER
         */
        public bool Delete(int id)
        {
            bool status;
            var item = _context.Users.Find(id);
            try
            {
                _context.Users.Remove(item);
                _context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                var exp = ex;
                status = false;
            }
            return status;
        }

    }
}
