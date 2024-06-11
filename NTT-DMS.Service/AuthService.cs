using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTT_DMS.Data;
using Humanizer;

namespace NTT_DMS.Service
{
    public class AuthService
    {
        private readonly DMSContext _context;
        public AuthService(DMSContext db)
        {
            _context = db;
        }

        /*
         * USER LOGIN CREDENTIAL CHECK
         */
        public List<User> CheckCredential(UserLogin user)
        {
            var _user = _context.Users.FirstOrDefault(x => x.UserEmail == user.UserEmail
             && x.password == user.password);
            if (_user == null)
            {
                return null;
            }
            return _context.Users.Where(x => x.UserEmail == _user.UserEmail).Select(x => new User
            {
                UserId = x.UserId,
                UserName = x.UserName,
                UserEmail = x.UserEmail,
                UserRole = x.UserRole
            }).ToList();
        }
    }
}
