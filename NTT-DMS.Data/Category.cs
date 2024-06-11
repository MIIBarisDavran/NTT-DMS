using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NTT_DMS.Data
{
    public  class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public int UsersUserId { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual User Users { get; set; }
    }
}
