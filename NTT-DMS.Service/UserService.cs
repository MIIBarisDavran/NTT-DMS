using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

        [HttpPost]
        public UserViewModel GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == id);
            var userViewModel = new UserViewModel
            {
                UserEmail = user.UserEmail,
                UserId = user.UserId,
                UserName = user.UserName,
                UserRole = user.UserRole,
                Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "User", Text = "User" },
                new SelectListItem { Value = "Admin", Text = "Admin" }
            }
            };
            if (userViewModel == null)
            {
                return null;
            }
            return userViewModel;
        }

        /*
         * CREATE USER
         */
        public bool UpdateUser(UserViewModel user)
        {
            bool status;
            User item = new User
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                password = user.password,
                UserRole = user.UserRole,
            };
            try
            {
                _context.Users.Update(item);
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
         * CREATE USER
         */
        public async Task<bool> Create(User user, string userEmail)
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
                //_context.SaveChanges();
                status = await _context.SaveChangesAsync(userEmail) == 1 ? true : false;
                //status = true;
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
        [HttpPost]
        public bool Delete(int[] userId)
        {
            bool status = true;
            if (userId.IsNullOrEmpty())
            {
                status = false;
            }
            foreach (var item in userId)
            {
                var i = _context.Users.Find(item);
                try
                {
                    _context.Users.Remove(i);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    var exp = ex;
                    status = false;
                    break;
                }
            }
            return status;
        }

    }
}
