using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public sealed class ControllerAction : Entity
    {
        public ControllerAction(): base(null) { }

        public ControllerAction(
            string id,
            string controller,
            string action
            ) : base(id)
        {
            Controller = controller;
            Action = action;
            PermissionControllerActions = new List<PermissionControllerAction>();
        }

        public string Controller { get; set; }
        public string Action { get; set; }
        public ICollection<PermissionControllerAction> PermissionControllerActions { get; set; }
    }
}
