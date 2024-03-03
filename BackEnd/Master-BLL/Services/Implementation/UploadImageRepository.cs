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
        private readonly IHelpherMethods _helpherMethods;

        public UploadImageRepository(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IHelpherMethods helpherMethods)
        {
            _contextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _helpherMethods = helpherMethods;
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
                    if(File.Exists(webRootPath))
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
                List<string> multipleImageURLs = new List<string>();
                List<string> deleteImageFile = new List<string>();

                // Loop through each file provided for update
                foreach (var uploadedFile in file)
                {
                    // Get the filename from the uploaded file
                    var filenameFromUploadedFile = Path.GetFileName(uploadedFile.FileName);
                    string filenameFromUploadedFiles = Path.GetFileNameWithoutExtension(uploadedFile.FileName);

                    // Check if the filename matches any existing image
                    bool foundMatch = false;
                    foreach (var existingImageURL in ImageURLs)
                    {
                        var webRootPath = Path.Combine(_webHostEnvironment.WebRootPath, existingImageURL);
                        var fileName = Path.GetFileName(webRootPath);

                        // Get the filename from the path
                        var filename = Path.GetFileName(webRootPath);
                        var imageName = filename.Split('~');

                        bool checkImage = _helpherMethods.CompareImage(uploadedFile, webRootPath);

                        // If a match is found, add the existing URL and skip to the next file
                        if (imageName[0] == filenameFromUploadedFiles)
                        {
                            multipleImageURLs.Add(existingImageURL);
                            foundMatch = true;
                            break;
                        }
                    }

                    // If no match is found, upload the new image
                    if (!foundMatch)
                    {
                        var updateImage = await UploadImage(uploadedFile);
                        multipleImageURLs.Add(updateImage);
                    }
                }

                // Now we have the updated list of image URLs
                // Let's find images to delete (if any)
                foreach (var existingImageURL in ImageURLs)
                {
                    // If the existing image URL is not in the list of updated URLs, add it to delete
                    if (!multipleImageURLs.Contains(existingImageURL))
                    {
                        deleteImageFile.Add(existingImageURL);
                    }
                }

                // Delete the images that are no longer in the updated list
                if (deleteImageFile.Count > 0)
                {
                    DeleteMultipleImage(deleteImageFile);
                }

                // Return the updated list of image URLs
                return multipleImageURLs;
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
                string uploadFolderPathForFiles = Path.Combine(_webHostEnvironment.WebRootPath, "Files");
                if (!Directory.Exists(uploadFolderPath) || !Directory.Exists(uploadFolderPathForFiles))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                    Directory.CreateDirectory(uploadFolderPathForFiles);
                }

                if(_helpherMethods.IsImage(file.ContentType))
                {
                    string uniqueFile = Guid.NewGuid().ToString();
                    string originalFileName = Path.GetFileName(file.FileName);
                    string filenameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                    string fileExtension = Path.GetExtension(originalFileName);

                    //Combine the uploadfolder path with the uniquefile name
                    string filepath = Path.Combine(uploadFolderPath, filenameWithoutExtension + '~' + uniqueFile + fileExtension);

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
                        string getFileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalfilenameAfterCompression);
                        string fileExtensionAfterCompression = Path.GetExtension(originalfilenameAfterCompression);
                        string filepathAfterCompression = Path.Combine(uploadFolderPath, getFileNameWithoutExtension + '~' + uniqueFileAfterCompression + fileExtensionAfterCompression);
                        image.Write(filepathAfterCompression);

                        System.IO.File.Delete(filepath);

                        return Path.Combine("Images/", getFileNameWithoutExtension + '~' + uniqueFileAfterCompression + fileExtensionAfterCompression);
                    }

                }
                else
                {
                    string uniqueFile = Guid.NewGuid().ToString();
                    string originalFileName = Path.GetFileName(file.FileName);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                    string FileExtension = Path.GetExtension(originalFileName);


                    //Combine uploadFolderPath with unique file and fileExtension 
                    string filepath = Path.Combine(uploadFolderPathForFiles, fileNameWithoutExtension + '~' + uniqueFile + FileExtension);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    string uniqueFileNameAfterCompression = Guid.NewGuid().ToString();
                    string filenameAfterCompression = Path.GetFileName(file.FileName);
                    string getFileNameWithoutExtensionAfter = Path.GetFileNameWithoutExtension(filenameAfterCompression);
                    string ExtensionAfterCompression = Path.GetExtension(filenameAfterCompression);

                    string filePathAfterCompression = Path.Combine(uploadFolderPathForFiles, getFileNameWithoutExtensionAfter + '~' + uniqueFileNameAfterCompression + ExtensionAfterCompression);
                    _helpherMethods.CompressFile(filepath, filePathAfterCompression);

                    //Remove the original File
                    System.IO.File.Delete(filepath);

                    return Path.Combine("Files/", getFileNameWithoutExtensionAfter + '~' + uniqueFileNameAfterCompression + ExtensionAfterCompression);

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
                string uploadFolderPathForFiles = Path.Combine(_webHostEnvironment.WebRootPath, "Files");
                if(!Directory.Exists(uploadFolderPath) || !Directory.Exists(uploadFolderPathForFiles))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                    Directory.CreateDirectory(uploadFolderPathForFiles);
                }

                foreach(var image in files)
                {
                   

                    if (_helpherMethods.IsImage(image.ContentType))
                    {

                        string uniqueFile = Guid.NewGuid().ToString();
                        string originalFileName = Path.GetFileName(image.FileName);
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                        string FileExtension = Path.GetExtension(originalFileName);


                        //Combine uploadFolderPath with unique file and fileExtension 
                        string filepath = Path.Combine(uploadFolderPath, fileNameWithoutExtension + '~' + uniqueFile + FileExtension);

                        //copy images to the server
                        using (var fileStream = new FileStream(filepath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);

                        }

                        using (MagickImage img = new MagickImage(filepath))
                        {
                            if (FileExtension == ".jpg" || FileExtension == ".jpeg")
                            {
                                img.Format = MagickFormat.Jpg;

                            }
                            if (FileExtension == ".png")
                            {
                                img.Format = MagickFormat.Png;
                            }

                            img.Resize(1000, 1000);

                            img.Quality = 60;

                            string uniqueFileAfterCompression = Guid.NewGuid().ToString();
                            string FileNameAfterCompression = Path.GetFileName(image.FileName);
                            string getFileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileNameAfterCompression);
                            string ExtensionAfterCompression = Path.GetExtension(FileNameAfterCompression);

                            string FilePathAfterCompression = Path.Combine(uploadFolderPath, getFileNameWithoutExtension + '~' + uniqueFileAfterCompression + ExtensionAfterCompression);
                            img.Write(FilePathAfterCompression);

                            System.IO.File.Delete(filepath);

                            filename.Add(Path.Combine("Images/", getFileNameWithoutExtension + '~' + uniqueFileAfterCompression + ExtensionAfterCompression));
                        }

                    }
                    else
                    {

                        string uniqueFile = Guid.NewGuid().ToString();
                        string originalFileName = Path.GetFileName(image.FileName);
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                        string FileExtension = Path.GetExtension(originalFileName);


                        //Combine uploadFolderPath with unique file and fileExtension 
                        string filepath = Path.Combine(uploadFolderPathForFiles, fileNameWithoutExtension + '~' + uniqueFile + FileExtension);
                        using(var fileStream =  new FileStream(filepath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }

                        string uniqueFileNameAfterCompression = Guid.NewGuid().ToString();
                        string filenameAfterCompression = Path.GetFileName(image.FileName);
                        string getFileNameWithoutExtensionAfter = Path.GetFileNameWithoutExtension(filenameAfterCompression);
                        string ExtensionAfterCompression = Path.GetExtension(filenameAfterCompression);

                        string filePathAfterCompression = Path.Combine(uploadFolderPathForFiles, getFileNameWithoutExtensionAfter + '~' + uniqueFileNameAfterCompression + ExtensionAfterCompression);
                        _helpherMethods.CompressFile(filepath, filePathAfterCompression);

                        //Remove the original File
                        System.IO.File.Delete(filepath);

                        filename.Add(Path.Combine("Files/", getFileNameWithoutExtensionAfter + '~' + uniqueFileNameAfterCompression + ExtensionAfterCompression));


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
