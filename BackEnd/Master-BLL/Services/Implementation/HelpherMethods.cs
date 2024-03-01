using Master_BLL.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Implementation
{
    public class HelpherMethods : IHelpherMethods
    {
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
