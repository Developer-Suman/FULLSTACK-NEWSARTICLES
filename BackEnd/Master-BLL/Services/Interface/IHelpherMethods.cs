using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Interface
{
    public interface IHelpherMethods
    {
        bool IsImage(string contentType);
        void CompressFile(string inputFilePath, string outputFilePath);
    }
}
