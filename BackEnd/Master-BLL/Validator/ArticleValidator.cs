using Master_BLL.DTOs.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Validator
{
    public class ArticleValidator
    {
        public static List<string> Validate(ArticlesCreateDTOs articlesCreateDTOs)
        {

            var errors = new List<string>();
            //Perform Validatation Rules
            if (articlesCreateDTOs == null)
            {
                errors.Add("Articles data is null");
            }

            if (string.IsNullOrEmpty(articlesCreateDTOs.ArticlesTitle))
            {
                errors.Add("Title is required.");
            }
            if(string.IsNullOrEmpty(articlesCreateDTOs.ArticlesContent))
            {
                errors.Add("Content is required");
            }
            if (articlesCreateDTOs.filesList == null )
            {
                errors.Add("At least one image is required");
            }

            return errors;
        }
    }
}
