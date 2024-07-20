using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class Permission : Entity
    {
        public Permission() : base(null) { }

        public Permission(
            string id,
            string permissions
            ) : base(id)
        {
            Permissions = permissions;
            UserPermissions = new List<UserPermission>();
            PermissionControllerActions = new List<PermissionControllerAction>();

        }
        public string Permissions { get; set; }
        public ICollection<UserPermission> UserPermissions { get; set; }
        public ICollection<PermissionControllerAction> PermissionControllerActions { get; set; }


    }
}
