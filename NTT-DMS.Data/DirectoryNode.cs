using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTT_DMS.Data
{
    public class DirectoryNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDirectory { get; set; }
        public List<DirectoryNode> Children { get; set; } = new List<DirectoryNode>();
    }
}
