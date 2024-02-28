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

        public void DeleteMultipleImage(List<string> ImageUrls)
        {
            try
            {
                foreach(var imageUrl in ImageUrls)
                {
                    var webRootPath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                    if(File.Exists(webRootPath) && File.Exists(imageUrl))
                    {
                        File.Delete(webRootPath);
                    }
                }

            }catch(Exception ex )
            {
                throw new Exception("An error occured while deleting image");
            }
        }

        public async Task<string> UpdateImage(IFormFile file, string ImageURL)
        {
            try
            {
                if(file is null || file.Length == 0)
                {
                    return ImageURL;
                }
                DeleteImage(ImageURL);
                var saveImage = await UploadImage(file);

                return saveImage;

            }
            catch(Exception ex)
            {
                throw new Exception("An error occured while Uploading image");
            }
        }

        public async Task<List<string>> UpdateMultipleImage(List<IFormFile> file, List<string> ImageURLs)
        {
            try
            {

                //foreach(var imageUrl in ImageURLs)
                //{
                //    var webRootPath = Path.Combine(_webHostEnvironment.WebRootPath,imageUrl);
                //    if(File.Exists(webRootPath) =file)
                //    {

                //    }
                //}



                List<string> multipleImageURLs = new List<string>();

                //Iterate through each file and corrosponding URL
                if(file is not null)
                {
                    for (int i = 0; i < file.Count; i++)
                    {
                        IFormFile imgfile = file[i];
                        string oldImageURLs = ImageURLs[i];
                        var webRootPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImageURLs);
                        var fileName = Path.GetFileName(webRootPath);

                        //Get the filename from the path
                        var filename = Path.GetFileName(webRootPath);

                        var imegeName = filename.Split('~');

                        //Get the filename from the uploaded file
                        var filenameFromUploadedFile = Path.GetFileName(imgfile.FileName);



                        string filenameFromUploadedFiles = Path.GetFileNameWithoutExtension(imgfile.FileName);


                        if (imegeName[0] == filenameFromUploadedFile)
                        {
                            multipleImageURLs.Add(oldImageURLs);
                        }
                        else
                        {
                            //If new file is provided update images
                            var updateImage = await UploadImage(imgfile);
                            multipleImageURLs.Add(updateImage);

                        }



                    }
                }
                else
                {
                    return ImageURLs;
                }
         

                //Keep the remaining old imageURls that were not updated
                for (int i = file.Count; i < ImageURLs.Count; i++)
                {
                    multipleImageURLs.Add(ImageURLs[i]);
                }

                //Delete Old image that were Replaced
                DeleteMultipleImage(ImageURLs);

                return multipleImageURLs;
                #region Using ForeachLoop
                //List<string> multipleImgURL = new List<string>();
                //foreach(var imgfile in file)
                //{
                //    if(imgfile is not null && imgfile.Length > 0)
                //    {
                //        //If a new File is provided, update the image
                //        var saveImage = await UploadImage(imgfile);
                //        multipleImgURL.Add(saveImage);
                //    }
                //    else
                //    {
                //        //If no new file is provided, keep the old image url
                //        multipleImgURL.Add(ImageURLs.FirstOrDefault());
                //    }


                //}
                //DeleteMultipleImage(ImageURLs);

                //return multipleImgURL;

                #endregion



            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while Uploading multiple Image");
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

        public async Task<List<string>> UploadMultipleImage(List<IFormFile> files)
        {
            try
            {
                List<string> filename = new List<string>();
                string uploadFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                if(!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                foreach(var image in files)
                {
                    string uniqueFile = Guid.NewGuid().ToString();
                    string originalFileName = Path.GetFileName(image.FileName);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                    string FileExtension = Path.GetExtension(originalFileName);

                    //Combine uploadFolderPath with unique file and fileExtension 
                    string filepath = Path.Combine(uploadFolderPath, fileNameWithoutExtension+'~'+ uniqueFile + FileExtension);

                    //copy file to the server
                    using(var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);

                    }

                    using(MagickImage img = new MagickImage(filepath))
                    {
                        if(FileExtension == ".jpg" ||  FileExtension ==".jpeg")
                        {
                            img.Format = MagickFormat.Jpg;

                        }
                        if(FileExtension == ".png")
                        {
                            img.Format = MagickFormat.Png;
                        }

                        img.Resize(1000, 1000);

                        img.Quality = 60;

                        string uniqueFileAfterCompression = Guid.NewGuid().ToString();
                        string FileNameAfterCompression = Path.GetFileName(image.FileName);
                        string getFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithoutExtension);
                        string ExtensionAfterCompression = Path.GetExtension(FileNameAfterCompression);

                        string FilePathAfterCompression = Path.Combine(uploadFolderPath, getFileNameWithoutExtension+'~'+ uniqueFileAfterCompression + ExtensionAfterCompression);
                        img.Write(FilePathAfterCompression);

                        System.IO.File.Delete(filepath);

                        filename.Add(Path.Combine("Images/", getFileNameWithoutExtension +'~'+uniqueFileAfterCompression + ExtensionAfterCompression));
                    }

                }

                return filename;



            }
            catch(Exception ex)
            {
                throw new Exception("An error occured while Uploading multiple image");
            }
        }
    }
}
