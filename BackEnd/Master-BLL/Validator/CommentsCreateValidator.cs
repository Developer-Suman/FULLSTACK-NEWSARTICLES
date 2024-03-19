using Master_BLL.DTOs.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Validator
{
    public class CommentsCreateValidator
    {
        public static List<string> Validate(CommentsCreateDTOs commentsCreateDTOs)
        {
            var errors = new List<string>();

            //Perform Validation Rules
            if(commentsCreateDTOs is null)
            {
                errors.Add("Comments data should not null");
            }

            if(!string.IsNullOrEmpty(commentsCreateDTOs.CommentDescription))
            {
                errors.Add("Comments description is null");
            }

            return errors;

        }
    }
}
