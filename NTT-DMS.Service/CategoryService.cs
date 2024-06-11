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
            return categories;
        }

        /*
         * CREATE CATEGORY
         */
        public bool CreateCategory(Category Cat, string email)
        {
            bool status;
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
            Category item = new Category();
            item.CategoryName = Cat.CategoryName;
            item.UsersUserId = user.UserId;

            try
            {
                _context.Categories.Add(item);
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
         * DELETE CATEGORY
         */
        public bool DeleteCategory(int id)
        {
            bool status;
            var item = _context.Categories.Find(id);

            try
            {
                _context.Categories.Remove(item);
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
