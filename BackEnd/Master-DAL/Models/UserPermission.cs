using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class UserPermission : Entity
    {
        public UserPermission(): base(null) { }

        public UserPermission(
            string id,
            string userId,
            string permissionId
            ): base(id)
        {
            UserId = userId;
            PermissionId = permissionId;
            
        }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
        public string PermissionId { get; set; }
        [ForeignKey(nameof(PermissionId))]
        public Permission Permissions { get; set; }

    }
}
