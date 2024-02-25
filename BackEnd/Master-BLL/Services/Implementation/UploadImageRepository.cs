using ImageMagick;
using Master_BLL.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Hosting;
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
            try
            {
                var webRootPath = Path.Combine(_webHostEnvironment.WebRootPath, ImageUrl);
                if(File.Exists(webRootPath))
                {
                    File.Delete(webRootPath);
                }
            }catch(Exception ex)
            {
                throw new Exception("An error occured while deleting Image");
            }
        }

        public async Task<string> UpdateImage(IFormFile file, string ImageURL)
        {
            try
            {
                DeleteImage(ImageURL);
                var saveImage = await UploadImage(file);

                return saveImage;

            }
            catch(Exception ex)
            {
                throw new Exception("An error occured while Uploading image");
            }
        }

        public void UpdateMultipleImage(List<IFormFile> file)
        {
            try
            {
               

            }
            catch(Exception ex)
            {
                throw new Exception("An error occured while Uploading Image");
            }
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            try
            {
                string uploadFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                if (!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                string uniqueFile = Guid.NewGuid().ToString();
                string originalFileName = Path.GetFileName(file.FileName);
                string fileExtension = Path.GetExtension(originalFileName);

                //Combine the uploadfolder path with the uniquefile name
                string filepath = Path.Combine(uploadFolderPath, uniqueFile + fileExtension);

                //Copy the file to the server
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }


                using (MagickImage image = new MagickImage(filepath))
                {
                    //Set the desired format like .png,.jpg
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg")
                    {
                        image.Format = MagickFormat.Jpg;

                    }
                    if (fileExtension == ".png")
                    {
                        image.Format = MagickFormat.Png;
                    }

                    //Resize the image if necessary
                    image.Resize(1000, 1000);

                    //Set the compression Quality(0-100)
                    image.Quality = 60; //This is the compression level

                    string uniqueFileAfterCompression = Guid.NewGuid().ToString();
                    string originalfilenameAfterCompression = Path.GetFileName(file.FileName);
                    string fileExtensionAfterCompression = Path.GetExtension(originalfilenameAfterCompression);
                    string filepathAfterCompression = Path.Combine(uploadFolderPath, uniqueFileAfterCompression + fileExtensionAfterCompression);
                    image.Write(filepathAfterCompression);

                    System.IO.File.Delete(filepath);

                    return Path.Combine("Images/", uniqueFileAfterCompression + fileExtensionAfterCompression);
                }


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
