using Microsoft.IdentityModel.Tokens;
using NTT_DMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTT_DMS.Service
{
    public class CategoryService
    {
        private readonly DMSContext _context;
        public CategoryService(DMSContext db)
        {
           _context = db;
        }

        /*
         * GET LIST OF CATEGORY
         */
        public List<Category> GetAll(string email)
        {
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
            var categories = _context.Categories.Where(x => x.Users.UserId == user.UserId).ToList();
            _context.CustomLogAction(email, "Get Category", "Category", "*ALL");
            return categories;
        }

        public List<Category> GetAllFiltered(string email, string search)
        {
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
            var categories = _context.Categories.Where(x => x.Users.UserId == user.UserId).ToList();
            if (!search.IsNullOrEmpty())
            {
                return categories.Where(o => o.CategoryName.Contains(search)).ToList();
            }
            _context.CustomLogAction(email, "Get Category", "Category", "*ALL");
            return categories;
        }

        /*
         * CREATE CATEGORY
         */
        public async Task<bool> CreateCategory(Category Cat, string email)
        {
            if (Cat == null)
            {
                return false;
            }
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
            Category item = new Category()
            {
                CategoryName = Cat.CategoryName,
                UsersUserId = user.UserId,
            };
            try
            {
                _context.Categories.Add(item);
                await _context.SaveChangesAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                var exp = ex;
                return false;
            }
        }

        /*
         * DELETE CATEGORY
         */
        public async Task<bool> DeleteCategory(int[] categoryIds, string email)
        {
            var item = _context.Categories.Where(o => categoryIds.Contains(o.CategoryId)).ToList();
            try
            {
                _context.Categories.RemoveRange(item);
                await _context.SaveChangesAsync(email);
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
