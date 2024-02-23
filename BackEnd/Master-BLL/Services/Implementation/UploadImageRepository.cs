﻿using Master_BLL.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Implementation
{
    public class UploadImageRepository : IUploadImageRepository
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadImageRepository(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _contextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            
        }
        public void DeleteImage(string ImageUrl)
        {
            throw new NotImplementedException();
        }

        public void UpdateImage(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public void UpdateMultipleImage(List<IFormFile> file)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            try
            {
                string uploadFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                if(!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                string uniqueFile = Guid.NewGuid().ToString();
                string originalFileName = Path.GetFileName(file.FileName);
                string fileExtension = Path.GetExtension(originalFileName);

                //Combine the uploadfolder path with the uniquefile name
                string filepath = Path.Combine(uploadFolderPath, uniqueFile + fileExtension);

                //Copy the file to the server
                using(var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return Path.Combine("Images", uniqueFile + fileExtension);

            }catch(Exception ex)
            {
                throw new Exception("An error occur while Upload Image");
            }
        }

        public Task<List<string>> UploadMultipleImage(List<IFormFile> files)
        {
            throw new NotImplementedException();
        }
    }
}
