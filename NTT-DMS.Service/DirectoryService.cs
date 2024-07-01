using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NTT_DMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTT_DMS.Service
{
    public class DirectoryService
    {
        private readonly DMSContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly ILogger<DocumentService> _logger;

        public DirectoryService(DMSContext db, IHostingEnvironment appEnvironment, ILogger<DocumentService> logger)
        {
            _context = db;
            _appEnvironment = appEnvironment;
            _logger = logger;

        }
        public List<DirectoryNode> GetDirectoryTree()
        {
            string path = Path.Combine(_appEnvironment.WebRootPath, "Documents");

            var directoryTree = GetDirectoryNodes(new DirectoryInfo(path));
            return directoryTree;
        }

        private List<DirectoryNode> GetDirectoryNodes(DirectoryInfo directoryInfo)
        {
            var nodes = new List<DirectoryNode>();

            foreach (var directory in directoryInfo.GetDirectories())
            {
                nodes.Add(new DirectoryNode
                {
                    Name = directory.Name,
                    Path = directory.FullName,
                    IsDirectory = true,
                    Children = GetDirectoryNodes(directory)
                });
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                nodes.Add(new DirectoryNode
                {
                    Name = file.Name,
                    Path = file.FullName,
                    IsDirectory = false
                });
            }

            return nodes;
        }
    }
}
