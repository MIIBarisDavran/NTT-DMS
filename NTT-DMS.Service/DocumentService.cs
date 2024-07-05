using NTT_DMS.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace NTT_DMS.Service
{
    public class DocumentService
    {
        private readonly DMSContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly ILogger<DocumentService> _logger;
            
            
        public DocumentService(DMSContext db, IHostingEnvironment appEnvironment, ILogger<DocumentService> logger)
        {
            _context = db;
            _appEnvironment = appEnvironment;
            _logger = logger;

        }

        /*
         * GET LIST OF DOCUMENTS
         */
        public async Task<IQueryable<DocumentViewModel>> GetList(string email, string str)
        {
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
            var doc = from x in _context.Documents
                      where x.UsersUserId == user.UserId
                      select x;
            var items = from x in _context.Documents
                        where x.UsersUserId == user.UserId
                        select new DocumentViewModel
                        {   
                            DocumentId = x.DocumentId,
                            DocumentPath = x.DocumentPath,
                            DocumentName = x.DocumentName,
                            CategoryId = x.CategoryId,
                            CategoryName = x.Category.CategoryName
                        };
            if (!string.IsNullOrEmpty(str))
            {
                var searcheditems = from x in doc.Where(x => x.DocumentTags.Contains(str) || x.DocumentName.Contains(str) || x.Category.CategoryName.Contains(str))
                                    select new DocumentViewModel
                                    {
                                        DocumentId = x.DocumentId,
                                        DocumentPath = x.DocumentPath,
                                        DocumentName = x.DocumentName,
                                        CategoryId = x.CategoryId,
                                        CategoryName = x.Category.CategoryName
                                    };
                return searcheditems.AsQueryable();
            }
            return items.AsQueryable();
        }

        /*
         * UPLOAD DOCUMENT
         */
        public async Task<Dictionary<string, string>> Upload(IFormFile file, string path, Document document, string email)
        {
            var response = new Dictionary<string, string>
            {
                {"error", "Something went wrong."}
            };
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
            if (user == null)
            {
                response = new Dictionary<string, string>
            {
                {"error", "User not found."}
            };
                return response;
            }
            string pathRoot = path;
            string userFolderPath = "Documents\\" + user.UserId.ToString();
            string filePath = Path.Combine(userFolderPath, file.GetFilename());
            if (!CreateDirectory(pathRoot, userFolderPath))
            {
                response = new Dictionary<string, string>
            {
                {"error", "Failed to create user directory."}
            };
                return response;
            }
            var existingDocument = _context.Documents.FirstOrDefault(d => d.UsersUserId == user.UserId && d.DocumentName == file.GetFilename());
            if (existingDocument != null)
            {
                response = new Dictionary<string, string>
            {
                {"error", "You have already uploaded a document with the same name."}
            };
                return response;
            }
            string extention = Path.GetExtension(file.FileName);
            //var validateExtResponse = this.ValidateExtention(file);
            //if (validateExtResponse["status"] == false)
            //{
            //    response = new Dictionary<string, string>
            //    {
            //        {"error", "Invalid document extention. [allowed types: pdf/doc/docx/csv/png/jpg/jpeg/txt]"}
            //    };
            //}
            var validateFileSizeResponse = this.ValidateFileSize(file);
            if (validateFileSizeResponse["status"] == false)
            {
                response = new Dictionary<string, string>
                {
                    {"error", "File empty or max size exeeds."}
                };
            }
            else
            {
                try
                {
                    string fullPath = Path.Combine(path, filePath);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    Document item = new Document
                    {
                        DocumentPath = filePath,
                        DocumentName = file.FileName,
                        DocumentTags = file.FileName,
                        CategoryId = document.CategoryId,
                        UsersUserId = user.UserId
                    };
                    _context.Add(item);
                    await _context.SaveChangesAsync(email);
                    response = new Dictionary<string, string>
                    {
                        {"success", "File uploaded successfully."}
                    };
                }
                catch (Exception ex)
                {
                    var exp = ex;
                    response = new Dictionary<string, string>
                    {
                        {"error", "Something went wrong.Try again"}
                    };
                }

            }
            return response;
        }

        [HttpPost]
        public bool Delete(int[] documentIds)
        {
            try
            {
                var documents = _context.Documents.Where(d => documentIds.Contains(d.DocumentId)).ToList();
                string pathRoot = _appEnvironment.WebRootPath;
                foreach (var item in documents)
                {
                    var path = Path.Combine(_appEnvironment.WebRootPath, "Documents", item.UsersUserId.ToString() ,item.DocumentName);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    else
                    {
                        throw new FileNotFoundException("Document not found");
                    }
                }
                _context.Documents.RemoveRange(documents);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /* 
         * GET DOCUMENT PATH
         */
        public string GetPath(int userId, int documentId)
        {
            var item = _context.Documents.Where(
                x => x.DocumentId == documentId && x.User.UserId == userId
                ).FirstOrDefault();
            return item.DocumentPath;
        }

        public bool CreateDirectory(string pathRoot, string userFolderPath)
        {
            try
            {
                var absolutePath = Path.Combine(_appEnvironment.WebRootPath, userFolderPath);
                if (!Directory.Exists(absolutePath))
                {
                    Directory.CreateDirectory(absolutePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new directory");
                throw;
            }
        }

        /* 
         * GET DOCUMENT NAME
         */
        public string GetName(int userId, int documentId)
        {
            var item = _context.Documents.Where(x => x.DocumentId == documentId && x.User.UserId == userId).FirstOrDefault();
            return item.DocumentName;
        }

        /*
         * DOCUMENT EXT TYPE VALIDATION
         */

        public Dictionary<string, bool> ValidateExtention(IFormFile file)
        {
            var response = new Dictionary<string, bool>
            {
                {"status", false}
            };
            string[] allowedTypes = { ".doc", ".docx", ".pdf", ".txt", ".png", ".jpg", ".jpeg", ".gif", ".csv" };
            var isAllowedExtention = Array.Exists(allowedTypes, element => element == Path.GetExtension(file.FileName).ToLower());
            if (isAllowedExtention)
            {
                response = new Dictionary<string, bool>
                {
                    {"status", true}
                };
            }
            return response;
        }

        /*
         * DOCUMENT MAX SIZE VALIDATION
         */
        public Dictionary<string, bool> ValidateFileSize(IFormFile file)
        {
            var response = new Dictionary<string, bool>
            {
                {"status", true}
            };
            if (file == null || file.Length == 0 || file.Length > 40000000)
            {
                response = new Dictionary<string, bool>
                {
                    {"status", false},
                };
            }

            return response;
        }

        /*
         * DOCUMENT PERMISSION RULE
         */
        public bool DocumentPermissionRule(int userId, int documentId)
        {
            var response = false;
            var itemCount = _context.Documents.Where(x => x.DocumentId == documentId && x.User.UserId == userId).Count();
            if (itemCount == 1)
            {
                response = true;
            }
            return response;
        }


    }
}
