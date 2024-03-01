﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Interface
{
    public interface IUploadImageRepository
    {
        Task<string> UploadImage(IFormFile file);   
        Task<List<string>> UploadMultipleImage(List<IFormFile> files);
        void DeleteImage(string ImageUrl);
        void DeleteMultipleImage(List<string> ImageUrls);
        Task<string> UpdateImage(IFormFile file, string ImageURL);
        Task<List<string>> UpdateMultipleImage(List<IFormFile> file, List<string> ImageURLs);
    }
}
