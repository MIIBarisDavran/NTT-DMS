using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTT_DMS.Data
{
    [Table("N_Log")]
    public class Log
    {
        [Key]
        public Guid LogID { get; set; }

        [Required]
        public string EventType { get; set; }

        [Required]
        public string TableName { get; set; }

        public string ActionID { get; set; }

        [Required]
        public string RecordID { get; set; }

        [Required]
        public string ColumnName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        [Required]
        public string Created_by { get; set; }

        [Required]
        public DateTime Created_date { get; set; }
    }
}
