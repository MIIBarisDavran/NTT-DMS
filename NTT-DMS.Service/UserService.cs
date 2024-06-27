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
using Microsoft.AspNetCore.Identity;

namespace NTT_DMS.Service
{
    public class UserService
    {
        private readonly DMSContext _context;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(DMSContext context, IConfiguration config, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _config = config;
            _passwordHasher = passwordHasher;
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
            User item = new User()
            {
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                UserRole = user.UserRole,
            };
            item.password = _passwordHasher.HashPassword(item, item.password);
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
        public async Task<bool> Delete(int[] userIds, string userEmail)
        {
            if (userIds.Length == 0)
            {
                return false;
            }
            try
            {
                var _user = _context.Users.Where(o => userIds.Contains(o.UserId)).ToList();
                _context.Users.RemoveRange(_user);
                await _context.SaveChangesAsync(userEmail);
                return true;
            }
            catch (Exception ex)
            {
                var exp = ex;
                return false;
            }

        }

    }
}
