using Master_BLL.Services.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Implementation
{
    public class HelpherMethods : IHelpherMethods
    {
        public bool CompareImage(IFormFile imageFile1, string imagePath2)
        {
            using (var imageStream1 = Image.FromStream(imageFile1.OpenReadStream()))
            using (var image1 = new Bitmap(imageStream1))
            using (var image2 = new Bitmap(imagePath2))
            {
                if (image1.Width != image2.Width || image1.Height != image2.Height)
                {
                    // Images are of different dimensions, so they cannot be the same
                    return false;
                }

                for (int x = 0; x < image1.Width; x++)
                {
                    for (int y = 0; y < image1.Height; y++)
                    {
                        if (image1.GetPixel(x, y) != image2.GetPixel(x, y))
                        {
                            // Pixels are different, images are not the same
                            return false;
                        }
                    }
                }

                // All pixels are the same, images are identical
                return true;
            }
        }

        public void CompressFile(string inputFilePath, string outputFilePath)
        {
            using(FileStream inputStream = new FileStream(inputFilePath, FileMode.Open))
            {
                using(FileStream outputStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    using(var zipArchieve = new ZipArchive(outputStream, ZipArchiveMode.Create))
                    {
                        var zipEntry = zipArchieve.CreateEntry(Path.GetFileName(inputFilePath), CompressionLevel.Optimal);
                        using(var entryStream = zipEntry.Open())
                        {
                            inputStream.CopyTo(entryStream);
                        }
                    }

                }
            }
        }

        public bool IsImage(string contentType)
        {
            return contentType.StartsWith("image/jpeg") ||
           contentType.StartsWith("image/png") ||
           contentType.StartsWith("image/jpg");
        }
    }
}
