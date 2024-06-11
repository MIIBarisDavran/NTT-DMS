using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NTT_DMS.Data
{
    public class User
    {
        public User()
        {
            this.Categories = new HashSet<Category>();
            this.Documents = new HashSet<Document>();
        }
        public static ClaimsIdentity Identity { get; set; }
        [Key]
        public int UserId { get; set; }
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string UserRole { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}
