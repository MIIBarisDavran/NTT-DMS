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
        public List<User> GetAll(string str)
        {
            var _users = _context.Users.ToList();
            if (!string.IsNullOrEmpty(str))
            {
                var searchedItems = _users.Where(x => x.UserEmail.Contains(str) || x.UserName.Contains(str) || x.UserRole.Contains(str)).ToList();
                return searchedItems;
            }
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
        public async Task<bool> UpdateUser(UserViewModel user, string userEmail)
        {
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.UserEmail) || string.IsNullOrWhiteSpace(user.password)
                || string.IsNullOrWhiteSpace(user.UserRole) || string.IsNullOrWhiteSpace(user.UserId.ToString()))
            {
                return false;
            }
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
                await _context.SaveChangesAsync(userEmail);
                return true;
            }
            catch (Exception ex)
            {
                var exp = ex;
                return false;
            }
        }

        /*
         * CREATE USER
         */
        public async Task<bool> Create(User user, string userEmail)
        {
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.UserEmail) || string.IsNullOrWhiteSpace(user.password)
                || string.IsNullOrWhiteSpace(user.UserRole))
            {
                return false;
            }
            User item = new User();
            item.UserName = user.UserName;
            item.UserEmail = user.UserEmail;
            item.password = user.password;
            item.UserRole = user.UserRole;
            try
            {
                _context.Users.Add(item);
                await _context.SaveChangesAsync(userEmail);
                return true;
            }
            catch (Exception ex)
            {
                var exp = ex;
                return false;
            }
        }

        /*
         * DELETE USER
         */
        [HttpPost]
        public async Task<bool> Delete(int[] userId, string userEmail)
        {
            bool status = true;
            if (userId.IsNullOrEmpty())
            {
                status = false;
                return status;
            }
            foreach (var item in userId)
            {
                var i = _context.Users.Find(item);
                try
                {
                    _context.Users.Remove(i);
                    await _context.SaveChangesAsync(userEmail);
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
