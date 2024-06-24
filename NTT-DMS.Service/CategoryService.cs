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
            Category item = new Category();
            item.CategoryName = Cat.CategoryName;
            item.UsersUserId = user.UserId;

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
        public async Task<bool> DeleteCategory(int id, string email)
        {
            var item = _context.Categories.Find(id);
            try
            {
                _context.Categories.Remove(item);
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
