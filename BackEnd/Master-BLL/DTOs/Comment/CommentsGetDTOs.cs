﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Comment
{
    public record CommentsGetDTOs(
        string CommentsId,
        string Content,
        string ArticlesId
        );
}
