using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTT_DMS.Data
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }
        [Required]
        public string DocumentPath { get; set; }
        [Required]
        public string DocumentName { get; set; }
        [Required]
        public string DocumentTags { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("User")]
        public int UsersUserId { get; set; }
        public virtual Category Category { get; set; }
        public virtual User User { get; set; }


    }
}
