using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class PermissionControllerAction : Entity
    {
        public PermissionControllerAction(): base(null) { }

        public PermissionControllerAction(
            string id,
            string permissionId,
            string controlleractionId
            ) : base(id)
        {
            PermissionId = permissionId;
            ControlleractionId = controlleractionId;
            
        }

        public string PermissionId { get;set; }
        [ForeignKey(nameof(PermissionId))]

        public Permission Permission {  get;set; }
        [ForeignKey(nameof(ControlleractionId))]
        public string ControlleractionId { get; set; }
        public ControllerAction ControllerAction { get; set; }

    }
}
