using Master_DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Articles
{
    [DebuggerDisplay("ArticlesTitle = {ArticlesTitle}, ArticlesContent = {ArticlesContent,nq}")]
    //This helps developer for debugging complex scenerio by giving direct value during debugging, nq erase out ""
    public record ArticlesCreateDTOs(string ArticlesTitle, string ArticlesContent);

}
